using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using GalaSoft.MvvmLight;
using KCSG.AutomatedWarehouse.Enumeration;
using KCSG.AutomatedWarehouse.Model;

namespace KCSG.AutomatedWarehouse.ViewModel
{
    public class MessageViewModel : ViewModelBase
    {
        #region Constructors

        /// <summary>
        ///     Initiate service with default settings.
        /// </summary>
        public MessageViewModel()
        {
            _maxMessage = 1000;
            _systemMessages = new ObservableCollection<SystemMessageBox>();

#if FORGING_MESSAGES
            
            var msgInfo = new SystemMessageBox();
            msgInfo.Type = MessageType.Information;
            msgInfo.Time = DateTime.Now;
            msgInfo.Message = $"Info message";

            var msgError = new SystemMessageBox();
            msgError.Type = MessageType.Error;
            msgError.Time = DateTime.Now;
            msgError.Message = $"Error";

            var msgSuccess = new SystemMessageBox();
            msgSuccess.Type = MessageType.Success;
            msgSuccess.Time = DateTime.Now;
            msgSuccess.Message = $"Success message";

            var msgStatusRequest = new SystemMessageBox();
            msgStatusRequest.Type = MessageType.StatusRequest;
            msgStatusRequest.Time = DateTime.Now;
            msgStatusRequest.Message = $"Status request message";

            var msgReceive = new SystemMessageBox();
            msgReceive.Type = MessageType.Receive;
            msgReceive.Time = DateTime.Now;
            msgReceive.Prefix = "Received";
            msgReceive.Message = $"0xxx";

            var msgSent = new SystemMessageBox();
            msgSent.Type = MessageType.Broadcast;
            msgSent.Time = DateTime.Now;
            msgSent.Prefix = "Sent";
            msgSent.Message = $"0xxx";

            _systemMessages.Add(msgInfo);
            _systemMessages.Add(msgError);
            _systemMessages.Add(msgSuccess);
            _systemMessages.Add(msgStatusRequest);
            _systemMessages.Add(msgReceive);
            _systemMessages.Add(msgSent);
#endif
        }

        #endregion

        #region Properties

        public delegate void MessageHandler(string message);

        public MessageHandler MessageSent;

        /// <summary>
        ///     List of system messages.
        /// </summary>
        private readonly ObservableCollection<SystemMessageBox> _systemMessages;

        /// <summary>
        ///     Maximum number of messages which array of messages can contain.
        /// </summary>
        private int _maxMessage;

        /// <summary>
        ///     List of messages.
        /// </summary>
        public ObservableCollection<SystemMessageBox> SystemMessages
        {
            get { return _systemMessages; }
        }

        /// <summary>
        ///     Maximum number of messages.
        /// </summary>
        public int MaxMessage
        {
            get { return _maxMessage; }
            set { Set(nameof(MaxMessage), ref _maxMessage, value); }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Initiate system message into array.
        /// </summary>
        /// <param name="systemMessage"></param>
        public void InitiateMessage(SystemMessageBox systemMessage)
        {
            if (systemMessage == null)
                return;

            if (systemMessage.Type == MessageType.Error)
                return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                var messageCounter = _systemMessages.Count;
                messageCounter++;
                if (messageCounter > _maxMessage)
                    _systemMessages.RemoveAt(0);

                _systemMessages.Add(systemMessage);
                RaisePropertyChanged(nameof(SystemMessages));
            });

            MessageSent?.Invoke(systemMessage.Message);
        }


        /// <summary>
        ///     Initiate system message into array.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="type"></param>
        /// <param name="content"></param>
        public void InitiateMessage(DateTime time, MessageType type, string content)
        {
            if (type == MessageType.Error)
                return;

            // Initiate system message instance.
            var systemMessage = new SystemMessageBox
            {
                Time = time,
                Type = type,
                Message = content
            };

            InitiateMessage(systemMessage);
        }

        /// <summary>
        ///     Initiate system message into array.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="type"></param>
        /// <param name="prefix"></param>
        /// <param name="content"></param>
        public void InitiateMessage(DateTime time, MessageType type, string prefix, string content)
        {
            if (type == MessageType.Error)
                return;

            // Initiate system message instance.
            var systemMessage = new SystemMessageBox
            {
                Time = time,
                Type = type,
                Prefix = prefix,
                Message = content
            };

            InitiateMessage(systemMessage);
        }

        /// <summary>
        ///     Display messagebox onto screen.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        /// <param name="messageBoxButton"></param>
        /// <param name="messageBoxImage"></param>
        /// <param name="messageBoxResult"></param>
        /// <returns></returns>
        public MessageBoxResult ShowMessageBox(Window owner, string messageBoxText, string caption,
            MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage, MessageBoxResult messageBoxResult)
        {
            var result = messageBoxResult;
            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    result = MessageBox.Show(owner, messageBoxText, caption, messageBoxButton, messageBoxImage,
                        messageBoxResult);
                });
            return result;
        }

        /// <summary>
        /// Clear messages from list.
        /// </summary>
        public void ClearMessages()
        {
            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    SystemMessages.Clear();
                });
        }

        /// <summary>
        ///     Send terminal message to external system.
        /// </summary>
        /// <param name="terminalMessage"></param>
        /// <param name="endPoint"></param>
        public void Send(TerminalMessage terminalMessage, IPEndPoint endPoint)
        {
            using (var tcpClient = new TcpClient())
            {
                tcpClient.Connect(endPoint);
                using (var stream = tcpClient.GetStream())
                using (var streamWriter = new StreamWriter(stream))
                {
                    streamWriter.AutoFlush = true;
                    streamWriter.WriteLine(terminalMessage);
                }
            }
        }

        /// <summary>
        ///     Send terminal message to external system.
        /// </summary>
        /// <param name="terminalMessage"></param>
        /// <param name="outgoing">Outgoing gateway</param>
        public void Send(TerminalMessage terminalMessage, ConnectionSetting outgoing)
        {
            using (var tcpClient = new TcpClient())
            {
                tcpClient.Connect(outgoing.Address, outgoing.Port);
                using (var stream = tcpClient.GetStream())
                using (var streamWriter = new StreamWriter(stream))
                {
                    streamWriter.AutoFlush = true;
                    streamWriter.WriteLine(terminalMessage);
                }
            }
        }

        /// <summary>
        ///     Send terminal message to external system.
        /// </summary>
        /// <param name="terminalMessage"></param>
        /// <param name="outgoing">Outgoing gateway</param>
        public void Send(string terminalMessage, ConnectionSetting outgoing)
        {
            using (var tcpClient = new TcpClient())
            {
                tcpClient.Connect(outgoing.Address, outgoing.Port);
                using (var stream = tcpClient.GetStream())
                using (var streamWriter = new StreamWriter(stream))
                {
                    streamWriter.AutoFlush = true;
                    streamWriter.WriteLine(terminalMessage);
                }
            }
        }

        #endregion
    }
}