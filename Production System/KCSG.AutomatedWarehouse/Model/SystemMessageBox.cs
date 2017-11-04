using System;
using KCSG.AutomatedWarehouse.Enumeration;

namespace KCSG.AutomatedWarehouse.Model
{
    public class SystemMessageBox
    {
        #region Properties

        /// <summary>
        /// Time when message was broadcasted.
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Type of message.
        /// </summary>
        public MessageType Type { get; set; }
        
        /// <summary>
        /// Prefix of message
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Message of system.
        /// </summary>
        public string Message { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initiate system message box with default information.
        /// </summary>
        public SystemMessageBox() { }

        /// <summary>
        /// Initiate system message box with default informations.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="messageType"></param>
        /// <param name="prefix"></param>
        /// <param name="message"></param>
        public SystemMessageBox(DateTime dateTime, MessageType messageType, string prefix, string message)
        {
            Time = dateTime;
            Type = messageType;
            Prefix = prefix;
            Message = message;
        }

        #endregion
    }
}