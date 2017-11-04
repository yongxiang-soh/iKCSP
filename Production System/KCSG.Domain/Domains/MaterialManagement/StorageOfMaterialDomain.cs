using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.Domain.Models.MaterialManagement;

namespace KCSG.Domain.Domains.MaterialManagement
{
    public class StorageOfMaterialDomain : BaseDomain, IStorageOfMaterialDomain
    {
        private readonly INotificationService _notificationService;

        public StorageOfMaterialDomain(IUnitOfWork iUnitOfWork, INotificationService notificationService, IConfigurationService configurationService)
            : base(iUnitOfWork, configurationService)
        {
            _notificationService = notificationService;
        }

        public bool CheckStorageMaterialShelfStatus(string liqClass)
        {
            var emptyShelf = ((int)Constants.F31_ShelfStatus.EmptyShelf).ToString();
            
            if(liqClass == "1")
            {
                var liqShelfStatus =
                _unitOfWork.MaterialShelfStatusRepository.GetMany(
                    m =>
                        m.F31_ShelfStatus.Equals(emptyShelf) &&
                        (m.F31_LqdShfAgnOrd.HasValue));

                if (liqShelfStatus.Any())
                    return true;
            }
            else
            {
                var shelfStatus =
                _unitOfWork.MaterialShelfStatusRepository.GetMany(
                    m =>
                        m.F31_ShelfStatus.Equals(emptyShelf) &&
                        (m.F31_CmnShfAgnOrd.HasValue || m.F31_LqdShfAgnOrd.HasValue));

                if (shelfStatus.Any())
                    return true;
            }
            
            return false;
        }

        /// <summary>
        ///     Find material shelf stocks by using pallet no.
        /// </summary>
        /// <param name="palletNo"></param>
        /// <returns></returns>
        public async Task<List<TX33_MtrShfStk>> FindMaterialShelfStockByPalletNo(string palletNo)
        {
            return
                await _unitOfWork.MaterialShelfStockRepository.GetAll()
                    .Where(m => m.F33_PalletNo.Trim().Equals(palletNo))
                    .ToListAsync();
        }

        public void Store(StorageOfMaterialItem model, string terminalNo)
        {
            //Delete all [TX33_MtrShfStk] records whose [Pallet No.] is current [Pallet No.] and [F33_StockFlag] is “0” (not in stock).
            //this is old function used in communication, and you can delete it
            DeleteNotStock(model.PalletNo.Trim());

            //Delete all tx32_mtrshf records whose [Pallet No.] is current [Pallet No.].
            //this is old function used in communication, and you can delete it
            DeleteMaterialShelf(model.PalletNo.Trim());

            //	Create a new [tx32_MtrShf] record 

            var materialShelfItem = new TX32_MtrShf();
            materialShelfItem.F32_PalletNo = model.PalletNo;
            materialShelfItem.F32_PrcOrdNo = model.F30_PrcOrdNo;
            materialShelfItem.F32_PrtDvrNo = model.F30_PrtDvrNo;
            materialShelfItem.F32_MegaMsrMacSndEndFlg = ((int)Constants.TX32_MSndEndFlg.NotSend).ToString();
            materialShelfItem.F32_GnrlMsrMacSndEndFlg = ((int)Constants.TX32_CSndEndFlg.NotSend).ToString();
            materialShelfItem.F32_AddDate = DateTime.Now;
            materialShelfItem.F32_UpdateDate = DateTime.Now;
            materialShelfItem.F32_UpdateCount = 0;
            _unitOfWork.MaterialShelfRepository.AddOrUpdate(materialShelfItem);

            var material = _unitOfWork.MaterialRepository.GetById(model.F01_MaterialCode);
            //Update material shelf status table (table [tx31_mtrshfsts]) 

            //var shelfStatuses = _unitOfWork.MaterialShelfStatusRepository.GetShelfStorageMaterial(material.F01_LiquidClass);
            var shelfStatus = Constants.F31_ShelfStatus.EmptyShelf.ToString("D");
            var lstShelfStatuses =
                _unitOfWork.MaterialShelfStatusRepository.GetMany(i => i.F31_ShelfStatus.Trim().Equals(shelfStatus));

            //	[Is_LiquidClass] is pupulated from field [F01_LiquidClass] of Material Master DB record (based on current [Material Code]).

            var tm01 =
                _unitOfWork.MaterialRepository.Get(x => x.F01_MaterialCode.Trim().Equals(model.F01_MaterialCode.Trim()));

            if (tm01.F01_LiquidClass.Equals("0"))
            {
                var cmnShelfStatuses =
                _unitOfWork.MaterialShelfStatusRepository.GetMany(i => i.F31_ShelfStatus.Trim().Equals(shelfStatus) && (i.F31_CmnShfAgnOrd.HasValue || i.F31_LqdShfAgnOrd.HasValue));
                lstShelfStatuses = cmnShelfStatuses;
            }
            else
            {
                var liqShelfStatuses =
                _unitOfWork.MaterialShelfStatusRepository.GetMany(i => i.F31_ShelfStatus.Trim().Equals(shelfStatus) && i.F31_LqdShfAgnOrd.HasValue);
                lstShelfStatuses = liqShelfStatuses.OrderBy(x => x.F31_LqdShfAgnOrd.HasValue);
            }
            // update material shelf status and get F31_ShelfRow,F31_ShelfBay,F31_ShelfLevel
            var row = string.Empty;
            var bay = string.Empty;
            var level = string.Empty;
            var materialShelfStatus = lstShelfStatuses.FirstOrDefault();
            if (materialShelfStatus != null)
            //var lasttx31 = tx31s.LastOrDefault();
            //foreach (var shelfStatuses in tx31s)
            {
                materialShelfStatus.F31_ShelfStatus = "4";
                materialShelfStatus.F31_TerminalNo = terminalNo;
                materialShelfStatus.F31_UpdateDate = DateTime.Now;
                //if (shelfStatuses.Equals(lasttx31))
                //{
                materialShelfStatus.F31_StockTakingFlag = Constants.TX31_StkTkgFlg.InvNotChk.ToString("D");
                materialShelfStatus.F31_PalletNo = model.PalletNo;
                materialShelfStatus.F31_SupplierCode = string.Empty;
                materialShelfStatus.F31_LoadAmount = 0;
                materialShelfStatus.F31_Amount = Convert.ToDouble(model.GrandTotal);
                materialShelfStatus.F31_UpdateDate = DateTime.Now;
                //shelfStatuses.F31_UpdateCount += 1;

                row = materialShelfStatus.F31_ShelfRow;
                bay = materialShelfStatus.F31_ShelfBay;
                level = materialShelfStatus.F31_ShelfLevel;
                //}
                _unitOfWork.MaterialShelfStatusRepository.AddOrUpdate(materialShelfStatus);
            }

            //Update material shelf status table
            //var firstShelfStatus = lstShelfStatuses.FirstOrDefault();
            //var materialShelfStatusItem = _unitOfWork.MaterialShelfStatusRepository.Search(row, bay, level);

            //Insert record into material shelf stock table
            if (model.MaterialLotNo01 != null)
                CreateMaterialSheftStock(model.PalletNo, model.F01_MaterialCode, model.MaterialLotNo01, model.Total01);
            if (model.MaterialLotNo02 != null)
                CreateMaterialSheftStock(model.PalletNo, model.F01_MaterialCode, model.MaterialLotNo02, model.Total02);
            if (model.MaterialLotNo03 != null)
                CreateMaterialSheftStock(model.PalletNo, model.F01_MaterialCode, model.MaterialLotNo03, model.Total03);
            if (model.MaterialLotNo04 != null)
                CreateMaterialSheftStock(model.PalletNo, model.F01_MaterialCode, model.MaterialLotNo04, model.Total04);
            if (model.MaterialLotNo05 != null)
                CreateMaterialSheftStock(model.PalletNo, model.F01_MaterialCode, model.MaterialLotNo05, model.Total05);


            /*
             * 	Update [Delivered Quantity] of corresponding Material Reception based on current [P.O. No.] and [Partial Delivery] as [Delivered Quantity] + [Grand Total].
             * */

            var materialReceptions =
                _unitOfWork.ReceptionRepository.GetMany(
                    i =>
                        i.F30_PrcOrdNo.Trim().Equals(model.F30_PrcOrdNo) &&
                        i.F30_PrtDvrNo.Trim().Equals(model.F30_PrtDvrNo)).FirstOrDefault();

            //Insert record into material shelf stock table
            if (materialReceptions != null)
            {
                materialReceptions.F30_StoragedAmount = materialReceptions.F30_StoragedAmount +
                                                        Convert.ToDouble(model.GrandTotal);
                _unitOfWork.MaterialShelfStatusRepository.AddOrUpdate(materialReceptions);
            }


            //Update Or Insert tx48_nomanage which whose [f48_systemid] = “0000”
            var isNoManage = false;
            var f48_MtrWhsCmdNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNoManage,
                Constants.GetColumnInNoManager.MtrWhsCmdNo);
            //var f48_MtrWhsCmdNo = CreateOrUpdateTX48(ref isNoManage);

            //[As_ConvCode] is [f05_conveyorcode] of [tm05_conveyor] whose [f05_terminalno] is current application terminal
            var asConvCode = GetConveyorCode(terminalNo);

            //[Ls_ShelfNo] is total of [As_ Col] ,[As_ Bay] and [As_ Level] which are defined as above.
            var lsShelfNo = row + bay + level;

            //	Create a new [tx34_mtrwhscmd] record using the following pseudo code (under SQL format)
            InsertTX34(Constants.F34_CommandNo.Storage, f48_MtrWhsCmdNo, Constants.CmdType.cmdType,
                Constants.TX34_StrRtrType.Material.ToString("D"), Constants.TC_CMDSTS.TC_CMDSTS_0, model.PalletNo,
                asConvCode, lsShelfNo,
                terminalNo,
                Constants.PictureNo.TCRM031F);

            //	Send message to server C1. The content of the message contains [MsgId], [TermNo] and [PicNo],
            var msgid = "0001";
            var termNo = terminalNo;
            var picNo = Constants.PictureNo.TCRM031F;

            _notificationService.SendMessageToC1(new
            {
                msgid,
                termNo,
                picNo
            });
            _unitOfWork.Commit();
        }

        public void DeleteNotStock(string palletNo)
        {
            var notInStockFlag = Constants.TX33_MtrShfStk.NotInStock.ToString("D");
            _unitOfWork.MaterialShelfStockRepository.Delete(
                m => m.F33_PalletNo.Trim().Equals(palletNo) && m.F33_StockFlag.Equals(notInStockFlag));
            _unitOfWork.Commit();
        }

        public IQueryable<TX34_MtrWhsCmd> GetListMaterialWarehouseCommand()
        {
            var pictureNo1 = Constants.PictureNo.TCRM031F;
            var pictureNo2 = Constants.PictureNo.TCRM051F;

            var status1 = Constants.F34_Status.status6;
            var status2 = Constants.F34_Status.status7;
            var status3 = Constants.F34_Status.status8;
            var status4 = Constants.F34_Status.status9;

            //find all material warehouse command 
            var materialWarehouseCommands =
                _unitOfWork.MaterialWarehouseCommandRepository.GetMany(
                    i =>
                        (i.F34_PictureNo.Trim().Equals(pictureNo1) || i.F34_PictureNo.Trim().Equals(pictureNo2)) &&
                        (i.F34_Status.Equals(status1) || i.F34_Status.Equals(status2) || i.F34_Status.Equals(status3) ||
                         i.F34_Status.Equals(status4))).OrderBy(i => i.F34_AddDate);

            return materialWarehouseCommands;
        }

        /// <summary>
        ///     process data when receive message for C1
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="pictureNo"></param>
        public IList<FirstCommunicationResponse> PostStoreMaterial(string terminalNo, string pictureNo)
        {
            var materialWarehouseCommands =
                GetListMaterialWarehouseCommand().Where(i => i.F34_PictureNo.Trim().Equals(pictureNo.Trim()));
            var shelfRow = "";
            var shelfBay = "";
            var shelfLevel = "";

            var proceededRecordsList = new List<FirstCommunicationResponse>();

            foreach (var materialWarehouseCommand in materialWarehouseCommands)
            {
                var materialShelfStock =
                    _unitOfWork.MaterialShelfStockRepository.Get(
                        x => x.F33_PalletNo.Equals(materialWarehouseCommand.F34_PalletNo));

                var instance = Mapper.Map<FirstCommunicationResponse>(materialWarehouseCommand);
                if (materialShelfStock == null)
                    instance.MaterialCode = "";
                else
                    instance.MaterialCode = materialShelfStock.F33_MaterialCode;
                instance.OldStatus = materialWarehouseCommand.F34_Status;
                proceededRecordsList.Add(instance);

                if (materialWarehouseCommand.F34_Status.Equals(Constants.F34_Status.status6))
                {
                    materialWarehouseCommand.F34_Status = Constants.F34_Status.statusC;
                    materialWarehouseCommand.F34_UpdateDate = DateTime.Now;
                    //_unitOfWork.MaterialWarehouseCommandRepository.Update(materialWarehouseCommand);
                }
                else if (materialWarehouseCommand.F34_Status.Equals(Constants.F34_Status.status7))
                {
                    if (materialWarehouseCommand.F34_PictureNo.Trim().Equals(Constants.PictureNo.TCRM031F))
                    {
                        //find shelfRow,shelfBay,shelfLevel
                        shelfRow = materialWarehouseCommand.F34_To.Substring(0, 2);
                        shelfBay = materialWarehouseCommand.F34_To.Substring(2, 2);
                        shelfLevel = materialWarehouseCommand.F34_To.Substring(4, 2);

                        //Find material shelf status record
                        var tx31 =
                            _unitOfWork.MaterialShelfStatusRepository.Get(
                                i =>
                                    i.F31_ShelfRow.Trim().Equals(shelfRow) && i.F31_ShelfBay.Trim().Equals(shelfBay) &&
                                    i.F31_ShelfLevel.Trim().Equals(shelfLevel));

                        var tx32 =
                            _unitOfWork.MaterialShelfRepository.Get(
                                i => i.F32_PalletNo.Trim().Equals(materialWarehouseCommand.F34_PalletNo.Trim()));

                        if (tx32 != null)
                        {
                            //Find material reception record whose [P. O. No.] is [f32_prcordno] 
                            var materialReception =
                                _unitOfWork.ReceptionRepository.Get(
                                    i =>
                                        i.F30_PrcOrdNo.Trim().Equals(tx32.F32_PrcOrdNo.Trim()) &&
                                        i.F30_PrtDvrNo.Trim().Equals(tx32.F32_PrtDvrNo.Trim()));
                            //update material reception record above
                            if ((tx31 != null) && (materialReception != null))
                            {
                                var ammount = tx31.F31_Amount ?? 0;
                                materialReception.F30_StoragedAmount -= ammount;
                                materialReception.F30_UpdateDate = DateTime.Now;
                                _unitOfWork.ReceptionRepository.Update(materialReception);
                            }
                        }
                    }

                    //Update material warehouse command record above
                    materialWarehouseCommand.F34_Status = Constants.F34_Status.statusD;
                    materialWarehouseCommand.F34_UpdateDate = DateTime.Now;
                    //  _unitOfWork.MaterialWarehouseCommandRepository.Update(materialWarehouseCommand);
                }
                else if (materialWarehouseCommand.F34_Status.Equals(Constants.F34_Status.status8))
                {
                    shelfRow = materialWarehouseCommand.F34_To.Substring(0, 2);
                    shelfBay = materialWarehouseCommand.F34_To.Substring(2, 2);
                    shelfLevel = materialWarehouseCommand.F34_To.Substring(4, 2);


                    if (materialWarehouseCommand.F34_PictureNo.Equals(Constants.PictureNo.TCRM031F))
                    {
                        var materialShelfStatus =
                            _unitOfWork.MaterialShelfStatusRepository.Get(
                                i =>
                                    i.F31_ShelfRow.Trim().Equals(shelfRow) && i.F31_ShelfBay.Trim().Equals(shelfBay) &&
                                    i.F31_ShelfLevel.Trim().Equals(shelfLevel));
                        if (materialShelfStatus != null)
                        {
                            var liquiclass = materialShelfStatus.F31_LiquidClass;
                            var shelfStatus = Constants.F31_ShelfStatus.EmptyShelf.ToString("D");
                            var materialShelfStatuses = _unitOfWork.MaterialShelfStatusRepository.GetAll();
                            if (liquiclass == Constants.F31_LiquidClass.NonLiquid.ToString("D"))
                                materialShelfStatuses =
                                    materialShelfStatuses.Where(
                                        i =>
                                            i.F31_ShelfStatus.Equals(shelfStatus) && (i.F31_LiquidClass == "0") &&
                                            i.F31_CmnShfAgnOrd.HasValue).OrderBy(i => i.F31_CmnShfAgnOrd);
                            else
                                materialShelfStatuses = materialShelfStatuses.Where(
                                    i =>
                                        i.F31_ShelfStatus.Equals(shelfStatus) && (i.F31_LiquidClass == "1") &&
                                        i.F31_LqdShfAgnOrd.HasValue).OrderBy(i => i.F31_LqdShfAgnOrd);
                            foreach (var materialShelfStatuse in materialShelfStatuses)
                            {
                                materialShelfStatuse.F31_ShelfStatus =
                                    Constants.TX31SheflStatus.TX31_MtrShfSts_RsvStg.ToString("D");
                                materialShelfStatuse.F31_TerminalNo = terminalNo;
                                materialShelfStatuse.F31_PalletNo = materialWarehouseCommand.F34_PalletNo;
                                materialShelfStatuse.F31_SupplierCode = null;
                                materialShelfStatuse.F31_StockTakingFlag =
                                    Constants.TX31_StkTkgFlg.InvNotChk.ToString("d");
                                materialShelfStatuse.F31_LoadAmount = 0;
                                materialShelfStatuse.F31_UpdateDate = DateTime.Now;
                                _unitOfWork.MaterialShelfStatusRepository.AddOrUpdate(materialShelfStatuse);
                            }
                        }

                        materialWarehouseCommand.F34_Status = Constants.F34_Status.statusD;
                        materialWarehouseCommand.F34_UpdateDate = DateTime.Now;

                        _unitOfWork.MaterialWarehouseCommandRepository.Update(materialWarehouseCommand);
                    }
                    //Update or insert TX48
                    var isNoManage = false;
                    var asSer = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNoManage,
                        Constants.GetColumnInNoManager.MtrWhsCmdNo, 1, 0, 0, 0).ToString("D4");

                    //Insert data into tx34_mtrwhscmd
                    var materialWarehouseCommandItem = new TX34_MtrWhsCmd();
                    materialWarehouseCommandItem.F34_CommandNo = Constants.F34_CommandNo.TwoTimesIn.ToString("D");
                    materialWarehouseCommandItem.F34_Status = Constants.F34_Status.statusE;
                    materialWarehouseCommandItem.F34_CmdSeqNo = asSer;
                    materialWarehouseCommandItem.F34_CommandType = Constants.CommandType.CmdType_0;
                    materialWarehouseCommandItem.F34_Priority = 0;
                    materialWarehouseCommandItem.F34_PalletNo = materialWarehouseCommand.F34_PalletNo;
                    materialWarehouseCommandItem.F34_From = GetConveyorCode(terminalNo);
                    materialWarehouseCommandItem.F34_To = shelfRow + shelfBay + shelfLevel;
                    materialWarehouseCommandItem.F34_TerminalNo = terminalNo;
                    materialWarehouseCommandItem.F34_StrRtrType = materialWarehouseCommand.F34_StrRtrType;
                    materialWarehouseCommandItem.F34_PictureNo = pictureNo;
                    materialWarehouseCommandItem.F34_UpdateDate = DateTime.Now;
                    materialWarehouseCommandItem.F34_AddDate = DateTime.Now;
                    materialWarehouseCommandItem.F34_UpdateCount = 0;

                    _unitOfWork.MaterialWarehouseCommandRepository.AddOrUpdate(materialWarehouseCommandItem);


                    //Update
                    //If current screen is “TCRM031F”, the system will send message to server C1
                    var msgId = "0002";
                    var picNo = materialWarehouseCommandItem.F34_PictureNo;

                    _notificationService.SendMessageToC1(new
                    {
                        msgId,
                        terminalNo,
                        picNo
                    });
                }
            }
            _unitOfWork.Commit();

            return proceededRecordsList;
        }

        public bool CheckUnitOfMaterialDB(string materialCode, double fraction1, double fraction2, double fraction3,
            double fraction4, double fraction5)
        {
            var material = _unitOfWork.MaterialRepository.Get(i => i.F01_MaterialCode.Trim().Equals(materialCode));
            if ((material.F01_Unit != "K") &&
                ((fraction1 > material.F01_PackingUnit) || (fraction2 > material.F01_PackingUnit) ||
                 (fraction3 > material.F01_PackingUnit) || (fraction4 > material.F01_PackingUnit) ||
                 (fraction5 > material.F01_PackingUnit)))
                return false;
            return true;
        }

        public void DeleteMaterialShelf(string palletNo)
        {
            _unitOfWork.MaterialShelfRepository.Delete(m => m.F32_PalletNo.Trim().Equals(palletNo));
            _unitOfWork.Commit();
        }

        private void CreateMaterialSheftStock(string palletNo, string materialCode, string materialNo, double total)
        {
            var entity = new TX33_MtrShfStk();

            entity.F33_PalletNo = palletNo;
            entity.F33_MaterialCode = materialCode;
            entity.F33_MaterialLotNo = materialNo;
            entity.F33_Amount = total;
            entity.F33_StockFlag = Constants.TX33_StkFlg.Store.ToString("D");
            entity.F33_AddDate = DateTime.Now;
            entity.F33_UpdateDate = DateTime.Now;
            _unitOfWork.MaterialShelfStockRepository.Add(entity);
        }

        public bool CheckMaterialCode(string materialCode, string poNo, string partialDelivery)
        {
            var receptions = _unitOfWork.ReceptionRepository.GetAll();
            var materials = _unitOfWork.MaterialRepository.GetAll();
            if (!string.IsNullOrEmpty(materialCode))
                receptions =
                    receptions.Where(
                        i =>
                            i.F30_MaterialCode.Trim().Equals(materialCode.Trim()));
            if (!string.IsNullOrEmpty(poNo) && !string.IsNullOrEmpty(partialDelivery))
                receptions =
                    receptions.Where(
                        i =>
                            i.F30_PrcOrdNo.ToUpper().Contains(poNo.ToUpper()) &&
                            i.F30_PrtDvrNo.ToUpper().Contains(partialDelivery.ToUpper()));
            var result = from reception in receptions
                         join material in materials on reception.F30_MaterialCode equals material.F01_MaterialCode
                         //where
                         //(
                         //    reception.F30_MaterialCode = material.F01_MaterialCode
                         //)
                         select new
                         {
                             material.F01_MaterialCode,
                             material.F01_MaterialDsp,
                             material.F01_PackingUnit,
                             reception.F30_PrcOrdNo,
                             reception.F30_PrtDvrNo
                         };

            if (!result.Any())
                return false;
            return true;
        }
    }
}