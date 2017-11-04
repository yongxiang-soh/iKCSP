using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DocumentFormat.OpenXml.VariantTypes;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Core.Resources;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.MaterialManagement
{
    public class RetrieveOfMaterialDomain : BaseDomain, IRetrieveOfMaterialDomain
    {
        private readonly INotificationService _notificationService;

        #region Constructor

        /// <summary>
        /// Initiate domain with IoC.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="notificationService"></param>
        /// <param name="communications"></param>
        public RetrieveOfMaterialDomain(IUnitOfWork unitOfWork, INotificationService notificationService, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _notificationService = notificationService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Find material shelf stock statuses by searching their material code, quantity and termial number.
        /// </summary>
        /// <param name="materialCode"></param>
        /// <returns></returns>
        public async Task<IQueryable<TX31_MtrShfSts>> FindMaterialShelfStatusesAsync(string materialCode)
        {
            var shelfStatus = Constants.F31_ShelfStatus.Material.ToString("D");
            var acceptClass = Constants.F30_AcceptClass.TX30_AcpCls_Acp.ToString("D");

            //find material reception record with F30_MaterialCode is current [Material Code]
            var materialReceptions =
                _unitOfWork.ReceptionRepository.GetAll()
                    .Where(i => i.F30_MaterialCode.Trim().Equals(materialCode.Trim()) && i.F30_AcceptClass.Trim().Equals(acceptClass));

            //Find  tx31_mtrshfsts record with  [f31_shelfstatus] of [tx31_mtrshfsts] is 3.
            var materialShelfStatuses =
                _unitOfWork.MaterialShelfStatusRepository.GetAll()
                .Where(i => i.F31_ShelfStatus.Trim().Equals(shelfStatus));

            //Find tx32_mtrshf record
            var materialShelfs = _unitOfWork.MaterialShelfRepository.GetAll();


            var result = from materialReception in materialReceptions
                         join materialShelf in materialShelfs on new { a = materialReception.F30_PrcOrdNo, b = materialReception.F30_PrtDvrNo }
                         equals new { a = materialShelf.F32_PrcOrdNo, b = materialShelf.F32_PrtDvrNo }
                         join materialShelfStatus in materialShelfStatuses on materialShelf.F32_PalletNo.Trim() equals materialShelfStatus.F31_PalletNo.Trim()
                         //where
                         //(
                         //    (materialReception.F30_PrtDvrNo.Equals(materialShelf.F32_PrtDvrNo) &&
                         //    materialReception.F30_PrcOrdNo.Equals(materialShelf.F32_PrcOrdNo)) &&
                         //    materialShelf.F32_PalletNo.Equals(materialShelfStatus.F31_PalletNo)
                         //    //materialReception.F30_MaterialCode.Trim().Equals(materialCode.Trim()) &&
                         //    //materialReception.F30_AcceptClass.Equals(acceptClass) &&
                         //    //materialShelfStatus.F31_ShelfStatus.Equals(shelfStatus)
                         //)                         
                         orderby new
                         {
                             materialShelfStatus.F31_StorageDate
                         }
                         select materialShelfStatus;
            return result;
        }

        /// <summary>
        ///     Retrieve or assign pallet information.
        ///     Refer BR 30 - SRS 1.1 for more information.
        /// </summary>
        /// <param name="materialCode"></param>
        /// <param name="quantity"></param>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        public async Task<ResponseResult<AssignPalletItem>> RetrieveOrAssignPallet(string materialCode, double quantity,
            string terminalNo)
        {
            var result = await FindMaterialShelfStatusesAsync(materialCode);

            var shelfStatus = Constants.F31_ShelfStatus.Material.ToString("D");
            double? totalAmount = 0;
            double? amount = 0;

            foreach (var item in result)
            {
                // Check F31_ShelfRow,F31_ShelfBay,F31_ShelfLevel exits
                var materialShelfStatusItem =
                    _unitOfWork.MaterialShelfStatusRepository.GetAll()
                        .FirstOrDefault(
                            i => i.F31_ShelfRow.Equals(item.F31_ShelfRow) && i.F31_ShelfBay.Equals(item.F31_ShelfBay) &&
                                 i.F31_ShelfLevel.Equals(item.F31_ShelfLevel) && i.F31_ShelfStatus.Trim().Equals(shelfStatus));


                if (materialShelfStatusItem == null)
                {
                    return new ResponseResult<AssignPalletItem>(null, false, "MSG43-MSG47");

                }

                item.F31_ShelfStatus =
                    Constants.F37_ShelfStatus.ReservedForRetrieval.ToString("D");
                item.F31_TerminalNo = terminalNo;
                item.F31_UpdateDate = DateTime.Now;

                // Count the number of updated record.
                _unitOfWork.MaterialShelfStatusRepository.Update(item);

                totalAmount += item.F31_Amount;

                //amount = item.F31_Amount;
                if (totalAmount >= quantity)
                    break;
                //return null;
            }
            // Count the number of records which have been updated.
            var updatedRecordsCounter = await _unitOfWork.CommitAsync();
            if (updatedRecordsCounter < 1)
                return new ResponseResult<AssignPalletItem>(null, false, "MSG43-MSG47");

            var assignPalletItem = new AssignPalletItem
            {
                Tallet = totalAmount ?? 0,
                Total = updatedRecordsCounter
            };

            return new ResponseResult<AssignPalletItem>(assignPalletItem, true);
        }

        /// <summary>
        ///     Find Pallets List
        ///     Refer Br32 - SRS 1.1 for more information.
        /// </summary>
        /// <param name="materialCode"></param>
        /// <param name="quantity"></param>
        /// <param name="terminalNo"></param>
        /// <param name="gridSettings"></param>
        /// <returns></returns>
        public async Task<ResponseResult<GridResponse<PalletGridDetail>>> FindPalletsList(string materialCode,
            double quantity, string terminalNo, GridSettings gridSettings)
        {
            var materialShelfStatus = await FindMaterialShelfStatusesForDetailAsync(materialCode);

            //var t = result.ToList();
            //var shelfStatus = Constants.F31_ShelfStatus.ReservedForRetrieval.ToString("D");

            //var materialShelfStatus = result.Where(i => i.F31_ShelfStatus.Trim().Equals(shelfStatus));

            var itemCount = materialShelfStatus.Count();

            materialShelfStatus =
                materialShelfStatus.Skip((gridSettings.PageIndex - 1) * gridSettings.PageSize).Take(gridSettings.PageSize);

            var lstResult =
                Mapper.Map<IEnumerable<TX31_MtrShfSts>, IEnumerable<PalletGridDetail>>(materialShelfStatus.ToList());
            var resultModel = new GridResponse<PalletGridDetail>(lstResult, itemCount);
            return new ResponseResult<GridResponse<PalletGridDetail>>(resultModel, true);
        }
    
        ///// <summary>
        /////     Find pallet Details-Find 5 record in table material shelf stock
        /////     Refer Br32 - SRS 1.1 for more information.
        ///// </summary>
        ///// <param name="palletNo"></param>
        ///// <returns></returns>
        //public async Task<IList<TX33_MtrShfStk>> FindPalletDetails(string palletNo)
        //{
        //    //Get TX33_MtrShfStk record with F33_PalletNo is palletNo
        //    var materialShelfStocks =
        //        _unitOfWork.MaterialShelfStockRepository.GetAll()
        //            .Where(i => i.F33_PalletNo.Trim().Equals(palletNo.Trim()));
        //    var lstResult = new List<TX33_MtrShfStk>();
        //    var isCount = 0;
        //    foreach (var materialShelfStock in materialShelfStocks)
        //    {
        //        isCount++;
        //        lstResult.Add(materialShelfStock);
        //        if (isCount == 5)
        //            break;
        //    }
        //    return lstResult;
        //}

        /// <summary>
        /// Calculate Tally
        /// Refer Br33 - SRS 1.1 for more information.
        /// </summary>
        /// <param name="materialCode"></param>
        /// <returns></returns>
        public async Task<double> ReCalculateTally(string materialCode)
        {
            //find all material shelf status record with paramater is materialCode with f31_shelfstatus is 5
            var result = await FindMaterialShelfStatusesForDetailAsync(materialCode);

            double? tally = 0;

            var shelfStatus = Constants.F37_ShelfStatus.ReservedForRetrieval.ToString("D");

            //Caculate tally as total [f31_Amount] of result
            foreach (var item in result)
            {
                tally += item.F31_Amount;
            }

            return tally ?? 0;
        }

        /// <summary>
        ///     Find Pallets List
        ///     Refer Br33 - SRS 1.1 for more information.
        /// </summary>
        /// <param name="shelfRow"></param>
        /// <param name="shelfBay"></param>
        /// <param name="shelfLevel"></param>
        /// <returns></returns>
        public async Task UnassignSpecificPallet(string shelfRow, string shelfBay, string shelfLevel)
        {
            //get material shelf status record
            var materialShelfStatus =
                _unitOfWork.MaterialShelfStatusRepository.GetAll()
                    .FirstOrDefault(
                        i =>
                            i.F31_ShelfRow.Trim().Equals(shelfRow.Trim()) &&
                            i.F31_ShelfBay.Trim().Equals(shelfBay.Trim()) &&
                            i.F31_ShelfLevel.Trim().Equals(shelfLevel.Trim()));

            //update data into material shelf status
            if (materialShelfStatus != null)
            {
                materialShelfStatus.F31_ShelfStatus = Constants.F31_ShelfStatus.Material.ToString("D");
                materialShelfStatus.F31_TerminalNo = null;

                _unitOfWork.MaterialShelfStatusRepository.Update(materialShelfStatus);
            }

            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// Retrieval Material
        /// Refer Br35 - SRS1.1 for more information
        /// </summary>
        /// <param name="materialCode"></param>
        /// <param name="quantity"></param>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        public async Task RetrieveMaterial(string materialCode, double quantity, string terminalNo)
        {

            //get record in Actionable item
            var materialStatus = (await FindMaterialShelfStatusesForDetailAsync(materialCode)).OrderByDescending(x => x.F31_StorageDate).FirstOrDefault();
            
            //Get palletNo
            if (materialStatus == null)
                throw new Exception(HttpMessages.InvalidMaterialStatus);

            // Find pallet number from material status.
            var palletNo = materialStatus.F31_PalletNo.Trim();

            var shelf = materialStatus.F31_ShelfRow + materialStatus.F31_ShelfBay + materialStatus.F31_ShelfLevel;
            //find material shelf stock with f33_palletno is f31_palletno 
            var materialShelfStocks =
                _unitOfWork.MaterialShelfStockRepository.GetAll()
                    .Where(i => i.F33_PalletNo.Trim().Equals(palletNo));

            var lsConveyorCode = GetConveyorCode(terminalNo);
            foreach (var materialShelfStock in materialShelfStocks)
            {
                materialShelfStock.F33_StockFlag = Constants.F33_StockFlag.TX33_StkFlg_Rtr;
                _unitOfWork.MaterialShelfStockRepository.Update(materialShelfStock);
            }
            var isNoManage = false;
            var f48MtrWhsCmdNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNoManage,
                  Constants.GetColumnInNoManager.MtrWhsCmdNo, 1);

            var tx34Mtrwhscmd = InsertTX34(Constants.F34_CommandNo.Retrieval, f48MtrWhsCmdNo,
           Constants.CmdType.cmdType,
           Constants.TX34_StrRtrType.Material.ToString("D"),
           Constants.TC_CMDSTS.TC_CMDSTS_0, palletNo, shelf, lsConveyorCode, terminalNo,
           Constants.PictureNo.TCRM071F);

            //[MsgId] is “0011”.
            var msgId = "2000";
            //[PicNo] is [f34_pictureNo]
            var picNo = tx34Mtrwhscmd.F34_PictureNo;
            _notificationService.SendMessageToC1(new
            {
                msgId,
                terminalNo,
                picNo
            });




            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        ///     Unassign Pallets List
        ///     Refer BR 31 - SRS 1.1 for more information.
        /// </summary>
        /// <param name="materialCode"></param>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        public async Task UnassignPalletsList(string materialCode, string terminalNo)
        {
            ////find material reception record
            //var reception =
            //    _unitOfWork.ReceptionRepository.GetAll()
            //        .FirstOrDefault(i => i.F30_MaterialCode.Trim().Equals(materialCode));

            //var shelfStatus = Constants.F31_ShelfStatus.ReservedForRetrieval.ToString("D");

            ////Find tx32_mtrshf record
            //var materialShelfs =
            //    _unitOfWork.MaterialShelfRepository.GetAll()
            //        .Where(
            //            i =>
            //                i.F32_PrcOrdNo.Trim().Equals(reception.F30_PrcOrdNo.Trim()) &&
            //                i.F32_PrtDvrNo.Trim().Equals(reception.F30_PrtDvrNo.Trim()));

            //foreach (var materialShelf in materialShelfs)
            //{
            //    //Get material shelf status record
            //    var materialShelfStatus =
            //        _unitOfWork.MaterialShelfStatusRepository.GetAll().Where(
            //                i =>
            //                    i.F31_PalletNo.Trim().Equals(materialShelf.F32_PalletNo.Trim()) &&
            //                    i.F31_ShelfStatus.Trim().Equals(shelfStatus)).ToList();

            //    //Update data in record
            //    foreach (var materialShelfStatu in materialShelfStatus)
            //    {
            //        materialShelfStatu.F31_ShelfStatus = Constants.F31_ShelfStatus.Material.ToString("D");
            //        materialShelfStatu.F31_TerminalNo = "";
            //        materialShelfStatu.F31_UpdateDate = DateTime.Now;

            //        _unitOfWork.MaterialShelfStatusRepository.Update(materialShelfStatu);
            //    }
            //}

            var materials = await FindMaterialShelfStatusesForDetailAsync(materialCode);
            foreach (var material in materials)
            {
                if (!terminalNo.Equals(material.F31_TerminalNo.Trim()))
                    continue;

                if (Constants.F31_ShelfStatus.ReservedForRetrieval.ToString("D").Equals(material.F31_ShelfStatus.Trim()))
                    material.F31_ShelfStatus = Constants.F31_ShelfStatus.Material.ToString("D");

                material.F31_TerminalNo = "";
                material.F31_UpdateDate = DateTime.Now;
            }

            // Save changes to database asynchronously.
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        ///     Post Retrieve Material when C1 return notification
        ///     Refer Br36 - SRS 1.1 for more infomation
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="materialCode"></param>
        /// <returns></returns>
        public async Task<IList<FirstCommunicationResponse>> PostRetrieveMaterial(string terminalNo, string materialCode)
        {
            var pictureNo = Constants.PictureNo.TCRM071F;
            var status1 = Constants.F34_Status.status6;
            var status2 = Constants.F34_Status.status7;
            var status3 = Constants.F34_Status.status9;

            var items = new List<FirstCommunicationResponse>();

            //find material warehouse command record with f34_terminalNo is current terminalNo and f31_pictureno is "TCRM071F" and f31_status is 6,7 or 8
            var materialWarehouseCommands =
                _unitOfWork.MaterialWarehouseCommandRepository.GetAll()
                    .Where(
                        i =>
                            i.F34_TerminalNo.Trim().Equals(terminalNo) && i.F34_PictureNo.Trim().Equals(pictureNo) &&
                            (i.F34_Status.Equals(status1) || i.F34_Status.Equals(status2) ||
                             i.F34_Status.Equals(status3))).OrderBy(i => i.F34_AddDate);

            foreach (var materialWarehouseCommand in materialWarehouseCommands)
            {
                var item = Mapper.Map<FirstCommunicationResponse>(materialWarehouseCommand);
                item.OldStatus = materialWarehouseCommand.F34_Status;
                item.MaterialCode = materialCode;
                switch (materialWarehouseCommand.F34_Status)
                {
                    case "6":
                        materialWarehouseCommand.F34_Status = Constants.F34_Status.statusC;
                        break;
                    case "7":
                        materialWarehouseCommand.F34_Status = Constants.F34_Status.statusD;
                        break;
                    case "9":
                        materialWarehouseCommand.F34_Status = Constants.F34_Status.statusF;
                        break;
                }


                materialWarehouseCommand.F34_UpdateDate = DateTime.Now;

                items.Add(item);
            }

            await _unitOfWork.CommitAsync();
            return items;
        }

        public async Task<bool> CheckAssignPallet(string materialCode)
        {
            var result = await FindMaterialShelfStatusesAsync(materialCode);
            var shelftStatus = Constants.F37_ShelfStatus.ReservedForRetrieval.ToString("D");
            return result.Any(i => i.F31_ShelfStatus == shelftStatus);
        }

        /// <summary>
        ///     Find material shelf stock statuses = 5 by searching their material code, quantity and termial number.
        /// </summary>
        /// <param name="materialCode"></param>
        /// <returns></returns>
        public async Task<IQueryable<TX31_MtrShfSts>> FindMaterialShelfStatusesForDetailAsync(string materialCode)
        {
            //find material reception record with F30_MaterialCode is current [Material Code]
            var materialReceptions =
                _unitOfWork.ReceptionRepository.GetAll()
                    .Where(i => i.F30_MaterialCode.Trim().Equals(materialCode.Trim()));

            //Find  tx31_mtrshfsts record with  [f31_shelfstatus] of [tx31_mtrshfsts] is 5.
            var materialShelfStatuses =
                _unitOfWork.MaterialShelfStatusRepository.GetAll();

            //Find tx32_mtrshf record
            var materialShelfs = _unitOfWork.MaterialShelfRepository.GetAll();
            var shelfStatus = Constants.F31_ShelfStatus.ReservedForRetrieval.ToString("D");
            var acceptClass = Constants.F30_AcceptClass.TX30_AcpCls_Acp.ToString("D");

            var result = from materialReception in materialReceptions
                         from materialShelfStatus in materialShelfStatuses
                         from materialShelf in materialShelfs
                         where
                         (
                             materialReception.F30_MaterialCode.Trim().Equals(materialCode) &&
                             materialReception.F30_AcceptClass.Equals(acceptClass) &&
                             materialReception.F30_PrtDvrNo.Equals(materialShelf.F32_PrtDvrNo) &&
                             materialReception.F30_PrcOrdNo.Equals(materialShelf.F32_PrcOrdNo) &&
                             materialShelf.F32_PalletNo.Equals(materialShelfStatus.F31_PalletNo) &&
                             materialShelfStatus.F31_ShelfStatus.Equals(shelfStatus)
                         )
                         orderby new
                         {
                             materialShelfStatus.F31_StorageDate
                         }
                         select materialShelfStatus;
            return result;
        }
        #endregion
    }
}