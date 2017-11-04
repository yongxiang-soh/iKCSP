using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.ProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;

namespace KCSG.Domain.Domains.ProductManagement
{
    public class InterFloorMovementOfProductDomain : BaseDomain, IInterFloorMovementOfProductDomain
    {
        /// <summary>
        ///     Notification service which handles messages send/receive between servers.
        /// </summary>
        private readonly INotificationService _notificationService;

        public InterFloorMovementOfProductDomain(IUnitOfWork unitOfWork, 
            INotificationService notificationService,
            IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        ///     Check Conveyor status
        ///     refer BR 68 - srs product management sub system v1.1
        /// </summary>
        /// <returns></returns>
        public bool CheckConveyorStatus()
        {
            var terminalNoA004 = Constants.TerminalNo.A004;
            var terminalNoA019 = Constants.TerminalNo.A019;
            var status = Constants.F05_StrRtrSts.Error.ToString("D");
            var conveyor =
                _unitOfWork.ConveyorRepository.GetMany(
                    i =>
                        i.F05_TerminalNo.Trim().Equals(terminalNoA004) ||
                        i.F05_TerminalNo.Trim().Equals(terminalNoA019));
            var conveyorStatus = conveyor.Where(i => i.F05_StrRtrSts.Equals(status));
            if (!conveyor.Any() || conveyorStatus.Any())
                return false;
            return true;
        }

        /// <summary>
        ///     Check [f14_devicestatus] and [f14_usepermitclass] from TM14_DEVICE
        ///     refer BR 68 - srs product management sub system v1.1
        /// </summary>
        /// <returns></returns>
        public bool CheckDeviceStatus(string deviceCode)
        {
            return CheckStatusOfDeviceRecord(deviceCode);
        }

        public bool CheckConveyorCode(int from)
        {
            var terminalNo = Constants.TerminalNo.A019;
            if (from == 1)
                terminalNo = Constants.TerminalNo.A004;

            var conveyors = _unitOfWork.ConveyorRepository.GetMany(i => i.F05_TerminalNo.Trim().Equals(terminalNo));
            return conveyors.Any();
        }

        public void TranferInterFloor(int from, string terminalNo)
        {
            //Insert Or Update tx48_nomanage
            var isNoManage = true;
            var sequenceNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNoManage,
                Constants.GetColumnInNoManager.Pdtwhscmdno, 0, 0, 0, 0, 1).ToString("D4");

            var f47From = GetConveyorCode(terminalNo);
            var toTerminalNo = Constants.TerminalNo.A019;
            
            if (from == 1)
            {
               toTerminalNo = Constants.TerminalNo.A004;
            }
            var f47To = GetConveyorCode(toTerminalNo);

            //o	Insert data to TX47_PDTWHSCMD 

            var item = _unitOfWork.ProductWarehouseCommandRepository.InsertProductWarehouseCommand(
                Constants.F47_CommandNo.Move.ToString("D"), sequenceNo,
                Constants.CommandType.CmdType_0,
                Constants.F47_StrRtrType.Product.ToString("D"), Constants.F47_Status.AnInstruction.ToString("D"), null,
                f47From, f47To, terminalNo, Constants.PictureNo.TCPR081F);
            _unitOfWork.Commit();

            // Broadcast notification message to C3.
            _notificationService.SendMessageToC3("TCPR081F",
                _notificationService.FormatThirdCommunicationMessageResponse(item));
        }

        /// <summary>
        ///     Respond message sent back from C3.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        public List<ThirdCommunicationResponse> Reply(string terminalNo)
        {
            var items = new List<ThirdCommunicationResponse>();

            var lstStatus = new List<string> {"6", "7"};
            var lstTx47 =
                _unitOfWork.ProductWarehouseCommandRepository.GetByTerminalNoAndPictureNoAndStatus(terminalNo,
                    Constants.PictureNo.TCPR081F, lstStatus);
            
            foreach (var tx47 in lstTx47)
            {
                if (string.IsNullOrEmpty(tx47.F47_Status) || tx47.F47_Status.Length != 1)
                    continue;
                
                var newStatus = "";
                var thirdCommunicationResponseItem = Mapper.Map<ThirdCommunicationResponse>(tx47);
                thirdCommunicationResponseItem.ProductCode = "";
                if (string.IsNullOrEmpty(thirdCommunicationResponseItem.F47_PalletNo))
                    thirdCommunicationResponseItem.F47_PalletNo = "";
                thirdCommunicationResponseItem.OldStatus = tx47.F47_Status;

                switch (tx47.F47_Status[0])
                {
                    case '6': //Command End
                        newStatus = "C";
                        break;
                    case '7': //Command Cancel
                        newStatus = "D";
                        break;
                    default:
                        continue;
                }

                tx47.F47_Status = newStatus;
                tx47.F47_UpdateDate = DateTime.Now;
                items.Add(thirdCommunicationResponseItem);
            }

            _unitOfWork.Commit();

            return items;
        }
    }
}