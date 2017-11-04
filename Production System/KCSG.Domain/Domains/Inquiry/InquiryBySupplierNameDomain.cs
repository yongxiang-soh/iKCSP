using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
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
using KCSG.Domain.Models.Inquiry.BySupplierName;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.Inquiry
{
    public class InquiryBySupplierNameDomain : BaseDomain, IInquiryBySupplierNameDomain
    {
        #region Constructor

        public InquiryBySupplierNameDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        public ResponseResult<GridResponse<MaterialShelfStatusItem>> SearchCriteria(string supplierCode,
            GridSettings gridSettings, out double total)
        {
            // Find all tm01
            var materials = _unitOfWork.MaterialRepository.GetAll();

            // Find all tx31
            var materialShelfStatuss = _unitOfWork.MaterialShelfStatusRepository.GetAll();

            // Find all tx33
            var materialShelfStocks = _unitOfWork.MaterialShelfStockRepository.GetAll();

            var shelfStatus = Constants.F31_ShelfStatus.Material.ToString("D");
            if (!string.IsNullOrEmpty(supplierCode))
            {
                var result = from material in materials
                    join materialShelfStock in materialShelfStocks on material.F01_MaterialCode equals
                    materialShelfStock.F33_MaterialCode
                    join materialShelfStatus in materialShelfStatuss on materialShelfStock.F33_PalletNo equals
                    materialShelfStatus.F31_PalletNo

                    where material.F01_SupplierCode.Trim().Equals(supplierCode.Trim()) &&
                          materialShelfStatus.F31_ShelfStatus.Trim().Equals(shelfStatus)
                    orderby new
                    {
                        materialShelfStock.F33_MaterialCode,
                        materialShelfStock.F33_MaterialLotNo,
                        materialShelfStatus.F31_ShelfRow,
                        materialShelfStatus.F31_ShelfBay,
                        materialShelfStatus.F31_ShelfLevel
                    }
                    select
                    new MaterialShelfStatusItem
                    {
                        MaterialName = material.F01_MaterialDsp,
                        F33_MaterialLotNo = materialShelfStock.F33_MaterialLotNo,
                        F33_Amount = materialShelfStock.F33_Amount,
                        ShelfNo1 =
                            materialShelfStatus.F31_ShelfRow + "-" + materialShelfStatus.F31_ShelfBay + "-" +
                            materialShelfStatus.F31_ShelfLevel,
                        MaterialCode = materialShelfStock.F33_MaterialCode
                    };


                var itemCount = result.Count();
                double tempTotal = 0;
                if (gridSettings != null)
                    OrderByAndPaging(ref result, gridSettings);
                if (result.Any())
                {
                    tempTotal = result.Sum(i => i.F33_Amount);
                }
                total = tempTotal;
                var resultModel = new GridResponse<MaterialShelfStatusItem>(result, itemCount);
                return new ResponseResult<GridResponse<MaterialShelfStatusItem>>(resultModel, true);
            }
            else
            {
                var result = from material in materials
                    join materialShelfStock in materialShelfStocks on material.F01_MaterialCode equals
                    materialShelfStock.F33_MaterialCode
                    join materialShelfStatus in materialShelfStatuss on materialShelfStock.F33_PalletNo equals
                    materialShelfStatus.F31_PalletNo

                    where materialShelfStatus.F31_ShelfStatus.Trim().Equals(shelfStatus)
                    orderby new
                    {
                        materialShelfStock.F33_MaterialCode,
                        materialShelfStock.F33_MaterialLotNo,
                        materialShelfStatus.F31_ShelfRow,
                        materialShelfStatus.F31_ShelfBay,
                        materialShelfStatus.F31_ShelfLevel
                    }
                    select
                    new MaterialShelfStatusItem
                    {
                        MaterialName = material.F01_MaterialDsp,
                        F33_MaterialLotNo = materialShelfStock.F33_MaterialLotNo,
                        F33_Amount = materialShelfStock.F33_Amount,
                        ShelfNo1 =
                            materialShelfStatus.F31_ShelfRow + "-" + materialShelfStatus.F31_ShelfBay + "-" +
                            materialShelfStatus.F31_ShelfLevel,
                        MaterialCode = materialShelfStock.F33_MaterialCode
                    };


                var itemCount = result.Count();
                double tempTotal = 0;
                if (gridSettings != null)
                    OrderByAndPaging(ref result, gridSettings);
                if (result.Any())
                {
                    tempTotal = result.Sum(i => i.F33_Amount);
                }
                total = tempTotal;
                var resultModel = new GridResponse<MaterialShelfStatusItem>(result, itemCount);
                return new ResponseResult<GridResponse<MaterialShelfStatusItem>>(resultModel, true);
            }
        }

        /// <summary>
        ///     This function is for exporting data from kneading command database to list.
        /// </summary>
        /// <returns></returns>
        public async Task<PrintSupplierNameItem> SearchRecordsForPrintingNormal()
        {
            try
            {
                // Find all tm01
                var materials = _unitOfWork.MaterialRepository.GetAll();

                // Find all tx31
                var materialShelfStatuss = _unitOfWork.MaterialShelfStatusRepository.GetAll();

                // Find all tx33
                var materialShelfStocks = _unitOfWork.MaterialShelfStockRepository.GetAll();

                var condition1 = Constants.F31_ShelfStatus.Material.ToString("D");
                var condition2 = Constants.TM01_Material_EntrustedClass.Normal.ToString("D");

                // Record which should be printed.
                var printSupplierNameItem = new PrintSupplierNameItem();

                // Find Material Name item.
                var supplierNameItems = from material in materials
                    join materialShelfStock in materialShelfStocks on material.F01_MaterialCode equals
                    materialShelfStock.F33_MaterialCode
                    join materialShelfStatus in materialShelfStatuss on materialShelfStock.F33_PalletNo equals
                    materialShelfStatus.F31_PalletNo

                    where materialShelfStatus.F31_ShelfStatus.Trim().Equals(condition1)
                          && material.F01_EntrustedClass.Trim().Equals(condition2)
                    orderby new
                    {
                        materialShelfStock.F33_MaterialCode,
                        materialShelfStock.F33_MaterialLotNo
                    }
                    select new FindPrintSupplierNameItem()
                    {
                        MaterialCode = materialShelfStock.F33_MaterialCode,
                        MaterialName = material.F01_MaterialDsp,
                        PalletNo = materialShelfStock.F33_PalletNo,
                        MaterialLotNo = materialShelfStock.F33_MaterialLotNo,
                        ShelfNo =
                            materialShelfStatus.F31_ShelfRow + "-" + materialShelfStatus.F31_ShelfBay + "-" +
                            materialShelfStatus.F31_ShelfLevel,
                        Amount = materialShelfStock.F33_Amount
                    };
                var groupItems =
               supplierNameItems.GroupBy(x => new { x.MaterialCode })
                   .Select(y => new GroupValueItems()
                   {
                       MaterialCode = y.Key.MaterialCode,
                       Total = y.Sum(c => c.Amount)
                   });

                foreach (var groupitem in groupItems.ToList())
                {
                    var printProductNameGroupItem = new PrintSupplierNameGroupItem()
                    {
                        MaterialCode = groupitem.MaterialCode,
                        LotTotal = groupitem.Total,
                        LotTotalString = String.Format("{0:#,##0.00}", groupitem.Total)
                    };
                    foreach (var item in supplierNameItems.ToList())
                    {
                        if (item.MaterialCode == groupitem.MaterialCode)
                        {
                            printProductNameGroupItem.FindPrintSupplierNameItem.Add(item);
                        }
                    }
                    printSupplierNameItem.PrintSupplierNameGroupItem.Add(printProductNameGroupItem);
                }

                //•	o	Show “PAGE: “ + current page with format as ###            
                //printExternalPreProductNameItem.Page = "1";

                //// Get Total quantity
                //printProductNameItem.PreproductTotal = printExternalPreProductNameItem.PrintExternalPreProductNameGroupItem.Sum(c => c.LotTotal);
                //printExternalPreProductNameItem.PreproductTotalString = String.Format("{0:#,##0.00}", printExternalPreProductNameItem.PreproductTotal);

                printSupplierNameItem.Datetime = DateTime.Today.ToString("dd/MM/yyyy HH:mm:ss");

                await _unitOfWork.CommitAsync();

                return printSupplierNameItem;

            }
            catch (Exception exception)
            {
                var exceptionMessage = exception.Message;
                return null;
                //throw;
            }

        }

        public async Task<PrintSupplierNameItem> SearchRecordsForPrintingBailment()
        {
            try
            {
                // Find all tm01
                var materials = _unitOfWork.MaterialRepository.GetAll();

                // Find all tx31
                var materialShelfStatuss = _unitOfWork.MaterialShelfStatusRepository.GetAll();

                // Find all tx33
                var materialShelfStocks = _unitOfWork.MaterialShelfStockRepository.GetAll();

                var condition1 = Constants.F31_ShelfStatus.Material.ToString("D");
                var condition2 = Constants.TM01_Material_EntrustedClass.Bailment.ToString("D");

                // Record which should be printed.
                var printSupplierNameItem = new PrintSupplierNameItem();

                // Find Material Name item.
                var supplierNameItems = from material in materials
                                        join materialShelfStock in materialShelfStocks on material.F01_MaterialCode equals materialShelfStock.F33_MaterialCode
                                        join materialShelfStatus in materialShelfStatuss on materialShelfStock.F33_PalletNo equals materialShelfStatus.F31_PalletNo

                                        where materialShelfStatus.F31_ShelfStatus.Trim().Equals(condition1)
                                        && material.F01_EntrustedClass.Trim().Equals(condition2)
                                        orderby new
                                        {
                                            materialShelfStock.F33_MaterialCode,
                                            materialShelfStock.F33_MaterialLotNo
                                        }
                                        select new FindPrintSupplierNameItem()
                                        {
                                            MaterialCode = materialShelfStock.F33_MaterialCode,
                                            MaterialName = material.F01_MaterialDsp,
                                            PalletNo = materialShelfStock.F33_PalletNo,
                                            MaterialLotNo = materialShelfStock.F33_MaterialLotNo,
                                            ShelfNo = materialShelfStatus.F31_ShelfRow + "-" + materialShelfStatus.F31_ShelfBay + "-" + materialShelfStatus.F31_ShelfLevel,
                                            Amount = materialShelfStock.F33_Amount
                                        };

                var groupItems =
         supplierNameItems.GroupBy(x => new { x.MaterialCode })
             .Select(y => new GroupValueItems()
             {
                 MaterialCode = y.Key.MaterialCode,
                 Total = y.Sum(c => c.Amount)
             });

                foreach (var groupitem in groupItems.ToList())
                {
                    var printProductNameGroupItem = new PrintSupplierNameGroupItem()
                    {
                        MaterialCode = groupitem.MaterialCode,
                        LotTotal = groupitem.Total,
                        LotTotalString = String.Format("{0:#,##0.00}", groupitem.Total)
                    };
                    foreach (var item in supplierNameItems.ToList())
                    {
                        if (item.MaterialCode == groupitem.MaterialCode)
                        {
                            printProductNameGroupItem.FindPrintSupplierNameItem.Add(item);
                        }
                    }
                    printSupplierNameItem.PrintSupplierNameGroupItem.Add(printProductNameGroupItem);
                }

                //•	o	Show “PAGE: “ + current page with format as ###            
                //printExternalPreProductNameItem.Page = "1";

                //// Get Total quantity
                //printProductNameItem.PreproductTotal = printExternalPreProductNameItem.PrintExternalPreProductNameGroupItem.Sum(c => c.LotTotal);
                //printExternalPreProductNameItem.PreproductTotalString = String.Format("{0:#,##0.00}", printExternalPreProductNameItem.PreproductTotal);

                printSupplierNameItem.Datetime = DateTime.Today.ToString("dd/MM/yyyy HH:mm:ss");

                await _unitOfWork.CommitAsync();

                return printSupplierNameItem;
            }
            catch (Exception exception)
            {
                var exceptionMessage = exception.Message;
                return null;
                //throw;
            }
            
        }

        public string GetById(string supplierCode)
        {            
            var entity = _unitOfWork.SupplierRepossitories.GetAll().FirstOrDefault(i => i.F04_SupplierCode.Trim().Equals(supplierCode.Trim()));
            //var result = Mapper.Map<SupplierItem>(entity);
            if (entity == null)
            {
                return "";
            }
            return entity.F04_SupplierName;
        }
        public class GroupValueItems
        {
            public string MaterialCode { get; set; }
            public double Total { get; set; }

        }
    }
}
