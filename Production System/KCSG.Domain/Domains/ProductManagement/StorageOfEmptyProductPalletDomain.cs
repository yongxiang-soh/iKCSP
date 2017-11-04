using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using KCSG.Core.Constants;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.ProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;

namespace KCSG.Domain.Domains.ProductManagement
{
    public class StorageOfEmptyProductPalletDomain : BaseDomain, IStorageOfEmptyProductPalletDomain
    {
        /// <summary>
        /// Notification service handles notifications broadcasted between servers.
        /// </summary>
        private readonly INotificationService _notificationService;

        /// <summary>
        /// Initiate domain with IoC.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="notificationService"></param>
        public StorageOfEmptyProductPalletDomain(IUnitOfWork unitOfWork, INotificationService notificationService, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _notificationService = notificationService;
        }

        #region Methods

        /// <summary>
        /// System checks maximum number of IP Address
        /// Refer br 86 - srs product management v1.1
        /// </summary>
        /// <returns></returns>
        public TM14_Device CheckMaxiumNumberOfIpAddress()
        {
            var device = _unitOfWork.DeviceRepository.Get(i => i.F14_DeviceCode.Trim().Equals("ATW003"));
            return device;
        }

        public bool CheckPalletNumber(int palletLoadNumber)
        {
            var device = CheckMaxiumNumberOfIpAddress();
            if (palletLoadNumber > device.F14_IPAddress1)
                return false;
            return true;
        }


        public void StoreTheEmptyPallet(int palletLoadNumber, string terminalNo)
        {
            //	System will get empty pallet and update Product Shelf Status table
            var shelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_Epy;
            var shelfType = Constants.F51_ShelfType.Normal.ToString("D");
            var productShelfStatuses =
                _unitOfWork.ProductShelfStatusRepository.GetMany(
                    i =>
                        i.F51_ShelfStatus.Equals(shelfStatus) && i.F51_ShelfType.Equals(shelfType) &&
                        i.F51_CmdShfAgnOrd.HasValue).OrderByDescending(i => i.F51_CmdShfAgnOrd);
            //If no record found, means no empty pallet available in the warehouse, then system shows the message MSG 19,
            if (!productShelfStatuses.Any())
                throw new Exception("MSG19");
            var shelfNo = "";
            var from = GetConveyorCode(terminalNo);
            //o	Update TX51_PDTSHFSTS
            foreach (var productShelfStatus in productShelfStatuses)
            {
                productShelfStatus.F51_ShelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_RsvStr;
                productShelfStatus.F51_LoadAmount = palletLoadNumber;
                productShelfStatus.F51_TerminalNo = terminalNo;
                productShelfStatus.F51_StorageDate = DateTime.Now;
                productShelfStatus.F51_UpdateDate = DateTime.Now;

                _unitOfWork.ProductShelfStatusRepository.Update(productShelfStatus);

                shelfNo = productShelfStatus.F51_ShelfRow + productShelfStatus.F51_ShelfBay +
                              productShelfStatus.F51_ShelfLevel;
                break;
            }

            //	System will make out Storage command
            //Insert Or Update tx48_nomanage
            var isNoManage = true;
            var sequenceNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNoManage,
                Constants.GetColumnInNoManager.Pdtwhscmdno, 0, 0, 0, 0, 1).ToString("D4");

            //o	Insert data to TX47_PDTWHSCMD
            var item =
                _unitOfWork.ProductWarehouseCommandRepository.InsertProductWarehouseCommand(
                    Constants.F47_CommandNo.Storage.ToString("D"), sequenceNo,
                    Constants.CommandType.CmdType_0, Constants.F47_StrRtrType.PalletForWarehouse.ToString("D"),
                    Constants.F47_Status.AnInstruction.ToString("D"), "", from, shelfNo, terminalNo,
                    Constants.PictureNo.TCPR121F);

            _notificationService.SendMessageToC3("TCPR121F",
            _notificationService.FormatThirdCommunicationMessageResponse(item));

            _unitOfWork.ProductWarehouseCommandRepository.Add(item);

            _unitOfWork.Commit();

        }

        /// <summary>
        /// Respond reply from C3.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        public List<ThirdCommunicationResponse> RespondingReplyFromC3(string terminalNo)
        {
            var lstStatus = new List<string>() {"6", "7", "8"};
            var lstTx47 =
                _unitOfWork.ProductWarehouseCommandRepository.GetByTerminalNoAndPictureNoAndStatus(terminalNo,
                    Constants.PictureNo.TCPR121F, lstStatus);

            var items = new List<ThirdCommunicationResponse>();

            int li_loadamount = 0;
            foreach (var tx47 in lstTx47)
            {
                var newStatus = "";
                
                switch (tx47.F47_Status[0])
                {
                    case '6': //Command End

                        newStatus = "C";

                        break;
                    case '7': //Command Cancel
                        newStatus = "D";

                        break;
                    case '8':
                        if (!Getloadamount(tx47.F47_To, ref li_loadamount)) continue;
                        var shelfNo = tx47.F47_To;
                        if (!AssignSpaceShelf(ref shelfNo, li_loadamount, terminalNo))
                        {
                            newStatus = "B";
                        }
                        else
                        {
                            if (InsertCommand(shelfNo, terminalNo))
                            {
                                continue;
                            }
                            // f_tcsendmsgtoc3(TX47_CmdNo_TwoTimes, is_serialno)    
                            newStatus = "E";
                        }
                        break;

                    case '9': //Command Error
                        newStatus = "F";

                        break;
                }

                var item = Mapper.Map<ThirdCommunicationResponse>(tx47);
                item.ProductCode = "";
                item.F47_PalletNo = "";
                item.OldStatus = tx47.F47_Status;
                tx47.F47_Status = newStatus;
                tx47.F47_UpdateDate = DateTime.Now;
                
                _unitOfWork.ProductWarehouseCommandRepository.Update(tx47);
                items.Add(item);

                _notificationService.SendMessageToC3("TCPR121F",
                    _notificationService.FormatThirdCommunicationMessageResponse(tx47));
            }

            _unitOfWork.Commit();

            return items;
        }

        private bool AssignSpaceShelf(ref string ls_ShelfNo, int liLoadamount, string terminalNo)
        {
            if (!CheckStockConveyor(terminalNo, 2).IsSuccess)
            {
                return false;
            }
            var lstTx51 =
                _unitOfWork.ProductShelfStatusRepository.GetByShelfStatusAndShelfType(
                    Constants.F51_ShelfStatus.TX51_ShfSts_Epy, Constants.F51_ShelfType.Normal.ToString("d"))
                    .Where(i => i.F51_CmdShfAgnOrd.HasValue).OrderByDescending(i => i.F51_CmdShfAgnOrd);

            foreach (var tx51 in lstTx51)
            {
                tx51.F51_ShelfStatus = Constants.TX51SheflStatus.TX51_ShfSts_RsvStr.ToString("d");
                tx51.F51_LoadAmount = liLoadamount;
                tx51.F51_TerminalNo = terminalNo;
                tx51.F51_StorageDate = DateTime.Now;
                tx51.F51_UpdateDate = DateTime.Now;
                ls_ShelfNo = tx51.F51_ShelfRow + tx51.F51_ShelfBay + tx51.F51_ShelfLevel
                    ;
                _unitOfWork.ProductShelfStatusRepository.Update(tx51);
                break;
            }
            return true;
        }

        private bool InsertCommand(string shelfNo, string terminalNo)
        {
            var ls_cmdno = Constants.F47_CommandNo.TwoTimes.ToString("D");
            var ls_strtype = Constants.F47_StrRtrType.PalletForWarehouse.ToString("D");
            var isNoMange = true;
            var is_serialno = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNoMange,
                Constants.GetColumnInNoManager.Pdtwhscmdno, 0, 0, 0, 0, 1).ToString("D4");
            var ls_from = _unitOfWork.ConveyorRepository.GetbyTerminerNo(terminalNo).F05_ConveyorCode;
            var ls_to = shelfNo;
            var tx47 = new TX47_PdtWhsCmd()
            {
                F47_CommandNo = ls_cmdno,
                F47_CmdSeqNo = is_serialno,
                F47_CommandType = Constants.CmdType.cmdType,
                F47_StrRtrType = ls_strtype,
                F47_Status = "0",
                F47_Priority = 0,
                F47_PalletNo = null,
                F47_From = ls_from,
                F47_To = ls_to,
                F47_CommandSendDate = null,
                F47_TerminalNo = terminalNo,
                F47_PictureNo = Constants.PictureNo.TCPR111F,
                F47_AbnormalCode = null,
                F47_RetryCount = 0,
                F47_AddDate = DateTime.Now,
                F47_UpdateDate = DateTime.Now
            };
            _unitOfWork.ProductWarehouseCommandRepository.Add(tx47);
            return true;
        }

        #endregion

        private bool Getloadamount(string as_shelfno, ref int ai_loadamount)
        {
            var lsShelfrow = as_shelfno.Substring(0, 2);
            var lsShelfbay = as_shelfno.Substring(2, 2);
            var lsShelflevel = as_shelfno.Substring(4, 2);

            var tx51 = _unitOfWork.ProductShelfStatusRepository.GetByRowBayLevel(lsShelfrow, lsShelfbay, lsShelflevel);
            if (tx51 == null) return false;

            ai_loadamount = tx51.F51_LoadAmount;
            return true;
        }


        public bool CheckProductShelfStatus()
        {
            var shelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_Epy;
            var shelfType = Constants.F51_ShelfType.Normal.ToString("D");
            var productShelfStatuses =
                                _unitOfWork.ProductShelfStatusRepository.GetMany(
                                    i =>
                                        i.F51_ShelfStatus.Equals(shelfStatus) && i.F51_ShelfType.Equals(shelfType));
                //                _unitOfWork.ProductShelfStatusRepository.GetMany(
                //                    i =>
                //                        i.F51_ShelfStatus.Equals(shelfStatus) && i.F51_ShelfType.Equals(shelfType) &&
                //                        i.F51_CmdShfAgnOrd != null);

            return productShelfStatuses.Any();
        }
    }
}