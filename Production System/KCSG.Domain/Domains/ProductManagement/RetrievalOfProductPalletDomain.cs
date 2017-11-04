using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.ProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;

namespace KCSG.Domain.Domains.ProductManagement
{
    public class RetrievalOfProductPalletDomain : BaseDomain, IRetrievalOfProductPalletDomain
    {
        #region Constructor
        
        /// <summary>
        /// Service which is used for handling notifications.
        /// </summary>
        private readonly INotificationService _notificationService;

        /// <summary>
        /// Initiate domain with IoC.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="notificationService"></param>
        /// <param name="configurationService"></param>
        public RetrievalOfProductPalletDomain(IUnitOfWork unitOfWork, 
            INotificationService notificationService,
            IConfigurationService configurationService) : base(unitOfWork, configurationService)
        {
            _notificationService = notificationService;
        }

        #endregion

        /// <summary>
        ///     Get Possible Retrieval Quantity retrieved from sum of [f51_loadamount]
        ///     in TX51_PDTSHFSTS, where [f51_shelfstatus] = “Empty Pallet” (or 1) and [f51_shelftype] = “Normal Shelf” (or 0).
        /// </summary>
        /// <returns></returns>
        public double GetPossibleRetrievalQuantity()
        {
            var shelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_WhsPlt;
            var shelfType = Constants.F51_ShelfType.Normal.ToString("D");

            var productShelfStatuses =
                _unitOfWork.ProductShelfStatusRepository.GetMany(
                    i => i.F51_ShelfStatus.Equals(shelfStatus) && i.F51_ShelfType.Equals(shelfType));
            double totalLoadAmount = 0;

            foreach (var productShelfStatus in productShelfStatuses)
            {
                totalLoadAmount += productShelfStatus.F51_LoadAmount;
            }
            return totalLoadAmount;
        }

        /// <summary>
        ///     Retrieval The Empty Pallet
        ///     Refer BR 89 - SRS Product Management v1.1
        /// </summary>
        /// <param name="terminalNo"></param>
        public void Retrieval(string terminalNo)
        {
            var shelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_WhsPlt;
            var shelfType = Constants.F51_ShelfType.Normal.ToString("D");

            var productShelfStatuses =
                _unitOfWork.ProductShelfStatusRepository.GetMany(
                    i =>
                        i.F51_ShelfStatus.Equals(shelfStatus) && i.F51_ShelfType.Equals(shelfType) &&
                        i.F51_LoadAmount > 0).OrderBy(i => i.F51_CmdShfAgnOrd);
            var shelfNo = "";
            foreach (var productShelfStatus in productShelfStatuses)
            {
                //	Update TX51_PDTSHFSTS
                productShelfStatus.F51_ShelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_RsvRtr;
                productShelfStatus.F51_TerminalNo = terminalNo;
                productShelfStatus.F51_UpdateDate = DateTime.Now;

                _unitOfWork.ProductShelfStatusRepository.Update(productShelfStatus);

                shelfNo = productShelfStatus.F51_ShelfRow + productShelfStatus.F51_ShelfBay +
                          productShelfStatus.F51_ShelfLevel;
                break;
            }

            //get nomanage record where F48_SystemId is 00000
            var isNoManage = true;
            var serialNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNoManage,
                Constants.GetColumnInNoManager.Pdtwhscmdno, 0, 0, 0, 0, 1);

            var F47CmdSeqNo = serialNo.ToString("D4");

            var to = GetConveyorCode(terminalNo);

            //	Insert data into TX47_PDTWHSCMD
            var item = _unitOfWork.ProductWarehouseCommandRepository.InsertProductWarehouseCommand(
                Constants.F47_CommandNo.Retrieval.ToString("D"), F47CmdSeqNo,
                Constants.CommandType.CmdType_0,
                Constants.F47_StrRtrType.PalletForWarehouse.ToString("D"),
                Constants.F47_Status.AnInstruction.ToString("D"), "", shelfNo, to, terminalNo,
                Constants.PictureNo.TCPR131F);

            _unitOfWork.ProductWarehouseCommandRepository.Add(item);
           _unitOfWork.Commit();
        }

        /// <summary>
        /// Reply message sent back from C3.
        /// </summary>
        /// <param name="terminalNo"></param>
        public IList<ThirdCommunicationResponse> RespondingReplyFromC3(string terminalNo)
        {
            var lstStatus = new List<string>() { "6", "7", "9" };
            var lstTx47 =
                 _unitOfWork.ProductWarehouseCommandRepository.GetByTerminalNoAndPictureNoAndStatus(terminalNo,
                     Constants.PictureNo.TCPR131F, lstStatus);

            var items = new List<ThirdCommunicationResponse>();
            
            // Initiate c2 response.
            foreach (var tx47 in lstTx47)
            {
               
                var newStatus = "";

                switch (tx47.F47_Status)
                {
                    case "6": //Command End

                        newStatus = "C";
                       
                        break;
                    case "7": //Command Cancel
                        newStatus = "D";
                       
                        break;

                    case "9": //Command Error
                        newStatus = "F";
                       
                        break;
                }

                var item = Mapper.Map<ThirdCommunicationResponse>(tx47);
                item.F47_PalletNo = "";
                item.ProductCode = "";
                item.OldStatus = tx47.F47_Status;
                tx47.F47_Status = newStatus;
                tx47.F47_UpdateDate = DateTime.Now;
                _unitOfWork.ProductWarehouseCommandRepository.Update(tx47);

                

                items.Add(item);
            }

            _unitOfWork.Commit();

            return items;
        }
    }
}