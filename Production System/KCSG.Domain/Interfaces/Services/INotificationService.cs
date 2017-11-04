using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Interfaces.Services
{
    public interface INotificationService
    {
        #region Methods

        /// <summary>
        /// Automated warehouse screen name.
        /// </summary>
        string AutomatedWarehouseMaterialScreenName { get; }

        /// <summary>
        /// Automated warehouse product screen name.
        /// </summary>
        string AutomatedWarehouseProductScreenName { get; }

        /// <summary>
        /// Automated warehouse pre-product screen name.
        /// </summary>
        string AutomatedWarehousePreProductScreenName { get; }

        /// <summary>
        ///     Broadcast a notification message from a hub with extra information.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="ipAddress"></param>
        /// <param name="message"></param>
        void BroadcastNotificationMessage(string terminalNo, string ipAddress, string message);

        /// <summary>
        /// Broadcast message from domain to C1
        /// </summary>
        /// <param name="terminals"></param>
        /// <param name="message"></param>
        void SendMessageFromC1(IList<string> terminals, object message);

        /// <summary>
        /// Broadcast message from domain to C2.
        /// </summary>
        /// <param name="terminals"></param>
        /// <param name="message"></param>
        void SendMessageFromC2(IList<string> terminals, object message);

        /// <summary>
        /// Broadcast message from domain to C3.
        /// </summary>
        /// <param name="terminals"></param>
        /// <param name="message"></param>
        void SendMessageFromC3(IList<string> terminals, object message);

        /// <summary>
        /// Broadcast message from domain to C4.
        /// </summary>
        /// <param name="terminals"></param>
        /// <param name="message"></param>
        void SendMessageFromC4(IList<string> terminals, object message);

        /// <summary>
        /// Broadcast message from domain to C1
        /// </summary>
        /// <param name="message"></param>
        void SendMessageToC1(object message);

        /// <summary>
        /// Broadcast message from domain to C2.
        /// </summary>
        /// <param name="commandNo"></param>
        /// <param name="asCommandNo"></param>
        void SendMessageToC2(string commandNo, string asCommandNo);

        /// <summary>
        /// Broadcast message from domain to C3.
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="message"></param>
        void SendMessageToC3(string screenName, string message);

        /// <summary>
        /// Broadcast message from domain to C4.
        /// </summary>
        /// <param name="message"></param>
        void SendMessageToC4(object message);

        /// <summary>
        /// Obtain information and format the second notification.
        /// </summary>
        /// <param name="ai_inout"></param>
        /// <param name="as_status"></param>
        /// <param name="as_from"></param>
        /// <param name="as_to"></param>
        /// <param name="ai_stock"></param>
        /// <param name="ach_type"></param>
        /// <param name="as_code"></param>
        /// <param name="as_pltcntn"></param>
        /// <returns></returns>
        string FormatSecondNotificationMessage(string ai_inout, string as_status, string as_from, string as_to,
            string ai_stock, string ach_type, string as_code, string as_pltcntn);

        /// <summary>
        /// Format message which will be sent to C3.
        /// </summary>
        /// <param name="productWarehouseCommand"></param>
        /// <returns></returns>
        string FormatThirdCommunicationMessageResponse(TX47_PdtWhsCmd productWarehouseCommand);

        /// <summary>
        /// Send log information to screen.
        /// </summary>
        /// <param name="terminals"></param>
        /// <param name="screenName"></param>
        /// <param name="message"></param>
        void SendNoteInformation(IList<string> terminals, string screenName, string message);
        
        /// <summary>
        /// Send message to COM Port.
        /// </summary>
        /// <param name="message">Message which should be sent to com port, if object is sent, please convert it to JSON data type.</param>
        void SendMessageToComPort(string message);

        /// <summary>
        /// Send message through socket connection.
        /// </summary>
        /// <param name="materialWhsCmd"></param>
        int SendFromC1ToAw(TX34_MtrWhsCmd materialWhsCmd);

        int SendFromC2ToAw(TX50_PrePdtWhsCmd prePdtWhsCmd);

        int SendFromC3ToAw(TX47_PdtWhsCmd pdtWhsCmd);

        #endregion

    }
}