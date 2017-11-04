using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.ManagementReport;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.ManagementReport;
using KCSG.Domain.Models.Inquiry.ByPreProductName;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.Inquiry
{
    public class MaterialStockDomain : BaseDomain, IMaterialStockDomain
    {
        #region Constructor

        public MaterialStockDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion


        /// <summary>
        ///     This function is for exporting data from kneading command database to list.
        /// </summary>
        /// <returns></returns>
        public async Task<PrintMaterialNameItem> SearchRecordsForPrinting(string status)
        {
            // Find all tx31
            var materialShelfStatuss = _unitOfWork.MaterialShelfStatusRepository.GetAll();

            // Find all tx33
            var materialShelfStocks = _unitOfWork.MaterialShelfStockRepository.GetAll();

            // Find all materials.
            var materials = _unitOfWork.MaterialRepository.GetAll();

            // Record which should be printed.
            var printMaterialNameItem = new PrintMaterialNameItem();

            var condition1 = Constants.F31_ShelfStatus.Material.ToString("D");
            string condition2 = "0";
            if (status == "1")
            {
                condition2 = "1";
            }

            // Find Material Name item.
            var materialNameItems = from material in materials
                                    join materialShelfStock in materialShelfStocks on material.F01_MaterialCode equals materialShelfStock.F33_MaterialCode
                                    join materialShelfStatus in materialShelfStatuss on materialShelfStock.F33_PalletNo equals materialShelfStatus.F31_PalletNo
                                    where materialShelfStatus.F31_ShelfStatus.Trim().Equals(condition1)
                                     && material.F01_EntrustedClass.Equals(condition2)
                                    orderby new
                                    {
                                        //materialShelfStock.F33_MaterialCode,
                                        //materialShelfStock.F33_MaterialLotNo

                                        materialShelfStatus.F31_ShelfRow,
                                        materialShelfStatus.F31_ShelfBay,
                                        materialShelfStatus.F31_ShelfLevel
                                        //materialShelfStock.F33_MaterialCode,
                                        //materialShelfStock.F33_MaterialLotNo
                                    }
                                    select new FindPrintMaterialNameItem
                                    {
                                        MaterialCode = materialShelfStock.F33_MaterialCode,
                                        MaterialName = material.F01_MaterialDsp,
                                        LotNo = materialShelfStock.F33_MaterialLotNo,
                                        PalletNo = materialShelfStock.F33_PalletNo,
                                        RBL = materialShelfStatus.F31_ShelfRow + "-" + materialShelfStatus.F31_ShelfBay + "-" + materialShelfStatus.F31_ShelfLevel,
                                        Quantity = materialShelfStock.F33_Amount,
                                        //Total = materialShelfStocks.Sum(i => i.F33_Amount)
                                    };

            var firstResult = from materialNameItem in materialNameItems
                              group materialNameItems by new
                              {
                                  materialNameItem.MaterialCode
                              }
                                  into firstGroup
                              select firstGroup;

            var totalFirstItems = (await materialNameItems.SumAsync(x => (double?)x.Quantity)) ?? 0;


            //•	o	Show “PAGE: “ + current page with format as ###            
            printMaterialNameItem.Page = "1";

            // Get Total quantity
            printMaterialNameItem.Total = totalFirstItems.ToString("###,###,###,##0.00");

            // o	Show {current date/time} with format: dd-mm-yyyy hh:ss                                           
            printMaterialNameItem.Datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            /*
             * o	Show {Company Name} = value from profile:
             * ProfileString("toshiba.ini","server","companyname","")
            */
            printMaterialNameItem.CompanyName = WebConfigurationManager.AppSettings["CompanyName"];


            await _unitOfWork.CommitAsync();

            printMaterialNameItem.FindPrintMaterialNameItem = await materialNameItems.ToListAsync();

            var groupby =
                printMaterialNameItem.FindPrintMaterialNameItem.Select(m => new FindPrintMaterialNameItem()
                {
                    MaterialCode = m.MaterialCode,
                    MaterialName = m.MaterialName,
                    LotNo = m.LotNo,
                    PalletNo = m.PalletNo,
                    RBL = m.RBL,
                    Quantity = m.Quantity,
                    Quantityst = m.Quantity.ToString("###,###,###,##0.00"),

                }).ToList().GroupBy(m => m.RBL);
            try
            {
                var groupitems = new List<FindPrintMaterialNameGroupItem>();

                foreach (var items in groupby)
                {
                    var groupitem = new FindPrintMaterialNameGroupItem();
                    groupitem.FindPrintMaterialNameItem = items.ToList();
                    groupitem.Total = groupitem.FindPrintMaterialNameItem.Sum(i => i.Quantity).ToString("###,###,###,##0.00");
                    groupitems.Add(groupitem);
                }
                printMaterialNameItem.groupitems = groupitems;
            }
            catch (Exception ex)
            {
            }
            return printMaterialNameItem;
        }

        public async Task<PrintMaterialNameItem> SearchSupplementaryRecordsForPrintingAll()
        {
            // Find all 15
            var SubmaterialShelfStatuss = _unitOfWork.SubMaterialRepository.GetAll();

            // Find all tx33
            var SupmaterialStocks = _unitOfWork.SupMaterialStockRepository.GetAll();

            // Find all materials.
            var materials = _unitOfWork.MaterialRepository.GetAll();

            // Record which should be printed.
            var printMaterialNameItem = new PrintMaterialNameItem();

            var condition1 = Constants.F31_ShelfStatus.Material.ToString("D");


            // Find Material Name item.
            var materialNameItems = from submaterial in SubmaterialShelfStatuss
                                    join materialShelfStock in SupmaterialStocks on submaterial.F15_SubMaterialCode equals materialShelfStock.F46_SubMaterialCode
                                    where true
                                    orderby new
                                    {
                                        materialShelfStock.F46_SubMaterialCode
                                    }
                                    select new SubFindPrintMaterialNameItem
                                    {
                                        SupplementaryMaterialCode = materialShelfStock.F46_SubMaterialCode,
                                        SupplementaryMaterialName = submaterial.F15_MaterialDsp,
                                        PackingUnit = submaterial.F15_PackingUnit,
                                        Unit = submaterial.F15_Unit,
                                        Quantity = materialShelfStock.F46_Amount,
                                        //Total = materialShelfStocks.Sum(i => i.F33_Amount)
                                    };

#if DEBUG

            var a = await materialNameItems.ToListAsync();
#endif
            //•	o	Show “PAGE: “ + current page with format as ###            
            printMaterialNameItem.Page = "1";

            // Get Total quantity
            if (materialNameItems.Any())
            {
                printMaterialNameItem.Total = materialNameItems.Sum(i => i.Quantity).ToString("###,###,###,##0.00");
            }
            // o	Show {current date/time} with format: dd-mm-yyyy hh:ss                                           
            printMaterialNameItem.Datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            /*
             * o	Show {Company Name} = value from profile:
             * ProfileString("toshiba.ini","server","companyname","")
            */
            printMaterialNameItem.CompanyName = WebConfigurationManager.AppSettings["CompanyName"];


            await _unitOfWork.CommitAsync();

            printMaterialNameItem.FindPrintSubMaterialNameItem = await materialNameItems.ToListAsync();
            printMaterialNameItem.FindPrintSubMaterialNameItem =
               printMaterialNameItem.FindPrintSubMaterialNameItem.Select(m => new SubFindPrintMaterialNameItem()
               {
                   SupplementaryMaterialCode = m.SupplementaryMaterialCode,
                   SupplementaryMaterialName = m.SupplementaryMaterialName,
                   PackingUnitst = m.PackingUnit.ToString("###,###,###,##0.00"),
                   Unit = m.Unit,
                   Quantityst = m.Quantity.ToString("###,###,###,##0.00"),

               }).ToList();
            return printMaterialNameItem;
        }
        public async Task<PrintMaterialNameItem> SearchMetarialShelftForPrintingAll()
        {
            // Find all 01
            var Materials = _unitOfWork.MaterialRepository.GetAll();

            // Find all tx33
            var SupmaterialStocks = _unitOfWork.MaterialShelfStockRepository.GetAll();
            // Find all tx32
            var MaterialShelfs = _unitOfWork.MaterialShelfRepository.GetAll();
            // Find all tx31
            var MaterialShelfStatuss = _unitOfWork.MaterialShelfStatusRepository.GetAll();
            // Find all tx30
            var Receptions = _unitOfWork.ReceptionRepository.GetAll();


            // Record which should be printed.
            var printMaterialNameItem = new PrintMaterialNameItem();

            var condition1 = Constants.F31_ShelfStatus.Material.ToString("D");


            // Find Material Name item.
            var materialNameItems = from material in Materials
                                    join supmaterialStock in SupmaterialStocks on material.F01_MaterialCode equals
                                    supmaterialStock.F33_MaterialCode
                                    join materialShelfStatus in MaterialShelfStatuss on supmaterialStock.F33_PalletNo equals
                                    materialShelfStatus.F31_PalletNo
                                    join materialShelf in MaterialShelfs on supmaterialStock.F33_PalletNo equals materialShelf.F32_PalletNo
                                    join reception in Receptions on
                                    new { test1 = materialShelf.F32_PrtDvrNo, test2 = materialShelf.F32_PrcOrdNo } equals
                                    new { test1 = reception.F30_PrtDvrNo, test2 = reception.F30_PrcOrdNo }
                                    where materialShelfStatus.F31_ShelfStatus.Equals("3")
                                    orderby new
                                    {
                                        materialShelfStatus.F31_ShelfRow,
                                        materialShelfStatus.F31_ShelfBay,
                                        materialShelfStatus.F31_ShelfLevel
                                        //supmaterialStock.F33_MaterialCode,
                                        //supmaterialStock.F33_MaterialLotNo,

                                    }
                                    select new FindPrintMaterialSheifItem
                                    {
                                        MaterialCode = supmaterialStock.F33_MaterialCode,
                                        MaterialName = material.F01_MaterialDsp,
                                        LotNo = supmaterialStock.F33_MaterialLotNo,
                                        PalletNo = materialShelfStatus.F31_PalletNo,
                                        Quantity = supmaterialStock.F33_Amount,
                                        RBL = materialShelfStatus.F31_ShelfRow + "-" + materialShelfStatus.F31_ShelfBay + "-" + materialShelfStatus.F31_ShelfLevel,
                                        PONo = materialShelf.F32_PrcOrdNo,
                                        PD = materialShelf.F32_PrtDvrNo,
                                        SDate1 = materialShelfStatus.F31_StorageDate,
                                        BF = material.F01_EntrustedClass,
                                        AF = reception.F30_AcceptClass,
                                        //Total = materialShelfStocks.Sum(i => i.F33_Amount)
                                    };
            //var lstItem = materialNameItems.ToList().Select(m => new FindPrintMaterialSheifItem
            //{
            //    MaterialCode = m.MaterialCode,
            //    MaterialName = m.MaterialName,
            //    LotNo = m.LotNo,
            //    PalletNo = m.PalletNo,
            //    Quantity = m.Quantity,
            //    RBL = m.RBL,
            //    PONo = m.PONo,
            //    PD = m.PD,
            //    SDate = m.SDate,
            //    BF = m.BF,
            //    AF = m.AF,
            //});

#if DEBUG

            var a = await materialNameItems.ToListAsync();
#endif
            //•	o	Show “PAGE: “ + current page with format as ###            
            printMaterialNameItem.Page = "1";

            // Get Total quantity
            if (materialNameItems.Any())
            {
                printMaterialNameItem.Total = materialNameItems.Sum(i => i.Quantity).ToString("###,###,###,##0.00");
            }
            // o	Show {current date/time} with format: dd-mm-yyyy hh:ss                                           
            printMaterialNameItem.Datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            /*
             * o	Show {Company Name} = value from profile:
             * ProfileString("toshiba.ini","server","companyname","")
            */
            printMaterialNameItem.CompanyName = WebConfigurationManager.AppSettings["CompanyName"];


            await _unitOfWork.CommitAsync();

            printMaterialNameItem.FindPrintMaterialSheifItem = await materialNameItems.ToListAsync();
            var groupby = printMaterialNameItem.FindPrintMaterialSheifItem.Select(m => new FindPrintMaterialSheifItem
            {
                MaterialCode = m.MaterialCode,
                MaterialName = m.MaterialName,
                LotNo = m.LotNo,
                PalletNo = m.PalletNo,
                Quantity = m.Quantity,
                Quantityst = m.Quantity.ToString("###,###,###,##0.00"),
                RBL = m.RBL,
                PONo = m.PONo,
                PD = m.PD,
                SDate = m.SDate1.HasValue? m.SDate1.Value.ToString("dd/MM/yyyy") : "",
                Stime = m.SDate1.HasValue ? m.SDate1.Value.ToString("HH:mm") : "",
                BF = m.BF.Equals("0") ? "Norm" : (m.BF.Equals("1")? "Bail" : m.BF),
                AF = m.AF.Equals("0")? "Yet" : m.AF.Equals("1") ? "Done": m.AF.Equals("2") ? "Reject" : m.AF,
            }).ToList().GroupBy(m => m.RBL);
            try
            {
                var groupitems = new List<FindPrintMaterialNameGroupItem>();
                
                foreach (var items in groupby)
                {
                    var groupitem = new FindPrintMaterialNameGroupItem();
                    groupitem.FindPrintMaterialSheifItem = items.ToList();
                    groupitem.Total = groupitem.FindPrintMaterialSheifItem.Sum(i => i.Quantity).ToString("###,###,###,##0.00");
                    groupitems.Add(groupitem);
                }
                printMaterialNameItem.groupitems = groupitems;
            }
            catch (Exception ex)
            {
            }
            
            
            return printMaterialNameItem;
        }

        public async Task<PrintPreProductNameItem> SearchPreProductForPrint()
        {
            //Get all record tx37
            var preProductShelfStatuss = _unitOfWork.PreProductShelfStatusRepository.GetAll();
            //Get all record tx49
            var preProductShelfStocks = _unitOfWork.PreProductShelfStockRepository.GetAll();

            // Get all tm03.
            var preProducts = _unitOfWork.PreProductRepository.GetAll();

            var condition1 = Constants.F49_ShelfStatus.TX49_StkFlg_Stk;

            // Record which should be printed.
            var printPreProductNameItem = new PrintPreProductNameItem();

            // Find Pre Product Name item.
            var preProductNameItems = from preProduct in preProducts
                                      join preProductShelfStock in preProductShelfStocks on preProduct.F03_PreProductCode equals preProductShelfStock.F49_PreProductCode
                                      join preProductShelfStatus in preProductShelfStatuss on preProductShelfStock.F49_ContainerCode equals preProductShelfStatus.F37_ContainerCode
                                      where preProductShelfStatus.F37_ShelfStatus.Trim().Equals(condition1)
                                      //group materialShelfStock by materialShelfStock.F33_MaterialCode into grp
                                      orderby new
                                      {
                                          preProductShelfStatus.F37_ShelfRow,
                                          preProductShelfStatus.F37_ShelfBay,
                                          preProductShelfStatus.F37_ShelfLevel
                                      }

                                      select new FindPrintPreProductNameItem()
                                      {
                                          PreProductCode = preProductShelfStock.F49_PreProductCode,
                                          PreProductName = preProduct.F03_PreProductName,
                                          ContainerCode = preProductShelfStock.F49_ContainerCode,
                                          ContainerType = preProduct.F03_ContainerType,
                                          RBL = preProductShelfStatus.F37_ShelfRow + "-" + preProductShelfStatus.F37_ShelfBay + "-" + preProductShelfStatus.F37_ShelfLevel,
                                          ShelfNo = preProductShelfStatus.F37_ShelfRow + "-" + preProductShelfStatus.F37_ShelfBay + "-" + preProductShelfStatus.F37_ShelfLevel,
                                          LotNo = preProductShelfStock.F49_PreProductLotNo,
                                          SDate1 = preProductShelfStock.F49_StorageDate,
                                          Quantity = preProductShelfStock.F49_Amount,
                                      };


            var totalFirstItems = 0;



            var totalSecondItems = 0;





            //•	o	Show “PAGE: “ + current page with format as ###            
            printPreProductNameItem.Page = "1";

            // Get Total quantity
            printPreProductNameItem.Total1 = totalFirstItems;
            printPreProductNameItem.Total2 = totalSecondItems;
            //if (preProductNameItems.Any())
            //{
            //    printPreProductNameItem.Total = preProductNameItems.Sum(i => i.Amount);
            //}            
            // o	Show {current date/time} with format: dd-mm-yyyy hh:ss                                           
            printPreProductNameItem.Datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            /*
             * o	Show {Company Name} = value from profile:
             * ProfileString("toshiba.ini","server","companyname","")
            */
            printPreProductNameItem.CompanyName = WebConfigurationManager.AppSettings["CompanyName"];


            await _unitOfWork.CommitAsync();

            printPreProductNameItem.FindPrintPreProductNameItem = await preProductNameItems.ToListAsync();

            printPreProductNameItem.FindPrintPreProductNameItem =
                printPreProductNameItem.FindPrintPreProductNameItem.Select(m => new FindPrintPreProductNameItem()
                {
                    PreProductCode = m.PreProductCode,
                    PreProductName = m.PreProductName,
                    ContainerCode = m.ContainerCode,
                    ContainerType = m.ContainerType,
                    RBL = m.RBL,
                    ShelfNo = m.ShelfNo,
                    LotNo = m.LotNo,
                    SDate = m.SDate1.HasValue ? m.SDate1.Value.ToString("dd/MM/yyyy") : "",
                    Stime = m.SDate1.HasValue ? m.SDate1.Value.ToString("HH:mm") : "",
                    Quantityst = m.Quantity.ToString("###,###,###,##0.00"),
                }).OrderBy(m => m.RBL).ThenBy(m => m.PreProductCode).ThenBy(m => m.ContainerType).ThenBy(m => m.ContainerCode).ThenBy(m=> m.SDate).ThenBy(m => m.LotNo).ToList();
            return printPreProductNameItem;
        }

        public async Task<PrintPreProductNameItem> SearchExtPreProductForPrint()
        {
            //Get all record tx51
            var productShelfStatus = _unitOfWork.ProductShelfStatusRepository.GetAll();
            //Get all record tx53
            var outpreProductShelfStocks = _unitOfWork.OutSidePrePdtStkRepository.GetAll();

            // Get all tm03.
            var preProducts = _unitOfWork.PreProductRepository.GetAll();

            var condition1 = Constants.F51_ShelfStatus.TX51_ShfSts_ExtPrePdt;
            var condition2 = Constants.F51_ShelfType.Normal;

            // Record which should be printed.
            var printPreProductNameItem = new PrintPreProductNameItem();

            // Find Pre Product Name item.
            var preProductNameItems = from outpreProductShelfStock in outpreProductShelfStocks
                                      join preProduct in preProducts on outpreProductShelfStock.F53_OutSidePrePdtCode equals preProduct.F03_PreProductCode
                                      join preProductShelfStatu in productShelfStatus on outpreProductShelfStock.F53_PalletNo equals preProductShelfStatu.F51_PalletNo
                                      where preProductShelfStatu.F51_ShelfStatus.Trim().Equals(condition1) &&
                                      preProductShelfStatu.F51_ShelfType.Trim().Equals("0")
                                      select new FindPrintPreProductNameItem()
                                      {
                                          PreProductCode = outpreProductShelfStock.F53_OutSidePrePdtCode,
                                          PreProductName = preProduct.F03_PreProductName,
                                          RBL = preProductShelfStatu.F51_ShelfRow + "-" + preProductShelfStatu.F51_ShelfBay + "-" + preProductShelfStatu.F51_ShelfLevel,
                                          PalletNo = preProductShelfStatu.F51_PalletNo,
                                          LotNo = outpreProductShelfStock.F53_OutSidePrePdtLotNo,
                                          SDate1 = outpreProductShelfStock.F53_AddDate,
                                          Quantity = outpreProductShelfStock.F53_Amount,
                                      };


            var totalFirstItems = 0;



            var totalSecondItems = 0;





            //•	o	Show “PAGE: “ + current page with format as ###            
            printPreProductNameItem.Page = "1";

            // Get Total quantity
            printPreProductNameItem.Total1 = totalFirstItems;
            printPreProductNameItem.Total2 = totalSecondItems;
            //if (preProductNameItems.Any())
            //{
            //    printPreProductNameItem.Total = preProductNameItems.Sum(i => i.Amount);
            //}            
            // o	Show {current date/time} with format: dd-mm-yyyy hh:ss                                           
            printPreProductNameItem.Datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            /*
             * o	Show {Company Name} = value from profile:
             * ProfileString("toshiba.ini","server","companyname","")
            */
            printPreProductNameItem.CompanyName = WebConfigurationManager.AppSettings["CompanyName"];


            await _unitOfWork.CommitAsync();

            printPreProductNameItem.FindPrintPreProductNameItem = await preProductNameItems.ToListAsync();

            printPreProductNameItem.FindPrintPreProductNameItem =
                printPreProductNameItem.FindPrintPreProductNameItem.Select(m => new FindPrintPreProductNameItem()
                {
                    PreProductCode = m.PreProductCode,
                    PreProductName = m.PreProductName,
                    RBL = m.RBL,
                    PalletNo = m.PalletNo,
                    LotNo = m.LotNo,
                    SDate = m.SDate1.HasValue? m.SDate1.Value.ToString("dd/MM/yyyy") : "",
                    Stime = m.SDate1.HasValue? m.SDate1.Value.ToString("HH:mm") : "",
                    Quantityst = m.Quantity.ToString("###,###,###,##0.00"),
                }).ToList();
            return printPreProductNameItem;
        }

        public async Task<PrintMaterialNameItem> SearchProductForPrint()
        {
            //Get all record tx51
            var productShelfStatus = _unitOfWork.ProductShelfStatusRepository.GetAll();
            //Get all record tx40
            var productShelfStocks = _unitOfWork.ProductShelfStockRepository.GetAll();

            // Get all tm09.
            var products = _unitOfWork.ProductRepository.GetAll();

            var condition1 = Constants.F51_ShelfStatus.TX51_ShfSts_Pdt;
            var condition2 = Constants.F51_ShelfType.Normal;

            // Record which should be printed.
            var printPreProductNameItem = new PrintMaterialNameItem();

            // Find Pre Product Name item.
            var preProductNameItems = from productShelfStock in productShelfStocks
                                      join product in products on productShelfStock.F40_ProductCode equals product.F09_ProductCode
                                      join productShelfStatu in productShelfStatus on productShelfStock.F40_PalletNo equals productShelfStatu.F51_PalletNo
                                      where productShelfStatu.F51_ShelfStatus.Trim().Equals(condition1) &&
                                      productShelfStatu.F51_ShelfType.Trim().Equals("0")
                                      orderby new
                                      {
                                          productShelfStatu.F51_ShelfRow,
                                          productShelfStatu.F51_ShelfBay,
                                          productShelfStatu.F51_ShelfLevel
                                          //productShelfStock.F40_ProductCode,
                                          //productShelfStock.F40_ProductLotNo,
                                      }
                                      select new FindPrintMaterialNameItem()
                                      {
                                          PreProductCode = productShelfStock.F40_ProductCode,
                                          PreProductName = product.F09_ProductDesp,
                                          RBL = productShelfStatu.F51_ShelfRow + "-" + productShelfStatu.F51_ShelfBay + "-" + productShelfStatu.F51_ShelfLevel,
                                          PalletNo = productShelfStatu.F51_PalletNo,
                                          LotNo = productShelfStock.F40_ProductLotNo,
                                          CF = productShelfStock.F40_CertificationFlg,
                                          SDate1 = productShelfStatu.F51_StorageDate,
                                          Quantity = productShelfStock.F40_Amount,
                                      };


            var totalFirstItems = 0;



            var totalSecondItems = 0;





            //•	o	Show “PAGE: “ + current page with format as ###            
            printPreProductNameItem.Page = "1";

            // Get Total quantity
            //printPreProductNameItem.Total1 = totalFirstItems;
            //printPreProductNameItem.Total2 = totalSecondItems;
            //if (preProductNameItems.Any())
            //{
            //    printPreProductNameItem.Total = preProductNameItems.Sum(i => i.Amount);
            //}            
            // o	Show {current date/time} with format: dd-mm-yyyy hh:ss                                           
            printPreProductNameItem.Datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            /*
             * o	Show {Company Name} = value from profile:
             * ProfileString("toshiba.ini","server","companyname","")
            */
            printPreProductNameItem.CompanyName = WebConfigurationManager.AppSettings["CompanyName"];


            await _unitOfWork.CommitAsync();

            printPreProductNameItem.FindPrintMaterialNameItem = await preProductNameItems.ToListAsync();

            var groupby =
                printPreProductNameItem.FindPrintMaterialNameItem.Select(m => new FindPrintMaterialNameItem()
                {
                    PreProductCode = m.PreProductCode,
                    PreProductName = m.PreProductName,
                    LotNo = m.LotNo,
                    PalletNo = m.PalletNo,
                    RBL = m.RBL,
                    CF=m.CF.Equals("1") ? "OK": m.CF.Equals("2") ? "NG":"",
                    SDate = m.SDate1.HasValue ? m.SDate1.Value.ToString("dd/MM/yyyy") : "",
                    Stime = m.SDate1.HasValue ? m.SDate1.Value.ToString("HH:mm") : "",
                    Quantity = m.Quantity,
                    Quantityst = m.Quantity.ToString("###,###,###,##0.00"),
                }).ToList().GroupBy(m => m.RBL);

            var groupitems = new List<FindPrintMaterialNameGroupItem>();

            foreach (var items in groupby)
            {
                var groupitem = new FindPrintMaterialNameGroupItem();
                groupitem.FindPrintMaterialNameItem = items.ToList();
                groupitem.Total = groupitem.FindPrintMaterialNameItem.Sum(i => i.Quantity).ToString("###,###,###,##0.00");
                groupitems.Add(groupitem);
            }
            printPreProductNameItem.groupitems = groupitems;
            return printPreProductNameItem;
        }

        public async Task<PrintPreProductNameItem> SearchMaterialPalletForPrint()
        {
            //Get all record tx31
            var materialShelfStatuss = _unitOfWork.MaterialShelfStatusRepository.GetAll();

            // Record which should be printed.
            var printPreProductNameItem = new PrintPreProductNameItem();

            // Find Pre Product Name item.
            var preProductNameItems = from materialShelfStatus in materialShelfStatuss
                                      where materialShelfStatus.F31_ShelfStatus.Trim().Equals("1") ||
                                      materialShelfStatus.F31_ShelfStatus.Trim().Equals("2")
                                      select new FindPrintPreProductNameItem()
                                      {
                                          SupplierCode = materialShelfStatus.F31_SupplierCode,
                                          RBL = materialShelfStatus.F31_ShelfRow + "-" + materialShelfStatus.F31_ShelfBay + "-" + materialShelfStatus.F31_ShelfLevel,
                                          PalletClass = materialShelfStatus.F31_ShelfStatus,
                                          SDate1 = materialShelfStatus.F31_StorageDate,
                                          Status = materialShelfStatus.F31_ShelfStatus,
                                          Quantityint = materialShelfStatus.F31_LoadAmount
                                      };


            var totalFirstItems = 0;



            var totalSecondItems = 0;





            //•	o	Show “PAGE: “ + current page with format as ###            
            printPreProductNameItem.Page = "1";

            // Get Total quantity
            printPreProductNameItem.Total1 = totalFirstItems;
            printPreProductNameItem.Total2 = totalSecondItems;
            //if (preProductNameItems.Any())
            //{
            //    printPreProductNameItem.Total = preProductNameItems.Sum(i => i.Amount);
            //}            
            // o	Show {current date/time} with format: dd-mm-yyyy hh:ss                                           
            printPreProductNameItem.Datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            /*
             * o	Show {Company Name} = value from profile:
             * ProfileString("toshiba.ini","server","companyname","")
            */
            printPreProductNameItem.CompanyName = WebConfigurationManager.AppSettings["CompanyName"];


            await _unitOfWork.CommitAsync();

            printPreProductNameItem.FindPrintPreProductNameItem = await preProductNameItems.ToListAsync();

            printPreProductNameItem.FindPrintPreProductNameItem =
                printPreProductNameItem.FindPrintPreProductNameItem.Select(m => new FindPrintPreProductNameItem()
                {
                    SupplierCode = m.SupplierCode,
                    PalletClass = m.PalletClass,
                    RBL = m.RBL,
                    SDate = m.SDate1.HasValue? m.SDate1.Value.ToString("dd/MM/yyyy") : "",
                    Stime = m.SDate1.HasValue? m.SDate1.Value.ToString("HH:mm") : "",
                    LoadQuantity = m.Status.Equals("1") ? "" : m.Quantityint.HasValue ? m.Quantityint.Value.ToString("###,###,###,##0.00") : ""
                }).ToList();
            return printPreProductNameItem;
        }

        public async Task<PrintPreProductNameItem> SearchPreProductContainerForPrint()
        {
            //Get all record tx37
            var preProductContainerStatuss = _unitOfWork.PreProductShelfStatusRepository.GetAll();

            // Record which should be printed.
            var printPreProductNameItem = new PrintPreProductNameItem();

            // Find Pre Product Name item.
            var preProductNameItems = from preProductContainerStatus in preProductContainerStatuss
                                      where preProductContainerStatus.F37_ShelfStatus.Trim().Equals("1")
                                      orderby new
                                      {
                                          preProductContainerStatus.F37_ShelfRow,
                                          preProductContainerStatus.F37_ShelfBay,
                                          preProductContainerStatus.F37_ShelfLevel
                                      }
                                      select new FindPrintPreProductNameItem()
                                      {
                                          RBL = preProductContainerStatus.F37_ShelfRow + "-" + preProductContainerStatus.F37_ShelfBay + "-" + preProductContainerStatus.F37_ShelfLevel,
                                          ContainerType = preProductContainerStatus.F37_ContainerType,
                                          ContainerNo = preProductContainerStatus.F37_ContainerNo,
                                          SDate1 = preProductContainerStatus.F37_StorageDate
                                      };


            var totalFirstItems = 0;



            var totalSecondItems = 0;





            //•	o	Show “PAGE: “ + current page with format as ###            
            printPreProductNameItem.Page = "1";

            // Get Total quantity
            printPreProductNameItem.Total1 = totalFirstItems;
            printPreProductNameItem.Total2 = totalSecondItems;
            //if (preProductNameItems.Any())
            //{
            //    printPreProductNameItem.Total = preProductNameItems.Sum(i => i.Amount);
            //}            
            // o	Show {current date/time} with format: dd-mm-yyyy hh:ss                                           
            printPreProductNameItem.Datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            /*
             * o	Show {Company Name} = value from profile:
             * ProfileString("toshiba.ini","server","companyname","")
            */
            printPreProductNameItem.CompanyName = WebConfigurationManager.AppSettings["CompanyName"];


            await _unitOfWork.CommitAsync();

            printPreProductNameItem.FindPrintPreProductNameItem = await preProductNameItems.ToListAsync();

            printPreProductNameItem.FindPrintPreProductNameItem =
                printPreProductNameItem.FindPrintPreProductNameItem.Select(m => new FindPrintPreProductNameItem()
                {
                    ContainerType = m.ContainerType,
                    ContainerNo = m.ContainerNo,
                    RBL = m.RBL,
                    SDate = m.SDate1.HasValue? m.SDate1.Value.ToString("dd/MM/yyyy") : "",
                    Stime = m.SDate1.HasValue? m.SDate1.Value.ToString("HH:mm") : "",
                   }).ToList();
            return printPreProductNameItem;
        }


        public async Task<PrintPreProductNameItem> SearchProductPalletForPrint()
        {
            //Get all record tx31
            var productShelfStatuss = _unitOfWork.ProductShelfStatusRepository.GetAll();

            // Record which should be printed.
            var printPreProductNameItem = new PrintPreProductNameItem();

            // Find Pre Product Name item.
            var preProductNameItems = from productShelfStatus in productShelfStatuss
                                      where productShelfStatus.F51_ShelfStatus.Trim().Equals("1")
                                      orderby new
                                      {
                                          productShelfStatus.F51_ShelfRow,
                                          productShelfStatus.F51_ShelfBay,
                                          productShelfStatus.F51_ShelfLevel
                                      }
                                      select new FindPrintPreProductNameItem()
                                      {
                                          RBL = productShelfStatus.F51_ShelfRow + "-" + productShelfStatus.F51_ShelfBay + "-" + productShelfStatus.F51_ShelfLevel,
                                          SDate1 = productShelfStatus.F51_StorageDate,
                                          Quantityint = productShelfStatus.F51_LoadAmount
                                      };


            var totalFirstItems = 0;



            var totalSecondItems = 0;





            //•	o	Show “PAGE: “ + current page with format as ###            
            printPreProductNameItem.Page = "1";

            // Get Total quantity
            printPreProductNameItem.Total1 = totalFirstItems;
            printPreProductNameItem.Total2 = totalSecondItems;
            //if (preProductNameItems.Any())
            //{
            //    printPreProductNameItem.Total = preProductNameItems.Sum(i => i.Amount);
            //}            
            // o	Show {current date/time} with format: dd-mm-yyyy hh:ss                                           
            printPreProductNameItem.Datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            /*
             * o	Show {Company Name} = value from profile:
             * ProfileString("toshiba.ini","server","companyname","")
            */
            printPreProductNameItem.CompanyName = WebConfigurationManager.AppSettings["CompanyName"];


            await _unitOfWork.CommitAsync();

            printPreProductNameItem.FindPrintPreProductNameItem = await preProductNameItems.ToListAsync();

            printPreProductNameItem.FindPrintPreProductNameItem =
                printPreProductNameItem.FindPrintPreProductNameItem.Select(m => new FindPrintPreProductNameItem()
                {
                    RBL = m.RBL,
                    SDate = m.SDate1.HasValue ? m.SDate1.Value.ToString("dd/MM/yyyy") : "",
                    Stime = m.SDate1.HasValue ? m.SDate1.Value.ToString("HH:mm") : "",
                    LoadQuantity = m.Quantityint.HasValue ? m.Quantityint.Value.ToString("###,###,###,##0.00") : ""
                }).ToList();
            return printPreProductNameItem;
        }

        public async Task<PrintMaterialNameItem> SearchRecordsForPrintingAll()
        {
            // Find all tx31
            var materialShelfStatuss = _unitOfWork.MaterialShelfStatusRepository.GetAll();

            // Find all tx33
            var materialShelfStocks = _unitOfWork.MaterialShelfStockRepository.GetAll();

            // Find all materials.
            var materials = _unitOfWork.MaterialRepository.GetAll();

            // Record which should be printed.
            var printMaterialNameItem = new PrintMaterialNameItem();

            var condition1 = Constants.F31_ShelfStatus.Material.ToString("D");


            // Find Material Name item.
            var materialNameItems = from material in materials
                                    join materialShelfStock in materialShelfStocks on material.F01_MaterialCode equals materialShelfStock.F33_MaterialCode
                                    join materialShelfStatus in materialShelfStatuss on materialShelfStock.F33_PalletNo equals materialShelfStatus.F31_PalletNo
                                    where materialShelfStatus.F31_ShelfStatus.Trim().Equals(condition1)
                                     && (material.F01_EntrustedClass.Equals("0") || material.F01_EntrustedClass.Equals("1"))
                                    orderby new
                                    {
                                        //materialShelfStock.F33_MaterialCode,
                                        //materialShelfStock.F33_MaterialLotNo

                                        materialShelfStatus.F31_ShelfRow,
                                        materialShelfStatus.F31_ShelfBay, 
                                        materialShelfStatus.F31_ShelfLevel
                                        //materialShelfStock.F33_MaterialCode,
                                        //materialShelfStock.F33_MaterialLotNo
                                    }
                                    select new FindPrintMaterialNameItem
                                    {
                                        MaterialCode = materialShelfStock.F33_MaterialCode,
                                        MaterialName = material.F01_MaterialDsp,
                                        LotNo = materialShelfStock.F33_PalletNo,
                                        PalletNo = materialShelfStock.F33_MaterialLotNo,
                                        RBL = materialShelfStatus.F31_ShelfRow + "-" + materialShelfStatus.F31_ShelfBay + "-" + materialShelfStatus.F31_ShelfLevel,
                                        Quantity = materialShelfStock.F33_Amount,
                                        //Total = materialShelfStocks.Sum(i => i.F33_Amount)
                                    };

            //•	o	Show “PAGE: “ + current page with format as ###            
            printMaterialNameItem.Page = "1";

            // Get Total quantity
            if (materialNameItems.Any())
            {
                printMaterialNameItem.Total = materialNameItems.Sum(i => i.Quantity).ToString("###,###,###,##0.00");
            }
            // o	Show {current date/time} with format: dd-mm-yyyy hh:ss                                           
            printMaterialNameItem.Datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            /*
             * o	Show {Company Name} = value from profile:
             * ProfileString("toshiba.ini","server","companyname","")
            */
            printMaterialNameItem.CompanyName = WebConfigurationManager.AppSettings["CompanyName"];


            await _unitOfWork.CommitAsync();

            printMaterialNameItem.FindPrintMaterialNameItem = await materialNameItems.ToListAsync();

            var groupby =
                printMaterialNameItem.FindPrintMaterialNameItem.Select(m => new FindPrintMaterialNameItem()
                {
                    MaterialCode = m.MaterialCode,
                    MaterialName = m.MaterialName,
                    LotNo = m.LotNo,
                    PalletNo = m.PalletNo,
                    RBL = m.RBL,
                    Quantity = m.Quantity,
                    Quantityst = m.Quantity.ToString("###,###,###,##0.00"),
                }).ToList().GroupBy(m => m.RBL);

            var groupitems = new List<FindPrintMaterialNameGroupItem>();

            foreach (var items in groupby)
            {
                var groupitem = new FindPrintMaterialNameGroupItem();
                groupitem.FindPrintMaterialNameItem = items.ToList();
                groupitem.Total = groupitem.FindPrintMaterialNameItem.Sum(i => i.Quantity).ToString("###,###,###,##0.00");
                groupitems.Add(groupitem);
            }
            printMaterialNameItem.groupitems = groupitems;
            return printMaterialNameItem;
        }
        public MaterialItem GetById(string materialCode)
        {
            var entity = _unitOfWork.MaterialRepository.GetAll().FirstOrDefault(i => i.F01_MaterialCode.Trim().Equals(materialCode.Trim()));
            var result = Mapper.Map<MaterialItem>(entity);
            return result;
        }
    }
}
