using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Core.Resources;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.ProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.ProductManagement
{
    public class StockTakingOfProductDomain : BaseDomain, IStockTakingOfProductDomain
    {
        #region Properties

        /// <summary>
        /// Domain which provides common functions for another.
        /// </summary>
        private readonly ICommonDomain _commonDomain;

        /// <summary>
        /// Service which handles the notification business.
        /// </summary>
        private readonly INotificationService _notificationService;

        /// <summary>
        /// Service which handles the configuration settings.
        /// </summary>
        private readonly IConfigurationService _configurationService;

        #endregion

        #region Properties

        /// <summary>
        /// Initiate domain with dependency injection.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="commonDomain"></param>
        /// <param name="notificationService"></param>
        /// <param name="configurationService"></param>
        public StockTakingOfProductDomain(IUnitOfWork unitOfWork, 
            ICommonDomain commonDomain, 
            INotificationService notificationService,
            IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _commonDomain = commonDomain;
            _notificationService = notificationService;
            _configurationService = configurationService;

        }

        #endregion

        #region Methods

        /// <summary>
        /// Find list of stock taking of product asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseResult<GridResponse<StockTakingOfProductItem>>> FindStockTakingOfProductAsync(
            string startShelfRow, string startShelfBay, string startShelfLevel,
            string endShelfRow, string endShelfBay, string endShelfLevel,
            GridSettings gridSettings)
        {
            var stockTakingOfProducts = FindStockTakingOfProduct(startShelfRow, startShelfBay, startShelfLevel,
                endShelfRow, endShelfBay, endShelfLevel);

            // Count total item before pagination happens.
            var totalRecords = await stockTakingOfProducts.CountAsync();

            // Order and paging.
            if (gridSettings != null)
                OrderByAndPaging(ref stockTakingOfProducts, gridSettings);

            var resultModel = new GridResponse<StockTakingOfProductItem>(await stockTakingOfProducts.ToListAsync(), totalRecords);
            return new ResponseResult<GridResponse<StockTakingOfProductItem>>(resultModel, true);
        }

        /// <summary>
        /// Find product details asynchronously.
        /// </summary>
        /// <param name="palletNo"></param>
        /// <returns></returns>
        public async Task<IList<StockTakingOfProductDetailItem>> FindProductDetailAsync(string palletNo)
        {
            /*
             * 	On click any row in Data Window, system will load data for Product Code, Product Name, LotNo and Quantity textboxes respectively from 
             * [f40_productcode], [f09_productdesp], [f40_productlotno] and [f40_amount] in TX40_PDTSHFSTK, in which: 
             * o	[f09_productcode] in TM09_PRODUCT = [f40_productcode],
             * o	AND {[f40_palletno] = Pallet No column value of selected row, AND [f40_stockflag] = “'Stock take” (or 3)}.
             */
            var productShelfStocks = _unitOfWork.ProductShelfStockRepository.GetAll();
            var products = _unitOfWork.ProductRepository.GetAll();

            var productsDetail = from productShelfStock in productShelfStocks
                from product in products
                where productShelfStock.F40_ProductCode.Equals(product.F09_ProductCode)
                      && productShelfStock.F40_PalletNo.Trim().Equals(palletNo)
                      && Constants.F40_StockFlag.TX40_StkFlg_Stk.Equals(productShelfStock.F40_StockFlag.Trim())
                select new StockTakingOfProductDetailItem()
                {
                    F40_ProductCode = productShelfStock.F40_ProductCode,
                    F40_Amount = productShelfStock.F40_Amount,
                    F40_ProductLotNo = productShelfStock.F40_ProductLotNo,
                    F09_ProductDesp = product.F09_ProductDesp
                };


            return await productsDetail.ToListAsync();

        }

        /// <summary>
        /// Retrieve product details asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseResult<RetrievalItem>> RetrieveProductDetailsAsync(string palletNo, string row, string bay, string level, string terminalNo)
        {
            //o	System will get Serial No by doing as follow:
            //•	Suppose Sequence No is selected from [f48_pdtwhscmdno] in TX48_NOMANAGE, where [f48_systemid] = “00000”.
            var seqNo1 = string.Format("0000{0}", await _commonDomain.InsertIntoNoManageAsync());
            var seqNo = seqNo1.Substring(seqNo1.Length - 4, 4);

            /*
             * o	Update TX40_PDTSHFSTK, in which:
             * •	[f40_palletno] = Pallet No column value of selected row,
             * •	AND [f40_stockflag] = “Stock take” (or 3).
             * Then:
             * •	Set [f40_stockflag] = “Retrieval” (or 2).
             */
            var productShelfStocks = _unitOfWork.ProductShelfStockRepository.GetAll();
            var stockFlag = Constants.F40_StockFlag.TX40_StkFlg_Stk;
            productShelfStocks =
                productShelfStocks.Where(x => x.F40_StockFlag.Trim().Equals(stockFlag)
                && x.F40_PalletNo.Trim().Equals(palletNo));

            foreach (var productShelfStock in productShelfStocks)
            {
                productShelfStock.F40_StockFlag = Constants.F40_StockFlag.TX40_StkFlg_Rtr;
                _unitOfWork.ProductShelfStockRepository.Update(productShelfStock);
            }
                


            //o	Suppose Row, Bay and Level are 2 first ## characters, 2 middle ## characters and 2 last ## characters of Shelf No of selected record
            /*
             * o	Update TX51_PDTSHFSTS, in which:
             * •	[f51_shelfrow] = Row above,
             * •	AND [f51_shelfbay] = Bay above, 
             * •	AND [f51_shelflevel] = Level above,
             * •	AND [f51_shelfstatus] = “Products Stocked’ (or 2),
             * •	AND [f51_stocktakingflag] = “Yet to stock-take” (or 0).
             * */
            var productShelfStatuses = _unitOfWork.ProductShelfStatusRepository.GetAll();
            productShelfStatuses = productShelfStatuses.Where(x => x.F51_ShelfRow.Trim().Equals(row));
            productShelfStatuses = productShelfStatuses.Where(x => x.F51_ShelfBay.Trim().Equals(bay));
            productShelfStatuses = productShelfStatuses.Where(x => x.F51_ShelfLevel.Trim().Equals(level));
            productShelfStatuses = productShelfStatuses.Where(x => Constants.F51_ShelfStatus.TX51_ShfSts_Pdt.Equals(x.F51_ShelfStatus.Trim()));
            productShelfStatuses = productShelfStatuses.Where(x => Constants.F51_StockTakingFlag.TX51_StkTkgFlg_InvNotChk.Equals(x.F51_StockTakingFlag.Trim()));
            //productShelfStatuses = productShelfStatuses.Where(x => x.F51_ShelfRow.Trim().Equals(row));                                    

            // Count total records.
            var productShelfStatusesTotal = await productShelfStatuses.CountAsync();
            //* If there is no record updated from the update query, system shows message MSG 47, stops the use case and reloads the current page.
            if (productShelfStatusesTotal == 0)
                //throw new Exception("MSG47");
                return new ResponseResult<RetrievalItem>(null, false, "MSG47");            
                

            /*
             * Then:
             * •	Set [f51_shelfstatus] = “Assigned for Retrieval” (or 5),
             * •	Set [f51_terminalno] = Terminal No. which is retrieved from section “server” in “toshiba.ini” configurable file,
             * •	Set [f51_updatedate] = current date time.
            */
            foreach (var productShelfStatus in productShelfStatuses)
            {
                productShelfStatus.F51_ShelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_RsvRtr;
                productShelfStatus.F51_TerminalNo = terminalNo;
                productShelfStatus.F51_UpdateDate = DateTime.Now;
                _unitOfWork.ProductShelfStatusRepository.Update(productShelfStatus);
                break;
            }

            var condition = Constants.F47_Status.AnInstruction.ToString("D");
            var condition2 = Constants.F47_StrRtrType.Product.ToString("D");

            var pdtWhsCmd = new TX47_PdtWhsCmd();
            pdtWhsCmd.F47_CommandNo = "7000";
            pdtWhsCmd.F47_CmdSeqNo = seqNo;
            pdtWhsCmd.F47_CommandType = "0000";
            pdtWhsCmd.F47_StrRtrType = condition2;
            pdtWhsCmd.F47_Status = condition;

            pdtWhsCmd.F47_Priority = 0;
            pdtWhsCmd.F47_PalletNo = palletNo;
            pdtWhsCmd.F47_From = string.Format("{0}{1}{2}", row, bay, level);

            var conveyor = await _unitOfWork.ConveyorRepository.GetAll().FirstOrDefaultAsync(i => i.F05_TerminalNo.Trim().Equals(terminalNo));
            pdtWhsCmd.F47_To = conveyor.F05_ConveyorCode;
            pdtWhsCmd.F47_TerminalNo = terminalNo;
            pdtWhsCmd.F47_PictureNo = "TCPR061F";
            pdtWhsCmd.F47_RetryCount = 0;
            pdtWhsCmd.F47_AddDate = DateTime.Now;
            pdtWhsCmd.F47_UpdateDate = DateTime.Now;
            _unitOfWork.ProductWarehouseCommandRepository.Add(pdtWhsCmd);

            // Save changes into database asynchronously.
            await _unitOfWork.CommitAsync();

            

            
            var productWarehouses = _unitOfWork.ProductWarehouseCommandRepository.GetAll();
            productWarehouses =
                productWarehouses.Where(x => x.F47_CommandNo.Trim().Equals("7000") && x.F47_CmdSeqNo.Trim().Equals(seqNo)
                && x.F47_Status.Trim().Equals(condition));
            
            foreach (var item in productWarehouses)
            {
                var text1 = item.F47_TerminalNo;
                var text2 = item.F47_PictureNo;
                var text3 = item.F47_CommandType;
                var text4 = item.F47_Status;
                var text5 = item.F47_From;
                var text6 = item.F47_To;
                var text7 = item.F47_PalletNo;
                var message = "0011" + text1 + text2 + "0066" + seqNo + "7000" + text3 + text4 + text5 + text6 + text7 +
                              DateTime.Now.ToString("d");
                //_notificationService.SendMessageToC3(new
                //{
                //    "screenName",
                //    message
                //});
                _notificationService.SendMessageToC3("", message);               
            }
            var assignPalletItem = new RetrievalItem
            {
                Row = row,
                Bay = bay,
                Level = level
            };
            return new ResponseResult<RetrievalItem>(assignPalletItem, true, "");
            // TODO : Display the waiting notification message: “Shelf No [" + Shelf No of selected record + "] retrieving ..."
        }

        /// <summary>
        /// Retrieve product stock details asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task<object> FindProductConfirmDetails(string shelfRow, string shelfBay,
            string shelfLevel, string palletNo)
        {
            var products = _unitOfWork.ProductRepository.GetAll();
            var productStocks = _unitOfWork.ProductShelfStockRepository.GetAll();

            /*
             * 	Retrieve 
             * [f40_productcode], 
             * [f40_prepdtlotno], 
             * [f40_productlotno], 
             * [f40_stockflag], 
             * [f40_packageamount], 
             * [f40_fraction], 
             * [f40_amount], 
             * [f40_tabletingenddate], 
             * [f40_shippedamount], 
             * [f40_shipcommandno], 
             * [f40_assignamount], 
             * [f40_certificationflg], 
             * [f40_certificationdate], 
             * [f40_adddate], 
             * [f40_updatedate] 
             * and [f40_updatecount] 
             * from TX40_PDTSHFSTK AND [f09_productdesp], [f09_packingunit] from TM09_PRODUCT, in which:
             * o	[f09_productcode] = [f40_productcode],
             * o	AND [f40_palletno] = [Pallet No],
             * o	AND [f40_stockflag] = “Retrieval’ (or 2).

             */
            var result = from product in products
                         from productStock in productStocks
                         where 
                         product.F09_ProductCode.Equals(productStock.F40_ProductCode) &&
                         productStock.F40_PalletNo.Trim().Equals(palletNo.Trim()) &&
                         productStock.F40_StockFlag.Trim().Equals(Constants.F40_StockFlag.TX40_StkFlg_Rtr)
                select new
                {
                    productStock.F40_ProductCode,
                    productStock.F40_PrePdtLotNo,
                    productStock.F40_ProductLotNo,
                    productStock.F40_StockFlag,
                    productStock.F40_PackageAmount,
                    productStock.F40_Fraction,
                    productStock.F40_Amount,
                    productStock.F40_TabletingEndDate,
                    productStock.F40_ShippedAmount,
                    productStock.F40_ShipCommandNo,
                    productStock.F40_AssignAmount,
                    productStock.F40_CertificationFlg,
                    productStock.F40_CertificationDate,
                    productStock.F40_AddDate,
                    productStock.F40_UpdateDate,
                    productStock.F40_UpdateCount,
                    product.F09_ProductDesp,
                             product.F09_PackingUnit,
                             PackQty = (int)((productStock.F40_Amount - productStock.F40_ShippedAmount) / product.F09_PackingUnit),
                             Fraction = productStock.F40_Amount - productStock.F40_ShippedAmount - product.F09_PackingUnit * (int)((productStock.F40_Amount - productStock.F40_ShippedAmount) / product.F09_PackingUnit)
                };

            result = result
                .OrderBy(x => x.F40_ProductCode)
                .Skip(0)
                .Take(5);
            return await result.ToListAsync();
        }

        /// <summary>
        /// Restore product asynchronously
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="palletNo"></param>
        /// <param name="items"></param>
        /// <param name="row"></param>
        /// <param name="bay"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public async Task RestoreProductsAsync(string terminalNo, string palletNo, 
            IList<StockTakingOfProductConfirmItem> items,
            string row, string bay, string level)
        {
            // Find conveyor from terminal no.
            var conveyor = await _commonDomain.FindConveyorCodeAsync(terminalNo);

            // Convert enum to string.
            var conveyorStatusError = Constants.F05_StrRtrSts.Error.ToString("D");

            //If retrieval is failed or retrieved status is “Error” (or 9), system shows the message MSG 8, stop the use case.
            if (conveyor == null || conveyorStatusError.Equals(conveyor.F05_StrRtrSts.Trim()))
                throw new Exception(HttpMessages.InvalidConveyorStatus);

            /*
             * o	Retrieve [f14_devicestatus] and [f14_usepermitclass] from TM14_DEVICE, where [f14_devicecode] = Product Device Code under “Device” section of “toshiba.ini” configurable file.
             * If retrieval is failed OR retrieved status is “Error” (or 2) OR “Offline” (or 1) OR retrieved permit is “Prohibited” (or 1), system shows the message MSG 9, stop the use case.
             */
            var devices = _unitOfWork.DeviceRepository.GetAll();
            devices = devices.Where(x => x.F14_DeviceCode.Equals(_configurationService.ProductDeviceCode));

            // Status conversion.
            var statusError = Constants.F14_DeviceStatus.Error.ToString("D");
            var statusOffline = Constants.F14_DeviceStatus.Offline.ToString("D");
            var permissionProhibited = Constants.F14_UsePermitClass.Prohibited.ToString("D");

            // Find the first device in the query.
            var device = await devices.FirstOrDefaultAsync();
            if (device == null || statusError.Equals(device.F14_DeviceStatus) ||
                statusOffline.Equals(device.F14_DeviceCode) || permissionProhibited.Equals(device.F14_UsePermitClass))
                throw new Exception(HttpMessages.InvalidWarehouseStatus);

            /*
             * 	System will delete all data from TX40_PDTSHFSTK, in which [F40_PalletNo] = [Pallet No] textbox value.
             */
            _unitOfWork.ProductShelfStockRepository.Delete(x => x.F40_PalletNo.Trim().Equals(palletNo));

            foreach (var item in items)
            {
                var pdtShfStk = new TX40_PdtShfStk();
                pdtShfStk.F40_PalletNo = palletNo;
                pdtShfStk.F40_PrePdtLotNo = item.F40_PrePdtLotNo;
                pdtShfStk.F40_ProductCode = item.F40_ProductCode;
                pdtShfStk.F40_ProductLotNo = item.F40_ProductLotNo;
                pdtShfStk.F40_StockFlag = Constants.F40_StockFlag.TX40_StkFlg_Str;
                pdtShfStk.F40_PackageAmount = item.PackQty;
                pdtShfStk.F40_Fraction = item.Fraction;
                pdtShfStk.F40_Amount = item.F09_PackingUnit*item.PackQty + item.Fraction;
                pdtShfStk.F40_TabletingEndDate = item.F40_Tabletingenddate;
                pdtShfStk.F40_ShippedAmount = 0;
                pdtShfStk.F40_CertificationFlg = item.F40_CertificationFlg;
                pdtShfStk.F40_CertificationDate = item.F40_Certificationdate;
                pdtShfStk.F40_AddDate = item.F40_AddDate;
                pdtShfStk.F40_UpdateDate = DateTime.Now;

                _unitOfWork.ProductShelfStockRepository.Add(pdtShfStk);
            }
            /*
             * 	Update TX51_PDTSHFSTS, in which:
             * o	[f51_shelfrow] = 2 first ## characters of [Location],
             * o	AND [f51_shelfbay] = 2 middle ## characters of [Location],
             * o	AND [f51_shelflevel] = 2 last ## characters of [Location],
             * o	Then:
             * o	Set [f51_shelfstatus] = “Assigned for Storage” (or 4),
             * o	Set [f51_terminalno] = Terminal No. which is retrieved from section “server” in “toshiba.ini” configurable file,
             * o	Set [f51_updatedate] = current date time.
             */
            var productShelfStatuses = _unitOfWork.ProductShelfStatusRepository.GetAll();
            productShelfStatuses = productShelfStatuses.Where(x => x.F51_ShelfRow.Trim().Equals(row)
                                                                   && x.F51_ShelfBay.Trim().Equals(bay)
                                                                   && x.F51_ShelfLevel.Trim().Equals(level));

            foreach (var productShelfStatus in productShelfStatuses)
            {
                productShelfStatus.F51_ShelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_RsvStr;
                productShelfStatus.F51_TerminalNo = terminalNo;
                productShelfStatus.F51_UpdateDate = DateTime.Now;
                break;
            }

            //o	Insert data into TX47_PDTWHSCMD:
            var pdtWhsCmd = new TX47_PdtWhsCmd();
            var iSeqNo = await _commonDomain.InsertIntoNoManageAsync();
            pdtWhsCmd.F47_CmdSeqNo = iSeqNo.ToString("D4");
            pdtWhsCmd.F47_CommandNo = Constants.F47_CommandNo.StockTakingIn.ToString("D");
            pdtWhsCmd.F47_CommandType = "0000";
            pdtWhsCmd.F47_StrRtrType = Constants.F47_StrRtrType.Product.ToString("D");
            pdtWhsCmd.F47_Status = Constants.F47_Status.AnInstruction.ToString("D");
            pdtWhsCmd.F47_Priority = 0;
            pdtWhsCmd.F47_PalletNo = palletNo;
            pdtWhsCmd.F47_From = conveyor.F05_ConveyorCode;
            pdtWhsCmd.F47_To = string.Format("{0}{1}{2}", row, bay, level);
            pdtWhsCmd.F47_TerminalNo = terminalNo;
            pdtWhsCmd.F47_PictureNo = "TCPR062F";
            pdtWhsCmd.F47_RetryCount = 0;
            pdtWhsCmd.F47_AddDate = pdtWhsCmd.F47_UpdateDate = DateTime.Now;
            _unitOfWork.ProductWarehouseCommandRepository.Add(pdtWhsCmd);

            _unitOfWork.Commit();
            
            _notificationService.SendMessageToC3("TCPR062F", _notificationService.FormatThirdCommunicationMessageResponse(pdtWhsCmd));
        }
        
        /// <summary>
        /// Find stock taking of product.
        /// </summary>
        /// <param name="startShelfRow"></param>
        /// <param name="startShelfBay"></param>
        /// <param name="startShelfLevel"></param>
        /// <param name="endShelfRow"></param>
        /// <param name="endShelfBay"></param>
        /// <param name="endShelfLevel"></param>
        /// <returns></returns>
        private IQueryable<StockTakingOfProductItem> FindStockTakingOfProduct(string startShelfRow, string startShelfBay, string startShelfLevel,
        //private IQueryable<TX51_PdtShfSts> FindStockTakingOfProduct(string startShelfRow, string startShelfBay, string startShelfLevel,
            string endShelfRow, string endShelfBay, string endShelfLevel)
        {
            var productShelfStatuses = _unitOfWork.ProductShelfStatusRepository.GetAll();
            productShelfStatuses = productShelfStatuses.Where(x => 
            string.Compare(x.F51_ShelfRow, startShelfRow) != -1 
            && string.Compare(x.F51_ShelfBay, startShelfBay) != -1 
            && string.Compare(x.F51_ShelfLevel, startShelfLevel) != -1);

#if DEBUG
            var a = productShelfStatuses.Count();
#endif

            productShelfStatuses = productShelfStatuses.Where(x =>
            string.Compare(x.F51_ShelfRow, endShelfRow) != 1
            && string.Compare(x.F51_ShelfBay, endShelfBay) != 1
            && string.Compare(x.F51_ShelfLevel, endShelfLevel) != 1);

#if DEBUG
            var b = productShelfStatuses.Count();
#endif

            var result =
                productShelfStatuses.Where(
                    x => Constants.F51_ShelfStatus.TX51_ShfSts_Pdt.Equals(x.F51_ShelfStatus.Trim())
                    && Constants.F51_StockTakingFlag.TX51_StkTkgFlg_InvNotChk.Equals(x.F51_StockTakingFlag.Trim()))
                    .Select(x => new StockTakingOfProductItem
                    {
                        F51_ShelfRow = x.F51_ShelfRow,
                        F51_ShelfBay = x.F51_ShelfBay,
                        F51_ShelfLevel = x.F51_ShelfLevel,
                        F51_PalletNo = x.F51_PalletNo
                    });
            //.Select(x => new 
            //{
            //    F51_ShelfRow = x.F51_ShelfRow,
            //    F51_ShelfBay = x.F51_ShelfBay,
            //    F51_ShelfLevel = x.F51_ShelfLevel,
            //    F51_PalletNo = x.F51_PalletNo
            //});

#if DEBUG
            var c = result.Count();
#endif

            return result;
        }

        /// <summary>
        /// Proceed
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        public IList<ThirdCommunicationResponse> RespondingReplyFromC3RetrieveProduct(string terminalNo)
        {

            var lstTx47 =
                _unitOfWork.ProductWarehouseCommandRepository.GetByTerminalNoAndPictureNoAndStatus679(terminalNo,
                    Constants.PictureNo.TCPR061F);

            var items = new List<ThirdCommunicationResponse>();

            foreach (var tx47 in lstTx47)
            {
                var lstPreproductionCode =
                            _unitOfWork.OutSidePrePdtStkRepository.GetMany(i => i.F53_PalletNo == tx47.F47_PalletNo)
                                .Select(i => i.F53_OutSidePrePdtCode.Trim()).FirstOrDefault();

                var oldStatus = tx47.F47_Status;

                switch (tx47.F47_Status)
                {
                    case "6": //Command End
                        tx47.F47_Status = "C";
                        break;
                    case "7": //Command Cancel
                        tx47.F47_Status = "F";
                        break;
                    case "9": //Command Error
                        tx47.F47_Status = "D";

                        break;
                    default:
                        continue;
                }

                var item = Mapper.Map<ThirdCommunicationResponse>(tx47);
                item.OldStatus = oldStatus;
                item.ProductCode = string.IsNullOrEmpty(lstPreproductionCode) ? "" : lstPreproductionCode;
                item.F47_UpdateDate = tx47.F47_UpdateDate = DateTime.Now;
                _unitOfWork.ProductWarehouseCommandRepository.Update(tx47);

                items.Add(item);
            }

            _unitOfWork.Commit();

            return items;
        }
        
        /// <summary>
        /// Proceed respose mesage sent back from C3.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="shelfNo"></param>
        /// <returns></returns>
        public IList<ThirdCommunicationResponse> RespondingReplyFromC3RestoreProduct(string terminalNo, string row, string bay, string level)
        {
            // List of thrid communication response items.
            var items = new List<ThirdCommunicationResponse>();

            var lstTx47 =
                _unitOfWork.ProductWarehouseCommandRepository.GetByTerminalNoAndPictureNoAndStatus679(terminalNo,
                    "TCPR062F");
            var tx51 =
                _unitOfWork.ProductShelfStatusRepository.GetMany(
                    i => i.F51_ShelfRow == row && i.F51_ShelfBay == bay && i.F51_ShelfLevel == level).FirstOrDefault();
            if (tx51 != null)
            {
                tx51.F51_StockTakingFlag = Constants.TX51StockTakingFlag.TX51_StkTkgFlg_InvChk.ToString("d");
                tx51.F51_TerminalNo = terminalNo;
                _unitOfWork.ProductShelfStatusRepository.Update(tx51);
            }

            foreach (var tx47 in lstTx47)
            {
                if (string.IsNullOrEmpty(tx47.F47_Status) || tx47.F47_Status.Length != 1)
                    continue;

                var productShfStks = _unitOfWork.ProductShelfStockRepository.GetAll();
                productShfStks = productShfStks.Where(x => x.F40_PalletNo.Equals(tx47.F47_PalletNo));
                var productShfStk = productShfStks.FirstOrDefault();

                var newStatus = "";
                var item = new ThirdCommunicationResponse();
                item = Mapper.Map<ThirdCommunicationResponse>(tx47);
                item.OldStatus = tx47.F47_Status;

                switch (tx47.F47_Status[0])
                {
                    case '6': //Command End
                        newStatus = "C";
                        break;
                        case '7': //Command Cancel
                            newStatus = "F";
                        break;
                    case '9': //Command Error
                        newStatus = "D";
                        break;
                    default:
                        continue;
                }

                
                
                item.ProductCode = productShfStk == null ? "" : productShfStk.F40_ProductCode;

                item.F47_Status = tx47.F47_Status = newStatus;
                item.F47_UpdateDate = tx47.F47_UpdateDate = DateTime.Now;
                item.ProductWarehouseItem = tx51;

                _unitOfWork.ProductWarehouseCommandRepository.Update(tx47);
                items.Add(item);
            }
            
            _unitOfWork.Commit();

            return items;

        }
        #endregion
    }
}