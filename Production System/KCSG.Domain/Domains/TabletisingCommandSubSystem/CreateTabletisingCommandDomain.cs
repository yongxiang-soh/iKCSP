using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.ModelBinding;
using DocumentFormat.OpenXml.Wordprocessing;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Domains.Inquiry;
using KCSG.Domain.Enumerations;
using KCSG.Domain.Interfaces.TabletisingCommandSubSystem;
using KCSG.Domain.Models.Tabletising;
using KCSG.Domain.Models.TabletisingCommondSubSystem;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.TabletisingCommandSubSystem
{
    public class CreateTabletisingCommandDomain : ICreateTabletisingCommandDomain
    {
        #region Properties

        /// <summary>
        /// Unit of work which provides access to repositories.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize instance with dependency injections.
        /// </summary>
        /// <param name="unitOfWork"></param>
        public CreateTabletisingCommandDomain(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Find kneading command and do pagination.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="records"></param>
        /// <returns></returns>
        public async Task<FindTabletisingKneadingCommandItem> RetrieveTabletisingKneadingCommand(int page, int records)
        {
            // Find all kneading commands.
            var kneadingCommands = _unitOfWork.KneadingCommandRepository.GetAll();

            //	The system displays the list of Kneading Command that retrieved from “Input Kneading Command” form where [Status] = “Yet” or [Status] = “Command”.
            kneadingCommands =
                kneadingCommands.Where(
                    x =>
                        x.F42_Status.Equals(Constants.F42_Status.TX42_Sts_Tbtcmd) ||
                        x.F42_Status.Equals(Constants.F42_Status.TX42_Sts_Stored));

            // Find all product plans.
            var productPlans = _unitOfWork.PdtPlnRepository.GetAll();

            // Find all preproducts.
            var preproducts = _unitOfWork.PreProductRepository.GetAll();

            //var kneadingCommandItems = from kneadingCommand in kneadingCommands
            //                           from productPlan in productPlans
            //                           from preproduct in preproducts
            //                           where productPlan.F39_PreProductCode.Equals(kneadingCommand.F42_PreProductCode)
            //                           && productPlan.F39_KndEptBgnDate.Equals(kneadingCommand.F42_KndEptBgnDate)
            //                           && preproduct.F03_PreProductCode.Equals(kneadingCommand.F42_PreProductCode)
            //                           select new TabletisingKneadingCommandItem()
            //                           {
            //                               KneadingNo = kneadingCommand.F42_KndCmdNo,
            //                               PreproductCode = kneadingCommand.F42_PreProductCode,
            //                               PreproductName = preproduct.F03_PreProductName,
            //                               LotNo = kneadingCommand.F42_PrePdtLotNo,
            //                               Quantity = kneadingCommand.F42_ThrowAmount,
            //                               Status = kneadingCommand.F42_Status
            //                           };

            var kneadingCommandItems = from kneadingCommand in kneadingCommands
                                       from preproduct in preproducts
                                       where preproduct.F03_PreProductCode.Equals(kneadingCommand.F42_PreProductCode)
                                       select new TabletisingKneadingCommandItem()
                                       {
                                           KneadingNo = kneadingCommand.F42_KndCmdNo,
                                           PreproductCode = kneadingCommand.F42_PreProductCode,
                                           PreproductName = preproduct.F03_PreProductName,
                                           LotNo = kneadingCommand.F42_PrePdtLotNo,
                                           Quantity = kneadingCommand.F42_ThrowAmount,
                                           Status = kneadingCommand.F42_Status
                                       };

            // Count the total number matches with the result.
            var totalResults = await kneadingCommandItems.CountAsync();

            // Do pagination.
            kneadingCommandItems = kneadingCommandItems.OrderBy(x => x.KneadingNo)
                .Skip(page * records)
                .Take(records);

            return new FindTabletisingKneadingCommandItem()
            {
                KneadingCommands = await kneadingCommandItems.ToListAsync(),
                Total = totalResults
            };
        }

        /// <summary>
        /// Search kneading commands and gridize 'em
        /// </summary>
        /// <param name="gridSettings"></param>
        /// <returns></returns>
        public async Task<ResponseResult<GridResponse<object>>> SearchTabletisingKneadingCommands(
            GridSettings gridSettings)
        {
            var pageIndex = gridSettings.PageIndex - 1;
            if (pageIndex < 0)
                pageIndex = 0;

            var kneadingCommandResults =
                await RetrieveTabletisingKneadingCommand(pageIndex, gridSettings.PageSize);
            foreach (var kc in kneadingCommandResults.KneadingCommands)
            {
                if (kc.Status.Equals("4"))
                    kc.Status = "Yet";
                if (kc.Status.Equals("7"))
                    kc.Status = "Command";
            }
            // Build a grid and respond to client.
            var resultModel = new GridResponse<object>(kneadingCommandResults.KneadingCommands,
                kneadingCommandResults.Total);
            return new ResponseResult<GridResponse<object>>(resultModel, true);
        }

        /// <summary>
        /// Search kneading commands and gridize 'em
        /// </summary>
        /// <param name="gridSettings"></param>
        /// <param name="preproductCode"></param>
        /// <returns></returns>
        public async Task<ResponseResult<GridResponse<SearchProductInformationItem>>> SearchProductInformation(
            GridSettings gridSettings,
            string preproductCode)
        {
            var pageIndex = gridSettings.PageIndex - 1;
            if (pageIndex < 0)
                pageIndex = 0;

            var productGeneralInformation = _unitOfWork.ProductRepository.GetAll();

            productGeneralInformation = productGeneralInformation.Where(x => x.F09_PreProductCode.Trim().Equals(preproductCode.Trim()));
            var tabletisingCommandsCounter = await productGeneralInformation.CountAsync();

            productGeneralInformation = productGeneralInformation.OrderBy(x => x.F09_PreProductCode)
                .Skip(gridSettings.PageSize * pageIndex)
                .Take(gridSettings.PageSize);

            var productInformationList = await productGeneralInformation.ToListAsync();
            var lstSearchProductInformation = new List<SearchProductInformationItem>();
            var f56Status = Constants.F56_Status.NotTablet;
            var f40StockFlag = Constants.F40_StockFlag.TX40_StkFlg_Stk;
            foreach (var productInformation in productInformationList)
            {
                var searchProductInformation = new SearchProductInformationItem();
                searchProductInformation.F09_ProductCode = productInformation.F09_ProductCode;
                searchProductInformation.F09_ProductDesp = productInformation.F09_ProductDesp;
                searchProductInformation.Yieldrate = productInformation.F09_YieldRate;
                var tbtpdt =
                    _unitOfWork.TabletProductRepository.GetMany(
                        i =>
                            i.F56_ProductCode.Trim().Equals(productInformation.F09_ProductCode.Trim()) &&
                            i.F56_Status.Equals(f56Status));
                if (tbtpdt.Any())
                    searchProductInformation.CommandQty = tbtpdt.Sum(i => i.F56_TbtCmdAmt);
                else
                    searchProductInformation.CommandQty = 0;
                var tx40PdtShfStk =
                    _unitOfWork.ProductShelfStockRepository.GetMany(
                        i =>
                            i.F40_ProductCode.Trim().Equals(productInformation.F09_ProductCode.Trim()) &&
                            i.F40_StockFlag.Equals(f40StockFlag));
                if (tx40PdtShfStk.Any())
                    searchProductInformation.StorageQty = tx40PdtShfStk.Sum(i => i.F40_Amount);
                else
                    searchProductInformation.StorageQty = 0;

                lstSearchProductInformation.Add(searchProductInformation);
            }

            // Build a grid and respond to client.
            var resultModel = new GridResponse<SearchProductInformationItem>(lstSearchProductInformation,
                tabletisingCommandsCounter);
            return new ResponseResult<GridResponse<SearchProductInformationItem>>(resultModel, true);
        }

        /// <summary>
        /// Search product details and return a grid back.
        /// </summary>
        /// <param name="gridSettings"></param>
        /// <param name="commandNo"></param>
        /// <param name="prePdtLotNo"></param>
        /// <param name="preProductCode"></param>
        /// <returns></returns>
        public async Task<ResponseResult<GridResponse<object>>> SearchProductDetails(GridSettings gridSettings
            , string commandNo, string prePdtLotNo, string preProductCode)
        {
            if (string.IsNullOrEmpty(commandNo) || string.IsNullOrEmpty(prePdtLotNo))
                return null;

            var pageIndex = gridSettings.PageIndex - 1;
            if (pageIndex < 0)
                pageIndex = 0;
            var f56Status = Constants.F56_Status.NotTablet;

            var products =
                _unitOfWork.ProductRepository.GetMany(i => i.F09_PreProductCode.Trim().Equals(preProductCode.Trim()));

            var tableProducts = _unitOfWork.TabletProductRepository.GetAll();

            var results = from product in products
                          from tableProduct in tableProducts
                          where (
                              tableProduct.F56_ProductCode.Trim().Equals(product.F09_ProductCode.Trim()) &&
                              tableProduct.F56_KndCmdNo.Trim().Equals(commandNo.Trim()) &&
                              tableProduct.F56_PrePdtLotNo.Trim().Equals(prePdtLotNo.Trim()) &&
                              tableProduct.F56_Status.Equals(f56Status)
                              )
                          select new ProductLowerTable
                          {
                              ProductCode = tableProduct.F56_ProductCode,
                              ProductName = product.F09_ProductDesp,
                              TabletisingQuantity = tableProduct.F56_TbtCmdAmt,
                              LotNo = tableProduct.F56_ProductLotNo,
                              Yieldrate = product.F09_YieldRate,
                              UsedPreProduct = tableProduct.F56_TbtCmdAmt / (product.F09_YieldRate / 100)
                          };

            // Order and paging.
            results = results.OrderBy(x => x.ProductCode)
                .Skip(gridSettings.PageSize * pageIndex)
                .Take(gridSettings.PageSize);
            var totalRecords = results.Count();

            // Build a grid and respond to client.
            var resultModel = new GridResponse<object>(results, totalRecords);
            return new ResponseResult<GridResponse<object>>(resultModel, true);
        }


        public IList<ProductLowerTable> GetProductDetails(string productCode, string productName)
        {
            throw new NotImplementedException();
        }

        //public IList<ProductLowerTable> GetProductDetails(string productCode, string productName)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(productCode) || string.IsNullOrEmpty(productName))
        //            return null;


        //        var preproducts = _unitOfWork.PreProductRepository.GetAll();

        //        var products = _unitOfWork.ProductRepository.GetAll();
        //        products =
        //            products.Where(x => x.F09_ProductCode.Equals(productCode) && x.F09_ProductDesp.Equals(productName));

        //        // Find all kneading commands.
        //        var kneadingCommands = _unitOfWork.KneadingCommandRepository.GetAll();

        //        //	The system displays the list of Kneading Command that retrieved from “Input Kneading Command” form where [Status] = “Yet” or [Status] = “Command”.
        //        kneadingCommands =
        //            kneadingCommands.Where(
        //                x =>
        //                    x.F42_Status.Equals(Constants.F42_Status.TX42_Sts_Tbtcmd) ||
        //                    x.F42_Status.Equals(Constants.F42_Status.TX42_Sts_NotKnd));

        //        // Find the total used amount.
        //        double? tabletAmount = Convert.ToDouble(products.Sum(x => x.F09_TabletAmount));

        //        var result = from product in products
        //                     from preproduct in preproducts
        //                     from kneadingCommand in kneadingCommands
        //                     where product.F09_PreProductCode.Equals(preproduct.F03_PreProductCode)
        //                           && preproduct.F03_PreProductCode.Equals(kneadingCommand.F42_PreProductCode)
        //                     select new ProductLowerTable
        //                     {
        //                         ProductCode = product.F09_ProductCode,
        //                         ProductName = product.F09_ProductDesp,
        //                         TabletisingQuantity = preproduct.F03_BatchLot * (product.F09_YieldRate / 100),
        //                         UsedPreProduct = preproduct.F03_BatchLot - tabletAmount,
        //                         LotNo = kneadingCommand.F42_PrePdtLotNo,
        //                         Yieldrate = product.F09_YieldRate
        //                     };

        //        var results = from product in products
        //                      from tableProduct in tableProducts
        //                      where (
        //                          tableProduct.F56_ProductCode.Trim().Equals(product.F09_ProductCode.Trim()) &&
        //                          tableProduct.F56_KndCmdNo.Trim().Equals(commandNo.Trim()) &&
        //                          tableProduct.F56_PrePdtLotNo.Trim().Equals(prePdtLotNo.Trim()) &&
        //                          tableProduct.F56_Status.Equals(f56Status)
        //                          )
        //                      select new ProductLowerTable
        //                      {
        //                          ProductCode = tableProduct.F56_ProductCode,
        //                          ProductName = product.F09_ProductDesp,
        //                          TabletisingQuantity = tableProduct.F56_TbtCmdAmt,
        //                          LotNo = tableProduct.F56_ProductLotNo,
        //                          Yieldrate = product.F09_YieldRate,
        //                          UsedPreProduct = tableProduct.F56_TbtCmdAmt / (product.F09_YieldRate / 100),
        //                          tableQuanty = UsedPreProduct *(product.F09_YieldRate / 100)
        //                      }; 

        //        return result.ToList();
        //    }
        //    catch (Exception exception)
        //    {
        //        return null;
        //    }
        //}


        /// <summary>
        /// Search product details and return a grid back.
        /// </summary>
        /// <param name="gridSettings"></param>
        /// <returns></returns>
        public async Task<ResponseResult<GridResponse<object>>> SearchProductShelfStatuses(
            GridSettings gridSettings, string productCode, string lotNo)
        {
            try
            {
                //var firstChar = preProductCode[0];
                var pageIndex = gridSettings.PageIndex - 1;
                if (pageIndex < 0)
                    pageIndex = 0;

                //find record in TX51_PdtShfSts
                var productShelfStatuses = _unitOfWork.ProductShelfStatusRepository.GetAll();
                //find record in TX53_OutSidePrePdtStk
                var outsidePreProductStocks = _unitOfWork.OutSidePrePdtStkRepository.GetAll().Where(i => i.F53_OutSidePrePdtCode == productCode && i.F53_OutSidePrePdtLotNo == lotNo);


                var results = from productShelfStatus in productShelfStatuses
                              from outsidePreProductStock in outsidePreProductStocks
                              where productShelfStatus.F51_PalletNo.Trim().Equals(outsidePreProductStock.F53_PalletNo.Trim())
                              select new
                              {
                                  ShelfNo =
                                      productShelfStatus.F51_ShelfRow + "-" + productShelfStatus.F51_ShelfBay + "-" +
                                      productShelfStatus.F51_ShelfLevel,
                                  PalletNo = productShelfStatus.F51_PalletNo,
                                  Quantity = outsidePreProductStock.F53_Amount
                              };

                // Calculate the total number of records.
                var totalRecords = await results.CountAsync();

                // Order and paging.
                results = results.OrderBy(x => x.ShelfNo)
                    .Skip(gridSettings.PageSize * pageIndex)
                    .Take(gridSettings.PageSize);

                // Build a grid and respond to client.
                var resultModel = new GridResponse<object>(results, totalRecords);
                return new ResponseResult<GridResponse<object>>(resultModel, true);
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Search product details and return a grid back.
        /// </summary>
        /// <param name="gridSettings"></param>
        /// <returns></returns>
        public async Task<ResponseResult<GridResponse<object>>> SearchPreProductShelfStatuses(
            GridSettings gridSettings, string productCode, string lotNo)
        {
            try
            {
                //var firstChar = preProductCode[0];
                var pageIndex = gridSettings.PageIndex - 1;
                if (pageIndex < 0)
                    pageIndex = 0;

                var preProductSheltStatuses = _unitOfWork.PreProductShelfStatusRepository.GetAll();
                //find record in TX49_PrePdtShfStk
                var preProductShelfStocks = _unitOfWork.PreProductShelfStockRepository.GetAll().Where(i => i.F49_PreProductCode == productCode && i.F49_PreProductLotNo == lotNo);


                var results = from preProductSheltStatus in preProductSheltStatuses
                              from preProductShelfStock in preProductShelfStocks
                              where
                                  preProductSheltStatus.F37_ContainerCode.Trim()
                                      .Equals(preProductShelfStock.F49_ContainerCode.Trim())
                              select new
                              {
                                  ShelfNo =
                                      preProductSheltStatus.F37_ShelfRow + "-" + preProductSheltStatus.F37_ShelfBay + "-" +
                                      preProductSheltStatus.F37_ShelfLevel,
                                  ContainerCode = preProductShelfStock.F49_ContainerCode,
                                  Quantity = preProductShelfStock.F49_Amount
                              };

                // Calculate the total number of records.
                var totalRecords = await results.CountAsync();

                // Order and paging.
                results = results.OrderBy(x => x.ShelfNo)
                    .Skip(gridSettings.PageSize * pageIndex)
                    .Take(gridSettings.PageSize);

                // Build a grid and respond to client.
                var resultModel = new GridResponse<object>(results, totalRecords);
                return new ResponseResult<GridResponse<object>>(resultModel, true);
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Check whether the item is valid for delete or not.
        /// </summary>
        /// <param name="tabletisingKneadingCommandItem"></param>
        /// <returns></returns>
        public async Task<bool> IsValidDeleteItem(TabletisingKneadingCommandItem tabletisingKneadingCommandItem)
        {
            // 	If (the selected [Kneading No.] is not equal to 
            // [F41_KndCmdNo] column on table “tx41_tbtcmd”) 
            // or (the selected [Kneading No.] is equal to [F41_KndCmdNo] column on table “tx41_tbtcmd” 
            // and [F41_Status] is not “3” (Not Tablet)), the system will show message as MSG 11.
            var tabletCommands =
                _unitOfWork.TabletCommandRepository.GetMany(
                    x => x.F41_KndCmdNo.Equals(tabletisingKneadingCommandItem.KneadingNo));
            if (!(await tabletCommands.AnyAsync()))
                return false;

            tabletCommands =
                tabletCommands.Where(
                    x =>
                        !x.F41_Status.Equals(Constants.F41_Status.NotTablet));

            return !(await tabletCommands.AnyAsync());
        }

        /// <summary>
        /// This function is for deleting kneading command from tabletising list by using specific conditions.
        /// </summary>
        /// <param name="tabletisingKneadingCommandItem"></param>
        /// <returns></returns>
        public void DeleteTabletisingKneadingCommand(
            TabletisingKneadingCommandItem tabletisingKneadingCommandItem)
        {
            var lotNo = tabletisingKneadingCommandItem.LotNo;
            //	The system will delete the record from “tx56_tbtpdt” 
            //where [f56_kndcmdno] = [Kneading No.] column and [f56_prepdtlotno] = [Lot No.] column on the “List of Kneading Command” table on the Screen 1.
            _unitOfWork.TabletProductRepository.Delete(x =>
                x.F56_KndCmdNo.Trim().Equals(tabletisingKneadingCommandItem.KneadingNo.Trim()) &&
                x.F56_PrePdtLotNo.Trim().Equals(lotNo.Trim()));

            //	The system will also delete the record from “tx41_tbtcmd” 
            //where [f41_kndcmdno] = [Kneading No.] column and [f41_prepdtlotno] = [Lot No.] column on the “List of Kneading Command” table on the Screen 1.
            _unitOfWork.TabletCommandRepository.Delete(
                x =>
                    x.F41_KndCmdNo.Trim().Equals(tabletisingKneadingCommandItem.KneadingNo.Trim()) &&
                    x.F41_PrePdtLotNo.Trim().Equals(lotNo.Trim()));

            // Find the item in TX42_KndCmd which will be updated in the system.
            var kneadingCommands = _unitOfWork.KneadingCommandRepository.GetAll();
            kneadingCommands =
                kneadingCommands.Where(
                    x =>
                        x.F42_KndCmdNo.Trim().Equals(tabletisingKneadingCommandItem.KneadingNo.Trim()) &&
                        x.F42_PrePdtLotNo.Trim().Equals(lotNo.Trim()));

            foreach (var kneadingCommand in kneadingCommands)
            {
                kneadingCommand.F42_Status = Constants.F42_Status.TX42_Sts_Stored;
                kneadingCommand.F42_KndCmdNo = tabletisingKneadingCommandItem.KneadingNo;
            }

            _unitOfWork.Commit();
        }

        /// <summary>
        /// This function is for validating product planning.
        /// </summary>
        /// <param name="kneadingNo"></param>
        /// <param name="lotNo"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public async Task<string> ValidateProductPlanning(string kneadingNo, string lotNo, double quantity)
        {
            //•	If [f41_status] is not “3” in database and [f41_kndcmdno] = [Kneading No.] and [f41_prepdtlotno] = [Lot No.] on the table on “List of Kneading Command” section, the system will show message as MSG 16.
            var tabletCommands = _unitOfWork.TabletCommandRepository.GetAll();
            tabletCommands = tabletCommands.Where(x => !x.F41_Status.Trim().Equals(Constants.F41_Status.NotTablet));
            tabletCommands = tabletCommands.Where(x => x.F41_KndCmdNo.Trim().Equals(kneadingNo.Trim()));
            tabletCommands = tabletCommands.Where(x => x.F41_PrePdtLotNo.Trim().Equals(lotNo.Trim()));
            if (await tabletCommands.AnyAsync())
                return "MSG16";

            //•	Suppose that [remain_quantity] = [Quantity] on “List of Kneading Command” table – total of [UsedPreProd] on lower table on this section.
            //	If absolute value of [remain_quantity] <= 0.005, the system will show message as MSG 17. User clicks on “OK” button to continue saving data as BR 9. User clicks on “Cancel” to close alert, nothing changes.
            if (Math.Abs(quantity) <= 0.005)
                return "MSG17";

            //	If [remain_quantity] > 0.005, the system will show message as MSG 18. User clicks on “OK” button to continue processing as BR 8. User clicks on “Cancel” to close alert, nothing changes
            if (quantity > 0.005)
                return "MSG18";

            return "MSG19";
        }

        /// <summary>
        /// This function is for saving production planing.
        /// </summary>
        public async Task<InitiateTabletisingCommandResult> InitiateTabletisingCommand(InitiateTabletisingCommandViewModel item)
        {
            #region Command validation

            //•	If [f41_status] is not “3” in database and [f41_kndcmdno] = [Kneading No.] and [f41_prepdtlotno] = [Lot No.] on the table on “List of Kneading Command” section, the system will show message as MSG 16.
            var tabletCommands = _unitOfWork.TabletCommandRepository.GetAll();
            tabletCommands = tabletCommands.Where(x => !x.F41_Status.Trim().Equals(Constants.F41_Status.NotTablet));
            tabletCommands = tabletCommands.Where(x => x.F41_KndCmdNo.Trim().Equals(item.KneadingNo.Trim()));
            tabletCommands = tabletCommands.Where(x => x.F41_PrePdtLotNo.Trim().Equals(item.LotNo.Trim()));
            //if (await tabletCommands.AnyAsync())
            //    return "MSG16";

            if (await tabletCommands.AnyAsync())
                return InitiateTabletisingCommandResult.CommandExist;

            #endregion

            var lotNo = item.LotNo;
            var kneadingCommand =
                _unitOfWork.KneadingCommandRepository.Get(
                    i => i.F42_KndCmdNo.Trim().Equals(item.KneadingNo.Trim()));

            var tblCntAmt = kneadingCommand.F42_StgCtnAmt;
            //	Delete the record from “tx56_tbtpdt” table in database where [f56_kndcmdno] = [Kneading No.] column of table on “List of Kneading Command” section and [f56_prepdtlotno] = [Lot No.] column of table on “List of Kneading Command” section
            _unitOfWork.TabletProductRepository.Delete(
                x => x.F56_KndCmdNo.Trim().Equals(item.KneadingNo.Trim()) && x.F56_PrePdtLotNo.Trim().Equals(lotNo.Trim()));

            //o	If the system delete record successfully, 
            //the system will continue deleting record from “tx41_tbtcmd” in database where [f41_kndcmdno] = [Kneading No.] column on 
            //table on “List of Kneading Command” section and [f41_prepdtlotno] = [Lot No.] column on table on “List of Kneading Command” section
            _unitOfWork.TabletCommandRepository.Delete(
                x => x.F41_KndCmdNo.Trim().Equals(item.KneadingNo.Trim()) && x.F41_PrePdtLotNo.Trim().Equals(lotNo.Trim()));

            /*
             * 	If the system delete record from “tx56_tbtpdt” table and “tx41_tbtcmd” table successfully, the system will perform the following actions:
             * o	Insert new record into the “tx41_tbtcmd” table as below:
             * •	[f41_ kndcmdno] = [Kneading No.] column of table on “List of Kneading Command” section.
             * •	[f41_prepdtlotno] = [Lot No.] column of table on “List of Kneading Command” section.
             * •	[f41_preproductcode] = [Pre-Product Code] column of table on “List of Kneading Command” section.
             * •	[f41_status] = “3” (Not Tablet)
             * •	[f41_tblcntamt] = [f42_ stgctnamt] where [f42_ kndcmdno] = [Kneading No.] column of table on “List of Kneading Command” section and [f42_preproductcode] = [Pre-Product Code] column on table on “List of Kneading Command” section
             * •	[f41_rtrendcntamt] = “0”.
             * •	[f41_chgcntamt] = “0”.
             * •	[f41_adddate] = current date (format DD/MM/YYYY hh:mm:ss).
             * •	[f41_updatedate] = current date (format DD/MM/YYYY hh:mm:ss).
             * •	[f41_updatecount] = “0”
             */
            var systemTime = DateTime.Now;
            var tabletCommand = new TX41_TbtCmd();
            tabletCommand.F41_KndCmdNo = item.KneadingNo;
            tabletCommand.F41_PrePdtLotNo = lotNo;
            tabletCommand.F41_PreproductCode = item.PreproductCode;
            tabletCommand.F41_Status = Constants.F41_Status.NotTablet;
            tabletCommand.F41_TblCntAmt = tblCntAmt;
            tabletCommand.F41_RtrEndCntAmt = 0;
            tabletCommand.F41_ChgCntAmt = 0;
            tabletCommand.F41_AddDate = systemTime;
            tabletCommand.F41_UpdateDate = systemTime;
            tabletCommand.F41_UpdateCount = 0;
            _unitOfWork.TabletCommandRepository.Add(tabletCommand);

            // Find all kneading commands in database.
            var kneadingCommands = _unitOfWork.KneadingCommandRepository.GetAll();
            kneadingCommands =
                        kneadingCommands.Where(
                            x =>
                                x.F42_KndCmdNo.Trim().Equals(item.KneadingNo.Trim()) &&
                                x.F42_PrePdtLotNo.Trim().Equals(item.LotNo.Trim()));
            foreach (var command in kneadingCommands)
            {
                command.F42_Status = Constants.F42_Status.TX42_Sts_Tbtcmd;
                _unitOfWork.KneadingCommandRepository.Update(command);
            }

            /*
             * o	If the system insert new record into “tx41_tbtcmd” table in database successfully, the system will insert all the records of the lower table on Screen 2 into “tx56_tbtpdt” table in database, each record contains the data as follow:
             * •	[f56_kndcmdno] = [Kneading No.] column of table on “List of Kneading Command” section.
             * •	[f56_prepdtlotno] = [Lot No.] column of table on “List of Kneading Command” section.
             * •	[f56_productcode] = [Product Code] column of the lower table on “Create Tabletising Command” form.
             * •	[f56_productlotno] = [Lot No.] column of the lower table on “Create Tabletising Command” form.
             * •	[f56_status] = “Yet”.
             * •	[f56_tbtcmdamt] = [Tablet Qty] column on the lower table on “Create Tabletising Command” form.
             * •	[f56_tbtcmdendpackamt] = 0.0
             * •	[f56_tbtcmdendfrtamt] = 0.0
             * •	[f56_tbtcmdendamt] = 0.0
             * •	[f56_storageamt] = 0.0
             * •	[f56_certificationflag] = 
             * •	[f56_adddate] = current date (format DD/MM/YYYY hh:mm:ss).
             * •	[f56_updatedate] = current date (format DD/MM/YYYY hh:mm:ss).
             * •	[f56_updatecount] = 0.
             */
            if (item.Items != null)
            {
                foreach (var lowerItem in item.Items)
                {
                    var tabletProduct = new TX56_TbtPdt();
                    tabletProduct.F56_KndCmdNo = item.KneadingNo;
                    tabletProduct.F56_PrePdtLotNo = lotNo;
                    tabletProduct.F56_ProductCode = lowerItem.ProductCode;
                    tabletProduct.F56_ProductLotNo = lowerItem.LotNo;
                    tabletProduct.F56_CertificationFlag = Constants.F56_Status.NotTablet;
                    tabletProduct.F56_Status = Constants.F56_Status.NotTablet;
                    tabletProduct.F56_TbtCmdAmt = lowerItem.TabletisingQuantity;
                    tabletProduct.F56_TbtCmdEndPackAmt = 0;
                    tabletProduct.F56_TbtCmdEndFrtAmt = 0;
                    tabletProduct.F56_TbtCmdEndAmt = 0;
                    tabletProduct.F56_StorageAmt = 0;
                    tabletProduct.F56_AddDate = systemTime;
                    tabletProduct.F56_UpdateDate = systemTime;
                    _unitOfWork.TabletProductRepository.Add(tabletProduct);
                }
            }

            // Commit the changes.
            await _unitOfWork.CommitAsync();
            return InitiateTabletisingCommandResult.Success;
        }

        /// <summary>
        /// Update product quantity by using product code (Refer BR8)
        /// </summary>
        /// <param name="productCode"></param>
        /// <param name="quantity"></param>
        public void Update(string productCode, double quantity)
        {
            /*
             * 	New value of [UsedPreProd] = Old value of [UsedPreProd] + [remain_quantity] on BR 7.
             * 	New value of [Tablet Qty] = [remain_quantity] * ([Yield Rate]/100) where:
             * o	[remain_quantity] is calculated on BR 7.
             * o	[Yield Rate] = [f09_yieldrate] on table “tm09_product” on database where [f09_productcode] = [Product Code] of selected item on upper table
             */
            var products = _unitOfWork.ProductRepository.GetAll();
            products = products.Where(x => x.F09_ProductCode.Equals(productCode));

            var preproducts = _unitOfWork.PreProductRepository.GetAll();

            var results = from product in products
                          from preproduct in preproducts
                          where product.F09_PreProductCode.Equals(preproduct.F03_PreProductCode)
                          select new
                          {
                              product,
                              preproduct
                          };

            foreach (var result in results)
            {
                result.product.F09_TabletAmount = quantity * (result.product.F09_YieldRate / 100);
                result.preproduct.F03_BatchLot += (int)quantity;
            }

            _unitOfWork.Commit();
        }

        #endregion
    }
}