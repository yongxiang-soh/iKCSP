using System.Configuration;
using System.IO;
using System.Reflection;
using GalaSoft.MvvmLight;
using KCSG.AutomatedWarehouse.Model;
using Newtonsoft.Json;

namespace KCSG.AutomatedWarehouse.ViewModel
{
    public class SettingViewModel : ViewModelBase
    {
        #region Constructors

        /// <summary>
        ///     Initiate view model and information.
        /// </summary>
        public SettingViewModel()
        {
            TerminalsConfiguration = new TerminalConfiguration();
            MinimumWindowWidth = 640;
            MinimumWindowHeight = 480;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Time between 2 scan progress.
        /// </summary>
        private int _pendingCommandScanInterval;

        /// <summary>
        /// Time when sent command is timed out.
        /// </summary>
        private int _pendingCommandTimeout;

        /// <summary>
        ///     Load terminal settings from file by searching relative path.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public bool LoadTerminalSettingsFromFile(string relativePath)
        {
#if !FORGING_TERMINALS
// Find application full path.
            var applicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!Directory.Exists(applicationPath))
                return false;

            // Find setting file.
            var fullPath = Path.Combine(applicationPath, relativePath);
            if (!File.Exists(fullPath))
                return false;

            var content = File.ReadAllText(fullPath);
            TerminalsConfiguration = JsonConvert.DeserializeObject<TerminalConfiguration>(content);
#else
            var terminalNames = new[] {"Material", "Pre-Product", "Product"};
            TerminalSettings = new Dictionary<string, TerminalSetting>();
            var incomingPort = 5001;
            var outgoingPort = 6001;
            var i = 1;

            foreach (var terminalName in terminalNames)
            {
                var terminalSetting = new TerminalSetting();
                terminalSetting.Incoming = new ConnectionSetting();
                terminalSetting.Incoming.Address = "127.0.0.1";
                terminalSetting.Incoming.Port = incomingPort;

                terminalSetting.Outgoing.Address = $"192.168.2.20{i}";
                terminalSetting.Outgoing.Port = outgoingPort;
                TerminalSettings.Add(terminalName, terminalSetting);

                incomingPort++;
                outgoingPort++;
                i++;
            }
#endif
            return true;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Terminal settings.
        /// </summary>
        public TerminalConfiguration TerminalsConfiguration { get; set; }

        /// <summary>
        ///     Minimum width of window.
        /// </summary>
        public int MinimumWindowWidth { get; set; }

        /// <summary>
        ///     Minimum window height.
        /// </summary>
        public int MinimumWindowHeight { get; set; }

        /// <summary>
        ///     Command scan task interval (in millisecs)
        /// </summary>
        public int CommandScanTaskInterval
        {
            get
            {
                if (_pendingCommandScanInterval < 1)
                {
                    // Find the pending command scanner interval. If no value is set, change back to default value.
                    if (
                        !int.TryParse(ConfigurationManager.AppSettings["PendingCommandScannerInterval"],
                            out _pendingCommandScanInterval))
                        _pendingCommandScanInterval = 10000;
                }
                return _pendingCommandScanInterval;
            }
            set { _pendingCommandScanInterval = value; }
        }

        /// <summary>
        /// Life time of command.
        /// </summary>
        public int CommandTimeOut
        {
            get
            {
                if (_pendingCommandTimeout < 1)
                {
                    // Find the pending command scanner interval. If no value is set, change back to default value.
                    if (
                        !int.TryParse(ConfigurationManager.AppSettings["PendingCommandTimeOut"],
                            out _pendingCommandTimeout))
                        _pendingCommandTimeout = 10000;
                }
                return _pendingCommandTimeout;
            }
            set { _pendingCommandTimeout = value; }
        }
        #endregion
    }
}