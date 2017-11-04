using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.ProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.ProductManagement
{
    public class ForceRetrievalOfProductDomain : BaseDomain, IForcedRetrievalOfProductDomain
    {
        #region Properties

        /// <summary>
        /// </summary>
        private readonly INotificationService _notificationService;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initiate domain with IoC.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="notificationService"></param>
        /// <param name="configurationService"></param>
        public ForceRetrievalOfProductDomain(IUnitOfWork unitOfWork, INotificationService notificationService, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _notificationService = notificationService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Check Tx40 record exists
        ///     refer br40 srs product management sub system 1.1
        /// </summary>
        /// <param name="productCode"></param>
        /// <param name="productLotNo"></param>
        /// <returns></returns>
        public bool CheckRecordExistsFromTX40(string productCode, string productLotNo)
        {
            var stockFlag = Constants.F40_StockFlag.TX40_StkFlg_Stk;
            var productShelfStocks =
                _unitOfWork.ProductShelfStockRepository.GetMany(
                    i =>
                        i.F40_ProductCode.Trim().Equals(productCode) && i.F40_ProductLotNo.Trim().Equals(productLotNo) &&
                        i.F40_StockFlag.Equals(stockFlag));
            if (productShelfStocks.Any())
                return true;
            return false;
        }

        public bool CheckRecordExistsFormTX40AndTX57(string productCode, string productLotNo)
        {
            var productShelfStocks =
                _unitOfWork.ProductShelfStockRepository.GetMany(i => i.F40_Amount > i.F40_ShippedAmount + 0.005);
            var productShelfs = _unitOfWork.ProductShelfRepository.GetAll();

            var stockFlag = Constants.F40_StockFlag.TX40_StkFlg_Stk;

            var result = from productShelfStock in productShelfStocks
                         from productShelf in productShelfs
                         where productShelfStock.F40_PalletNo.Trim().Equals(productShelf.F57_PalletNo.Trim()) &&
                               productShelfStock.F40_ProductCode.Trim().Equals(productCode) &&
                               productShelfStock.F40_ProductLotNo.Trim().StartsWith(productLotNo) &&
                               productShelfStock.F40_StockFlag.Equals(stockFlag)
                         orderby new
                         {
                             productShelfStock.F40_TabletingEndDate,
                             productShelf.F57_StorageDate
                         }
                         select productShelfStock;
            if (result.Any())
                return true;
            return false;

        }

        public bool CheckRecordExistsFromTX40ForButtonRetrieval(string productCode, string productLotNo)
        {
            var stockFlag = Constants.F40_StockFlag.TX40_StkFlg_Shipping;
            var productShelfStocks =
                _unitOfWork.ProductShelfStockRepository.GetMany(
                    i =>
                        i.F40_ProductCode.Trim().Equals(productCode) && i.F40_ProductLotNo.Trim().Equals(productLotNo) &&
                        i.F40_StockFlag.Equals(stockFlag));
            if (productShelfStocks.Any())
                return true;
            return false;
        }

        public ResponseResult<GridResponse<TempTableItem>> GetData(string productCode, string productLotNo, bool isPallet, double requestRetrievalQuantity,
            GridSettings gridSettings)
        {
            var productShelfStocks =
                _unitOfWork.ProductShelfStockRepository.GetMany(i => i.F40_Amount > i.F40_ShippedAmount + 0.005);
            var productShelfs = _unitOfWork.ProductShelfRepository.GetAll();

            var stockFlag = Constants.F40_StockFlag.TX40_StkFlg_Stk;

            var result = from productShelfStock in productShelfStocks
                         from productShelf in productShelfs
                         where productShelfStock.F40_PalletNo.Trim().Equals(productShelf.F57_PalletNo.Trim()) &&
                               productShelfStock.F40_ProductCode.Trim().Equals(productCode) &&
                               productShelfStock.F40_ProductLotNo.Trim().StartsWith(productLotNo) &&
                               productShelfStock.F40_StockFlag.Equals(stockFlag)
                         orderby new
                         {
                             productShelfStock.F40_TabletingEndDate,
                             productShelf.F57_StorageDate
                         }
                         select productShelfStock;

            var tempTables = new List<TempTableItem>();


            foreach (var productShelfStockItem in result)
            {
                var palletNo = productShelfStockItem.F40_PalletNo;
                var remainingAmount = Math.Round(productShelfStockItem.F40_Amount - productShelfStockItem.F40_ShippedAmount, 3);

                var shelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_Pdt;

                var productShelfStatuses =
                    _unitOfWork.ProductShelfStatusRepository.GetMany(
                        i => i.F51_PalletNo.Trim().Equals(palletNo.Trim()))
                        .OrderBy(x => x.F51_StorageDate)
                        .Select(x => x);

                //var requestedQuantity = 0;
                if (productShelfStatuses.Any(i => i.F51_ShelfStatus.Equals(shelfStatus)))
                {
                    productShelfStatuses = productShelfStatuses.Where(i => i.F51_ShelfStatus.Equals(shelfStatus));
                    
                    foreach (var productShelfStatus in productShelfStatuses)
                    {
                        var tempTable = new TempTableItem();

                        var dSum = tempTables.Sum(x => x.RemainingAmount);
                        if (dSum >= requestRetrievalQuantity)
                            continue;

                        tempTable.F51_ShelfRow = productShelfStatus.F51_ShelfRow;
                        tempTable.F51_ShelfBay = productShelfStatus.F51_ShelfBay;
                        tempTable.F51_ShelfLevel = productShelfStatus.F51_ShelfLevel;
                        tempTable.F51_PalletNo = productShelfStatus.F51_PalletNo;
                        tempTable.F51_ShelfStatus = productShelfStatus.F51_ShelfStatus;
                        tempTable.RemainingAmount = remainingAmount;
                        tempTables.Add(tempTable);


                        productShelfStatus.F51_ShelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_RsvRtr;
                        productShelfStatus.F51_TerminalNo = Constants.TerminalNo.A003;
                        productShelfStatus.F51_UpdateDate = DateTime.Now;
                        _unitOfWork.ProductShelfStatusRepository.Update(productShelfStatus);

                        //break;
                    }
                }
                else
                {
                    var status = Constants.F51_ShelfStatus.TX51_ShfSts_RsvRtr;
                    productShelfStatuses = _unitOfWork.ProductShelfStatusRepository.GetMany(
                        i => i.F51_PalletNo.Trim().Equals(palletNo.Trim()) && i.F51_ShelfStatus.Equals(status));

                    foreach (var productShelfStatus in productShelfStatuses)
                    {
                        var dSum = tempTables.Sum(x => x.RemainingAmount);
                        if (dSum >= requestRetrievalQuantity)
                            continue;

                        var tempTable = new TempTableItem();
                        tempTable.F51_ShelfRow = productShelfStatus.F51_ShelfRow;
                        tempTable.F51_ShelfBay = productShelfStatus.F51_ShelfBay;
                        tempTable.F51_ShelfLevel = productShelfStatus.F51_ShelfLevel;
                        tempTable.F51_PalletNo = productShelfStatus.F51_PalletNo;
                        tempTable.F51_ShelfStatus = productShelfStatus.F51_ShelfStatus;
                        tempTable.RemainingAmount = remainingAmount;
                        tempTables.Add(tempTable);
                        //break;
                    }
                }

                //update data in TX40

                productShelfStockItem.F40_StockFlag = Constants.F40_StockFlag.TX40_StkFlg_Shipping;
                productShelfStockItem.F40_ShipCommandNo = "";
                if (isPallet)
                    productShelfStockItem.F40_AssignAmount = remainingAmount;
                else
                    productShelfStockItem.F40_AssignAmount = requestRetrievalQuantity > remainingAmount
                        ? remainingAmount
                        : requestRetrievalQuantity;

                productShelfStockItem.F40_UpdateDate = DateTime.Now;

                _unitOfWork.ProductShelfStockRepository.Update(productShelfStockItem);
                //break;
            }

            _unitOfWork.Commit();
            var itemCount = tempTables.Count();

            var tempTableQueryable = tempTables.AsQueryable();
            if (gridSettings != null)
                OrderByAndPaging(ref tempTableQueryable, gridSettings);

            // var lstResult = Mapper.Map<IEnumerable<TX51_PdtShfSts>, IEnumerable<TempTableItem>>(tempTableQueryable);

            var resultModel = new GridResponse<TempTableItem>(tempTableQueryable, itemCount);

            return new ResponseResult<GridResponse<TempTableItem>>(resultModel, true);
        }

        public double GetTotalTally(string productCode, string productLotNo, bool isPallet,
            double requestRetrievalQuantity)
        {
            var productShelfStocks =
                _unitOfWork.ProductShelfStockRepository.GetMany(i => i.F40_Amount > i.F40_ShippedAmount + 0.005);

            var productShelfs = _unitOfWork.ProductShelfRepository.GetAll();

            var stockFlag = Constants.F40_StockFlag.TX40_StkFlg_Stk;

            var lstResult = from productShelfStock in productShelfStocks
                            from productShelf in productShelfs
                            where productShelfStock.F40_PalletNo.Trim().Equals(productShelf.F57_PalletNo.Trim()) &&
                                  productShelfStock.F40_ProductCode.Trim().Equals(productCode) &&
                                  productShelfStock.F40_ProductLotNo.Trim().StartsWith(productLotNo) &&
                                  productShelfStock.F40_StockFlag.Equals(stockFlag)
                            orderby new
                            {
                                productShelfStock.F40_TabletingEndDate,
                                productShelf.F57_StorageDate
                            }
                            select productShelfStock;
            double tally = 0;
            var requestQuantity = requestRetrievalQuantity;

            foreach (var result in lstResult)
            {
                var remainingAmount = Math.Round(result.F40_Amount - result.F40_ShippedAmount, 3);

                requestQuantity -= remainingAmount;

            }
            return requestQuantity;
        }

        /// <summary>
        ///     De-assigning 1 Pallet Rules:
        ///     refer br46 - srs  product management sub system v1.1
        /// </summary>
        /// <param name="shelfNo"></param>
        /// <param name="palletNo"></param>
        /// <returns></returns>
        public void DeasigningPallet(string shelfNo, string palletNo)
        {
            if (!string.IsNullOrEmpty(palletNo))
            {
                var stockFlag = Constants.F40_StockFlag.TX40_StkFlg_Shipping;
                var productShelfStocks =
                    _unitOfWork.ProductShelfStockRepository.GetMany(
                        i =>
                            i.F40_PalletNo.Trim().Equals(palletNo.Trim()) && i.F40_StockFlag.Equals(stockFlag) &&
                            (i.F40_AssignAmount > 0));
                foreach (var productShelfStock in productShelfStocks)
                {
                    productShelfStock.F40_StockFlag = Constants.F40_StockFlag.TX40_StkFlg_Stk;
                    productShelfStock.F40_AssignAmount = 0;
                    productShelfStock.F40_UpdateDate = DateTime.Now;

                    _unitOfWork.ProductShelfStockRepository.Update(productShelfStock);
                }
            }


            var shelfRow = shelfNo.Split('-')[0];
            var shelfBay = shelfNo.Split('-')[1];
            var shelfLevel = shelfNo.Split('-')[2];
            var productShelfStatus =
                _unitOfWork.ProductShelfStatusRepository.GetMany(
                    i =>
                        i.F51_ShelfRow.Trim().Equals(shelfRow) &&
                        i.F51_ShelfBay.Trim().Equals(shelfBay) &&
                        i.F51_ShelfLevel.Trim().Equals(shelfLevel)).FirstOrDefault();
            if (productShelfStatus != null)
            {
                productShelfStatus.F51_ShelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_Pdt;
                productShelfStatus.F51_TerminalNo = "";
                productShelfStatus.F51_UpdateDate = DateTime.Now;
                _unitOfWork.ProductShelfStatusRepository.Update(productShelfStatus);
            }
            _unitOfWork.Commit();
        }

        /// <summary>
        /// Unassign pallet from product warehouse.
        /// </summary>
        /// <param name="productCode"></param>
        /// <param name="palletNo"></param>
        public void UnassignPallet(string productCode, string productLotNo)
        {
            // Find all product shelf stocks.
            var productShelfStocks =
                _unitOfWork.ProductShelfStockRepository.GetAll();

            // Find product shelf statuses.
            var productShelfStatuses = _unitOfWork.ProductShelfStatusRepository.GetAll();

            productShelfStocks = productShelfStocks.Where(x => x.F40_ProductCode.Trim().Equals(productCode) &&
                                       x.F40_ProductLotNo.Trim().Equals(productLotNo, StringComparison.InvariantCultureIgnoreCase));

            // Product shelf status.
            var shelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_RsvRtr;

            var filteredProductShelfStatuses = from productShelfStock in productShelfStocks
                                               from productShelfStatus in productShelfStatuses
                                               where productShelfStock.F40_PalletNo.Equals(productShelfStatus.F51_PalletNo)
                                               && productShelfStatus.F51_ShelfStatus.Trim().Equals(shelfStatus)
                                               select productShelfStatus;

            foreach (var productShelfStatus in filteredProductShelfStatuses)
                productShelfStatus.F51_ShelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_Pdt;

            foreach (var productShelfStock in productShelfStocks)
                productShelfStock.F40_StockFlag = Constants.F40_StockFlag.TX40_StkFlg_Stk;

            //foreach (var productShelfStockItem in result)
            //{
            //    var palletNo = productShelfStockItem.F40_PalletNo;

            //    var productShelfStatuses =
            //        _unitOfWork.ProductShelfStatusRepository.GetMany(
            //            i => i.F51_PalletNo.Trim().Equals(palletNo.Trim()));

            //        productShelfStatuses = productShelfStatuses.Where(i => i.F51_ShelfStatus.Equals(shelfStatus));
            //        foreach (var productShelfStatus in productShelfStatuses)
            //        {
            //            var tempTable = new TempTableItem();
            //            tempTable.F51_ShelfRow = productShelfStatus.F51_ShelfRow;
            //            tempTable.F51_ShelfBay = productShelfStatus.F51_ShelfBay;
            //            tempTable.F51_ShelfLevel = productShelfStatus.F51_ShelfLevel;
            //            tempTable.F51_PalletNo = productShelfStatus.F51_PalletNo;
            //            tempTable.F51_ShelfStatus = productShelfStatus.F51_ShelfStatus;
            //            tempTable.RemainingAmount = remainingAmount;
            //            tempTables.Add(tempTable);

            //            productShelfStatus.F51_ShelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_RsvRtr;
            //            productShelfStatus.F51_TerminalNo = Constants.TerminalNo.A003;
            //            productShelfStatus.F51_UpdateDate = DateTime.Now;
            //            _unitOfWork.ProductShelfStatusRepository.Update(productShelfStatus);

            //        }
            //    }

            //    //update data in TX40

            //    productShelfStockItem.F40_StockFlag = Constants.F40_StockFlag.TX40_StkFlg_Shipping;
            //    productShelfStockItem.F40_ShipCommandNo = "";
            //    if (isPallet)
            //        productShelfStockItem.F40_AssignAmount = remainingAmount;
            //    else
            //        productShelfStockItem.F40_AssignAmount = requestRetrievalQuantity > remainingAmount
            //            ? remainingAmount
            //            : requestRetrievalQuantity;

            //    productShelfStockItem.F40_UpdateDate = DateTime.Now;

            //    _unitOfWork.ProductShelfStockRepository.Update(productShelfStockItem);
            //}

            //_unitOfWork.Commit();
            //var itemCount = tempTables.Count();

            //var tempTableQueryable = tempTables.AsQueryable();
            //if (gridSettings != null)
            //    OrderByAndPaging(ref tempTableQueryable, gridSettings);

            //// var lstResult = Mapper.Map<IEnumerable<TX51_PdtShfSts>, IEnumerable<TempTableItem>>(tempTableQueryable);

            //var resultModel = new GridResponse<TempTableItem>(tempTableQueryable, itemCount);

            //return new ResponseResult<GridResponse<TempTableItem>>(resultModel, true);
            _unitOfWork.Commit();

        }

        public string GetPalletNo(string productCode, string productLotNo)
        {
            var stockFlag = Constants.F40_StockFlag.TX40_StkFlg_NotStk;
            var productShelfStocks =
                _unitOfWork.ProductShelfStockRepository.GetMany(
                    i =>
                        i.F40_ProductCode.Equals(productCode) && i.F40_ProductLotNo.Equals(productLotNo) &&
                        i.F40_StockFlag.Equals(stockFlag));

            var palletNo = "";

            if (productShelfStocks.Any())
                palletNo = productShelfStocks.FirstOrDefault().F40_PalletNo;
            return palletNo;
        }

        /// <summary>
        ///     De-assigning All Assigned Pallet Rules:
        ///     refer br47-srs product management sub system v1.1
        /// </summary>
        /// <param name="lstPalletNo"></param>
        /// <param name="productCode"></param>
        /// <param name="productLotNo"></param>
        /// <param name="terminalNo"></param>
        public void DeAssignAllAssignedPallet(string lstPalletNo, string productCode, string productLotNo,
            string terminalNo)
        {
            lstPalletNo = lstPalletNo.TrimEnd('-');
            var arrPalletNo = lstPalletNo.Split('-');
            var stockFlag = Constants.F40_StockFlag.TX40_StkFlg_Shipping;

            var count = arrPalletNo.Length;
            for (var j = 0; j < arrPalletNo.Length; j++)
            {
                var palletNo = arrPalletNo[j].Trim();
                var productShelfStocks =
                    _unitOfWork.ProductShelfStockRepository.GetMany(
                        i =>
                            i.F40_PalletNo.Trim().Equals(palletNo) && i.F40_ProductCode.Trim().Equals(productCode) &&
                            i.F40_ProductLotNo.Trim().StartsWith(productLotNo) && i.F40_StockFlag.Equals(stockFlag) &&
                            (i.F40_AssignAmount > 0));

                foreach (var productShelfStock in productShelfStocks)
                {
                    productShelfStock.F40_StockFlag = Constants.F40_StockFlag.TX40_StkFlg_Stk;
                    productShelfStock.F40_AssignAmount = 0;
                    productShelfStock.F40_UpdateDate = DateTime.Now;

                    _unitOfWork.ProductShelfStockRepository.Update(productShelfStock);
                }
            }

            var sheftStatus = Constants.F51_ShelfStatus.TX51_ShfSts_RsvRtr;
            var productShelfStatuses =
                _unitOfWork.ProductShelfStatusRepository.GetMany(
                    i => i.F51_TerminalNo.Trim().Equals(terminalNo) && i.F51_ShelfStatus.Equals(sheftStatus));

            foreach (var productShelfStatuse in productShelfStatuses)
                _unitOfWork.ProductShelfStatusRepository.UpdateProductShelfStatus(productShelfStatuse,
                    Constants.F51_ShelfStatus.TX51_ShfSts_Pdt, "");
            _unitOfWork.Commit();
        }

        /// <summary>
        ///     user to retrieve the product
        ///     Refer BR49 - srs product management sub system  v1.1
        /// </summary>
        /// <param name="items"></param>
        /// <param name="terminalNo"></param>
        public void RetrieveProduct(IList<TempTableItem> items, string terminalNo)
        {
            var palletNo = items[0].F51_PalletNo;
            var shelfNo = "";
            foreach (var item in items)
                shelfNo = item.ShelfNo;
            var lstShelfNo = shelfNo.Split('-');
            var from = lstShelfNo[0] + lstShelfNo[1] + lstShelfNo[2];
            //Insert Or Update tx48_nomanage
            var isNoManage = true;
            var serialNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNoManage,
                Constants.GetColumnInNoManager.Pdtwhscmdno, 0, 0, 0, 0, 1).ToString("D4");

            //Update TX40_PDTSHFSTK
            var stockFlag = Constants.F40_StockFlag.TX40_StkFlg_Shipping;

            var productShelfStocks =
                _unitOfWork.ProductShelfStockRepository.GetMany(
                    i =>
                        i.F40_PalletNo.Trim().Equals(palletNo) && i.F40_StockFlag.Equals(stockFlag) &&
                        (i.F40_AssignAmount > 0));
            foreach (var productShelfStock in productShelfStocks)
                _unitOfWork.ProductShelfStockRepository.UpdateProductShelfStock(productShelfStock,
                    Constants.F40_StockFlag.TX40_StkFlg_Rtr);
            //Insert data to TX47_PDTWHSCMD
            //•	[f47_to] = [f05_conveyorcode] from “tm05_conveyor” table, where [f05_terminalno] = Terminal No
            var to = GetConveyorCode(terminalNo);
            var productWarehouseCommand = _unitOfWork.ProductWarehouseCommandRepository.InsertProductWarehouseCommand(
                Constants.F47_CommandNo.Retrieval.ToString("D"), serialNo,
                Constants.CommandType.CmdType_0, Constants.F47_StrRtrType.Product.ToString("D"),
                Constants.F47_Status.AnInstruction.ToString("D"), palletNo, from, to, terminalNo,
                Constants.PictureNo.TCPR051F);

            _unitOfWork.Commit();

            // System will send message to C3 as the communication process by doing as follow:
            _notificationService.SendMessageToC3("TCPR061F",
                _notificationService.FormatThirdCommunicationMessageResponse(productWarehouseCommand));
        }

        /// <summary>
        ///     Find 5 record Product detail
        ///     Refer Br44 - SrsProduct Management v1.1
        /// </summary>
        /// <param name="productCode"></param>
        /// <param name="palletNo"></param>
        /// <returns></returns>
        public List<ForceRetrievalOfProductItem> ProductDetail(string productCode, string palletNo)
        {
            var stockFlag = Constants.F40_StockFlag.TX40_StkFlg_Shipping;
            var productShelfStocks =
                _unitOfWork.ProductShelfStockRepository.GetMany(
                    i =>
                        i.F40_ProductCode.Trim().Equals(productCode) && i.F40_PalletNo.Trim().Equals(palletNo) &&
                        i.F40_StockFlag.Equals(stockFlag) && (i.F40_AssignAmount > 0));
            var products = _unitOfWork.ProductRepository.GetAll();

            var result = (from productShelfStock in productShelfStocks
                          from product in products
                          where productShelfStock.F40_ProductCode.Trim().Equals(product.F09_ProductCode)
                          select new ForceRetrievalOfProductItem
                          {
                              F40_ProductCode = productShelfStock.F40_ProductCode,
                              ProductName = product.F09_ProductDesp,
                              F40_ProductLotNo = productShelfStock.F40_ProductLotNo,
                              F40_AssignAmount = Math.Round(productShelfStock.F40_AssignAmount.Value, 2),
                              F40_CertificationFlg = productShelfStock.F40_CertificationFlg
                          });

            return result.ToList();
        }

        public List<ThirdCommunicationResponse> RespondingReplyFromC3(string terminalNo, string productCode)
        {
            //            After system received response from C3 that the process in C3 has completed, system continues doing as follow:
            //	Suppose Command No 3, Sequence No 3, Status 3, Pallet No 3, From 3 and To 3 are retrieved respectively 
            //from [F47_CommandNo], [F47_CmdSeqNo], [F47_Status], [f47_palletno], [F47_From] and [F47_To] in TX47_PDTWHSCMD, 

            //	[F47_TerminalNo] = Terminal No. which is retrieved from section “server” in “toshiba.ini” configurable file,
            //	Upper [F47_PictureNo] = “TCPR051F”,
            //	[F47_Status] = 6 or 7 or 9,
            //	Order by [F47_AddDate].
            var lstTx47 =
                _unitOfWork.ProductWarehouseCommandRepository.GetByTerminalNoAndPictureNoAndStatus679(terminalNo,
                    Constants.PictureNo.TCPR051F);

            var items = new List<ThirdCommunicationResponse>();


            foreach (var tx47 in lstTx47)
            {

                var lstTx40 = _unitOfWork.ProductShelfStockRepository.GetByPalletNo(tx47.F47_PalletNo);
                var item = Mapper.Map<ThirdCommunicationResponse>(tx47);
                item.OldStatus = tx47.F47_Status;
                item.ProductCode = productCode;
                var newStatus = "";
                foreach (var tx40 in lstTx40)
                    switch (tx47.F47_Status[0])
                    {
                        case '6': //Command End

                            newStatus = "C";
                            _unitOfWork.ProductShelfStockRepository.UpdateProductShelfStock(tx40, true);
                            break;
                        case '7': //Command Cancel
                            newStatus = "D";
                            _unitOfWork.ProductShelfStockRepository.UpdateProductShelfStock(tx40, false);
                            break;
                        case '9': //Command Error
                            newStatus = "F";
                            _unitOfWork.ProductShelfStockRepository.UpdateProductShelfStock(tx40, false);
                            break;
                    }

                tx47.F47_Status = newStatus;
                tx47.F47_UpdateDate = DateTime.Now;

                items.Add(item);
                _unitOfWork.ProductWarehouseCommandRepository.Update(tx47);

                _notificationService.SendMessageToC3("TCPR051F",
                    _notificationService.FormatThirdCommunicationMessageResponse(tx47));
            }

            _unitOfWork.Commit();

            return items;
        }

        #endregion
    }
}