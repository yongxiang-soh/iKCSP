using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office.Word;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;

namespace KCSG.Domain.Domains.MaterialManagement
{
    public class StorageOfWarehousePalletDomain : BaseDomain, IStorageOfWarehousePalletDomain
    {
        private readonly INotificationService _notificationService;
        
        #region Constructor
        public StorageOfWarehousePalletDomain(IUnitOfWork unitOfWork, INotificationService notificationService, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _notificationService = notificationService;
        }
        #endregion
        //check do not have record TM05 with terminalno as current terminalno Or status is Error
        public bool CheckedRecordInTM05(string terminalNo)
        {
            var errorStatus = Constants.F05_StrRtrSts.Error.ToString("D");
            var conveyorForTerminalNo = _unitOfWork.ConveyorRepository.GetMany(i => i.F05_TerminalNo.Trim().Equals(terminalNo) && i.F05_StrRtrSts.Trim()!= errorStatus);
            //var conveyorForErrorStatus =
            //    _unitOfWork.ConveyorRepository.GetMany(i => i.F05_StrRtrSts.Trim().Equals(errorStatus));
            if (!conveyorForTerminalNo.Any())
                return false;
            return true;
        }
        //Check status for tx31_mtrshfsts
        public bool CheckStatusForTX31()
        {
            var status = Constants.F31_ShelfStatus.EmptyShelf.ToString("D");
            var mtrShfSts =
                _unitOfWork.MaterialShelfStatusRepository.GetMany(
                    i => i.F31_ShelfStatus.Trim().Equals(status) && i.F31_CmnShfAgnOrd!=null);
            if (mtrShfSts.Any())
                return true;
            return false;
        }

        public bool CheckStatusForTX31_1()
        {
            var status = Constants.F31_ShelfStatus.EmptyShelf.ToString("D");
            var mtrShfSts =
                _unitOfWork.MaterialShelfStatusRepository.GetMany(
                    i => i.F31_ShelfStatus.Trim().Equals(status) && (i.F31_CmnShfAgnOrd != null || i.F31_LqdShfAgnOrd != null));
            if (mtrShfSts.Any())
                return true;
            return false;
        }

        public bool CheckedRecordInTM01(string materialCode)
        {
            var materials = _unitOfWork.MaterialRepository.GetMany(i => i.F01_MaterialCode.Trim().Equals(materialCode));
            if (!materials.Any())
                return false;
            return true;
        }

        public ResponseResult CreateOrUpdate(string terminalNo)
        {
            var conveyorForTerminalNo = _unitOfWork.ConveyorRepository.Get(i => i.F05_TerminalNo.Trim() == terminalNo);

            var shelfStatus = Constants.F31_ShelfStatus.EmptyShelf.ToString("D");
            //var lstMtrShfStsItem = _unitOfWork.MaterialShelfStatusRepository.GetMany(i => i.F31_ShelfStatus.Trim().Equals(shelfStatus) && i.F31_CmnShfAgnOrd != null).OrderByDescending(i => i.F31_CmnShfAgnOrd).FirstOrDefault();
            var lstMtrShfStsItem = _unitOfWork.MaterialShelfStatusRepository.GetMany(i => i.F31_ShelfStatus.Trim().Equals(shelfStatus) && i.F31_CmnShfAgnOrd != null).OrderByDescending(i => i.F31_CmnShfAgnOrd);

            // Find first empty location in the system.
            var item = lstMtrShfStsItem.FirstOrDefault();
            
            // Empty location is not found. Display error message the screen.
            if (item == null)
                return new ResponseResult(true, "There is no empty location available in the warehouse now!");
            
            var asDestConvCode = item.F31_ShelfRow + item.F31_ShelfBay +
                                item.F31_ShelfLevel;

            ////Create Or Update F48_MtrWhsCmdNo
            //var isNoManage = false;
            //var F48_MtrWhsCmdNo = CreateOrUpdateTX48(ref isNoManage);


            //	If there is a [tx48_nomanage] record whose [f48_systemid] = “0000”, the system will:
            var isNoManage = false;

            var f48MtrWhsCmdNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNoManage,
                Constants.GetColumnInNoManager.MtrWhsCmdNo,1);

            //Update records in Material Shelf Status
            item.F31_ShelfStatus = Constants.F31_ShelfStatus.ReservedForStorage.ToString("D");
            item.F31_StockTakingFlag = Constants.TX31_StkTkgFlg.InvNotChk.ToString("D");
            item.F31_SupplierCode = "";
            item.F31_TerminalNo = terminalNo;
            item.F31_LoadAmount = 0;
            item.F31_PalletNo = "";
            item.F31_Amount = 0;
            item.F31_StorageDate = DateTime.Now;
            item.F31_UpdateDate = DateTime.Now;
            _unitOfWork.MaterialShelfStatusRepository.Update(item);



            InsertTX34(Constants.F34_CommandNo.Storage, f48MtrWhsCmdNo, Constants.CmdType.cmdType,
                Constants.TX34_StrRtrType.WarehousePallet.ToString("D"),
                    Constants.TC_CMDSTS.TC_CMDSTS_0, "   ", conveyorForTerminalNo.F05_ConveyorCode, asDestConvCode, terminalNo, Constants.PictureNo.TCRM101F);                
                    
            
            //After all actions above are success, the system will send a message to server C1
            var msgId = "0001";
            var pictureNo = Constants.PictureNo.TCRM101F;

            _notificationService.SendMessageToC1(new
            {
                msgId,
                terminalNo,
                pictureNo
            });

            _unitOfWork.Commit();
            return new ResponseResult(true);

        }
        public IList<FirstCommunicationResponse> PostRetrieveMaterial(string terminalNo)
        {
            var pictureNo = Constants.PictureNo.TCRM101F;
            var status6 = Constants.F34_Status.status6;
            var status7 = Constants.F34_Status.status7;
            var status8 = Constants.F34_Status.status8;
            var conveyorForTerminalNo = _unitOfWork.ConveyorRepository.Get(i => i.F05_TerminalNo.Trim() == terminalNo);
            var list = new List<FirstCommunicationResponse>();

            //find material warehouse command record
            var materialWarehouseCommands =
                _unitOfWork.MaterialWarehouseCommandRepository.GetMany(
                    i =>
                        i.F34_TerminalNo.Trim().Equals(terminalNo) && i.F34_PictureNo.Trim().Equals(pictureNo) &&
                        (i.F34_Status.Equals(status6) || i.F34_Status.Equals(status7) || i.F34_Status.Equals(status8)))
                    .OrderBy(i => i.F34_AddDate);

            var newStatus = "";

            foreach (var materialWarehouseCommand in materialWarehouseCommands)
            {
                var oldStatus = materialWarehouseCommand.F34_Status;
                if (materialWarehouseCommand.F34_Status == status6)
                {
                    materialWarehouseCommand.F34_Status = Constants.F34_Status.statusC;
                }
                else if (materialWarehouseCommand.F34_Status == status7)
                    materialWarehouseCommand.F34_Status = Constants.F34_Status.statusD;
                else
                {
                    var isNoManage = false;
                    var f48MtrWhsCmdNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNoManage,
                    Constants.GetColumnInNoManager.MtrWhsCmdNo,1);
                    var ls_strToCon = "";
                    if (Operatmtr(terminalNo,ref ls_strToCon))
                    {
                       
                       InsertTX34(Constants.F34_CommandNo.TwoTimesIn, f48MtrWhsCmdNo, Constants.CmdType.cmdType,
                    Constants.TX34_StrRtrType.WarehousePallet.ToString("D"),
                     Constants.TC_CMDSTS.TC_CMDSTS_0, "   ", conveyorForTerminalNo.F05_ConveyorCode, ls_strToCon, terminalNo, Constants.PictureNo.TCRM101F);
                     materialWarehouseCommand.F34_Status = Constants.F34_Status.statusE;
                    }
                    else
                    {
                        materialWarehouseCommand.F34_Status = Constants.F34_Status.statusB;
                    }
                   
                }
                materialWarehouseCommand.F34_UpdateDate = DateTime.Now;
                _unitOfWork.MaterialWarehouseCommandRepository.Update(materialWarehouseCommand);

                var item = Mapper.Map<FirstCommunicationResponse>(materialWarehouseCommand);
                item.OldStatus = oldStatus;
               
                list.Add(item);
            }

            _unitOfWork.Commit();

            return list;
        }

        public bool Operatmtr(string terminalNo,ref string as_strToConv)
        {
            var lstTx31 =
                _unitOfWork.MaterialShelfStatusRepository.GetMany(
                        i => i.F31_ShelfStatus == "0" && i.F31_CmnShfAgnOrd.HasValue)
                    .OrderByDescending(i => i.F31_CmnShfAgnOrd);
            if (!lstTx31.Any())
                return false;
           
            foreach (var tx31 in lstTx31)
            {
                tx31.F31_ShelfStatus = Constants.TX31SheflStatus.TX31_MtrShfSts_RsvStg.ToString("d");
                tx31.F31_StockTakingFlag = Constants.T31StockTakingFlag.TX31_StkTkgFlg_InvNotChk.ToString("d");
                tx31.F31_SupplierCode = null;
                tx31.F31_TerminalNo = terminalNo;
                tx31.F31_LoadAmount = 0;
                tx31.F31_PalletNo = null;
                tx31.F31_Amount = 0;
                tx31.F31_StorageDate = DateTime.Now;
                tx31.F31_UpdateDate = DateTime.Now;
                _unitOfWork.MaterialShelfStatusRepository.Update(tx31);
                as_strToConv = tx31.F31_ShelfRow + tx31.F31_ShelfBay + tx31.F31_ShelfLevel;

                break;
            }
            _unitOfWork.Commit();
            return true;
        }
    }
}
