using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.Domain.Models.MaterialManagement;

namespace KCSG.Domain.Domains.MaterialManagement
{
    public class RetrievalOfWarehousePalletDomain : BaseDomain, IRetrievalOfWarehousePalletDomain
    {
        private readonly INotificationService _notificationService;
        #region Constructor

        public RetrievalOfWarehousePalletDomain(IUnitOfWork unitOfWork, INotificationService notificationService, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _notificationService = notificationService;
        }

        #endregion

        public int GetPossibleQuantity()
        {
            var shelfStatus = Constants.F37_ShelfStatus.EmptyContainer.ToString("D");
            return
                _unitOfWork.MaterialShelfStatusRepository.GetMany(
                    m => m.F31_ShelfStatus.Equals(shelfStatus) && m.F31_CmnShfAgnOrd.HasValue).Count();
        }

        public ResponseResult Retrieval(RetrievalOfWarehousePalletItem model,string terminalNo)
        {           

            #region validate

            ////validate conveyor
            //var conveyor =
            //    _unitOfWork.ConveyorRepository.GetAll()
            //        .Where(
            //            t =>
            //                t.F05_TerminalNo.Equals(Constants.TerminalNo.A001) ||
            //                t.F05_StrRtrSts.Equals(statusError));
            //if (!conveyor.Any())
            //{
            //    //show msg 15
            //    var errorMsg = "error";
            //    return new ResponseResult(false, errorMsg);
            //}
            ////validate device
            //var device =
            //    _unitOfWork.DeviceRepository.GetAll()
            //        .Where(
            //            d =>
            //                d.F14_DeviceCode.Equals("ATW001") ||
            //                d.F14_DeviceStatus.Equals(Constants.F14_DeviceStatus.Offline.ToString()) ||
            //                d.F14_DeviceStatus.Equals(Constants.F14_DeviceStatus.Error.ToString()) ||
            //                d.F14_UsePermitClass.Equals(Constants.F14_UsePermitClass.Prohibited.ToString()));
            //if (!device.Any())
            //{
            //    //show msg 16
            //    var errorMg = "error";
            //    return new ResponseResult(false, errorMg);
            //}

            #endregion

            //TODO update select top ? smallest F31_CmnShfAgnOrd
            var shelfStatus = Constants.F31_ShelfStatus.WarehousePallet.ToString("D");
            var entity =
                _unitOfWork.MaterialShelfStatusRepository.GetMany(
                    m =>
                        m.F31_ShelfStatus.Equals(shelfStatus) &&
                        //m.F31_CmnShfAgnOrd.HasValue).OrderByDescending(i => i.F31_CmnShfAgnOrd).FirstOrDefault();
                        m.F31_CmnShfAgnOrd.HasValue).OrderBy(i => i.F31_CmnShfAgnOrd).Take(model.RequestRetrievalQuantity);
            var asDestConvCode = "";
           
          
                foreach (var item in entity)
                {
                    
                    item.F31_ShelfStatus = Constants.F31_ShelfStatus.ReservedForRetrieval.ToString("D");
                    item.F31_TerminalNo = terminalNo; //current terimal
                    item.F31_UpdateDate = DateTime.Now;
                    _unitOfWork.MaterialShelfStatusRepository.Update(item);

                    //	[As_DestConvCode] = [f31_shelfrow] + [f31_shelfbay] + [f31_shelflevel]. In which, [f31_shelfrow], [f31_shelfbay] and [f31_shelflevel] are corresponding field of [tx31_mtrshfsts] record whose [f31_shelfstatus] is “0” and [f31_cmnshfagnord] is not blank.
                    asDestConvCode = item.F31_ShelfRow + item.F31_ShelfBay + item.F31_ShelfLevel;

                    var isNoManage = false;
                    var f48MtrWhsCmdNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNoManage,
                            Constants.GetColumnInNoManager.MtrWhsCmdNo,1);

                    /*
             * 	Set [f33_stockflag] of corresponding [tx33_mtrshfstk] record 
             * whose [f33_palletno] is [f31_palletno] of current “Actionable Item” as 2
             */
                    //var materialShelfStocks = _unitOfWork.MaterialShelfStockRepository.GetAll();
                    //materialShelfStocks = materialShelfStocks.Where(x => x.F33_PalletNo.Equals(entity.F31_PalletNo));

                    //foreach (var materialShelfStock in materialShelfStocks)
                    //    materialShelfStock.F33_StockFlag = Constants.F33_StockFlag.TX33_StkFlg_Rtr;
                    

                    //	[As_ConvCode] is the value of field [f05_conveyorcode] in [tm05_conveyor] record whose [f05_terminalno] is current terminalNo
                    var conveyorItem = _unitOfWork.ConveyorRepository.GetMany(i => i.F05_TerminalNo.Trim().Equals(terminalNo)).FirstOrDefault();
                    var asConvCode = conveyorItem.F05_ConveyorCode;
                    var palletNo = item.F31_PalletNo;

                    //	Create a new [tx34_mtrwhscmd] record using the following pseudo code (under SQL format)
                    InsertTX34(Constants.F34_CommandNo.Retrieval, f48MtrWhsCmdNo, Constants.CmdType.cmdType,
                        Constants.TX34_StrRtrType.WarehousePallet.ToString("D"), Constants.TC_CMDSTS.TC_CMDSTS_0, palletNo, asDestConvCode, asConvCode,
                        terminalNo,
                        Constants.PictureNo.TCRM111F);
                }                
                                 
            //send a message to server C1
            var msgId = "0011";
            var pictureNo = Constants.PictureNo.TCRM111F;

            _notificationService.SendMessageToC1(new
            {
                msgId,
                terminalNo,
                pictureNo
            });

            _unitOfWork.Commit();
            return new ResponseResult(true);
        }


        public System.Collections.Generic.IList<Models.FirstCommunicationResponse> RetrieveWarehousePalletMessageC1Reply(string terminalNo)
        {
            var pictureNo = Constants.PictureNo.TCRM111F;
            var status1 = Constants.F34_Status.status6;
            var status2 = Constants.F34_Status.status7;
            var status3 = Constants.F34_Status.status9;

            var items = new List<FirstCommunicationResponse>();
            var materialWarehouseCommands =
                _unitOfWork.MaterialWarehouseCommandRepository.GetMany(
                    i =>
                        i.F34_TerminalNo.Trim().Equals(terminalNo.Trim()) && i.F34_PictureNo.Equals(pictureNo) &&
                        (i.F34_Status.Equals(status1) || i.F34_Status.Equals(status2)) || i.F34_Status.Equals(status3)).OrderBy(i=>i.F34_AddDate);
            foreach (var materialWarehouseCommand in materialWarehouseCommands)
            {
                var item = Mapper.Map<FirstCommunicationResponse>(materialWarehouseCommand);
                item.OldStatus = materialWarehouseCommand.F34_Status;
                //item.MaterialCode=materialWarehouseCommand.c
                switch (materialWarehouseCommand.F34_Status)
                {
                    case "6":
                        materialWarehouseCommand.F34_Status = Constants.F34_Status.statusC;
                        break;
                    case "7":
                        materialWarehouseCommand.F34_Status = Constants.F34_Status.statusD;
                        break;
                    case "9":
                        materialWarehouseCommand.F34_Status = Constants.F34_Status.statusE;
                        break;
                }
                materialWarehouseCommand.F34_UpdateDate = DateTime.Now;
                items.Add(item);
            }
            _unitOfWork.Commit();
            return items;
        }
    }
}