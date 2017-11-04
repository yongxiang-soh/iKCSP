using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;

namespace KCSG.Domain.Domains.MaterialManagement
{
   public class FontStorageOfSupplierPalletDomain :BaseDomain, IFontStorageOfSupplierPalletDomain
    {
        private IUnitOfWork _unitOfWork;
        public FontStorageOfSupplierPalletDomain(IUnitOfWork unitOfWork, INotificationService notificationService, IConfigurationService configurationService) :
            base(unitOfWork, configurationService)
        {
            _unitOfWork = unitOfWork;
        }
        public ResponseResult Storage(string suppliCode, int storageQuantity,string terminalNo)
        {
            var check = CheckStockConveyor(terminalNo, 0);
            if (!check.IsSuccess)
            {
                return new ResponseResult(false, "Conveyor status error");
            }
            
            var as_ToConv = "";
            var lsttx31 = _unitOfWork.MaterialShelfStatusRepository.GetMany(i=>i.F31_ShelfStatus=="0"&&i.F31_CmnShfAgnOrd.HasValue).OrderByDescending(i=>i.F31_CmnShfAgnOrd);
            foreach (var tx31 in lsttx31)
            {
                tx31.F31_ShelfStatus = Constants.TX31SheflStatus.TX31_MtrShfSts_RsvStg.ToString("d");
                tx31.F31_SupplierCode = suppliCode;
                tx31.F31_LoadAmount = storageQuantity;
                tx31.F31_StorageDate = DateTime.Now;
                tx31.F31_UpdateDate = DateTime.Now;
                tx31.F31_TerminalNo = terminalNo;
                _unitOfWork.MaterialShelfStatusRepository.Update(tx31);
                as_ToConv = tx31.F31_ShelfRow + tx31.F31_ShelfBay + tx31.F31_ShelfLevel;

                break;
            }
            Insttab(as_ToConv, terminalNo);
            _unitOfWork.Commit();
            return new ResponseResult(true);
        }

        public List<FirstCommunicationResponse> CommutionC1(string terminal)
        {
            var pictureNo = Constants.PictureNo.TCRM123F;
            var status6 = Constants.F34_Status.status6;
            var status7 = Constants.F34_Status.status7;
            var status8 = Constants.F34_Status.status8;
            var conveyorForTerminalNo = _unitOfWork.ConveyorRepository.Get(i => i.F05_TerminalNo.Trim() == terminal);
            var list = new List<FirstCommunicationResponse>();

            //find material warehouse command record
            var materialWarehouseCommands =
                _unitOfWork.MaterialWarehouseCommandRepository.GetMany(
                    i =>
                        i.F34_TerminalNo.Trim().Equals(terminal) && i.F34_PictureNo.Trim().Equals(pictureNo) &&
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
                    Constants.GetColumnInNoManager.MtrWhsCmdNo, 1);
                    var ls_strToCon = "";
                    var suppliCode = "";
                    var li_qty = GetQuantity(materialWarehouseCommand.F34_To, ref suppliCode);
                    if (Operatmtr(terminal, ref ls_strToCon,suppliCode,li_qty))
                    {

                        InsertTX34(Constants.F34_CommandNo.TwoTimesIn, f48MtrWhsCmdNo, Constants.CmdType.cmdType,
                     Constants.TX34_StrRtrType.WarehousePallet.ToString("D"),
                      Constants.TC_CMDSTS.TC_CMDSTS_0, "   ",
                      conveyorForTerminalNo.F05_ConveyorCode, ls_strToCon, terminal, Constants.PictureNo.TCRM123F);
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

        private bool Operatmtr(string terminalNo, ref string lsStrToCon, string suppliCode, int storageQuantity)
        {
            var lsttx31 = _unitOfWork.MaterialShelfStatusRepository.GetMany(i => i.F31_ShelfStatus == "0" && i.F31_CmnShfAgnOrd.HasValue).OrderByDescending(i => i.F31_CmnShfAgnOrd);
            foreach (var tx31 in lsttx31)
            {
                tx31.F31_ShelfStatus = Constants.TX31SheflStatus.TX31_MtrShfSts_RsvStg.ToString("d");
                tx31.F31_SupplierCode = suppliCode;
                tx31.F31_LoadAmount = storageQuantity;
                tx31.F31_StorageDate = DateTime.Now;
                tx31.F31_UpdateDate = DateTime.Now;
                tx31.F31_TerminalNo = terminalNo;
                _unitOfWork.MaterialShelfStatusRepository.Update(tx31);
                lsStrToCon = tx31.F31_ShelfRow + tx31.F31_ShelfBay + tx31.F31_ShelfLevel;

                break;
            }
            return true;
        }

        public int GetQuantity(string shelftNo, ref string SuppliCode)
        {
            var row = shelftNo.Substring(0, 2);
            var bay = shelftNo.Substring(2, 2);
            var level = shelftNo.Substring(4, 2);
            var tx31 = _unitOfWork.MaterialShelfStatusRepository.GetMany(
                i => i.F31_ShelfRow == row && i.F31_ShelfBay == bay && i.F31_ShelfLevel == level).FirstOrDefault();
            if (tx31!=null)
            {
                SuppliCode = tx31.F31_SupplierCode;
                return tx31.F31_LoadAmount.Value;
             }
            return -1;
        }

        public bool Insttab( string as_tocv,   string terminalNo)
        {
           
            var lsConveyorCode = GetConveyorCode(terminalNo);
            var isNoManage = false;
            var f48MtrWhsCmdNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNoManage, Constants.GetColumnInNoManager.MtrWhsCmdNo, 1);
            InsertTX34(Constants.F34_CommandNo.Storage, f48MtrWhsCmdNo,
                Constants.CmdType.cmdType,
                Constants.TX34_StrRtrType.SupplierPallet.ToString("D"), Constants.TC_CMDSTS.TC_CMDSTS_0, null, lsConveyorCode
                , as_tocv,
                terminalNo,
                Constants.PictureNo.TCRM123F);
            return true;
        }
    }
}
