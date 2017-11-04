using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.MaterialManagement
{
    public class StockTakingOfMaterialDomain : BaseDomain, IStockTakingOfMaterialDomain
    {
        private readonly INotificationService _notificationService;
        public StockTakingOfMaterialDomain(IUnitOfWork iUnitOfWork, INotificationService notificationService, IConfigurationService configurationService)
            : base(iUnitOfWork, configurationService)
        {
            _notificationService = notificationService;
        }

        public ResponseResult<GridResponse<StockTakingOfMaterialItem>> SearchMaterialStock(string shelfNoFrom,
            string shelfNoTo, GridSettings gridSettings)
        {
            var shelfNoFromInt = 0;
            var shelfNoToInt = 0;

            if (!string.IsNullOrEmpty(shelfNoFrom) && !string.IsNullOrEmpty(shelfNoTo))
            {
                 shelfNoFromInt = Convert.ToInt32(shelfNoFrom.Replace("-", ""));
                 shelfNoToInt = Convert.ToInt32(shelfNoTo.Replace("-", ""));

            }
            
            
            var reception = _unitOfWork.ReceptionRepository.GetMany(m => true);
            var material = _unitOfWork.MaterialRepository.GetMany(m => true);
            var materialShelf = _unitOfWork.MaterialShelfRepository.GetMany(m => true);
            var materialShelfStatus = _unitOfWork.MaterialShelfStatusRepository.GetMany(m => true);


            var materialShelfStock = _unitOfWork.MaterialShelfStockRepository.GetMany(m => true);

            var inventoryNotChecked = ((int) Constants.TX31_StkTkgFlg.InvNotChk).ToString();
            var shelfStatusMaterial = ((int) Constants.F31_ShelfStatus.Material).ToString();
            var stocked = ((int) Constants.TX33_StkFlg.Stocked).ToString();
            var normal = ((int) Constants.TM01_Material_EntrustedClass.Normal).ToString();


            var result = (from r in reception
                join ms in materialShelf
                    on new {A = r.F30_PrcOrdNo, B = r.F30_PrtDvrNo} equals
                    new {A = ms.F32_PrcOrdNo, B = ms.F32_PrtDvrNo}
                join msts in materialShelfStatus
                    on ms.F32_PalletNo equals msts.F31_PalletNo
                join mstk in materialShelfStock
                    on ms.F32_PalletNo equals mstk.F33_PalletNo
                join m in material
                    on mstk.F33_MaterialCode equals m.F01_MaterialCode
                where msts.F31_StockTakingFlag.Equals(inventoryNotChecked)
                      && msts.F31_ShelfStatus.Equals(shelfStatusMaterial)
                      && mstk.F33_StockFlag.Equals(stocked)
                      && m.F01_EntrustedClass.Equals(normal)
                select new StockTakingOfMaterialItem
                {
                    F33_MaterialCode = mstk.F33_MaterialCode,
                    F01_MaterialDsp = m.F01_MaterialDsp,
                    F32_PrcOrdNo = ms.F32_PrcOrdNo,
                    F32_PrtDvrNo = ms.F32_PrtDvrNo,
                    F30_AcceptClass = r.F30_AcceptClass,
                    F31_ShelfRow = msts.F31_ShelfRow,
                    F31_ShelfBay = msts.F31_ShelfBay,
                    F31_ShelfLevel = msts.F31_ShelfLevel,
                    F31_UpdateDate = msts.F31_UpdateDate,
                    F33_PalletNo = mstk.F33_PalletNo
                }).Distinct();

            var filteredResult = result.AsEnumerable()
                .Where(s => (Convert.ToInt32(s.F31_ShelfRow + s.F31_ShelfBay + s.F31_ShelfLevel) >= shelfNoFromInt)
                            && (Convert.ToInt32(s.F31_ShelfRow + s.F31_ShelfBay + s.F31_ShelfLevel) <= shelfNoToInt))
                .OrderBy(a => a.F31_ShelfRow).ThenBy(a => a.F31_ShelfBay).ThenBy(a => a.F31_ShelfLevel);

            var itemCount = filteredResult.Count();
            var result1 =
                filteredResult.Skip((gridSettings.PageIndex - 1)*gridSettings.PageSize).Take(gridSettings.PageSize);
            var resultModel = new GridResponse<StockTakingOfMaterialItem>(result1, itemCount);

            return new ResponseResult<GridResponse<StockTakingOfMaterialItem>>(resultModel, true);
        }

        public MaterialShelfStatusItem SearchMaterialShelfStatus(string shelfRow, string shelfBay, string shelfLevel)
        {
            var result =
                _unitOfWork.MaterialShelfStatusRepository.GetAll()
                    .FirstOrDefault(
                        m =>
                            m.F31_ShelfRow.Equals(shelfRow) && m.F31_ShelfBay.Equals(shelfBay) &&
                            m.F31_ShelfLevel.Equals(shelfLevel));
            var item = Mapper.Map<MaterialShelfStatusItem>(result);
            return item;
        }

        public IEnumerable<StockTakingOfMaterialStockItem> GetMaterialShelfStocks(string shelfNo, string materialCode)
        {
            var material = _unitOfWork.MaterialRepository.GetMany(m => true);
            var materialShelfStatus = _unitOfWork.MaterialShelfStatusRepository.GetMany(m => true);
            var materialShelfStock = _unitOfWork.MaterialShelfStockRepository.GetMany(m => true);
            shelfNo = shelfNo.Replace("-","");
            var result = (from m in material
                join mstk in materialShelfStock
                    on m.F01_MaterialCode equals mstk.F33_MaterialCode
                join msts in materialShelfStatus
                    on mstk.F33_PalletNo equals msts.F31_PalletNo
                where msts.F31_ShelfRow.Equals(shelfNo.Substring(0, 2))
                      && msts.F31_ShelfBay.Equals(shelfNo.Substring(2, 2))
                      && msts.F31_ShelfLevel.Equals(shelfNo.Substring(4, 2))
                      && mstk.F33_MaterialCode.Equals(materialCode)
                select new StockTakingOfMaterialStockItem
                {
                    F33_MaterialLotNo = mstk.F33_MaterialLotNo,
                    F01_PackingUnit = m.F01_PackingUnit,
                    F33_Amount = mstk.F33_Amount,
                    F33_AddDate = mstk.F33_AddDate
                }).Distinct();
            return result.AsEnumerable();
        }


        public void Retrieve(string firstRowShelfNo, string firstRowPalletNo, string firstRowMaterialCode,
            string firstRowMaterialName,
            string currentRowPalletNo, string terminalNo)
        {
            //TODO: Validate Conveyor and Device here. Since the system could not detect Terminal no at this moment, this task will be postponed.
            //This is actually the common function f_tccheckstockconveyor in the old system. This validation should be implemented as the same.

            var shelfRow = firstRowShelfNo.Substring(0, 2);
            var shelfBay = firstRowShelfNo.Substring(2, 2);
            var shelfLevel = firstRowShelfNo.Substring(4, 2);

            var materialShelfStatusItem =
                _unitOfWork.MaterialShelfStatusRepository.GetMany(
                    i =>
                        i.F31_ShelfRow.Trim().Equals(shelfRow) && i.F31_ShelfBay.Trim().Equals(shelfBay) &&
                        i.F31_ShelfLevel.Trim().Equals(shelfLevel)).FirstOrDefault();
            if (materialShelfStatusItem != null)
            {
                materialShelfStatusItem.F31_ShelfStatus = Constants.F31_ShelfStatus.ReservedForRetrieval.ToString("D");
                materialShelfStatusItem.F31_TerminalNo = terminalNo;
                materialShelfStatusItem.F31_UpdateDate = DateTime.Now;

                _unitOfWork.MaterialShelfStatusRepository.Update(materialShelfStatusItem);
            }


            var isNoManage = false;
            var f48MtrWhsCmdNo = CreateOrUpdateTX48(ref isNoManage);

            //select record in TM05 table with terminalNo as current terminalNo
            var lsCnvCode =
                _unitOfWork.ConveyorRepository.Get(i => i.F05_TerminalNo.Trim().Equals(terminalNo)).F05_ConveyorCode;

            //[As_Pallet] is [f31_palletno] of the 1st record in the result search.
            var palletNo = materialShelfStatusItem.F31_PalletNo;
            
            InsertTX34(Constants.F34_CommandNo.StockTakingOff, f48MtrWhsCmdNo, Constants.CmdType.cmdType,
                Constants.TX34_StrRtrType.Material.ToString("D"), Constants.TC_CMDSTS.TC_CMDSTS_0, palletNo,
                firstRowShelfNo, lsCnvCode,
                terminalNo,
                Constants.PictureNo.TCRM081F);
            

            _unitOfWork.Commit();

            //Send message to server C1. The content of the message contains [MsgId], [TermNo] and [PicNo]
            var msgId = "0011";
            var pictureNo = Constants.PictureNo.TCRM081F;

            _notificationService.SendMessageToC1(new
            {
                msgId,
                terminalNo,
                pictureNo
            });
        }

        public void UpdateMaterialShelfStatus(MaterialShelfStatusItem shelfStatus)
        {
            var entity = Mapper.Map<TX31_MtrShfSts>(shelfStatus);
            _unitOfWork.MaterialShelfStatusRepository.Update(entity);
            _unitOfWork.Commit();
        }

        public void RestoreMaterialStocks(RestoreMaterialViewModel model, string termianlNo)
        {
            #region Re-store material stocks

            //Delete [TX33_MtrShfStk] records whose [F33_PalletNo] is [f33_palletno] of selected [tx33_mtrshfstk] record
            //(the selected item in screen SC 20 – Stock-taking of Raw Material (TCRM081F))  and [F33_MaterialCode] is [Material Code] in current screen.
            Delete(model.PalletNo, model.MaterialCode);
            
            //For each item in the table listing whose [Material Lot No] is not blank and [Total] is greater than 0,
            //the system will create and save a new [TX33_MtrShfStk] record into table [TX33_MtrShfStk]

            foreach (var item in model.Materials)
            {
                if (!string.IsNullOrEmpty(item.MaterialLotNo) && (item.Total > 0.0))
                    Create(model.PalletNo, model.MaterialCode, item.MaterialLotNo, item.Total);
            }
            
            //Update material shelf status table (table [tx31_mtrshfsts]) whose [F31_ShelfRow] is 2 first characters of
            //[Shelf No], [F31_ShelfBay] is 2 next characters of [Shelf No] and [F31_ShelfLevel] is 2 last characters of [Shelf No]:
            string shelfNo = "";
            var materialShelfStatusItem =
                _unitOfWork.MaterialShelfStatusRepository.GetAll()
                    .FirstOrDefault(
                        m =>
                            m.F31_ShelfRow.Equals(model.ShelfNo.Substring(0, 2)) &&
                            m.F31_ShelfBay.Equals(model.ShelfNo.Substring(3, 2)) &&
                            m.F31_ShelfLevel.Equals(model.ShelfNo.Substring(6, 2)));
            if (materialShelfStatusItem != null)
            {
                materialShelfStatusItem.F31_ShelfStatus =
                    ((int) Constants.F31_ShelfStatus.ReservedForRetrieval).ToString();
                materialShelfStatusItem.F31_Amount = model.GrandTotal;
                materialShelfStatusItem.F31_TerminalNo = termianlNo;
                materialShelfStatusItem.F31_UpdateDate = DateTime.Now;
                materialShelfStatusItem.F31_UpdateCount += 1;
                shelfNo = materialShelfStatusItem.F31_ShelfRow + materialShelfStatusItem.F31_ShelfBay
                          + materialShelfStatusItem.F31_ShelfLevel;
                materialShelfStatusItem.F31_StockTakingFlag = Constants.F31_ShelfStatus.EmptyShelf.ToString("D");
                _unitOfWork.MaterialShelfStatusRepository.Update(materialShelfStatusItem);
            }
            var isNoManage = false;
            //var f48_MtrWhsCmdNo = CreateOrUpdateTX48(ref isNoManage);
            var f48_MtrWhsCmdNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNoManage,
                            Constants.GetColumnInNoManager.MtrWhsCmdNo);

            var asConvCode =
                _unitOfWork.ConveyorRepository.Get(i => i.F05_TerminalNo.Trim().Equals(termianlNo)).F05_ConveyorCode;
            
            InsertTX34(Constants.F34_CommandNo.StockTakingIn, f48_MtrWhsCmdNo,
                Constants.CmdType.cmdType,
                Constants.TX34_StrRtrType.Material.ToString("D"), Constants.TC_CMDSTS.TC_CMDSTS_0, model.PalletNo,
                asConvCode, shelfNo, termianlNo,
                Constants.PictureNo.TCRM082F);


            //Send message to C1.
            var msgId = "0001";
            var pictureNo = Constants.PictureNo.TCRM082F;

            _notificationService.SendMessageToC1(new
            {
                msgId,
                termianlNo,
                pictureNo
            });
            _unitOfWork.Commit();

            #endregion
        }


        public List<MaterialShelfStockItem> GetStockByPalletNo(string palletNo)
        {
            var materialShelfStocks =
                _unitOfWork.MaterialShelfStockRepository.GetAll();

            var result = from materialShelfStock in materialShelfStocks
                where (materialShelfStock.F33_PalletNo.Trim().Equals(palletNo))
                select new MaterialShelfStockItem
                {
                    F33_MaterialLotNo = materialShelfStock.F33_MaterialLotNo,
                    F33_Amount = materialShelfStock.F33_Amount
                };
            
            return result.ToList();
        }


        private void Create(string palletNo, string materialCode, string materialLotNo, double total05)
        {
            var entity = new TX33_MtrShfStk();
            entity.F33_PalletNo = palletNo;
            entity.F33_MaterialCode = materialCode;
            entity.F33_MaterialLotNo = materialLotNo;
            entity.F33_Amount = total05;
            entity.F33_StockFlag = Constants.TX33_StkFlg.Store.ToString("D");
            entity.F33_AddDate = DateTime.Now;
            entity.F33_UpdateDate = DateTime.Now;
            _unitOfWork.MaterialShelfStockRepository.Add(entity);
            _unitOfWork.Commit();
        }

        private void Delete(string palletNo, string materialCode)
        {
            _unitOfWork.MaterialShelfStockRepository.Delete(s => s.F33_PalletNo.Equals(palletNo)
                                                                 && s.F33_MaterialCode.Equals(materialCode));
            _unitOfWork.Commit();
        }
        /// <summary>
        /// Post retrieve Material
        /// Refer UC28 srs material Management 1.0.1 for more information
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="materialCode"></param>
        public IList<FirstCommunicationResponse> PostRetrieveMaterial(string terminalNo,string materialCode)
        {
            var pictureNo = Constants.PictureNo.TCRM081F;
            var status1 = Constants.F34_Status.status6;
            var status2 = Constants.F34_Status.status7;
            var status3 = Constants.F34_Status.status9;

            var list = new List<FirstCommunicationResponse>();

            //find material warehouse command record
            var materialWarehouseCommands =
                _unitOfWork.MaterialWarehouseCommandRepository.GetMany(
                    i =>
                        i.F34_TerminalNo.Trim().Equals(terminalNo) && i.F34_PictureNo.Trim().Equals(pictureNo) &&
                        (i.F34_Status.Equals(status1) || i.F34_Status.Equals(status2) || i.F34_Status.Equals(status3)))
                    .OrderBy(i => i.F34_AddDate);

            var newStatus = "";
            
            foreach (var materialWarehouseCommand in materialWarehouseCommands)
            {
                var oldStatus = materialWarehouseCommand.F34_Status;
                if (materialWarehouseCommand.F34_Status == status1)
                {
                    materialWarehouseCommand.F34_Status = Constants.F34_Status.statusC;
                }
                else if (materialWarehouseCommand.F34_Status == status2)
                    materialWarehouseCommand.F34_Status = Constants.F34_Status.statusD;
                else
                    materialWarehouseCommand.F34_Status = Constants.F34_Status.statusF;
                
                materialWarehouseCommand.F34_UpdateDate = DateTime.Now;
                _unitOfWork.MaterialWarehouseCommandRepository.Update(materialWarehouseCommand);

                var item = Mapper.Map<FirstCommunicationResponse>(materialWarehouseCommand);
                item.OldStatus = oldStatus;
                list.Add(item);
            }
            
            _unitOfWork.Commit();

            return list;
        }

        public IList<FirstCommunicationResponse> CompleteStoraging(string terminalNo, string materialCode)
        {
            var pictureNo = Constants.PictureNo.TCRM082F;
            var status1 = Constants.F34_Status.status6;
            var status2 = Constants.F34_Status.status7;
            var status3 = Constants.F34_Status.status9;

            var list = new List<FirstCommunicationResponse>();

            //find material warehouse command record
            var materialWarehouseCommands =
                _unitOfWork.MaterialWarehouseCommandRepository.GetMany(
                    i =>
                        i.F34_TerminalNo.Trim().Equals(terminalNo) && i.F34_PictureNo.Trim().Equals(pictureNo) &&
                        (i.F34_Status.Equals(status1) || i.F34_Status.Equals(status2) || i.F34_Status.Equals(status3)))
                    .OrderBy(i => i.F34_AddDate);

            var newStatus = "";

            foreach (var materialWarehouseCommand in materialWarehouseCommands)
            {
                var oldStatus = materialWarehouseCommand.F34_Status;
                if (materialWarehouseCommand.F34_Status == status1)
                {
                    materialWarehouseCommand.F34_Status = Constants.F34_Status.statusC;
                }
                else if (materialWarehouseCommand.F34_Status == status2)
                    materialWarehouseCommand.F34_Status = Constants.F34_Status.statusD;
                else
                    materialWarehouseCommand.F34_Status = Constants.F34_Status.statusF;

                materialWarehouseCommand.F34_UpdateDate = DateTime.Now;
                _unitOfWork.MaterialWarehouseCommandRepository.Update(materialWarehouseCommand);

                var item = Mapper.Map<FirstCommunicationResponse>(materialWarehouseCommand);
                item.OldStatus = oldStatus;
                list.Add(item);
            }

            _unitOfWork.Commit();

            return list;
        }
    }
}