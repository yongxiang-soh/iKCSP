using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Core.Resources;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.ProductShippingCommand;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.ProductManagement
{
    public class ProductShippingCommandDomain : BaseDomain, IProductShippingCommandDomain
    {
        /// <summary>
        ///     Domain which handles common functions.
        /// </summary>
        private readonly ICommonDomain _commonDomain;

        /// <summary>
        ///     Service which handles configuration of system.
        /// </summary>
        private readonly IConfigurationService _configurationService;

        /// <summary>
        ///     Service which handles message comes / goes.
        /// </summary>
        private readonly INotificationService _notificationService;

        private string sShelfNo;
        /// <summary>
        ///     Initialize product shipping command domain with dependency injection.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="configurationService"></param>
        /// <param name="commonDomain"></param>
        /// <param name="notificationService"></param>
        public ProductShippingCommandDomain(IUnitOfWork unitOfWork, ICommonDomain commonDomain,
            IConfigurationService configurationService, INotificationService notificationService)
            : base(unitOfWork, configurationService)
        {
            _notificationService = notificationService;
            _commonDomain = commonDomain;
            _configurationService = configurationService;
        }

        /// <summary>
        ///     Find shipping commands asynchronously.
        /// </summary>
        /// <param name="shippingNo"></param>
        /// <param name="gridSettings"></param>
        /// <returns></returns>
        public async Task<ResponseResult<GridResponse<ProductShippingCommandItem>>> FindProductShippingCommandsAsync(
            string shippingNo, GridSettings gridSettings)
        {
            // Find the list of shipping commands.
            var shippingCommands = FindProductShippingCommands(shippingNo);

            // Count total number of records match with the conditions.
            var total = await shippingCommands.CountAsync();

            if (gridSettings != null)
                OrderByAndPaging(ref shippingCommands, gridSettings);

            var resultModel = new GridResponse<ProductShippingCommandItem>(shippingCommands, total);
            return new ResponseResult<GridResponse<ProductShippingCommandItem>>(resultModel, true);
        }

        /// <summary>
        ///     Find shipping commands asynchronously.
        /// </summary>
        /// <param name="gridSettings"></param>
        /// <param name="row"></param>
        /// <param name="bay"></param>
        /// <param name="level"></param>
        /// <param name="palletNo"></param>
        /// <param name="productLotNo"></param>
        /// <returns></returns>
        //public ResponseResult<GridResponse<object>> FindProductShippingCommandDetailsAsync(
        //    GridSettings gridSettings, string palletNo, string productLotNo, string productNo, double? requestAmount)
        //{
        //    //var listrecoud = QueryUpdate(false, "", productNo,
        //    //    productLotNo, 0, 0, "");

        //    //    var listPalletNos = listrecoud.Select(i => i.ls_palletno);
        //    //    var lstShelfNos  =  listrecoud.ToDictionary(i=>i.ls_palletno,j=>j.ls_shelfno);
        //        int rAmount = 0;
        //        if (requestAmount.HasValue)
        //        {
        //            rAmount = (int)requestAmount.Value;
        //        }
        //        var pdtShelfStatuses = _unitOfWork.ProductShelfStatusRepository.GetAll();

        //        var packingUnit = _unitOfWork.ProductRepository.GetById(productNo) != null ? _unitOfWork.ProductRepository.GetById(productNo).F09_PackingUnit : 0;

        //        //if (listPalletNos.Any())
        //        //    pdtShelfStatuses = pdtShelfStatuses.Where(x => listPalletNos.Contains(x.F51_PalletNo));

        //        pdtShelfStatuses = pdtShelfStatuses.Where(x => x.F51_PalletNo.Contains(x.F51_PalletNo));
        //        var productShelfStocks = _unitOfWork.ProductShelfStockRepository.GetAll();

        //        if (!string.IsNullOrEmpty(productLotNo))
        //            productShelfStocks = productShelfStocks.Where(x => x.F40_ProductLotNo.Trim().Equals(productLotNo));


        //        var productDetails = from pdtShelfStatus in pdtShelfStatuses
        //                             from productShelfStock in productShelfStocks
        //                             where pdtShelfStatus.F51_PalletNo.Equals(productShelfStock.F40_PalletNo)
        //                             //  && pdtShelfStatus.F51_StockTakingFlag.Equals(productShelfStock.F40_StockFlag)
        //                             select new
        //                             {
        //                                 pdtShelfStatus.F51_ShelfRow,
        //                                 pdtShelfStatus.F51_ShelfBay,
        //                                 pdtShelfStatus.F51_ShelfLevel,
        //                                 //ShelfNo = lstShelfNos.FirstOrDefault(i=>i.Key==pdtShelfStatus.F51_PalletNo).Value,
        //                                 ShelfNo = 
        //                                 pdtShelfStatus.F51_ShelfRow + "-" + pdtShelfStatus.F51_ShelfBay + "-" +
        //                                 pdtShelfStatus.F51_ShelfLevel,
        //                                 pdtShelfStatus.F51_PalletNo,
        //                                 productShelfStock.F40_ProductLotNo,
        //                                 F40_Amount = (int)(productShelfStock.F40_Amount - productShelfStock.F40_ShippedAmount) * packingUnit,
        //                                 F40_PackageAmount = (int)(productShelfStock.F40_Amount - productShelfStock.F40_ShippedAmount)
        //                             };
        //        var productDetailslst = productDetails.ToList().Select(m => new
        //        {
        //            m.F51_ShelfRow,
        //            m.F51_ShelfBay,
        //            m.F51_ShelfLevel,
        //            m.ShelfNo,
        //            m.F51_PalletNo,
        //            m.F40_ProductLotNo,
        //            F40_Amount = packingUnit == 0 ? 0 : m.F40_Amount,
        //            F40_PackageAmount = packingUnit == 0 ? "0.00" : ((int)(m.F40_Amount / packingUnit)).ToString("#,###,##0.00"),

        //        });



        //    // Count total of records.
        //    var totalProductDetails = productDetails.Count();

        //    if (gridSettings != null)
        //        OrderByAndPaging(ref productDetails, gridSettings);

        //    var resultModel = new GridResponse<object>(productDetails, totalProductDetails);
        //    return new ResponseResult<GridResponse<object>>(resultModel, true);
        //}

        public ResponseResult<GridResponse<object>> FindProductShippingCommandDetailsAsync(
            GridSettings gridSettings, string lstPalletNo, string productLotNo, string productNo, double? requestAmount,string shelfNo)
        {
            if (shelfNo == null)
            {
                return new ResponseResult<GridResponse<object>>(null, false);                
            }
            var listrecoud = QueryUpdate(false, "", productNo,
                productLotNo, 0, 0, "", lstPalletNo);

            var listPalletNos = listrecoud.Select(i => i.ls_palletno);
            //var lstShelfNos = listrecoud.Select(c => new ShelfnopandPalletno()
            //{
            //    Palletno = c.ls_palletno,
            //    Shelfno = c.ls_shelfno
            //}).ToList();            
            string[] test = null;
            var lstShelfNoSelect = shelfNo.Split('|');
            var pdtShelfStatuses = _unitOfWork.ProductShelfStatusRepository.GetAll();
            if (listPalletNos.Any())
                pdtShelfStatuses = pdtShelfStatuses.Where(x => listPalletNos.Contains(x.F51_PalletNo));

            var listTX51 = new List<TX51_PdtShfSts>();
            for (int i = 0; i < lstShelfNoSelect.Length - 1 ; i++)
            {
                test = lstShelfNoSelect[i].Split('-');
                foreach (var item in pdtShelfStatuses)
                {
                    var item1 = new TX51_PdtShfSts();
                    if (item.F51_ShelfRow == test[0] && item.F51_ShelfBay == test[1] && item.F51_ShelfLevel == test[2])
                    {
                        item1.F51_ShelfRow = item.F51_ShelfRow;
                        item1.F51_ShelfBay = item.F51_ShelfBay;
                        item1.F51_ShelfLevel = item.F51_ShelfLevel;
                        item1.F51_PalletNo = item.F51_PalletNo;
                        listTX51.Add(item1);
                    }                    
                }
            }            
            int rAmount = 0;
            if (requestAmount.HasValue)
            {
                rAmount = (int)requestAmount.Value;
            }            
            var packingUnit = _unitOfWork.ProductRepository.GetById(productNo) != null ? _unitOfWork.ProductRepository.GetById(productNo).F09_PackingUnit : 0;
                            
            //pdtShelfStatuses = pdtShelfStatuses.Where(x => x.F51_PalletNo.Contains(x.F51_PalletNo));

            var productShelfStocks = _unitOfWork.ProductShelfStockRepository.GetAll();
            if (!string.IsNullOrEmpty(productLotNo))
                productShelfStocks = productShelfStocks.Where(x => x.F40_ProductLotNo.Trim().Equals(productLotNo));


            var productDetails = from pdtShelfStatus in listTX51
                                 from productShelfStock in productShelfStocks
                                 where pdtShelfStatus.F51_PalletNo.Equals(productShelfStock.F40_PalletNo)
                                 //  && pdtShelfStatus.F51_StockTakingFlag.Equals(productShelfStock.F40_StockFlag)
                                 select new
                                 {
                                     pdtShelfStatus.F51_ShelfRow,
                                     pdtShelfStatus.F51_ShelfBay,
                                     pdtShelfStatus.F51_ShelfLevel,                                     
                                     pdtShelfStatus.F51_PalletNo,
                                     productShelfStock.F40_ProductLotNo,
                                     F40_Amount = Math.Truncate((productShelfStock.F40_Amount - productShelfStock.F40_ShippedAmount) / packingUnit) * packingUnit,
                                     F40_PackageAmount = (int)(productShelfStock.F40_Amount - productShelfStock.F40_ShippedAmount)
                                 };
            var productDetailslst = new List<ProductDetailsItem>();
            double dftvalues=0.00;
            foreach (var product in productDetails)
            {
                var item = new ProductDetailsItem();
                item.F51_ShelfRow = product.F51_ShelfRow;
                item.F51_ShelfBay = product.F51_ShelfBay;
                item.F51_ShelfLevel = product.F51_ShelfLevel;
                item.ShelfNo = product.F51_ShelfRow + "-" + product.F51_ShelfBay + "-" + product.F51_ShelfLevel;
                //item.ShelfNo = GetShelfNoByPalletNo(lstShelfNos, product.F51_PalletNo);
                item.F51_PalletNo = product.F51_PalletNo;
                item.F40_ProductLotNo = product.F40_ProductLotNo;
                item.F40_Amount = packingUnit == 0 ? 0 : product.F40_Amount;
                item.F40_PackageAmount = packingUnit == 0
                    ? dftvalues
                   
                    : ((int)(product.F40_Amount / packingUnit));
                    //: ((int)(product.F40_Amount / packingUnit)).ToString("#,###,##0.00");
                productDetailslst.Add(item);
            }

            //var productDetailslst = productDetails.ToList().Select(m => new
            //{
            //    m.F51_ShelfRow,
            //    m.F51_ShelfBay,
            //    m.F51_ShelfLevel,
            //    GetShelfNoByPalletNo(lstShelfNos,m.F51_PalletNo),
            //    m.F51_PalletNo,
            //    m.F40_ProductLotNo,
            //    F40_Amount = packingUnit == 0 ? 0 : m.F40_Amount,
            //    F40_PackageAmount = packingUnit == 0 ? "0.00" : ((int)(m.F40_Amount / packingUnit)).ToString("#,###,##0.00"),

            //});

            // Count total of records.
            var totalProductDetails = productDetails.Count();

            //if (gridSettings != null)
            //    OrderByAndPaging(ref productDetails, gridSettings);

            var resultModel = new GridResponse<object>(productDetailslst, totalProductDetails);
            return new ResponseResult<GridResponse<object>>(resultModel, true);
        }

        public string GetShelfNoByPalletNo(List<ShelfnopandPalletno> lstShelfNos, string palletNo)
        {
            string shelfNo = string.Empty;
            var firstOrDefault = lstShelfNos.FirstOrDefault(c => c.Palletno == palletNo);
            if (firstOrDefault != null)
            {
                shelfNo = firstOrDefault.Shelfno;
            }
            return shelfNo;
        }

        /// <summary>
        ///     3.4.5	UC 17: De-assign Product
        /// </summary>
        /// <returns></returns>
        public async Task UnassignProduct(string shippingNo, string terminalNo)
        {
            //o	If there is no command/message is being received:
            //•	Get Shipping No. of selected row.

            /*
             * •	Update TX51_PDTSHFSTS, 
             * where [f51_shelfstatus] = “Reserved for Retrieval” (or 5), 
             * and [f51_terminalno] = Terminal No. which is retrieved from section “server” in “toshiba.ini” configurable file,   
             * 	Set [f51_shelfstatus] = “Product Stock” (or 2),
             * 	Set [f51_terminalno] = blank,
             * 	Set [f51_updatedate] = Current date time.
             */
            var productShelfStatuses = _unitOfWork.ProductShelfStatusRepository.GetAll();
            productShelfStatuses =
                productShelfStatuses.Where(
                    x => Constants.F51_ShelfStatus.TX51_ShfSts_RsvRtr.Equals(x.F51_ShelfStatus.Trim()));
            productShelfStatuses =
                productShelfStatuses.Where(
                    x => x.F51_TerminalNo.Equals(terminalNo));

            foreach (var productShelfStatus in productShelfStatuses)
            {
                productShelfStatus.F51_ShelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_Pdt;
                productShelfStatus.F51_TerminalNo = null;
                productShelfStatus.F51_UpdateDate = DateTime.Now;
            }

            /*
             * •	Update TX40_PDTSHFSTK, 
             * where [f40_stockflag] = “Not in Stock because of shipping” (or 4) and [f40_shipcommandno] = Shipping No of selected record.
             * 	Set [f40_stockflag] = “Not in Stock” (or 0), 
             * 	Set [f40_shipcommandno] = blank,
             * 	Set [f40_assignamount] = 0,
             * 	Set [f40_updatedate] = Current date time.
             */
            var productShelfStocks = _unitOfWork.ProductShelfStockRepository.GetAll();
            productShelfStocks =
                productShelfStocks.Where(
                    x => Constants.F40_StockFlag.TX40_StkFlg_NotStkShip.Equals(x.F40_StockFlag.Trim())
                         && x.F40_ShipCommandNo.Trim().Equals(shippingNo));

            foreach (var productShelfStock in productShelfStocks)
            {
                productShelfStock.F40_StockFlag = Constants.F40_StockFlag.TX40_StkFlg_NotStk;
                productShelfStock.F40_ShipCommandNo = "";
                productShelfStock.F40_AssignAmount = 0;
                productShelfStock.F40_UpdateDate = DateTime.Now;
            }

            /*
             * •	Update TX40_PDTSHFSTK, where [f40_stockflag] = “Shipping” (or 5) and [f40_shipcommandno] = Shipping No of selected record.
             * 	Set [f40_stockflag] = “Stocked” (or 3), 
             * 	Set [f40_shipcommandno] = blank,
             * 	Set [f40_assignamount] = 0,
             * 	Set [f40_updatedate] = Current date time.
             */
            var tx40ProductShelfStocks = _unitOfWork.ProductShelfStockRepository.GetAll();
            tx40ProductShelfStocks =
                tx40ProductShelfStocks.Where(
                    x => Constants.F40_StockFlag.TX40_StkFlg_Shipping.Equals(x.F40_StockFlag.Trim()));
            tx40ProductShelfStocks =
                tx40ProductShelfStocks.Where(x => x.F40_ShipCommandNo.Trim().Equals(shippingNo));

            foreach (var tx40ProductShelfStock in tx40ProductShelfStocks)
            {
                tx40ProductShelfStock.F40_StockFlag = Constants.F40_StockFlag.TX40_StkFlg_Stk;
                tx40ProductShelfStock.F40_ShipCommandNo = "";
                tx40ProductShelfStock.F40_AssignAmount = 0;
                tx40ProductShelfStock.F40_UpdateDate = DateTime.Now;
            }

            // Save changes asynchronously.
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        ///     Assign a product asynchronously.
        /// </summary>
        /// <param name="shippingNo"></param>
        /// <param name="productCode"></param>
        /// <param name="productLotNo"></param>
        /// <param name="shippingQuantity"></param>
        /// <param name="shippedAmount"></param>
        /// <returns></returns>
        public List<AssignProductShippingCommandResult> AssignProductAsync(string shippingNo, string productCode,
            string productLotNo, double shippingQuantity, double shippedAmount, string terminalNo)
        {            
            return QueryUpdate(true, shippingNo, productCode,
                productLotNo, shippingQuantity, shippedAmount, terminalNo,"");
        }


        /// <summary>
        ///     Retrieve  a specific product shipping command.
        /// </summary>
        /// <param name="shippingNo"></param>
        /// <param name="productCode"></param>
        /// <param name="productLotNo"></param>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        public ResponseResult RetrieveProductAsync(string shippingNo, string productCode, string productLotNo, string row,
            string bay, string level, string terminalNo, string palletNo)
        {
            var conveyor = _unitOfWork.ConveyorRepository.Get(e => e.F05_TerminalNo.Trim().Equals(terminalNo.Trim()));
            var productShelfStocks = _unitOfWork.ProductShelfStockRepository.GetAll();

            // Conveyor is not found.
            if (conveyor == null)
                return new ResponseResult(false, "MSG8");

            if (conveyor.F05_StrRtrSts.Equals(Constants.F05_StrRtrSts.Error.ToString("D")))
                return new ResponseResult(false, "MSG8");


            var device = _unitOfWork.DeviceRepository.GetByDeviceCode(_configurationService.ProductDeviceCode);
            if (device == null)
            {
                return new ResponseResult(false, "MSG9");
            }
            if (device.F14_DeviceStatus == "2" || device.F14_DeviceStatus == "1" || device.F14_UsePermitClass == "1")
                return new ResponseResult(false, "MSG9");


            /*
             * Retrieve[f40_palletno], [f40_prepdtlotno], [f40_productcode], [f40_productlotno] and[f40_assignamount] from TX40_PDTSHFSTK, where[f40_stockflag] = “Not in Stock because of shipping” (or 4) and[f40_shipcommandno] = Shipping No of selected record
             */
            var lstpdtShfStk =
                productShelfStocks.Where(
                    i => i.F40_StockFlag.Equals("4") && i.F40_ShipCommandNo.Trim().Equals(shippingNo));
            foreach (var pdtShfStk in lstpdtShfStk)
            {
                #region make out Product Shipping Command data and update Product Shelf Stock (Reference Iv)
                var assignAmount = pdtShfStk.F40_AssignAmount ?? 0;
                //Insert data to TH70_PDTSHIPHST
                var productShipHistory =
                    _unitOfWork.PdtShipHstRepository.Get(
                        i =>
                            i.F70_ShipCommandNo.Equals(shippingNo) && i.F70_PalletNo.Equals(pdtShfStk.F40_PalletNo) &&
                            i.F70_PrePdtLotNo.Equals(pdtShfStk.F40_PrePdtLotNo) &&
                            i.F70_ProductCode.Equals(pdtShfStk.F40_ProductCode));
                if (productShipHistory == null)
                {
                    var pdtShipHst = new TH70_PdtShipHst
                    {
                        F70_ShipCommandNo = shippingNo,
                        F70_PalletNo = pdtShfStk.F40_PalletNo,
                        F70_PrePdtLotNo = pdtShfStk.F40_PrePdtLotNo,
                        F70_ProductCode = pdtShfStk.F40_ProductCode,
                        F70_ShelfNo = "",
                        F70_ProductLotNo = pdtShfStk.F40_ProductLotNo,
                        F70_ShippedAmount = assignAmount,
                        F70_AddDate = DateTime.Now,
                        F70_UpdateDate = DateTime.Now,
                        F70_UpdateCount = 0
                    };
                    _unitOfWork.PdtShipHstRepository.Add(pdtShipHst);
                }


                //Update TX40_PDTSHFSTK                

                pdtShfStk.F40_StockFlag = Constants.F40_StockFlag.TX40_StkFlg_NotStk;
                pdtShfStk.F40_ShippedAmount = pdtShfStk.F40_ShippedAmount + assignAmount;
                pdtShfStk.F40_ShipCommandNo = "";
                pdtShfStk.F40_AssignAmount = 0;
                pdtShfStk.F40_UpdateDate = DateTime.Now;
                _unitOfWork.ProductShelfStockRepository.Update(pdtShfStk);



                //Update TX44_SHIPPINGPLAN
                var shippingPlan =

                    _unitOfWork.ShippingPlanRepository.GetAll()
                        .FirstOrDefault(e => e.F44_ShipCommandNo.Trim().Equals(shippingNo));
                if (shippingPlan != null)
                {
                    shippingPlan.F44_ShippedAmount = shippingPlan.F44_ShippedAmount + assignAmount;
                    shippingPlan.F44_UpdateDate = DateTime.Now;
                    _unitOfWork.ShippingPlanRepository.Update(shippingPlan);
                }

                #endregion

            }
            #region Make out Warehouse Retrieval command and transmits to communication process (Reference v)

            var isNoManage = false;
            var serialNo = "";
            var sequenceNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNoManage,
                Constants.GetColumnInNoManager.Pdtwhscmdno,
                0, 0, 0, 0, 1);
            serialNo = sequenceNo.ToString("D4");

            //Insert TX47_PDTWHSCMD
            var pdtWhsCmd = new TX47_PdtWhsCmd
            {
                F47_CommandNo = "2000",
                F47_CmdSeqNo = serialNo,
                F47_CommandType = Constants.CmdType.cmdType,
                F47_StrRtrType = Constants.F47_StrRtrType.Product.ToString("D"),
                F47_Status = Constants.F47_Status.AnInstruction.ToString("D"),
                F47_Priority = 0,
                //F47_PalletNo = pdtShfStk.F40_PalletNo,
                F47_PalletNo = palletNo,
                F47_From = row + bay + level,
                F47_To = conveyor.F05_ConveyorCode,
                F47_TerminalNo = terminalNo,
                F47_PictureNo = Constants.PictureNo.TCPR041F,
                F47_AbnormalCode = "",
                F47_RetryCount = 0,
                F47_AddDate = DateTime.Now,
                F47_UpdateDate = DateTime.Now,
                F47_UpdateCount = 0
            };
            if (
                _unitOfWork.ProductWarehouseCommandRepository.Get(
                    i => i.F47_CommandNo == pdtWhsCmd.F47_CommandNo && i.F47_CmdSeqNo == pdtWhsCmd.F47_CmdSeqNo) !=
                null)
            {
                _unitOfWork.ProductWarehouseCommandRepository.Update(pdtWhsCmd);
            }
            else
            {
                _unitOfWork.ProductWarehouseCommandRepository.Add(pdtWhsCmd);
            }

            var productShelfStockShippings =
                    _unitOfWork.ProductShelfStockRepository.GetMany(
                        e => e.F40_StockFlag.Trim().Equals("5") && e.F40_ShipCommandNo.Trim().Equals(shippingNo));
            foreach (var productShelfStockShipping in productShelfStockShippings)
            {
                productShelfStockShipping.F40_StockFlag = Constants.F40_StockFlag.TX40_StkFlg_Rtr;
                productShelfStockShipping.F40_UpdateDate = DateTime.Now;
                _unitOfWork.ProductShelfStockRepository.Update(productShelfStockShipping);

            }


            var message =
                string.Format(
                    "“0011” {0} {1} “0066” {2} “2000” {3} {4} {5} {6} {7} {8}",
                    pdtWhsCmd.F47_TerminalNo, pdtWhsCmd.F47_PictureNo, sequenceNo, pdtWhsCmd.F47_CommandType,
                    pdtWhsCmd.F47_Status, pdtWhsCmd.F47_From, pdtWhsCmd.F47_To, pdtWhsCmd.F47_PalletNo, DateTime.Now);

            // TODO: Update screen name.
            _notificationService.SendMessageToC3(null, message);

            #endregion

            #region If (Reference iv) and (Reference v) run well, then update TX44_SHIPPINGPLAN

            //if (lstpdtShfStk.Any())
            //{
            //    var productShelfStockShippings =
            //        _unitOfWork.ProductShelfStockRepository.GetMany(
            //            e => e.F40_StockFlag.Trim().Equals("5") && e.F40_ShipCommandNo.Trim().Equals(shippingNo));
            //    foreach (var productShelfStockShipping in productShelfStockShippings)
            //    {
            //        productShelfStockShipping.F40_StockFlag = Constants.F40_StockFlag.TX40_StkFlg_Rtr;
            //        productShelfStockShipping.F40_UpdateDate = DateTime.Now;
            //        _unitOfWork.ProductShelfStockRepository.Update(productShelfStockShipping);

            //    }
            //}
            //else
            //{
            //    double remainingAmount = 0;
            //    bool result = true;
            //    var productShelfStockShippings =
            //        _unitOfWork.ProductShelfStockRepository.GetMany(
            //            e => e.F40_ProductCode.Trim().Equals(productCode) &&
            //                e.F40_ProductLotNo.Trim().Equals(productLotNo) &&
            //                (e.F40_StockFlag.Trim().Equals("0") || e.F40_StockFlag.Trim().Equals("3")) &&
            //                e.F40_CertificationFlg.Trim().Equals("1") &&
            //                e.F40_CertificationDate.HasValue &&
            //                e.F40_Amount > e.F40_ShippedAmount).FirstOrDefault();
            //    //var remainingAmount = 0;
            //    if (productShelfStockShippings != null)
            //    {
            //        remainingAmount = productShelfStockShippings.F40_Amount -
            //                          productShelfStockShippings.F40_ShippedAmount;
            //    }
            //    var product =
            //        _unitOfWork.ProductRepository.GetAll()
            //            .FirstOrDefault(x => x.F09_ProductCode.Trim().Equals(productCode));

            //    if (remainingAmount <= product.F09_PackingUnit)
            //    {
            //        result = false;
            //    }
            //    var tx44 = _unitOfWork.ShippingPlanRepository.Get(i => i.F44_ShipCommandNo == shippingNo);


            //    if (tx44 != null)
            //    {
            //        if ((tx44.F44_ShpRqtAmt == tx44.F44_ShippedAmount) || result == false)
            //        {
            //            tx44.F44_Status = Constants.F44_Status.F44_Sts_ShipOver;
            //            tx44.F44_UpdateDate = DateTime.Now;
            //            _unitOfWork.ShippingPlanRepository.Update(tx44);
            //            var result45 = _unitOfWork.ShipCommandRepository.GetAll()
            //                .FirstOrDefault(x => x.F45_ShipCommandNo.Trim().Equals(shippingNo));
            //            if (result45 == null)
            //            {
            //                var tx45 = new TX45_ShipCommand()
            //                {
            //                    F45_ShipCommandNo = shippingNo,
            //                    F45_ShipDate = DateTime.Now,
            //                    F45_ShipAmount = tx44.F44_ShpRqtAmt,
            //                    F45_ShippedAmount = tx44.F44_ShippedAmount,
            //                    F45_AddDate = DateTime.Now,
            //                    F45_UpdateDate = DateTime.Now,
            //                    F45_UpdateCount = 0
            //                };
            //                _unitOfWork.ShipCommandRepository.Add(tx45);
            //            }
            //        }
            //    }
            //}
            
            #endregion

            
            //Update TX44_SHIPPINGPLAN
            var shippingPlanU =
                _unitOfWork.ShippingPlanRepository.Get(x => x.F44_ShipCommandNo.Trim().Equals(shippingNo));
            shippingPlanU.F44_Status = "1";
            shippingPlanU.F44_UpdateDate = DateTime.Now;
            _unitOfWork.ShippingPlanRepository.Update(shippingPlanU);

            RetrieveProductFinallyAsync(palletNo);

            _unitOfWork.Commit();
            return new ResponseResult(true);
        }

        /// <summary>
        ///     system performs some updates in finally RetrieveProductAsync
        /// </summary>
        /// <param name="palletno"></param>
        /// <returns></returns>
        public void RetrieveProductFinallyAsync(string palletno)
        {
            var proShelfs = _unitOfWork.ProductShelfRepository.GetAll();
            var proShelfsStocks = _unitOfWork.ProductShelfStockRepository.GetAll();
            /*
             * Suppose Pre-product LotNo 2, Product Code 2 and Flag 2 are retrieved respectively from [f40_prepdtlotno], [f40_productcode] and [f57_outflg] in TX40_PDTSHFSTK and TX57_PDTSHF, where [f57_palletno] = [f40_palletno] and [f40_palletno] = Pallet No of the selected record in Data window 2.
             */
            var result1 = (from proShelf in proShelfs
                           join proShelfsStock in proShelfsStocks on proShelf.F57_PalletNo.Trim() equals
                           proShelfsStock.F40_PalletNo.Trim()
                           where proShelfsStock.F40_PalletNo.Trim().Equals(palletno)
                           select new
                           {
                               LotNo2 = proShelfsStock.F40_PrePdtLotNo,
                               ProductCode2 = proShelfsStock.F40_ProductCode,
                               Flag = proShelf.F57_OutFlg
                           }).FirstOrDefault();
            /*
             * 	Retrieve sum of [f40_amount] and sum of [f40_shippedamount] in tx40_pdtshfstk, where [f40_prepdtlotno] = Pre-product LotNo 2 and [f40_productcode] = Product Code 2.
             */
            var result2 =
            proShelfsStocks.Where(
                x =>
                    x.F40_PrePdtLotNo.Trim().Equals(result1.LotNo2) &&
                    x.F40_ProductCode.Trim().Equals(result1.ProductCode2)).ToList();
            var sumF40Amount = result2.Sum(x => x.F40_Amount);
            var sumf40Shippedamount = result2.Sum(x => x.F40_ShippedAmount);
            if (sumF40Amount.Equals(sumf40Shippedamount))
            {
                if (result1.Flag.Equals("0"))
                {
                    var entity =
                        _unitOfWork.TabletProductRepository.Get(
                            x =>
                                x.F56_PrePdtLotNo.Trim().Equals(result1.LotNo2) &&
                                x.F56_ProductCode.Trim().Equals(result1.ProductCode2));
                    if (entity != null)
                    {
                        entity.F56_ShipDate = DateTime.Now;
                        entity.F56_UpdateDate = DateTime.Now;
                        _unitOfWork.TabletProductRepository.Update(entity);
                    }                    
                }
                if (result1.Flag.Equals("1"))
                {
                    var entity =
                        _unitOfWork.OutOfPlanProductRepository.Get(
                            x =>
                                x.F58_PrePdtLotNo.Trim().Equals(result1.LotNo2) &&
                                x.F58_ProductCode.Trim().Equals(result1.ProductCode2));
                    if (entity != null)
                    {
                        entity.F58_ShipDate = DateTime.Now;
                        entity.F58_UpdateDate = DateTime.Now;
                        _unitOfWork.OutOfPlanProductRepository.Update(entity);
                    }                    
                }
                _unitOfWork.Commit();
            }
        }

        /// <summary>
        ///     Find product shipping command for printing purpose.
        /// </summary>
        /// <param name="shippingNo"></param>
        /// <param name="productCode"></param>
        /// <param name="productLotNo"></param>
        /// <param name="gridSettings"></param>
        /// <returns></returns>
        public async Task<object> FindProductShippingCommandsForPrinting(string shippingNo, string productCode, string lstPalletNo,string productNo,
            string productLotNo, GridSettings gridSettings,string shelfNo)
        {
            //var listrecoud = QueryUpdate(false, "", productNo,
            //   productLotNo, 0, 0, "", lstPalletNo);

            //var listPalletNos = listrecoud.Select(i => i.ls_palletno);
            var updateCount = "0:00";
            var shippingPlans = _unitOfWork.ShippingPlanRepository.GetAll().Where(x => x.F44_ShipCommandNo.Trim().Equals(shippingNo));
            if (shippingPlans.Any())
            {
                updateCount = shippingPlans.First().F44_UpdateCount.HasValue ? shippingPlans.First().F44_UpdateCount.Value.ToString("###,###,##0.00") : "0.00";
            }
            var productShelfStocks = _unitOfWork.ProductShelfStockRepository.GetAll();
            productShelfStocks = productShelfStocks.Where(x => x.F40_ProductCode.Trim().Equals(productCode));
            if (!string.IsNullOrEmpty(productLotNo))
                productShelfStocks = productShelfStocks.Where(x => x.F40_ProductLotNo.Trim().Equals(productLotNo));

            var productShelfStatuses = _unitOfWork.ProductShelfStatusRepository.GetAll();

            var lstPalletNoSelect = lstPalletNo.Split('|');
                //lstpdtShfStk = lstpdtShfStk.Where(i => lstPalletNoSelect.Contains(i.F40_PalletNo.Trim()) &&

            productShelfStatuses = productShelfStatuses.Where(x => lstPalletNoSelect.Contains(x.F51_PalletNo));

            string[] test = null;
            var lstShelfNoSelect = shelfNo.Split('|');            

            var listTX51 = new List<TX51_PdtShfSts>();
            for (int i = 0; i < lstShelfNoSelect.Length - 1; i++)
            {
                test = lstShelfNoSelect[i].Split('-');
                foreach (var item in productShelfStatuses)
                {
                    var item1 = new TX51_PdtShfSts();
                    if (item.F51_ShelfRow == test[0] && item.F51_ShelfBay == test[1] && item.F51_ShelfLevel == test[2])
                    {
                        item1.F51_ShelfRow = item.F51_ShelfRow;
                        item1.F51_ShelfBay = item.F51_ShelfBay;
                        item1.F51_ShelfLevel = item.F51_ShelfLevel;
                        item1.F51_PalletNo = item.F51_PalletNo;
                        listTX51.Add(item1);
                    }
                }
            } 


            var packingUnit = _unitOfWork.ProductRepository.GetById(productCode) != null ? _unitOfWork.ProductRepository.GetById(productCode).F09_PackingUnit : 0;

            //var results =
            //    from productShelfStock in productShelfStocks
            //    from productShelfStatus in listTX51
            //    where productShelfStock.F40_PalletNo.Equals(productShelfStatus.F51_PalletNo)
            //    //Checking ShelfStatus is not defined on SRS
            //    //&& productShelfStock.F40_StockFlag.Equals(productShelfStatus.F51_ShelfStatus)
            //    orderby productShelfStock.F40_ProductCode
            //    select new
            //    {
            //        //productShelfStock.F40_Amount,
            //        //shippingPlan.F44_UpdateCount,
            //        productShelfStock.F40_ProductLotNo,
            //        productShelfStatus.F51_ShelfRow,
            //        productShelfStatus.F51_ShelfBay,
            //        productShelfStatus.F51_ShelfLevel,
            //        productShelfStock.F40_PalletNo,
            //        F40_Amount = Math.Truncate((productShelfStock.F40_Amount - productShelfStock.F40_ShippedAmount) / packingUnit) * packingUnit,
            //        F40_PackageAmount = (int)(productShelfStock.F40_Amount - productShelfStock.F40_ShippedAmount)

            //    };

            var productDetails = from pdtShelfStatus in listTX51
                                 from productShelfStock in productShelfStocks
                                 where pdtShelfStatus.F51_PalletNo.Equals(productShelfStock.F40_PalletNo)
                                 //  && pdtShelfStatus.F51_StockTakingFlag.Equals(productShelfStock.F40_StockFlag)
                                 select new
                                 {
                                     pdtShelfStatus.F51_ShelfRow,
                                     pdtShelfStatus.F51_ShelfBay,
                                     pdtShelfStatus.F51_ShelfLevel,
                                     productShelfStock.F40_PalletNo,
                                     productShelfStock.F40_ProductLotNo,
                                     F40_Amount = Math.Truncate((productShelfStock.F40_Amount - productShelfStock.F40_ShippedAmount) / packingUnit) * packingUnit,
                                     F40_PackageAmount = (int)(productShelfStock.F40_Amount - productShelfStock.F40_ShippedAmount)
                                 };








            double dftvalues = 0.00;
            var result = productDetails.ToList().Select(m => new
            {
                //m.F40_Amount = Convert.ToDouble(String.Format("{0:#,##0.00}", m.F40_Amount)),
                F40_Amount1 = m.F40_Amount,
                F40_Amount = m.F40_Amount.ToString("###,###,##0.00"),
                m.F40_PalletNo,
                F51_ShelfRow = m.F51_ShelfRow + "-" + m.F51_ShelfBay + "-" + m.F51_ShelfLevel,
                m.F40_ProductLotNo,
                //F44_UpdateCount = updateCount,
                F40_PackageAmount = packingUnit == 0
                    ? "0.00"

                    : (m.F40_Amount/packingUnit).ToString("###,###,##0.00")
            });
            //if (gridSettings != null)
            //    result = result.Skip((gridSettings.PageIndex-1)*gridSettings.PageSize).Take(gridSettings.PageSize);

            var totals = result.Sum(x => (double?)x.F40_Amount1);            
            return new
            {
                items = result,
                //total = totals.HasValue ? totals.Value.ToString("####,###,##0.00") : "0.00"
                total = totals.HasValue ? totals.Value.ToString("#,###,##0.00") : "0.00"
            };
        }

        public List<ThirdCommunicationResponse> RespondingReplyFromC3(string terminalNo,string productCode)
        {
            //            After system received response from C3 that the process in C3 has completed, system continues doing as follow:
            //	Suppose Command No 3, Sequence No 3, Status 3, Pallet No 3, From 3 and To 3 are retrieved respectively 
            //from [F47_CommandNo], [F47_CmdSeqNo], [F47_Status], [f47_palletno], [F47_From] and [F47_To] in TX47_PDTWHSCMD, in which:
            //	[F47_TerminalNo] = Terminal No. which is retrieved from section “server” in “toshiba.ini” configurable file,
            //	Upper [F47_PictureNo] = “TCPR041F”,
            //	[F47_Status] = “Instruction complete” (or 6), OR “Instruction cancel” (or 7), OR “Empty retrieval” (or 9),
            //	Order by [F47_AddDate].
            //	Suppose Pre-product Lotno 3, Product Code 3, Shipping No 3, Product Lotno 3 and Assigned Amount 3 are temporary variables
            // retrieved from [f40_prepdtlotno], [f40_productcode], [f40_shipcommandno], [f40_productlotno] and [f40_assignamount] in TX40_PDTSHFSTK, where:
            //	[f40_palletno] = Pallet No 3,
            //	[f40_assignamount] > 0.
            var lstTx47 =
                _unitOfWork.ProductWarehouseCommandRepository.GetByTerminalNoAndPictureNoAndStatus679(terminalNo,
                    Constants.PictureNo.TCPR041F);
            var items = new List<ThirdCommunicationResponse>();
            foreach (var tx47 in lstTx47)                                                                                                                                                                                               
            {
                var lstTx40 =
                    _unitOfWork.ProductShelfStockRepository.GetByPalletNo(tx47.F47_PalletNo)
                        .Where(i => i.F40_AssignAmount > 0);
                
                var oldStatus = tx47.F47_Status;
                foreach (var tx40 in lstTx40)
                {
                    var shipCommandNo = tx40.F40_ShipCommandNo;
                    var assignAmount = tx40.F40_AssignAmount;
                    switch (tx47.F47_Status)
                    {
                        case "6": //Command End

                            tx47.F47_Status = "C";
                            tx40.F40_UpdateDate = DateTime.Now;
                            var entity =
                        _unitOfWork.PdtShipHstRepository.Get(
                            x =>
                                x.F70_ShipCommandNo.Trim().Equals(tx40.F40_ShipCommandNo) &&
                                x.F70_PalletNo.Trim().Equals(tx47.F47_PalletNo) &&
                                x.F70_PrePdtLotNo.Trim().Equals(tx40.F40_PrePdtLotNo) &&
                                x.F70_ProductCode.Trim().Equals(tx40.F40_ProductCode));
                            if (entity == null)
                            {
                                var th70 = new TH70_PdtShipHst
                                {
                                    F70_ShipCommandNo = tx40.F40_ShipCommandNo,
                                    //F70_PalletNo = tx40.F40_PalletNo,
                                    F70_PalletNo = tx47.F47_PalletNo,
                                    F70_PrePdtLotNo = tx40.F40_PrePdtLotNo,
                                    F70_ProductCode = tx40.F40_ProductCode,
                                    F70_ShelfNo = tx47.F47_From,
                                    F70_ProductLotNo = tx40.F40_ProductLotNo,
                                    F70_ShippedAmount = (double) tx40.F40_AssignAmount,
                                    F70_AddDate = DateTime.Now,
                                    F70_UpdateDate = DateTime.Now,
                                    F70_UpdateCount = 0                                    
                                };
                                _unitOfWork.PdtShipHstRepository.Add(th70);                                                               
                            }
                            _unitOfWork.ProductShelfStockRepository.UpdateProductShelfStock(tx40, true);
                            _unitOfWork.ShippingPlanRepository.UpdateShippedAmount(shipCommandNo,
                                    (double)assignAmount); 
                            break;                               
                        case "7": //Command Cancel
                            tx47.F47_Status = "F";
                            tx40.F40_UpdateDate = DateTime.Now;
                            _unitOfWork.ProductShelfStockRepository.UpdateProductShelfStock(tx40, false);
                            break;
                        case "9": //Command Error
                            tx47.F47_Status = "D";
                            tx40.F40_UpdateDate = DateTime.Now;
                            _unitOfWork.ProductShelfStockRepository.UpdateProductShelfStock(tx40, false);
                            break;
                    }

                    var itemtx40 = Mapper.Map<ThirdCommunicationResponse>(tx47);
                    itemtx40.OldStatus = oldStatus;
                    itemtx40.ProductCode = productCode;
                    items.Add(itemtx40);
                    _unitOfWork.ProductWarehouseCommandRepository.Update(tx47);
                    
                    _notificationService.SendMessageToC3("TCPR041F",
                        _notificationService.FormatThirdCommunicationMessageResponse(tx47));
                }
                if (!lstTx40.Any())
                {
                    switch (tx47.F47_Status)
                    {
                        case "6":
                            tx47.F47_Status = "C";
                            break;
                        case "7":
                            tx47.F47_Status = "F";
                            break;
                        case "9":
                            tx47.F47_Status = "D";
                            break;
                    }

                    tx47.F47_UpdateDate = DateTime.Now;
                    var item = Mapper.Map<ThirdCommunicationResponse>(tx47);
                    item.OldStatus = oldStatus;
                    item.ProductCode = productCode;
                    items.Add(item);
                    _unitOfWork.ProductWarehouseCommandRepository.Update(tx47);
                }
            }
            _unitOfWork.Commit();

            return items;
        }

        /// <summary>
        ///     Find all product shipping plan ssynchronously.
        /// </summary>
        /// <param name="shippingNo"></param>
        /// <returns></returns>
        private IQueryable<ProductShippingCommandItem> FindProductShippingCommands(string shippingNo)
        {
            // Find all products in database.
            var products = _unitOfWork.ProductRepository.GetAll();

            // Find all shipping plans in database.
            var shippingPlans = _unitOfWork.ShippingPlanRepository.GetAll();

            var records = from product in products
                          from shippingPlan in shippingPlans
                          where product.F09_ProductCode.Equals(shippingPlan.F44_ProductCode)
                                && (shippingPlan.F44_ShpRqtAmt > shippingPlan.F44_ShippedAmount)
                                && !Constants.F44_Status.F44_Sts_ShipOver.Equals(shippingPlan.F44_Status)
                          orderby shippingPlan.F44_ShipCommandNo ascending
                          select new ProductShippingCommandItem
                          {
                              F44_ShipCommandNo = shippingPlan.F44_ShipCommandNo,
                              F44_ProductCode = shippingPlan.F44_ProductCode,
                              F09_ProductDesp = product.F09_ProductDesp,
                              F44_ProductLotNo = shippingPlan.F44_ProductLotNo,
                              F44_ShpRqtAmt = shippingPlan.F44_ShpRqtAmt,
                              F44_ShippedAmount = shippingPlan.F44_ShippedAmount
                          };

            if (!string.IsNullOrEmpty(shippingNo))
                records = records.Where(x => x.F44_ShipCommandNo.Contains(shippingNo));
            return records;
        }

        private List<AssignProductShippingCommandResult> QueryUpdate(bool update, string shippingNo, string productCode,
            string productLotNo, double shippingQuantity, double shippedAmount, string terminalNo,string lstPalletNo)
        {
            //o	Suppose Request Amt and Assign Amt is temporary variables, which represents for requested amount and actual assigned amount.
            //At the beginning, Request Amt = Shipping Qty – Shipped Amt
            var requestAmt = shippingQuantity - shippedAmount;
            double assignAmt = 0;
            var certificationFlagOk = Constants.F40_CertificationFlag.TX40_CrtfctnFlg_Ok.ToString("D");

            var lstassignProductShippingCommandResult = new List<AssignProductShippingCommandResult>();
            var productShelfStocks = _unitOfWork.ProductShelfStockRepository.GetAll();
            var productShelves = _unitOfWork.ProductShelfRepository.GetAll();
            var lstpdtShfStk = from productShelfStock in productShelfStocks
                               from productShelf in productShelves
                               where productShelfStock.F40_PalletNo.Equals(productShelf.F57_PalletNo)
                                     && productShelfStock.F40_ProductCode.Trim().Equals(productCode)
                                     && productShelfStock.F40_ProductLotNo.Trim().Equals(productLotNo)

                                     && (productShelfStock.F40_Amount > productShelfStock.F40_ShippedAmount)
                                     && certificationFlagOk.Equals(productShelfStock.F40_CertificationFlg.Trim())
                                     && (productShelfStock.F40_CertificationDate != null)
                               orderby new
                               {
                                   productShelfStock.F40_TabletingEndDate,
                                   productShelf.F57_StorageDate
                               }
                               select productShelfStock;

            if (update)
            {
                lstpdtShfStk = lstpdtShfStk.Where(i =>
                     (Constants.F40_StockFlag.TX40_StkFlg_NotStk.Equals(i.F40_StockFlag.Trim()) ||
                      Constants.F40_StockFlag.TX40_StkFlg_Stk.Equals(i.F40_StockFlag.Trim())));
            }
            else
            {
                var lstPalletNoSelect = lstPalletNo.Split('|');
                lstpdtShfStk = lstpdtShfStk.Where(i => lstPalletNoSelect.Contains(i.F40_PalletNo.Trim()) &&
          (Constants.F40_StockFlag.TX40_StkFlg_NotStkShip.Equals(i.F40_StockFlag.Trim()) ||
           Constants.F40_StockFlag.TX40_StkFlg_Shipping.Equals(i.F40_StockFlag.Trim())));
            }
            if (!lstpdtShfStk.Any())
                return null;

            //o	System gets the assigned amount by doing as follow:
            //•	Retrieve packing unit from [f09_packingunit] in TM09_PRODUCT, where [f09_productcode] = Product Code of selected record.
            foreach (var pdtShfStk in lstpdtShfStk)
            {
                var assignProductShippingCommandResult = new AssignProductShippingCommandResult();
                var product =
                    _unitOfWork.ProductRepository.GetAll()
                        .FirstOrDefault(x => x.F09_ProductCode.Trim().Equals(productCode));

                //assignAmt = pdtShfStk.F40_Amount - pdtShfStk.F40_ShippedAmount;
                assignAmt = Math.Truncate(((pdtShfStk.F40_Amount - pdtShfStk.F40_ShippedAmount)/product.F09_PackingUnit)) * product.F09_PackingUnit;

                //•	If packing unit = 0 OR packing unit >= Amount 1 – Shipped Amount 1, then set Assign Amt = 0.
                if ((product == null) || product.F09_PackingUnit.Equals(0))
                    assignAmt = 0;

                string shelfRow = null;
                string shelfBay = null;
                string shelfLevel = null;

                if (assignAmt <= 0) return null;
                if (Constants.F40_StockFlag.TX40_StkFlg_NotStk.Equals(pdtShfStk.F40_StockFlag.Trim()) ||
                    Constants.F40_StockFlag.TX40_StkFlg_NotStkShip.Equals(pdtShfStk.F40_StockFlag.Trim()))
                {
                    if (update)
                    {
                        var productShelfStockUpdates = _unitOfWork.ProductShelfStockRepository.GetAll();
                        productShelfStockUpdates =
                            productShelfStockUpdates.Where(x => x.F40_PalletNo.Equals(pdtShfStk.F40_PalletNo)
                                                                && x.F40_PrePdtLotNo.Equals(pdtShfStk.F40_PrePdtLotNo)
                                //&& x.F40_ProductCode.Equals(pdtShfStk.F40_ProductCode)
                                                                && x.F40_ProductCode.Equals(productCode)
                                                                &&
                                                                Constants.F40_StockFlag.TX40_StkFlg_NotStk.Equals(
                                                                    x.F40_StockFlag));
                        foreach (var pdtShfStkUpdate in productShelfStockUpdates)
                        {
                            pdtShfStkUpdate.F40_StockFlag = Constants.F40_StockFlag.TX40_StkFlg_NotStkShip;
                            pdtShfStkUpdate.F40_ShipCommandNo = shippingNo;
                            //pdtShfStkUpdate.F40_AssignAmount = Math.Max(requestAmt, assignAmt);
                            if (requestAmt > assignAmt)
                            {
                                pdtShfStkUpdate.F40_AssignAmount = assignAmt;
                            }
                            else
                            {
                                pdtShfStkUpdate.F40_AssignAmount = requestAmt;
                            }
                            pdtShfStkUpdate.F40_UpdateDate = DateTime.Now;
                            _unitOfWork.ProductShelfStockRepository.Update(pdtShfStkUpdate);
                        }
                    }
                    assignProductShippingCommandResult.ib_outstk = true;
                    assignProductShippingCommandResult.ls_shelfno = "";
                    assignProductShippingCommandResult.ls_palletno = pdtShfStk.F40_PalletNo;
                    assignProductShippingCommandResult.lc_assign = assignAmt;
                }
                else if (Constants.F40_StockFlag.TX40_StkFlg_Stk.Equals(pdtShfStk.F40_StockFlag.Trim()) ||
                    Constants.F40_StockFlag.TX40_StkFlg_Shipping.Equals(pdtShfStk.F40_StockFlag.Trim()))
                {
                    var productShelfStatuses = _unitOfWork.ProductShelfStatusRepository.GetAll();
                    productShelfStatuses =
                        productShelfStatuses.Where(
                            x => Constants.F51_ShelfStatus.TX51_ShfSts_Pdt.Equals(x.F51_ShelfStatus.Trim())
                                //Constants.F51_ShelfStatus.TX51_ShfSts_RsvRtr.Equals(x.F51_ShelfStatus.Trim()))
                                 && x.F51_PalletNo.Equals(pdtShfStk.F40_PalletNo));                    
                    if (update)
                    {
                        foreach (var productShelfStatus in productShelfStatuses)
                        {
                            productShelfStatus.F51_ShelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_RsvRtr;
                            productShelfStatus.F51_TerminalNo = terminalNo;
                            productShelfStatus.F51_UpdateDate = DateTime.Now;

                            sShelfNo += string.Format("{0}-{1}-{2}|", productShelfStatus.F51_ShelfRow, productShelfStatus.F51_ShelfBay,
                        productShelfStatus.F51_ShelfLevel);

                            //shelfRow = productShelfStatus.F51_ShelfRow;
                            //shelfBay = productShelfStatus.F51_ShelfBay;
                            //shelfLevel = productShelfStatus.F51_ShelfLevel;
                            _unitOfWork.ProductShelfStatusRepository.Update(productShelfStatus);

                            break;
                        }
                        var productShelfStockUpdates = _unitOfWork.ProductShelfStockRepository.GetAll();
                        productShelfStockUpdates =
                            productShelfStockUpdates.Where(x => x.F40_PalletNo.Equals(pdtShfStk.F40_PalletNo)
                                                                && x.F40_PrePdtLotNo.Equals(pdtShfStk.F40_PrePdtLotNo)
                                //&& x.F40_ProductCode.Equals(pdtShfStk.F40_ProductCode)
                                                                && x.F40_ProductCode.Equals(productCode)
                                                                &&
                                                                Constants.F40_StockFlag.TX40_StkFlg_Stk.Equals(
                                                                    x.F40_StockFlag));

                        foreach (var pdtShfStkUpdate in productShelfStockUpdates)
                        {
                            pdtShfStkUpdate.F40_StockFlag = Constants.F40_StockFlag.TX40_StkFlg_Shipping;
                            pdtShfStkUpdate.F40_ShipCommandNo = shippingNo;
                            //pdtShfStkUpdate.F40_AssignAmount = Math.Max(requestAmt, assignAmt);
                            if (requestAmt > assignAmt)
                            {
                                pdtShfStkUpdate.F40_AssignAmount = assignAmt;
                            }
                            else
                            {
                                pdtShfStkUpdate.F40_AssignAmount = requestAmt;
                            }
                            pdtShfStkUpdate.F40_UpdateDate = DateTime.Now;
                            _unitOfWork.ProductShelfStockRepository.Update(pdtShfStkUpdate);
                        }
                    }
                    //foreach (var productShelfStatus in productShelfStatuses)
                    //{
                    //    shelfRow = productShelfStatus.F51_ShelfRow;
                    //    shelfBay = productShelfStatus.F51_ShelfBay;
                    //    shelfLevel = productShelfStatus.F51_ShelfLevel;                        
                    //}
                    assignProductShippingCommandResult.ib_instk = true;
                    //if (shelfBay != null && shelfRow != null && shelfLevel != null)
                    //{
                    //    assignProductShippingCommandResult.ls_shelfno = string.Format("{0}-{1}-{2}|", shelfRow, shelfBay,
                    //    shelfLevel);
                    //}
                    //else
                    //{
                    //    assignProductShippingCommandResult.ls_shelfno = "";
                    //}
                    assignProductShippingCommandResult.ls_shelfno = sShelfNo;
                    assignProductShippingCommandResult.ls_palletno = pdtShfStk.F40_PalletNo;
                    assignProductShippingCommandResult.lc_assign = assignAmt;
                }
                lstassignProductShippingCommandResult.Add(assignProductShippingCommandResult);
            }
            _unitOfWork.Commit();
            return lstassignProductShippingCommandResult;
        }
    }

    public class ShelfnopandPalletno
    {
        public string Palletno { get; set; }
        public string Shelfno { get; set; }
    }

    public class ProductDetailsItem
    {
        public string F51_ShelfRow { get; set; }
        public string F51_ShelfBay { get; set; }
        public string F51_ShelfLevel { get; set; }
        public string ShelfNo { get; set; }
        public string F51_PalletNo { get; set; }
        public string F40_ProductLotNo { get; set; }
        public double F40_Amount { get; set; }
        public double F40_PackageAmount { get; set; }
    }
}