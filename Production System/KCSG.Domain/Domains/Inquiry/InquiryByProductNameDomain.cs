using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.Inquiry;
using KCSG.Domain.Models.Inquiry;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.Data.Repositories;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.Domain.Models.Inquiry.ByExternalPre_ProductName;
using KCSG.Domain.Models.Inquiry.ByPreProductName;
using KCSG.Domain.Models.Inquiry.ByProductName;
using KCSG.Domain.Models.Inquiry.BySupplierName;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.Inquiry
{
    public class InquiryByProductNameDomain : BaseDomain, IInquiryByProductNameDomain
    {
        #region Constructor

        public InquiryByProductNameDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        public ResponseResult<GridResponse<ForceRetrievalOfProductItem>> SearchCriteria(string productCode,
            GridSettings gridSettings, out double total, out double deliveryTotal, out double cerTotal, out double nonCerTotal)
        {            
            // Find all tx40
            var productShelfStocks = _unitOfWork.ProductShelfStockRepository.GetAll();

        //    // Find all tx51
            var productShelfStatuss = _unitOfWork.ProductShelfStatusRepository.GetAll();

            var condition1 = Constants.F40_StockFlag.TX40_StkFlg_Stk;
            //Expression<Func<TX44_ShippingPlan, double>> expression = i => i.F44_ShpRqtAmt;
            if (!string.IsNullOrEmpty(productCode))
            {
                var result = from productShelfStock in productShelfStocks
                             join productShelfStatus in productShelfStatuss on productShelfStock.F40_PalletNo equals productShelfStatus.F51_PalletNo

                             where productShelfStock.F40_ProductCode.Trim().Equals(productCode.Trim()) &&
                                   productShelfStock.F40_StockFlag.Trim().Equals(condition1)
                             orderby new
                             {
                                 productShelfStock.F40_ProductLotNo,
                                 productShelfStatus.F51_ShelfRow,
                                 productShelfStatus.F51_ShelfBay,
                                 productShelfStatus.F51_ShelfLevel                                 
                             }
                             select
                             
                             //new StockTakingOfProductItem
                             new ForceRetrievalOfProductItem
                             {
                                 ProductLotNo = productShelfStock.F40_ProductLotNo,
                                 ShelfNo1 = productShelfStatus.F51_ShelfRow + "-" + productShelfStatus.F51_ShelfBay + "-" + productShelfStatus.F51_ShelfLevel,
                                 Amount = productShelfStock.F40_Amount,
                                 //Amount = Convert.ToDouble(String.Format("{0:#,##0.00}", productShelfStock.F40_Amount)),
                                 PreProdLotNo = productShelfStock.F40_PrePdtLotNo,
                                 CerfDate = productShelfStock.F40_CertificationDate,
                                 CerfFlag = productShelfStock.F40_CertificationFlg
                             };

                // Get GrandTotal
                double tempTotal = 0;
                if (result.Any())
                {
                    tempTotal = result.Sum(i => i.Amount);
                }
                total = tempTotal;

                //Get Delivery Total
                var status = Constants.F44_Status.F44_Sts_NotShip;
                var shippingPlans =
                    _unitOfWork.ShippingPlanRepository.GetMany(i => i.F44_ProductCode.Trim().Equals(productCode.Trim())
                                                                    && i.F44_Status.Trim().Equals(status));
                deliveryTotal = shippingPlans.Any() ? shippingPlans
                                        .Sum(i => i.F44_ShpRqtAmt) : 0;

                //Get Certified Total
                var stockFlag = Constants.F40_StockFlag.TX40_StkFlg_Stk;
                var cerFlag1 = Constants.F40_CertificationFlag.TX40_CrtfctnFlg_Ok.ToString("D");
                var cerFlag2 = Constants.F40_CertificationFlag.TX40_CrtfctnFlg_NG.ToString("D");
                var resultCerTotal = from productShelfStock in productShelfStocks
                                     join productShelfStatus in productShelfStatuss on productShelfStock.F40_PalletNo equals productShelfStatus.F51_PalletNo

                                     where productShelfStock.F40_StockFlag.Trim().Equals(stockFlag)
                                    && productShelfStock.F40_ProductCode.Trim().Equals(productCode.Trim())
                                    && (productShelfStock.F40_CertificationFlg.Trim().Equals(cerFlag1) || productShelfStock.F40_CertificationFlg.Trim().Equals(cerFlag2))
                                     select
                                     new ForceRetrievalOfProductItem
                                     {
                                         Amount = productShelfStock.F40_Amount
                                     };
                double tempCerTotal = 0;
                if (resultCerTotal.Any())
                {
                    tempCerTotal = resultCerTotal.Sum(i => i.Amount);
                }
                cerTotal = tempCerTotal;

                //Get Non-Certified Total                
                var cerFlag3 = Constants.F40_CertificationFlag.TX40_CrtfctnFlg_Normal.ToString("D");
                var cerFlag4 = Constants.F40_CertificationFlag.TX40_CrtfctnFlg_OutofSpec.ToString("D");
                var resultNonCerTotal = from productShelfStock in productShelfStocks
                                        join productShelfStatus in productShelfStatuss on productShelfStock.F40_PalletNo equals productShelfStatus.F51_PalletNo

                                        where productShelfStock.F40_StockFlag.Trim().Equals(stockFlag)
                                       && productShelfStock.F40_ProductCode.Trim().Equals(productCode.Trim())
                                       && (productShelfStock.F40_CertificationFlg.Trim().Equals(cerFlag3) || productShelfStock.F40_CertificationFlg.Trim().Equals(cerFlag4))
                                        select
                                        new ForceRetrievalOfProductItem
                                        {
                                            Amount = productShelfStock.F40_Amount
                                        };
                double tempNonCerTotal = 0;
                if (resultNonCerTotal.Any())
                {
                    tempNonCerTotal = resultNonCerTotal.Sum(i => i.Amount);
                }
                nonCerTotal = tempNonCerTotal;


                var itemCount = result.Count();
                
                if (gridSettings != null)
                    OrderByAndPaging(ref result, gridSettings);
                var resultModel = new GridResponse<ForceRetrievalOfProductItem>(result, itemCount);
                return new ResponseResult<GridResponse<ForceRetrievalOfProductItem>>(resultModel, true);
            }
            else
            {
                var result = from productShelfStock in productShelfStocks
                             join productShelfStatus in productShelfStatuss on productShelfStock.F40_PalletNo equals productShelfStatus.F51_PalletNo

                             where productShelfStock.F40_StockFlag.Trim().Equals(condition1)
                             orderby new
                             {
                                 productShelfStock.F40_ProductLotNo,
                                 productShelfStatus.F51_ShelfRow,
                                 productShelfStatus.F51_ShelfBay,
                                 productShelfStatus.F51_ShelfLevel
                             }
                             select
                             new ForceRetrievalOfProductItem
                             {
                                 ProductLotNo = productShelfStock.F40_ProductLotNo,
                                 ShelfNo1 = productShelfStatus.F51_ShelfRow + "-" + productShelfStatus.F51_ShelfBay + "-" + productShelfStatus.F51_ShelfLevel,
                                 Amount = productShelfStock.F40_Amount,
                                 PreProdLotNo = productShelfStock.F40_PrePdtLotNo,
                                 CerfDate = productShelfStock.F40_CertificationDate,
                                 CerfFlag = productShelfStock.F40_CertificationFlg
                             };

                // Get GrandTotal
                double tempTotal = 0;
                if (result.Any())
                {
                    tempTotal = result.Sum(i => i.Amount);
                }
                total = tempTotal;

                //Get Delivery Total
                var status = Constants.F44_Status.F44_Sts_NotShip;
                var shippingPlans =
                    _unitOfWork.ShippingPlanRepository.GetMany(i => i.F44_ProductCode.Trim().Equals(productCode.Trim())
                                                                    && i.F44_Status.Trim().Equals(status));
                deliveryTotal = shippingPlans.Any() ? shippingPlans
                                        .Sum(i => i.F44_ShpRqtAmt) : 0;

                //Get Certified Total
                var stockFlag = Constants.F40_StockFlag.TX40_StkFlg_Stk;
                var cerFlag1 = Constants.F40_CertificationFlag.TX40_CrtfctnFlg_Ok.ToString("D");
                var cerFlag2 = Constants.F40_CertificationFlag.TX40_CrtfctnFlg_NG.ToString("D");
                var resultCerTotal = from productShelfStock in productShelfStocks
                                    join productShelfStatus in productShelfStatuss on productShelfStock.F40_PalletNo equals productShelfStatus.F51_PalletNo

                                     where productShelfStock.F40_StockFlag.Trim().Equals(stockFlag)
                                    && productShelfStock.F40_ProductCode.Trim().Equals(productCode.Trim())
                                    && (productShelfStock.F40_CertificationFlg.Trim().Equals(cerFlag1) || productShelfStock.F40_CertificationFlg.Trim().Equals(cerFlag2))                                   
                                    select
                                    new ForceRetrievalOfProductItem
                                    {                                        
                                        Amount = productShelfStock.F40_Amount                                        
                                    };
                double tempCerTotal = 0;
                if (resultCerTotal.Any())
                {
                    tempCerTotal = resultCerTotal.Sum(i => i.Amount);
                }
                cerTotal = tempCerTotal;

                //Get Non-Certified Total                
                var cerFlag3 = Constants.F40_CertificationFlag.TX40_CrtfctnFlg_Normal.ToString("D");
                var cerFlag4 = Constants.F40_CertificationFlag.TX40_CrtfctnFlg_OutofSpec.ToString("D");
                var resultNonCerTotal = from productShelfStock in productShelfStocks
                                     join productShelfStatus in productShelfStatuss on productShelfStock.F40_PalletNo equals productShelfStatus.F51_PalletNo

                                     where productShelfStock.F40_StockFlag.Trim().Equals(stockFlag)
                                    && productShelfStock.F40_ProductCode.Trim().Equals(productCode.Trim())
                                    && (productShelfStock.F40_CertificationFlg.Trim().Equals(cerFlag3) || productShelfStock.F40_CertificationFlg.Trim().Equals(cerFlag4))
                                     select
                                     new ForceRetrievalOfProductItem
                                     {
                                         Amount = productShelfStock.F40_Amount
                                     };
                double tempNonCerTotal = 0;
                if (resultNonCerTotal.Any())
                {
                    tempNonCerTotal = resultNonCerTotal.Sum(i => i.Amount);
                }
                nonCerTotal = tempNonCerTotal;


                var itemCount = result.Count();
                
                if (gridSettings != null)
                    OrderByAndPaging(ref result, gridSettings);
                var resultModel = new GridResponse<ForceRetrievalOfProductItem>(result, itemCount);
                return new ResponseResult<GridResponse<ForceRetrievalOfProductItem>>(resultModel, true);
            }
        }

        /// <summary>
        ///     This function is for exporting data from kneading command database to list.
        /// </summary>
        /// <returns></returns>
        public async Task<PrintProductNameItem> SearchRecordsForPrintingCertified()
        {
            // Find all tm09
            var products = _unitOfWork.ProductRepository.GetAll();

            // Find all tx40
            var productShelfStocks = _unitOfWork.ProductShelfStockRepository.GetAll();

            // Find all tx51
            var productShelfStatuss = _unitOfWork.ProductShelfStatusRepository.GetAll();

            var shelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_Pdt;
            var shelfType = Constants.F51_ShelfType.Normal.ToString("D");

            var stockFlag = Constants.F40_StockFlag.TX40_StkFlg_Stk;
            var cerFlag1 = Constants.F40_CertificationFlag.TX40_CrtfctnFlg_Ok.ToString("D");
            var cerFlag2 = Constants.F40_CertificationFlag.TX40_CrtfctnFlg_NG.ToString("D");

            // Record which should be printed.
            // var printProductNameItem = new PrintProductNameItem();
            var printProductNameItem = new PrintProductNameItem();

            // Find Material Name item.
            var productNameItems = from product in products
                join productShelfStock in productShelfStocks on product.F09_ProductCode equals
                productShelfStock.F40_ProductCode
                                   from productShelfStatus in productShelfStatuss

                where
                (productShelfStock.F40_CertificationFlg.Trim().Equals(cerFlag1) ||
                 productShelfStock.F40_CertificationFlg.Trim().Equals(cerFlag2))
                                   && productShelfStatus.F51_PalletNo.Trim().Equals(productShelfStock.F40_PalletNo)
                                   && productShelfStatus.F51_ShelfStatus.Trim().Equals(shelfStatus)
                                   && productShelfStatus.F51_ShelfType.Trim().Equals(shelfType)
                                   orderby new
                                   {
                                       productShelfStock.F40_ProductCode,
                                       productShelfStock.F40_ProductLotNo,
                                       productShelfStock.F40_PalletNo
                                   }
                                   select new FindPrintProductNameItem()
                                   {
                                       ProductCode = productShelfStock.F40_ProductCode,
                                       ProductName = product.F09_ProductDesp,
                                       PalletNo = productShelfStock.F40_PalletNo,
                                       ProductLotNo = productShelfStock.F40_ProductLotNo,
                    ShelfNo =
                        productShelfStatus.F51_ShelfRow + "-" + productShelfStatus.F51_ShelfBay + "-" +
                        productShelfStatus.F51_ShelfLevel,
                                       Amount = productShelfStock.F40_Amount,
                    CerfFlag = productShelfStock.F40_CertificationFlg
                    //CerfFlag = ConvertCerfFlag(productShelfStock.F40_CertificationFlg)
                                       
                                  }; 

            var groupItems =
                 productNameItems.GroupBy(x => new { x.ProductCode })
                     .Select(y => new GroupValueItems()
                              {
                         ProductCode = y.Key.ProductCode,                         
                         Total = y.Sum(c => c.Amount)
                     });

            foreach (var groupitem in groupItems.ToList())
            {
                var printProductNameGroupItem = new PrintProductNameGroupItem()
                {
                    ProductCode = groupitem.ProductCode,                  
                    LotTotal = groupitem.Total,
                    LotTotalString = String.Format("{0:#,##0.00}", groupitem.Total)
                };
                foreach (var item in productNameItems.ToList())
                {
                    if (item.ProductCode == groupitem.ProductCode)
                    {
                        printProductNameGroupItem.FindPrintProductNameItem.Add(item);
                    }
                }
                printProductNameItem.PrintProductNameGroupItem.Add(printProductNameGroupItem);
                              }

            //•	o	Show “PAGE: “ + current page with format as ###            
            //printExternalPreProductNameItem.Page = "1";

            //// Get Total quantity
            //printProductNameItem.PreproductTotal = printExternalPreProductNameItem.PrintExternalPreProductNameGroupItem.Sum(c => c.LotTotal);
            //printExternalPreProductNameItem.PreproductTotalString = String.Format("{0:#,##0.00}", printExternalPreProductNameItem.PreproductTotal);            

            await _unitOfWork.CommitAsync();

            return printProductNameItem;
        }

        public async Task<PrintProductNameItem> SearchRecordsForPrintingNotCertified()
        {
            // Find all tm09
            var products = _unitOfWork.ProductRepository.GetAll();

            // Find all tx40
            var productShelfStocks = _unitOfWork.ProductShelfStockRepository.GetAll();

            // Find all tx51
            var productShelfStatuss = _unitOfWork.ProductShelfStatusRepository.GetAll();

            var shelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_Pdt;
            var shelfType = Constants.F51_ShelfType.Normal.ToString("D");

            var cerFlag1 = Constants.F40_CertificationFlag.TX40_CrtfctnFlg_Ok.ToString("D");
            var cerFlag2 = Constants.F40_CertificationFlag.TX40_CrtfctnFlg_NG.ToString("D");


            // Record which should be printed.
            var printProductNameItem = new PrintProductNameItem();

            // Find Material Name item.
            var productNameItems = from product in products
                                   join productShelfStock in productShelfStocks on product.F09_ProductCode equals productShelfStock.F40_ProductCode
                                   from productShelfStatus in productShelfStatuss

                                   where productShelfStock.F40_CertificationFlg.Trim() != cerFlag1
                                    && productShelfStatus.F51_PalletNo.Trim().Equals(productShelfStock.F40_PalletNo)
                                    && productShelfStatus.F51_ShelfStatus.Trim().Equals(shelfStatus)
                                    && productShelfStatus.F51_ShelfType.Trim().Equals(shelfType)
                                    && productShelfStock.F40_CertificationFlg.Trim() != cerFlag2
                                    orderby new
                                    {
                                        productShelfStock.F40_ProductCode,
                                        productShelfStock.F40_ProductLotNo,
                                        productShelfStock.F40_PalletNo
                                    }
                                    select new FindPrintProductNameItem()
                                    {
                                        ProductCode = productShelfStock.F40_ProductCode,
                                        ProductName = product.F09_ProductDesp,
                                        ProductLotNo = productShelfStock.F40_ProductLotNo,
                                        PalletNo = productShelfStock.F40_PalletNo,
                                        ShelfNo = productShelfStatus.F51_ShelfRow + "-" + productShelfStatus.F51_ShelfBay + "-" + productShelfStatus.F51_ShelfLevel,
                                        Amount = productShelfStock.F40_Amount,
                                    };

            var groupItems =
               productNameItems.GroupBy(x => new { x.ProductCode })
                   .Select(y => new GroupValueItems()
                   {
                       ProductCode = y.Key.ProductCode,
                       Total = y.Sum(c => c.Amount)
                   });

            foreach (var groupitem in groupItems.ToList())
                              {
                var printProductNameGroupItem = new PrintProductNameGroupItem()
                {
                    ProductCode = groupitem.ProductCode,
                    LotTotal = groupitem.Total,
                    LotTotalString = String.Format("{0:#,##0.00}", groupitem.Total)
                };
                foreach (var item in productNameItems.ToList())
                {
                    if (item.ProductCode == groupitem.ProductCode)
                    {
                        printProductNameGroupItem.FindPrintProductNameItem.Add(item);
                              }
                }
                printProductNameItem.PrintProductNameGroupItem.Add(printProductNameGroupItem);
            }

            //•	o	Show “PAGE: “ + current page with format as ###            
            //printExternalPreProductNameItem.Page = "1";

            //// Get Total quantity
            //printProductNameItem.PreproductTotal = printExternalPreProductNameItem.PrintExternalPreProductNameGroupItem.Sum(c => c.LotTotal);
            //printExternalPreProductNameItem.PreproductTotalString = String.Format("{0:#,##0.00}", printExternalPreProductNameItem.PreproductTotal);

            printProductNameItem.Datetime = DateTime.Today.ToString("dd/MM/yyyy HH:mm:ss");

            await _unitOfWork.CommitAsync();

            return printProductNameItem;
        }

        public string GetById(string productCode)
        {
            var entity = _unitOfWork.ProductRepository.GetAll().FirstOrDefault(i => i.F09_ProductCode.Trim().Equals(productCode.Trim()));
            var result = Mapper.Map<ProductItem>(entity);
            if (result == null)
            {
                return "";
            }
            return result.F09_ProductDesp;
        }
        public class GroupValueItems
        {
         

            public string ProductCode { get; set; }
            public double Total { get; set; }

        }
    }
}
