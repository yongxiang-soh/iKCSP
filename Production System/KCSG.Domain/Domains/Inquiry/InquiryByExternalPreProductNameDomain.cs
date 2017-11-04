using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
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
using KCSG.Domain.Models.Inquiry.ByExternalPre_ProductName;
using KCSG.Domain.Models.Inquiry.ByPreProductName;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;
namespace KCSG.Domain.Domains.Inquiry
{
    public class InquiryByExternalPreProductNameDomain : BaseDomain, IInquiryByExternalPreProductNameDomain
    {
        #region Constructor

        public InquiryByExternalPreProductNameDomain(
            IUnitOfWork unitOfWork,
            IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        public ResponseResult<GridResponse<StockTakingOfProductItem>> SearchCriteria(string extPreProductCode,
            GridSettings gridSettings, out double total)
        {
            // Find all tx51
            var productShelfStatuss = _unitOfWork.ProductShelfStatusRepository.GetAll();

            // Find all tx53
            var outSidePreShelfStocks = _unitOfWork.OutSidePrePdtStkRepository.GetAll();

            var shelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_ExtPrePdt;
            if (!string.IsNullOrEmpty(extPreProductCode))
            {
                var result = from productShelfStatus in productShelfStatuss
                             join outSidePreShelfStock in outSidePreShelfStocks on productShelfStatus.F51_PalletNo equals outSidePreShelfStock.F53_PalletNo

                             where outSidePreShelfStock.F53_OutSidePrePdtCode.Trim().Equals(extPreProductCode.Trim()) &&
                                   productShelfStatus.F51_ShelfStatus.Trim().Equals(shelfStatus)
                             orderby new
                             {
                                 outSidePreShelfStock.F53_OutSidePrePdtLotNo,
                                 productShelfStatus.F51_ShelfRow,
                                 productShelfStatus.F51_ShelfBay,
                                 productShelfStatus.F51_ShelfLevel
                             }
                             select
                             new StockTakingOfProductItem
                             {
                                 LotNo = outSidePreShelfStock.F53_OutSidePrePdtLotNo,
                                 Amount = outSidePreShelfStock.F53_Amount,
                                 ShelfNo1 = productShelfStatus.F51_ShelfRow + "-" + productShelfStatus.F51_ShelfBay + "-" + productShelfStatus.F51_ShelfLevel,
                             };


                var itemCount = result.Count();
                double tempTotal = 0;
                if (gridSettings != null)
                    OrderByAndPaging(ref result, gridSettings);
                if (result.Any())
                {
                    tempTotal = result.Sum(i => i.Amount);
                }
                total = tempTotal;
                var resultModel = new GridResponse<StockTakingOfProductItem>(result, itemCount);
                return new ResponseResult<GridResponse<StockTakingOfProductItem>>(resultModel, true);
            }
            else
            {
                var result = from productShelfStatus in productShelfStatuss
                             join outSidePreShelfStock in outSidePreShelfStocks on productShelfStatus.F51_PalletNo equals outSidePreShelfStock.F53_PalletNo

                             where productShelfStatus.F51_ShelfStatus.Trim().Equals(shelfStatus)
                             orderby new
                             {
                                 outSidePreShelfStock.F53_OutSidePrePdtLotNo,
                                 productShelfStatus.F51_ShelfRow,
                                 productShelfStatus.F51_ShelfBay,
                                 productShelfStatus.F51_ShelfLevel
                             }
                             select
                             new StockTakingOfProductItem
                             {
                                 LotNo = outSidePreShelfStock.F53_OutSidePrePdtLotNo,
                                 Amount = outSidePreShelfStock.F53_Amount,
                                 ShelfNo1 = productShelfStatus.F51_ShelfRow + "-" + productShelfStatus.F51_ShelfBay + "-" + productShelfStatus.F51_ShelfLevel,
                             };


                var itemCount = result.Count();
                double tempTotal = 0;
                if (gridSettings != null)
                    OrderByAndPaging(ref result, gridSettings);
                if (result.Any())
                {
                    tempTotal = result.Sum(i => i.Amount);
                }
                total = tempTotal;
                var resultModel = new GridResponse<StockTakingOfProductItem>(result, itemCount);
                return new ResponseResult<GridResponse<StockTakingOfProductItem>>(resultModel, true);
            }
        }


        /// <summary>
        ///     This function is for exporting data from kneading command database to list.
        /// </summary>
        /// <returns></returns>
        public async Task<PrintExternalPreProductNameItem> SearchRecordsForPrintingNormal()
        {
            //Get all record tx37
            var preProductShelfStatuss = _unitOfWork.PreProductShelfStatusRepository.GetAll();
            //Get all record tx49
            var preProductShelfStocks = _unitOfWork.PreProductShelfStockRepository.GetAll();

            // Get all tm03.
            var preProducts = _unitOfWork.PreProductRepository.GetAll();

            var condition1 = Constants.F49_ShelfStatus.TX49_StkFlg_Stk;

            // Find Material Name item.
            var extPreProductNameItems = from preProduct in preProducts
                                         join preProductShelfStock in preProductShelfStocks on preProduct.F03_PreProductCode equals preProductShelfStock.F49_PreProductCode
                                         join preProductShelfStatus in preProductShelfStatuss on preProductShelfStock.F49_ContainerCode equals preProductShelfStatus.F37_ContainerCode
                                         where preProductShelfStock.F49_ShelfStatus.Trim().Equals(condition1)
                                         //group materialShelfStock by materialShelfStock.F33_MaterialCode into grp
                                         orderby new
                                         {
                                             preProductShelfStock.F49_PreProductCode,
                                             preProductShelfStock.F49_PreProductLotNo,
                                             preProductShelfStock.F49_ContainerCode
                                         }
                                         select new FindPrintExternalPreProductNameItem()
                                         {
                                             PreProductCode = preProductShelfStock.F49_PreProductCode,
                                             PreProductName = preProduct.F03_PreProductName,
                                             ContainerCode = preProductShelfStock.F49_ContainerCode,
                                             PreProductLotNo = preProductShelfStock.F49_PreProductLotNo,
                                             ShelfNo = preProductShelfStatus.F37_ShelfRow + "-" + preProductShelfStatus.F37_ShelfBay + "-" + preProductShelfStatus.F37_ShelfLevel,
                                             Amount = preProductShelfStock.F49_Amount,
                                         };

            var Groupitems = extPreProductNameItems
     .GroupBy(x => new { PreProductCode = x.PreProductCode, PreProductLotNo = x.PreProductLotNo })
     .GroupBy(x => new { PreProductCode = x.Key.PreProductCode }).OrderBy(x => x.Key.PreProductCode);

            var listprintExternalPreProductNameItem = new PrintExternalPreProductNameItem();

            foreach (var GroupB in Groupitems)
            {
                var printExternalPreProductNameGroup = new PrintExternalPreProductNameGroup();
                foreach (var GroupA in GroupB)
                {
                    var listPrintExternalPreProductNameGroupItem = new PrintExternalPreProductNameGroupItem();

                    foreach (var item in GroupA)
                    {
                        var findPrintExternalPreProductNameItem = new FindPrintExternalPreProductNameItem()
                        {
                            PreProductCode = item.PreProductCode,
                            PreProductName = item.PreProductName,
                            PreProductLotNo = item.PreProductLotNo,
                            ContainerCode = item.ContainerCode,
                            ShelfNo = item.ShelfNo,
                            Amount = item.Amount,

                        };
                        listPrintExternalPreProductNameGroupItem.FindPrintExternalPreProductNameItem.Add(findPrintExternalPreProductNameItem);
                        listPrintExternalPreProductNameGroupItem.LotTotal =
                      listPrintExternalPreProductNameGroupItem.FindPrintExternalPreProductNameItem.Sum(x => x.Amount);
                        listPrintExternalPreProductNameGroupItem.LotTotalString = 
                            String.Format("{0:#,##0.00}",listPrintExternalPreProductNameGroupItem.LotTotal);
                    }

                    printExternalPreProductNameGroup.PrintExternalPreProductNameGroupItem.Add(listPrintExternalPreProductNameGroupItem);
                    printExternalPreProductNameGroup.PreProductTotal =
                        printExternalPreProductNameGroup.PrintExternalPreProductNameGroupItem.Sum(x => x.LotTotal);
                    printExternalPreProductNameGroup.PreProductTotalString = 
                        String.Format("{0:#,##0.00}", printExternalPreProductNameGroup.PreProductTotal);
                }

                listprintExternalPreProductNameItem.PrintExternalPreProductNameGroup.Add(printExternalPreProductNameGroup);
            }
            
            await _unitOfWork.CommitAsync();

            return listprintExternalPreProductNameItem;

        }

        public async Task<PrintExternalPreProductNameItem> SearchRecordsForPrintingExternal()
        {
            // Find all tx51
            var productShelfStatuss = _unitOfWork.ProductShelfStatusRepository.GetAll();

            // Find all tx53
            var outSidePreShelfStocks = _unitOfWork.OutSidePrePdtStkRepository.GetAll();

            // Get all tm03.
            var preProducts = _unitOfWork.PreProductRepository.GetAll();

            // Record which should be printed.
            var printExternalPreProductNameItem = new PrintExternalPreProductNameItem();

            var condition1 = Constants.F51_ShelfStatus.TX51_ShfSts_ExtPrePdt;
            var condition2 = Constants.F51_ShelfType.Normal.ToString("D");

            // Find Material Name item.
            var extPreProductNameItems = from preProduct in preProducts
                                         join outSidePreShelfStock in outSidePreShelfStocks on preProduct.F03_PreProductCode equals outSidePreShelfStock.F53_OutSidePrePdtCode
                                         join productShelfStatus in productShelfStatuss on outSidePreShelfStock.F53_PalletNo equals productShelfStatus.F51_PalletNo
                                         where productShelfStatus.F51_ShelfStatus.Trim().Equals(condition1)
                                        && productShelfStatus.F51_ShelfType.Trim().Equals(condition2)
                                         orderby new
                                         {
                                             outSidePreShelfStock.F53_OutSidePrePdtCode,
                                             outSidePreShelfStock.F53_OutSidePrePdtLotNo
                                         }
                                         select new FindPrintExternalPreProductNameItem()
                                         {
                                             OutsidePreProductCode = outSidePreShelfStock.F53_OutSidePrePdtCode,
                                             PreProductName = preProduct.F03_PreProductName,
                                             PalletNo = outSidePreShelfStock.F53_PalletNo,
                                             OutsidePreProductLotNo = outSidePreShelfStock.F53_OutSidePrePdtLotNo,
                                             ShelfNo = productShelfStatus.F51_ShelfRow + "-" + productShelfStatus.F51_ShelfBay + "-" + productShelfStatus.F51_ShelfLevel,
                                             Amount = outSidePreShelfStock.F53_Amount,
                                             //Total = materialShelfStocks.Sum(i => i.F33_Amount)
                                         };



            var Groupitems = extPreProductNameItems
     .GroupBy(x => new { OutsidePreProductCode = x.OutsidePreProductCode, OutsidePreProductLotNo = x.OutsidePreProductLotNo })
     .GroupBy(x => new { OutsidePreProductCode = x.Key.OutsidePreProductCode }).OrderBy(x => x.Key.OutsidePreProductCode);

            var listprintExternalPreProductNameItem = new PrintExternalPreProductNameItem();

            foreach (var GroupB in Groupitems)
            {
                var printExternalPreProductNameGroup = new PrintExternalPreProductNameGroup();
                foreach (var GroupA in GroupB)
                {
                    var listPrintExternalPreProductNameGroupItem = new PrintExternalPreProductNameGroupItem();

                    foreach (var item in GroupA)
                    {
                        var findPrintExternalPreProductNameItem = new FindPrintExternalPreProductNameItem()
                        {
                           OutsidePreProductCode = item.OutsidePreProductCode,
                           PreProductName = item.PreProductName,
                            OutsidePreProductLotNo = item.OutsidePreProductLotNo,
                            PalletNo = item.PalletNo,
                            ShelfNo = item.ShelfNo,
                            Amount = item.Amount,

                        };
                        listPrintExternalPreProductNameGroupItem.FindPrintExternalPreProductNameItem.Add(findPrintExternalPreProductNameItem);
                        listPrintExternalPreProductNameGroupItem.LotTotal =
                      listPrintExternalPreProductNameGroupItem.FindPrintExternalPreProductNameItem.Sum(x => x.Amount);
                        listPrintExternalPreProductNameGroupItem.LotTotalString =
                            String.Format("{0:#,##0.00}", listPrintExternalPreProductNameGroupItem.LotTotal);
                    }

                    printExternalPreProductNameGroup.PrintExternalPreProductNameGroupItem.Add(listPrintExternalPreProductNameGroupItem);
                    printExternalPreProductNameGroup.PreProductTotal =
                        printExternalPreProductNameGroup.PrintExternalPreProductNameGroupItem.Sum(x => x.LotTotal);
                    printExternalPreProductNameGroup.PreProductTotalString =
                        String.Format("{0:#,##0.00}", printExternalPreProductNameGroup.PreProductTotal);
                }

                listprintExternalPreProductNameItem.PrintExternalPreProductNameGroup.Add(printExternalPreProductNameGroup);
            }

            await _unitOfWork.CommitAsync();

            return listprintExternalPreProductNameItem;
        }

        public string GetById(string preProductCode)
        {
            var entity = _unitOfWork.PreProductRepository.GetAll().FirstOrDefault(i => i.F03_PreProductCode.Trim().Equals(preProductCode.Trim()));
            var result = Mapper.Map<PreProductItem>(entity);
            if (result == null)
            {
                return "";
            }
            return result.F03_PreProductName;
        }
        public class GroupValueItem
        {
            public string OutsidePreProductCode { get; set; }

            public string OutsidePreProductLotNo { get; set; }

            public string PreProductCode { get; set; }
            public string PreProductLotNo { get; set; }

            public double Total { get; set; }

        }
    }
}
