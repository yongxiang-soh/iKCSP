namespace KCSG.AutomatedWarehouse.Model
{
    public class TerminalSetting
    {
        #region Properties

        /// <summary>
        /// Device code of terminal.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Which port should be opened for listening to incoming connection.
        /// </summary>
        public ConnectionSetting Incoming { get; set; }

        /// <summary>
        /// Which address should be used for broadcasting outgoing connection.
        /// </summary>
        public ConnectionSetting Outgoing { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initiate terminal setting with incoming, outgoing information.
        /// </summary>
        public TerminalSetting()
        {
            Incoming = new ConnectionSetting();
            Outgoing = new ConnectionSetting();
        }

        #endregion
    }
}