using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using KCSG.AutomatedWarehouse.Enumeration;
using KCSG.AutomatedWarehouse.Model;
using KCSG.AutomatedWarehouse.ViewModel.AutoWarehouse;
using log4net;
using Microsoft.Practices.ServiceLocation;
using System.Text;
using KCSG.Core.Models;
using KCSG.Data.DatabaseContext;
using KCSG.Data.Infrastructure;

namespace KCSG.AutomatedWarehouse.ViewModel
{
    /// <summary>
    ///     This class contains properties that the main View can data bind to.
    ///     <para>
    ///         Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    ///     </para>
    ///     <para>
    ///         You can also use Blend to data bind with the tool's support.
    ///     </para>
    ///     <para>
    ///         See http://www.galasoft.ch/mvvm
    ///     </para>
    /// </summary>
    public class MainViewModel : ParentViewModel
    {
        #region Constructors

        private const int CommandStatusTime = 15000;
        
        /// <summary>
        ///     Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            _manualResetListenerEvent = new ManualResetEvent(false);
            _manualResetScannerEvent = new ManualResetEvent(false);
            _manualResetStatusRequest = new ManualResetEvent(false);

            _listenMessagesThread = new Thread(ListenIncomingConnection);
            _listenMessagesThread.IsBackground = true;

            _StatusRequestThread = new Thread(SendingStatusRequestCommands);
            _StatusRequestThread.IsBackground = true;

            _searchPendingCommandsThread = new Thread(SearchWarehousePendingCommands);
            _searchPendingCommandsThread.IsBackground = true;
            // Relay command initialization.
            RelayCommandOnLoad = new RelayCommand<Window>(OnLoad);
            ClickToggleRelayCommand = new RelayCommand<Window>(ExecuteClickToggle);
            ExitApplication = new RelayCommand(ExecuteExitApplication);
        }

        #endregion

        #region Methods

        #region Command implementation

        /// <summary>
        ///     Callback which is fired when window has been loaded successfully.
        /// </summary>
        public void OnLoad(Window window)
        {
            TerminalSetting = Setting.TerminalsConfiguration.Terminals
                .FirstOrDefault(x => x.Key.Equals(Setting.TerminalsConfiguration.DefaultTerminal, StringComparison.InvariantCultureIgnoreCase));

            // Run threads.
            _listenMessagesThread.Start();
            _searchPendingCommandsThread.Start();

            // TODO: Enable this one again
            _StatusRequestThread.Start();

            // Start application automatically.
            ExecuteClickToggle(window);

            Message.MessageSent += MessageSent;
        }

        private async void MessageSent(string message)
        {
            var screenName = "";

            if (TerminalName.Material.Equals(_terminalSetting.Key))
                screenName = ConfigurationManager.AppSettings["AWT001-ScreenName"];
            else if (TerminalName.PreProduct.Equals(_terminalSetting.Key))
                screenName = ConfigurationManager.AppSettings["AWT002-ScreenName"];
            else if (TerminalName.Product.Equals(_terminalSetting.Key))
                screenName = ConfigurationManager.AppSettings["AWT003-ScreenName"];

            if (string.IsNullOrWhiteSpace(screenName))
                return;

            var obj = new
            {
                message = new
                {
                    message = message,
                    screenName = screenName,
                    terminals = new string[] { "TM10" }
                }
            };

            await AutoController.SendHttpMessage(obj);
        }

        /// <summary>
        ///     Callback which is fired when start button is clicked.
        /// </summary>
        /// <param name="owner"></param>
        public void ExecuteClickToggle(Window owner)
        {
            // Listening progress is not running.
            if (!_isTaskRunning)
            {
                var terminalSetting = _terminalSetting.Value;
                var incoming = terminalSetting.Incoming;
                var outgoing = terminalSetting.Outgoing;

                // Update IP Endpoint.
                IPEndPoint endPoint;

                try
                {
                    endPoint = new IPEndPoint(IPAddress.Any, incoming.Port);
                }
                catch (Exception exception)
                {
                    Log.Error(exception.Message, exception);
                    Message.ShowMessageBox(owner, "Incoming message IP Endpoint is not valid.", "System Message",
                        MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    return;
                }

                try
                {
                    // Tcp listener has been initialized.
                    if (_tcpListener != null)
                        _tcpListener.Stop();

                    // Initiate tcp listener.
                    _tcpListener = new TcpListener(terminalSetting.Incoming.Port);

                    // Start the listener.
                    _tcpListener.Start();
                }
                catch (Exception exception)
                {
                    Log.Error(exception.Message, exception);
                    Message.ShowMessageBox(owner, "Something is wrong while listener opens.", "System Message",
                        MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    return;
                }

                // Clear all caches and messages.
                Message.ClearMessages();
                PreProductAutoWarehouseController.ClearQueue();
                MaterialAutoWarehouseController.ClearQueue();
                ProductAutoWarehouseController.ClearQueue();
                
                // Update window state.
                IsTaskRunning = true;

                // Signal threads.
                _manualResetListenerEvent.Set();
                _manualResetScannerEvent.Set();
                _manualResetStatusRequest.Set();

                // Display message to UI.
                Message.InitiateMessage(DateTime.Now, MessageType.Information,
                    $"Listener has been opened at {incoming.Address}:{incoming.Port}.");
                Message.InitiateMessage(DateTime.Now, MessageType.Information,
                    $"Pending commands scanner has been initialized at port {outgoing.Address}:{outgoing.Port}.");
                return;
            }

            //UninitiateIncomingMessageListeningTask();
            _manualResetListenerEvent.Reset();
            Message.InitiateMessage(DateTime.Now, MessageType.Information, "Listener has been manually stopped.");

            //UninitiatePendingCommandScanningTask();
            _manualResetScannerEvent.Reset();
            Message.InitiateMessage(DateTime.Now, MessageType.Information, "Scanning pending command task was manually stopped.");

            _manualResetStatusRequest.Reset();

            IsTaskRunning = false;

        }

        #endregion

        #region Connection broadcast & listen

        /// <summary>
        ///     Task which is used for listening to incoming connection.
        /// </summary>
        private void ListenIncomingConnection()
        {
            while (true)
            {
                if (!_isTaskRunning)
                    continue;

                if (_tcpListener == null)
                    continue;

                // Wait the thread for signal.
                _manualResetListenerEvent.WaitOne();


                try
                {
                    // Accept one connection at one time.
                    using (var tcpClient = _tcpListener.AcceptTcpClient())
                    using (var stream = tcpClient.GetStream())
                    {
                        // Read buffer from stream.
                        var buffer = new byte[64];
                        var rb = stream.Read(buffer, 0, buffer.Length);
                        if (rb != 64)
                            continue;

                        // Read commands sent from external.
                        var content = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                        content = content.Trim();
                        if (string.IsNullOrEmpty(content))
                            continue;

                        var information = $"(Port: {_terminalSetting.Value.Incoming.Port}) Received message: {content} from external component - {rb} bytes received";
                        Message.InitiateMessage(DateTime.Now, MessageType.Receive, "Received message", content);
                        Log.Info(information);

                        // Parse terminal message obtained from external terminal.
                        var terminalMessage = TerminalMessage.Parse(content, false);

                        if (terminalMessage == null)
                        {
                            Message.InitiateMessage(DateTime.Now, MessageType.Information,
                                $"Incoming message is incorrect. Restart listener in {Setting.CommandScanTaskInterval} millisecs.");
                            Thread.Sleep(Setting.CommandScanTaskInterval);
                            continue;
                        }

                        if ("5000".Equals(terminalMessage.CommandIndex))
                            continue;

                        // Display message to screen.
                        Log.Info("Command sending process is blocked for analyzing listening process.");
                        //Message.InitiateMessage(DateTime.Now, MessageType.Information, "Command sending process is blocked for analyzing listening process.");

                        try
                        {
                            // ProceedIncommingCommand incoming automated warehouse controller command
                            if (TerminalName.Material.Equals(_terminalSetting.Key))
                            {
                                MaterialAutoWarehouseController.ProceedIncommingCommand(terminalMessage,
                                    AutoController.MaterialAutoWarehouseDeviceCode);

                                // Remove command from cache
                                MaterialAutoWarehouseController.RemoveCommand(x => x.IsAck && x.CommandIndex.Equals(terminalMessage.CommandIndex, StringComparison.InvariantCultureIgnoreCase) && x.CommandSequence.Equals(terminalMessage.CommandSequence, StringComparison.InvariantCultureIgnoreCase));
                            }
                            else if (TerminalName.PreProduct.Equals(_terminalSetting.Key))
                            {
                                if ("0100".Equals(terminalMessage.Command) || "0101".Equals(terminalMessage.Command))
                                {
                                    PreProductAutoWarehouseController.ProceedIncommingCommand(terminalMessage,
                                    AutoController.PreProductAutoWarehouseDeviceCode);
                                }

                                // Remove command from cache
                                PreProductAutoWarehouseController.RemoveCommand(x => x.IsAck && x.CommandIndex.Equals(terminalMessage.CommandIndex, StringComparison.InvariantCultureIgnoreCase) && x.CommandSequence.Equals(terminalMessage.CommandSequence, StringComparison.InvariantCultureIgnoreCase));
                            }
                            else if (TerminalName.Product.Equals(_terminalSetting.Key))
                            {
                                ProductAutoWarehouseController.ProceedIncommingCommand(terminalMessage,
                                    AutoController.ProductAutoWarehouseDeviceCode);

                                // Remove command from cache
                                ProductAutoWarehouseController.RemoveCommand(x => x.IsAck && x.CommandIndex.Equals(terminalMessage.CommandIndex, StringComparison.InvariantCultureIgnoreCase) && x.CommandSequence.Equals(terminalMessage.CommandSequence, StringComparison.InvariantCultureIgnoreCase));
                            }

                            Message.InitiateMessage(DateTime.Now, MessageType.Information,
                                $"Command: {content} has been proceeded. Restart listener in {Setting.CommandScanTaskInterval} millisecs.");
                            Thread.Sleep(Setting.CommandScanTaskInterval);
                        }
                        catch (Exception exception)
                        {
                            Log.Error(exception.Message, exception);
                            Message.InitiateMessage(DateTime.Now, MessageType.Error,
                                $"Command: {content} hasn't been proceeded. Restart listener in {Setting.CommandScanTaskInterval} millisecs.");
                            Thread.Sleep(Setting.CommandScanTaskInterval);
                        }
                    }
                }
                catch (Exception exception)
                {
                    Log.Error(exception.Message, exception);
                }

                Message.InitiateMessage(DateTime.Now, MessageType.Information, "Command sending process is unblocked.");
            }
        }

        /// <summary>
        ///     Task which is used for scanning pending commands and broadcast to external terminals.
        /// </summary>
        private void SearchWarehousePendingCommands()
        {
            while (true)
            {
                // Task is not allowed to run.
                if (!_isTaskRunning)
                {
                    Thread.Sleep(Setting.CommandScanTaskInterval);
                    continue;
                }

                // Wait for signal from another thread to start this one.
                _manualResetScannerEvent.WaitOne();

                try
                {
                    // Message will be built base on the being activated automated warehouse controller.
                    var message = "";

                    #region Message construction

                    // Operating warehouse controller is material.
                    if (TerminalName.Material.Equals(_terminalSetting.Key))
                    {
                        #region Automated warehouse status validation.

                        // No material warehouse controller has been configured in application setting file.
                        if (string.IsNullOrEmpty(AutoController.MaterialAutoWarehouseDeviceCode))
                        {
                            Message.InitiateMessage(DateTime.Now, MessageType.Information,
                                $"No material warehouse controller device code has been configured into application setting file. Restart in {Setting.CommandScanTaskInterval} milliseconds.");

                            // Sleep the thread for 3 secs.
                            Thread.Sleep(Setting.CommandScanTaskInterval);
                            continue;
                        }

                        // Material auto warehouse controller is offline.
                        if (!MaterialAutoWarehouseController.IsAutoWarehouseOnline(AutoController.MaterialAutoWarehouseDeviceCode))
                        {
                            Message.InitiateMessage(DateTime.Now, MessageType.Information,
                                $"Material automated warehouse is offline. Restart in {Setting.CommandScanTaskInterval} milliseconds.");

                            // Sleep the thread for awhile.
                            Thread.Sleep(Setting.CommandScanTaskInterval);
                            continue;
                        }

                        #endregion

                        #region Message initialization

                        try
                        {
                            // Find pending material command in database.
                            var commands = AutoController.FindPendingMaterialCommands();

                            // Command is not valid.
                            if (commands == null || commands.Count < 1)
                            {
                                //Message.InitiateMessage(DateTime.Now, MessageType.Information,
                                //$"No pending command has been found. Broadcaster will be restarted in {Setting.CommandScanTaskInterval} milliseconds.");

                                Thread.Sleep(Setting.CommandScanTaskInterval);
                                continue;
                            }

                            // ProceedIncommingCommand database records.
                            MaterialAutoWarehouseController.ProcessDataBase(commands, AutoController.MaterialAutoWarehouseDeviceCode, _terminalSetting.Value.Outgoing);

                            Thread.Sleep(Setting.CommandScanTaskInterval);
                            continue;
                        }
                        catch (Exception exception)
                        {
                            Log.Error(exception.Message, exception);
                            Message.InitiateMessage(DateTime.Now, MessageType.Error, exception.Message);

                            Thread.Sleep(Setting.CommandScanTaskInterval);
                            continue;
                        }

                        #endregion
                    }
                    else if (TerminalName.PreProduct.Equals(_terminalSetting.Key))
                    {
                        using (var context = new KCSGDbContext())
                        using (var unitOfWork = new UnitOfWork(context))
                        { 
                            // delete off empty command
                            var preProduct = unitOfWork.PreProductWarehouseCommandRepository.GetMany(
                                i => i.F50_From.Trim().Equals(""))
                            .FirstOrDefault();

                            if (preProduct != null)
                            {
                                unitOfWork.PreProductWarehouseCommandRepository.Delete(preProduct);
                                unitOfWork.Commit();
                            }
                        }

                        #region Automated warehouse status validation.

                        // No material warehouse controller has been configured in application setting file.
                        if (string.IsNullOrEmpty(AutoController.PreProductAutoWarehouseDeviceCode))
                        {
                            Message.InitiateMessage(DateTime.Now, MessageType.Information,
                                $"No pre-product warehouse controller device code has been configured into application setting file. Restart in {Setting.CommandScanTaskInterval} milliseconds.");

                            // Sleep the thread for 3 secs.
                            Thread.Sleep(3000);
                            continue;
                        }

                        // Material auto warehouse controller is offline.
                        if (!PreProductAutoWarehouseController.IsAutoWarehouseOnline(AutoController.PreProductAutoWarehouseDeviceCode))
                        {
                            Message.InitiateMessage(DateTime.Now, MessageType.Information,
                                $"Pre-product automated warehouse is offline. Restart in {Setting.CommandScanTaskInterval} milliseconds.");

                            // Sleep the thread for awhile.
                            Thread.Sleep(Setting.CommandScanTaskInterval);
                            continue;
                        }

                        #endregion

                        #region Pre-product initialization

                        try
                        {
                            var commands = AutoController.FindPendingPreProductCommands();
                            if (commands == null || commands.Count < 1)
                            {
                                //Message.InitiateMessage(DateTime.Now, MessageType.Information,
                                //$"No pending command has been found. Listener will be restarted in {Setting.CommandScanTaskInterval} milliseconds.");

                                Thread.Sleep(Setting.CommandScanTaskInterval);
                                continue;
                            }

                            // ProceedIncommingCommand messages list.
                            PreProductAutoWarehouseController.ProceedMessages(commands,
                                AutoController.PreProductAutoWarehouseDeviceCode, _terminalSetting.Value.Outgoing);

                            Thread.Sleep(Setting.CommandScanTaskInterval);
                        }
                        catch (Exception exception)
                        {
                            Log.Error(exception.Message, exception);
                            Message.InitiateMessage(DateTime.Now, MessageType.Error, exception.Message);
                        }

                        #endregion
                    }
                    else if (TerminalName.Product.Equals(_terminalSetting.Key))
                    {
                        #region Automated warehouse status validation.

                        // No material warehouse controller has been configured in application setting file.
                        if (string.IsNullOrEmpty(AutoController.ProductAutoWarehouseDeviceCode))
                        {
                            Message.InitiateMessage(DateTime.Now, MessageType.Information,
                                $"No product warehouse controller device code has been configured into application setting file. Restart in {Setting.CommandScanTaskInterval} milliseconds.");

                            // Sleep the thread for 3 secs.
                            Thread.Sleep(3000);
                            continue;
                        }

                        // Material auto warehouse controller is offline.
                        if (!ProductAutoWarehouseController.IsAutoWarehouseOnline(AutoController.ProductAutoWarehouseDeviceCode))
                        {
                            Message.InitiateMessage(DateTime.Now, MessageType.Information,
                                $"Product automated warehouse is offline. Restart in {Setting.CommandScanTaskInterval} milliseconds.");

                            // Sleep the thread for awhile.
                            Thread.Sleep(Setting.CommandScanTaskInterval);
                            continue;
                        }

                        #endregion

                        #region Products initialization

                        try
                        {
                            // TODO: Disabled it at 2017-05-17 (Consider enable it back)
                            //var commands = AutoController.FindPendingProductCommands();
                            //if (commands == null || commands.Count < 1)
                            //{
                            //    //Message.InitiateMessage(DateTime.Now, MessageType.Information,
                            //    //$"No pending command has been found. Listener will be restarted in {Setting.CommandScanTaskInterval} milliseconds.");
                            //    Thread.Sleep(Setting.CommandScanTaskInterval);
                            //    continue;
                            //}
                            //ProductAutoWarehouseController.ProcessDataList(commands,
                            //    AutoController.ProductAutoWarehouseDeviceCode, _terminalSetting.Value.Outgoing);
                            //Thread.Sleep(Setting.CommandScanTaskInterval);

                            var command = AutoController.FindPendingProductCommands().FirstOrDefault();
                            if (command == null)
                            {
                                Thread.Sleep(Setting.CommandScanTaskInterval);
                                continue;
                            }

                            ProductAutoWarehouseController.ProcessDataList(new[] { command },
                                AutoController.ProductAutoWarehouseDeviceCode, _terminalSetting.Value.Outgoing);
                            Thread.Sleep(Setting.CommandScanTaskInterval);
                        }
                        catch (Exception exception)
                        {
                            Log.Error(exception.Message, exception);
                            Message.InitiateMessage(DateTime.Now, MessageType.Error, exception.Message);
                        }

                        #endregion
                    }
                    else
                    {
                        Message.InitiateMessage(DateTime.Now, MessageType.Error,
                            $"Material, Pre-Product, Product should be selected to broadcast message. Restart message broadcaster in {Setting.CommandScanTaskInterval} milliseconds.");
                        Thread.Sleep(Setting.CommandScanTaskInterval);
                        continue;
                    }

                    #endregion
                }
                catch (EntityException entityException)
                {
                    Log.Error(entityException.Message, entityException);
                    Message.ShowMessageBox(View.FindView(),
                        "There is something wrong with database. Please check database configuration in configuration file.",
                        "System Message", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);

                    Application.Current.Dispatcher.Invoke(() => { IsTaskRunning = false; });
                }
                catch (Exception exception)
                {
                    Log.Error(exception.Message, exception);
                }
            }
        }

        /// <summary>
        /// Task which is used for sending status request to external terminal.
        /// </summary>
        private void SendingStatusRequestCommands()
        {
            while (true)
            {
                // Task is not allowed to run.
                if (!_isTaskRunning)
                {
                    Thread.Sleep(Setting.CommandScanTaskInterval);
                    continue;
                }

                // Wait for status request.
                _manualResetStatusRequest.WaitOne();

                var terminalMessage = new TerminalMessage();

                try
                {
                    // Maximum command sequence number.
                    var maximumCommandSeq = 1;

                    using (var context = new KCSGDbContext())
                    using (var unitOfWork = new UnitOfWork(context))
                    {
                        // Find no manage in database.
                        var noManage = unitOfWork.NoManageRepository.GetAll().FirstOrDefault();
                        if (noManage == null)
                        {
                            Thread.Sleep(Setting.CommandScanTaskInterval);
                            continue;
                        }

                        if (TerminalName.Material.Equals(_terminalSetting.Key))
                        {
                            maximumCommandSeq = noManage.F48_MtrWhsCmdNo + 1;
                            if (maximumCommandSeq > 9999)
                            {
                                noManage.F48_MtrWhsCmdNo = 0;
                                maximumCommandSeq = 0;
                            }

                            noManage.F48_MtrWhsCmdNo = maximumCommandSeq;
                            
                        }
                        else if (TerminalName.PreProduct.Equals(_terminalSetting.Key))
                        {
                            // delete off empty command
                            var preProduct = unitOfWork.PreProductWarehouseCommandRepository.GetMany(
                                    i => i.F50_From.Trim().Equals(""))
                                .FirstOrDefault();

                            if (preProduct != null)
                            {
                                unitOfWork.PreProductWarehouseCommandRepository.Delete(preProduct);
                                unitOfWork.Commit();
                            }

                            maximumCommandSeq = noManage.F48_PrePdtWhsCmdNo + 1;
                            if (maximumCommandSeq > 9999)
                            {
                                noManage.F48_PrePdtWhsCmdNo = 0;
                                maximumCommandSeq = 0;
                            }
                            noManage.F48_PrePdtWhsCmdNo = maximumCommandSeq;
                        }
                        else if (TerminalName.Product.Equals(_terminalSetting.Key))
                        {
                            maximumCommandSeq = noManage.F48_PdtWhsCmdNo + 1;
                            if (maximumCommandSeq > 9999)
                            {
                                noManage.F48_PdtWhsCmdNo = 0;
                                maximumCommandSeq = 0;
                            }
                            noManage.F48_PdtWhsCmdNo = maximumCommandSeq;
                        }
                        
                        // Update no manage first.
                        unitOfWork.NoManageRepository.Update(noManage);
                        unitOfWork.Commit();
                    }

                    terminalMessage.CommandIndex = "5000";
                    terminalMessage.CommandSequence = maximumCommandSeq.ToString("D4");

                    using (var tcpClient = new TcpClient())
                    {
                        var endpoint = new IPEndPoint(IPAddress.Parse(_terminalSetting.Value.Outgoing.Address), _terminalSetting.Value.Outgoing.Port);
                        tcpClient.Connect(endpoint);
                        //using (var stream = tcpClient.GetStream())
                        //using (var streamWriter = new StreamWriter(stream, new UTF8Encoding()))
                        //{

                        //    streamWriter.AutoFlush = true;
                        //    streamWriter.WriteLine(terminalMessage);
                        //}

                        using (var stream = tcpClient.GetStream())
                        {
                            var bytes = Encoding.UTF8.GetBytes(terminalMessage.ToString());
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                       
                    }
                    
                    Message.InitiateMessage(DateTime.Now, MessageType.StatusRequest, "Sent status request", terminalMessage.ToString());
                }
                catch (Exception exception)
                {
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, $"Failed to send status request: {terminalMessage}");
                    Log.Debug(exception.Message, exception);
                }
                finally
                {
                    // 2017-05-15: Disabled status request message notification.
                    //Message.InitiateMessage(DateTime.Now, MessageType.Information, $"Status request will be sent after {CommandStatusTime} milliseconds");
                    Thread.Sleep(CommandStatusTime);
                }
            }
        }
        /// <summary>
        /// Shutdown application.
        /// </summary>
        private void ExecuteExitApplication()
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Find messages from external terminal.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private string FindIncommingTerminalMessage(NetworkStream stream)
        {
            // Message from external terminal is 64 characters length.
            var buffer = new byte[64];
            stream.Read(buffer, 0, buffer.Length);
            var content = Encoding.UTF8.GetString(buffer);
            // Flush the stream just in case of overflow.
            //stream.Flush();

            return content;
        }

        #endregion

        #endregion

        #region Properties

        #region Inversion of controls

        /// <summary>
        ///     Instance of application setting.
        /// </summary>
        private SettingViewModel _setting;

        /// <summary>
        ///     Instance of message setting.
        /// </summary>
        private MessageViewModel _message;

        /// <summary>
        ///     Instance of auto controller business.
        /// </summary>
        private AutoControllerViewModel _autoController;

        /// <summary>
        ///     Instance of material auto warehouse controller business.
        /// </summary>
        private MaterialAutoWarehouseController _materialAutoWarehouseController;

        /// <summary>
        ///     Instance of pre-product auto warehouse controller business.
        /// </summary>
        private PreProductAutoWarehouseController _preProductAutoWarehouseController;

        /// <summary>
        ///     Instance of product auto warehouse controller business.
        /// </summary>
        private ProductAutoWarehouseController _productAutoWarehouseController;

        /// <summary>
        ///     Instance of application setting.
        /// </summary>
        public SettingViewModel Setting
        {
            get
            {
                if (_setting == null)
                    _setting = ServiceLocator.Current.GetInstance<SettingViewModel>();

                return _setting;
            }
            set { _setting = value; }
        }

        /// <summary>
        ///     Instance for message handling.
        /// </summary>
        public MessageViewModel Message
        {
            get
            {
                if (_message == null)
                    _message = ServiceLocator.Current.GetInstance<MessageViewModel>();
                return _message;
            }
            set { _message = value; }
        }

        /// <summary>
        ///     Instance for automated controller business.
        /// </summary>
        public AutoControllerViewModel AutoController
        {
            get
            {
                if (_autoController == null)
                    _autoController = SimpleIoc.Default.GetInstance<AutoControllerViewModel>();
                return _autoController;
            }
            set { _autoController = value; }
        }

        /// <summary>
        ///     Instance for handling material auto warehouse business.
        /// </summary>
        public MaterialAutoWarehouseController MaterialAutoWarehouseController
        {
            get
            {
                if (_materialAutoWarehouseController == null)
                    _materialAutoWarehouseController = SimpleIoc.Default.GetInstance<MaterialAutoWarehouseController>();
                return _materialAutoWarehouseController;
            }
            set { _materialAutoWarehouseController = value; }
        }

        /// <summary>
        ///     Instance for handling pre-product auto warehouse business.
        /// </summary>
        public PreProductAutoWarehouseController PreProductAutoWarehouseController
        {
            get
            {
                return _preProductAutoWarehouseController ??
                       (_preProductAutoWarehouseController =
                           SimpleIoc.Default.GetInstance<PreProductAutoWarehouseController>());
            }
            set { _preProductAutoWarehouseController = value; }
        }

        /// <summary>
        ///     Instance for handling product auto warehouse business.
        /// </summary>
        public ProductAutoWarehouseController ProductAutoWarehouseController
        {
            get
            {
                return _productAutoWarehouseController ??
                       (_productAutoWarehouseController =
                           SimpleIoc.Default.GetInstance<ProductAutoWarehouseController>());
            }
            set { _productAutoWarehouseController = value; }
        }

        #endregion

        /// <summary>
        ///     Whether
        /// </summary>
        private bool _isTaskRunning;

        /// <summary>
        ///     Instance which handles logging business.
        /// </summary>
        private ILog _log;

        /// <summary>
        ///     Terminal configuration.
        /// </summary>
        private KeyValuePair<string, TerminalSetting> _terminalSetting;

        /// <summary>
        ///     Listener of incoming messages.
        /// </summary>
        private TcpListener _tcpListener;

        /// <summary>
        ///     Check whether background task is running or not.
        /// </summary>
        public bool IsTaskRunning
        {
            get { return _isTaskRunning; }
            set { Set(nameof(IsTaskRunning), ref _isTaskRunning, value); }
        }

        /// <summary>
        ///     Terminal information which is running on current thread.
        /// </summary>
        public KeyValuePair<string, TerminalSetting> TerminalSetting
        {
            get { return _terminalSetting; }
            set { Set(nameof(TerminalSetting), ref _terminalSetting, value); }
        }

        /// <summary>
        ///     Instance which is for handling logging business.
        /// </summary>
        public ILog Log
        {
            get { return _log ?? (_log = LogManager.GetLogger(typeof(MainViewModel))); }
            set { _log = value; }
        }

        /// <summary>
        ///     Command which is fired when onload is fired.
        /// </summary>
        public RelayCommand<Window> RelayCommandOnLoad { get; set; }

        /// <summary>
        ///     Command which triggered when start/stop button is clicked.
        /// </summary>
        public RelayCommand<Window> ClickToggleRelayCommand { get; private set; }

        /// <summary>
        /// Command which triggered to exit application.
        /// </summary>
        public RelayCommand ExitApplication { get; set; }

        /// <summary>
        ///     Event which is for signaling thread to start/stop
        /// </summary>
        private readonly ManualResetEvent _manualResetListenerEvent;

        /// <summary>
        ///     Event which is for signaling thread to start/stop
        /// </summary>
        private readonly ManualResetEvent _manualResetScannerEvent;

        private readonly ManualResetEvent _manualResetStatusRequest;


        /// <summary>
        ///     Thread which is for listening messages from external system.
        /// </summary>
        private readonly Thread _listenMessagesThread;

        /// <summary>
        ///     Thread which is for searching messages in database.
        /// </summary>
        private readonly Thread _searchPendingCommandsThread;

        /// <summary>
        /// Thread which is for sending status request to external terminal.
        /// </summary>
        private readonly Thread _StatusRequestThread;

        #endregion
    }
}