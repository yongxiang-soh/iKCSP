using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.Services;
using KCSG.Web.Hubs;
using log4net;
using Microsoft.AspNet.SignalR;

namespace KCSG.Web.Services
{
    public class NotificationService : INotificationService
    {
        #region Properties

        /// <summary>
        /// Hub which is used for broadcasting notification.
        /// </summary>
        public IHubContext NotificationHubContext { get; set; }

        /// <summary>
        /// List of communication hubs.
        /// </summary>
        public IDictionary<Core.Enumerations.CommunicationHubs, IHubContext> CommunicationHubs { get; set; }

        /// <summary>
        /// Unit of work which provides access to repositories.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Configuration service handles data stored in configuration file.
        /// </summary>
        private readonly IConfigurationService _configurationService;

        /// <summary>
        /// Instance which is for logging.
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// Automated warehouse material screen name.
        /// </summary>
        public string AutomatedWarehouseMaterialScreenName
        {
            get { return "aw-material"; }
        }

        /// <summary>
        /// Automated warehouse product screen name.
        /// </summary>
        public string AutomatedWarehouseProductScreenName
        {
            get { return "aw-product"; }
        }

        /// <summary>
        /// Automated warehouse pre-product screen name.
        /// </summary>
        public string AutomatedWarehousePreProductScreenName
        {
            get { return "aw-pre-product"; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initiate service and default settings.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="log"></param>
        /// <param name="configurationService"></param>
        public NotificationService(IUnitOfWork unitOfWork, ILog log, IConfigurationService configurationService)
        {
            // Find hub context in dependency injection.
            NotificationHubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();

            // Communication hub initialization.
            CommunicationHubs = new Dictionary<Core.Enumerations.CommunicationHubs, IHubContext>();
            CommunicationHubs.Add(Core.Enumerations.CommunicationHubs.C1, GlobalHost.ConnectionManager.GetHubContext<C1Hub>());
            CommunicationHubs.Add(Core.Enumerations.CommunicationHubs.C2, GlobalHost.ConnectionManager.GetHubContext<C2Hub>());
            CommunicationHubs.Add(Core.Enumerations.CommunicationHubs.C3, GlobalHost.ConnectionManager.GetHubContext<C3Hub>());
            CommunicationHubs.Add(Core.Enumerations.CommunicationHubs.C4, GlobalHost.ConnectionManager.GetHubContext<C4Hub>());
            CommunicationHubs.Add(Core.Enumerations.CommunicationHubs.Note, GlobalHost.ConnectionManager.GetHubContext<NoteHub>());

            _unitOfWork = unitOfWork;
            _log = log;
            _configurationService = configurationService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Broadcast message from hub to all client with extra information.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="ipAddress"></param>
        /// <param name="message"></param>
        public void BroadcastNotificationMessage(string terminalNo, string ipAddress, string message)
        {
            NotificationHubContext.Clients.All.receiveSystemNotification(terminalNo, ipAddress, message);
        }


        /// <summary>
        /// Send message from domain to C1
        /// </summary>
        /// <param name="terminals"></param>
        /// <param name="message"></param>
        public void SendMessageFromC1(IList<string> terminals, object message)
        {
#if !UNAUTHORIZED_DEBUG
            var connectionIndexes = FindConnectionIndexes(terminals);
            CommunicationHubs[Core.Enumerations.CommunicationHubs.C1].Clients.Clients(connectionIndexes).receiveMessageFromC1(message);
    #if REALTIME_CONNECTION_LOG
            foreach (var terminal in terminals)
                _log.Info(string.Format("Send message to terminal {0}", terminal));
    #endif
#else
            CommunicationHubs[Core.Enumerations.CommunicationHubs.C1].Clients.All.receiveMessageFromC1(message);
#endif

        }

        /// <summary>
        /// Send message from domain to C2
        /// </summary>
        /// <param name="terminals"></param>
        /// <param name="message"></param>
        public void SendMessageFromC2(IList<string> terminals, object message)
        {
#if !UNAUTHORIZED_DEBUG
            var connectionIndexes = FindConnectionIndexes(terminals);
            CommunicationHubs[Core.Enumerations.CommunicationHubs.C2].Clients.Clients(connectionIndexes).receiveMessageFromC2(message);

    #if REALTIME_CONNECTION_LOG
                foreach (var terminal in terminals)
                    _log.Info(string.Format("Send message to terminal {0}", terminal));
    #endif

#else
            CommunicationHubs[Core.Enumerations.CommunicationHubs.C2].Clients.All.receiveMessageFromC2(message);
#endif

        }

        /// <summary>
        /// Send message from domain to C3
        /// </summary>
        /// <param name="terminals">Terminals which should be received message</param>
        /// <param name="message"></param>
        public void SendMessageFromC3(IList<string> terminals, object message)
        {
#if !UNAUTHORIZED_DEBUG
            var connectionIndexes = FindConnectionIndexes(terminals);
            CommunicationHubs[Core.Enumerations.CommunicationHubs.C3].Clients.Clients(connectionIndexes).receiveMessageFromC3(message);

    #if REALTIME_CONNECTION_LOG
                foreach (var terminal in terminals)
                    _log.Info(string.Format("Send message to terminal {0}", terminal));
    #endif

#else
            CommunicationHubs[Core.Enumerations.CommunicationHubs.C3].Clients.All.receiveMessageFromC3(message);
#endif
        }

        /// <summary>
        /// Send log information to client.
        /// </summary>
        /// <param name="terminals"></param>
        /// <param name="screenName"></param>
        /// <param name="message"></param>
        public void SendNoteInformation(IList<string> terminals, string screenName, string message)
        {
            // Reformat the message with date time.
            message = string.Format("{0} - {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), message);
#if !UNAUTHORIZED_DEBUG

            if (terminals != null)
            {
                var connectionIndexes = FindConnectionIndexes(terminals);
                CommunicationHubs[Core.Enumerations.CommunicationHubs.Note].Clients.Clients(connectionIndexes)
                    .receiveNoteInformation(screenName, message);
            }
            else
                CommunicationHubs[Core.Enumerations.CommunicationHubs.Note].Clients.All.receiveNoteInformation(screenName, message);
#else
            CommunicationHubs[Core.Enumerations.CommunicationHubs.Note].Clients.All.receiveNoteInformation(screenName, message);
#endif
        }

        /// <summary>
        /// Send message from domain to C4
        /// </summary>
        /// <param name="terminals"></param>
        /// <param name="message"></param>
        public void SendMessageFromC4(IList<string> terminals, object message)
        {
            var connectionIndexes = FindConnectionIndexes(terminals);
            CommunicationHubs[Core.Enumerations.CommunicationHubs.C4].Clients.Clients(connectionIndexes).receiveMessageFromC4(message);
        }

        /// <summary>
        /// Send message from domain to C1
        /// </summary>
        /// <param name="message"></param>
        public void SendMessageToC1(object message)
        {
            NotificationHubContext.Clients.All.receiveMessageSentToC1(message);
        }

        /// <summary>
        /// Send message from domain to C2
        /// </summary>
        /// <param name="commandNo"></param>
        /// <param name="asCommandNo"></param>
        public void SendMessageToC2(string commandNo, string asCommandNo)
        {
            NotificationHubContext.Clients.All.receiveMessageSentToC2(commandNo, asCommandNo);
        }

        /// <summary>
        /// Send message from domain to C3.
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="message"></param>
        public void SendMessageToC3(string screenName, string message)
        {
#if !UNAUTHORIZED_DEBUG
            // Find connection index of terminal which is marked as C3.
            var connectionIndexes = FindConnectionIndexes(new List<string>() { Constants.TerminalNo.ThirdCommunication });
            NotificationHubContext.Clients.Clients(connectionIndexes).receiveMessageSentToC3(screenName, message);
#endif
        }

        /// <summary>
        /// Send message from domain to C4.
        /// </summary>
        /// <param name="message"></param>
        public void SendMessageToC4(object message)
        {
            NotificationHubContext.Clients.All.receiveMessageSentToC4(message);
        }

        /// <summary>
        /// Find connection indexes 
        /// </summary>
        /// <param name="terminals"></param>
        /// <returns></returns>
        private IList<string> FindConnectionIndexes(IList<string> terminals)
        {
            // Find all realtime connections.
            var realtimeConnections = _unitOfWork.RealtimeConnectionRepository.GetAll();
            realtimeConnections = realtimeConnections.Where(x => terminals.Contains(x.TerminalNo));

            // Find connections.
            return realtimeConnections.Select(x => x.Index).ToList();
        }


        /// <summary>
        /// Format message which will be sent to C3.
        /// </summary>
        /// <param name="productWarehouseCommand"></param>
        /// <returns></returns>
        public string FormatThirdCommunicationMessageResponse(TX47_PdtWhsCmd productWarehouseCommand)
        {
            if (productWarehouseCommand == null)
                return "";
            return string.Format("0002 {0} {1} 0066 {2} 1001 {3} {4} {5} {6} {7}",
                productWarehouseCommand.F47_TerminalNo, productWarehouseCommand.F47_PictureNo,
                productWarehouseCommand.F47_CommandType, productWarehouseCommand.F47_Status,
                productWarehouseCommand.F47_From,
                productWarehouseCommand.F47_To, productWarehouseCommand.F47_PalletNo, DateTime.Now.ToString("dd/MM/yyyy"));
        }

        /// <summary>
        /// From information to render message.
        /// </summary>
        /// <param name="ai_inout"></param>
        /// <param name="as_status"></param>
        /// <param name="as_from"></param>
        /// <param name="as_to"></param>
        /// <param name="ai_stock"></param>
        /// <param name="ach_type"></param>
        /// <param name="as_code"></param>
        /// <param name="as_pltcntn"></param>
        public string FormatSecondNotificationMessage(string ai_inout, string as_status, string as_from, string as_to, string ai_stock, string ach_type, string as_code, string as_pltcntn)
        {
            // Enumeration conversion.
            var TX34_StrRtrType_Mtr = Constants.TX34_StrRtrType.Material.ToString("D");
            var TX47_StrRtrType_Pdt = Constants.F47_StrRtrType.Product.ToString("D");
            var TX47_StrRtrType_ExtPrePdt = Constants.F47_StrRtrType.ExternalPreProduct.ToString("D");
            var TX47_StrRtrType_BadPrePdt = Constants.F47_StrRtrType.OutOfSpecPreProduct.ToString("D");
            var TX50_StrRtrType_PrePdt = Constants.F50_StrRtrType.StrRtrType_PrePdt.ToString("D");

            //	The system will generate the message for Warehouse as below
            var message = "Warehouse: ";

            //	The system will check the value of [ai_stock]:
            //o	If [ai_inout] = “0”: Set the [Message] as follow:
            if (ai_inout.Equals("0"))
                message += "Storage ";
            else if (ai_inout.Equals("1"))
                message += "Retrieval ";
            else if (ai_inout.Equals("2"))
                message += "Move ";

            //	Set the [Message] as follow
            //[Message] + " from "+as_From+" to "+as_To +"~r~n"
            message = string.Format("{0} from {1} to {2} ", message, as_from, as_to);

            //	The system will check the value of [ai_inout]:
            //o	If [ai_inout] = “1” and [ach_type] = “TX50_StrRtrType_PrePdt” : Set the [Message] as follow:
            if (ai_inout.Equals("0") && ach_type.Equals(TX34_StrRtrType_Mtr))
            {
                //[Message] + "Material Code: " + as_code + "~r~n" + "Pallet No: " + as_pltcntn + "~r~n"
                message = string.Format("{0} Material Code: {1} Pallet No: {2}", message, as_code, as_pltcntn);
            }
            else if (ai_inout.Equals("1") && ach_type.Equals(TX50_StrRtrType_PrePdt))
            {
                //[Message] + "Pre-product Code: " + as_code + "~r~n" + "Container Code: " + as_pltcntn + "~r~n"
                message = string.Format("{0} Pre-product Code: {1} Container Code: {2}", message, as_code, as_pltcntn);
            }
            else if (ai_inout.Equals("2"))
            {
                //•	[ach_type] = “TX47_StrRtrType_Pdt” (which is “0” (Pre-product) as pre-configured on “constant.txt file) : Set the [Message] as follow
                if (ach_type.Equals(TX47_StrRtrType_Pdt))
                    message = string.Format("{0} Product Code: {1} Pallet No: {2}", message, as_code, as_pltcntn);
                else if (ach_type.Equals(TX47_StrRtrType_ExtPrePdt))
                    message = string.Format("{0} External Pre-Product Code: {1} Pallet No: {2}", message, as_code,
                        as_pltcntn);
                else if (ach_type.Equals(TX47_StrRtrType_BadPrePdt))
                    message = string.Format("{0} Out-of-sign Pre-product ", message);
            }

            //	The system will check the value of [as_status]:
            //o	If [ai_status] = “tc_cmdsts_6”, set [Message] as follow:
            if (as_status.Equals(Constants.TC_CMDSTS.TC_CMDSTS_6))
                message = string.Format("{0} Status: success.", message);
            else if (as_status.Equals(Constants.TC_CMDSTS.TC_CMDSTS_7))
                message = string.Format("{0} Status: cancel.", message);
            else if (as_status.Equals(Constants.TC_CMDSTS.TC_CMDSTS_8))
                message = string.Format("{0} Status: two times storage.");
            else if (as_status.Equals(Constants.TC_CMDSTS.TC_CMDSTS_9))
                message = string.Format("{0} Status: empty retrieval.");

            return message;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void SendMessageToComPort(string message)
        {
            using (var serial = new SerialPort(_configurationService.CommunicationPortName, _configurationService.CommunicationPortBaudRate, _configurationService.CommunicationPortParity, _configurationService.CommunicationPortDataBits, _configurationService.CommunicationPortStopBits))
            {
                // Open connection to COM Port.
                serial.Open();

                // Log the message.
                _log.Info(string.Format("Send message to {0} with content: {1} ", _configurationService.CommunicationPortName, message));
                serial.WriteLine(message);
                serial.Close();
            }
        }


        public bool SendSocketMessage(IPAddress ipAddress, int port, string message)
        {
            throw new NotImplementedException();
        }

        public int SendFromC1ToAw(TX34_MtrWhsCmd materialWhsCmd)
        {
            throw new NotImplementedException();
        }

        public int SendFromC2ToAw(TX50_PrePdtWhsCmd prePdtWhsCmd)
        {
            return 1;
        }

        public int SendFromC3ToAw(TX47_PdtWhsCmd pdtWhsCmd)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}