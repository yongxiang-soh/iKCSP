using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.ProductionPlanning;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.MasterList;
using KCSG.Web.Attributes;

namespace KCSG.Web.Areas.ProductionPlanning.Controllers
{
    [MvcAuthorize("TCPP061F")]
    public class MasterListController : Controller
    {
        #region Constructors

        /// <summary>
        ///     Initiate controller with dependency injections.
        /// </summary>
        /// <param name="iMaterialDomain"></param>
        /// <param name="iProductDomain"></param>
        /// <param name="iPreProductDomain"></param>
        /// <param name="exportReportDomain"></param>
        public MasterListController(IMaterialDomain iMaterialDomain, IProductDomain iProductDomain,
            IPreProductDomain iPreProductDomain, IExportReportDomain exportReportDomain)
        {
            _materialDomain = iMaterialDomain;
            _productDomain = iProductDomain;
            _preProductDomain = iPreProductDomain;
            _exportReportDomain = exportReportDomain;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Domain which provides function to access material repository.
        /// </summary>
        private readonly IMaterialDomain _materialDomain;

        /// <summary>
        ///     Domain which provides function to access pre-product repository.
        /// </summary>
        private readonly IPreProductDomain _preProductDomain;

        /// <summary>
        ///     Domain which provides function to access product repository.
        /// </summary>
        private readonly IProductDomain _productDomain;

        /// <summary>
        ///     Domain which provides function to export report to excel file.
        /// </summary>
        private readonly IExportReportDomain _exportReportDomain;

        #endregion

        #region Methods

        /// <summary>
        ///     Render master list page.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.ScreenId = "TCPP061F";

            var model = new MasterListSearchViewModel
            {
                GridMaterial = GenerateGridMaterial(),
                GridPreProduct = GenerateGridPreProduct(),
                GridProduct = GenerateGridProduct()
            };
            return View(model);
        }

        /// <summary>
        ///     Search materials by using specific conditions.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="gridSettings"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SearchByMaterial(MasterListSearchViewModel model, GridSettings gridSettings)
        {
            var result = _materialDomain.SearchCriteria(model.MasterCode, gridSettings);
            if (!result.IsSuccess)
                return Json(null);
            return Json(result.Data);
        }

        /// <summary>
        ///     Search products by using specific conditions.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="gridSettings"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SearchByProduct(MasterListSearchViewModel model, GridSettings gridSettings)
        {
            var result = _productDomain.SearchPrint(model.MasterCode, gridSettings);
            if (!result.IsSuccess)
                return Json(null);
            return Json(result.Data);
        }

        /// <summary>
        ///     Search pre-products by using specific conditions.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="gridSettings"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SearchByPreProduct(MasterListSearchViewModel model, GridSettings gridSettings)
        {
            try
            {
                var results = _preProductDomain.SearchMaterialList(model.MasterCode, gridSettings);
                return Json(results.Data);
            }
            catch (Exception ex)
            {
                return Json(new {Success = false, ex.Message});
            }
        }

        /// <summary>
        ///     Depend on the user selection to export the related file as excel.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Print(MasterListSearchViewModel masterListSearchViewModel)
        {
            try
            {
                var companyName = WebConfigurationManager.AppSettings["CompanyName"];
                // Parameter hasn't been initialized.
                if (masterListSearchViewModel == null)
                {
                    masterListSearchViewModel = new MasterListSearchViewModel();
                    TryValidateModel(masterListSearchViewModel);
                }

                // Parameters are invalid.
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                string renderedData ;

                // Base on what user selects, print the related.
                switch (masterListSearchViewModel.SearchBy)
                {
                    case 1:

                        // Find the template absolute path.
                        var exportPreProductAbsolutePath =
                            Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.MasterListPreProduct));

                        // Read the template.
                        var masterListPreProductTemplate =
                            await _exportReportDomain.ReadTemplateFileAsync(exportPreProductAbsolutePath);

                        // Find all records.
                        var preProducts = _preProductDomain.MaterialListPrint(masterListSearchViewModel.MasterCode);
                        
//#if DEBUG
//                        preProducts.Data.data = preProducts.Data.data.Take(10);
//#endif
                        var lstPreProduct = new
                        {
                            Data = new
                            {
                                preProducts.Data.data,
                                currentDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                companyName
                            }

                        };
                        // Render template
                        renderedData =
                            await
                                _exportReportDomain.ExportToFlatFileAsync(lstPreProduct.Data,
                                    masterListPreProductTemplate);
                        break;
                    case 2:
                        // Find the template absolute path.
                        var exportProductAbsolutePath =
                            Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.MasterListProduct));

                        // Read the template.
                        var masterListProductTemplate =
                            await _exportReportDomain.ReadTemplateFileAsync(exportProductAbsolutePath);

                        // Find all records.
                        var products = _productDomain.SearchPrint(masterListSearchViewModel.MasterCode, null);
                      
                        var lstproduct = new
                        {
                            Data = new
                            {
                                products.Data.data,
                                currentDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                companyName
                            }

                        };
                        // Render template
                        renderedData =
                            await
                                _exportReportDomain.ExportToFlatFileAsync(lstproduct.Data,
                                    masterListProductTemplate);

                        break;
                    default:
                        // Find the template absolute path.
                        var exportMaterialAbsolutePath =
                            Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.MasterListMaterial));

                        // Read the template.
                        var masterListMaterialTemplate =
                            await _exportReportDomain.ReadTemplateFileAsync(exportMaterialAbsolutePath);

                      

                        // Find all records.
                        var materials = _materialDomain.SearchCriteria(masterListSearchViewModel.MasterCode, null);
                     //   materials.Data.data = materials.Data.data.ToList();
                        
                        var material = new
                        {
                            Data = new
                            {
                                materials.Data.data,
                                currentDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                companyName
                            }
                            
                        };
                        // Render template
                        renderedData =
                            await
                                _exportReportDomain.ExportToFlatFileAsync(material.Data,
                                    masterListMaterialTemplate);
                        break;
                }

                var jsonResult = Json(new
                {
                    render = renderedData,
                   
                });
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Initiate material grid view.
        /// </summary>
        /// <returns></returns>
        private Grid GenerateGridMaterial()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);
            return new Grid("MaterialGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSearchUrl(urlHelper.Action("SearchByMaterial", "MasterList", new {Area = "ProductionPlanning"}))
                .SetDefaultSorting("F01_MaterialCode", SortOrder.Asc)
                .SetFields(
                    new Field("F01_SupplierCode")
                        .SetTitle("Supplier Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F01_MaterialCode")
                        .SetTitle("Material Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F01_MaterialDsp")
                        .SetTitle("Material Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F01_Point")
                        .SetTitle("Factor")
                        .SetWidth(150),
                    new Field("F01_PackingUnit")
                        .SetTitle("Pack. Unit")
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("Bali")
                        .SetTitle("Bail. Class")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F01_Department")
                        .SetTitle("Department")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F01_Price")
                        .SetTitle("Price")
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("F01_FactoryClass")
                        .SetTitle("Factory")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("Liquid")
                        .SetTitle("Liq Class ")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("Comms")
                        .SetTitle("Weighing Machine Command ")
                        .SetWidth(200)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("Retrieval")
                        .SetTitle("Retr. Location ")
                        .SetWidth(120)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("Unit")
                        .SetTitle("Unit ")
                        .SetWidth(50)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F01_EMP")
                        .SetTitle("EMP")
                        .SetWidth(50)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("F01_ModifyClass")
                        .SetTitle("Modify class")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F01_State")
                        .SetTitle("State")
                        .SetWidth(50)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F01_Color")
                        .SetTitle("Color")
                        .SetWidth(50)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                );
        }

        /// <summary>
        ///     Initiate pre-product grid view.
        /// </summary>
        /// <returns></returns>
        private Grid GenerateGridPreProduct()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("PreProductGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSearchUrl(urlHelper.Action("SearchByPreProduct", "MasterList", new {Area = "ProductionPlanning"}))
                .SetDefaultSorting("WSeq", SortOrder.Asc)
                .SetFields(
                    new Field("WSeq")
                        .SetTitle("W.Seq.")
                        .SetItemTemplate("gridHelper.displayNumber")
                        .SetWidth(100),
                    new Field("MasterialCode")
                        .SetTitle("Material Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("MasterialName")
                        .SetTitle("Material Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("C_Pri")
                        .SetTitle("Ch.Pri.")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("Sum3F4F")
                        .SetTitle("Ch.3F Ch.4F")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetWidth(150),
                    new Field("PSeq")
                        .SetTitle("Pot Seq.")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("LoadPosition")
                        .SetTitle("Post.")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("Method")
                        .SetTitle("Weigh Meth.")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("Additive")
                        .SetTitle("Additive")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("CSeq")
                        .SetTitle("Ch.Seq.")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("F03_PreProductCode")
                        .SetTitle("Pre-Product  Code")
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F03_BatchLot")
                        .SetTitle("Batch / Lot")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("MixDate1")
                        .SetTitle("MixTime1")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("MixDate2")
                        .SetTitle("MixTime2")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("MixDate3")
                        .SetTitle("MixTime3")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("F03_YieldRate")
                        .SetTitle("Yield Rate")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("F03_PreProductName")
                        .SetTitle("Pre-Product Name")
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("LowTmpClass")
                        .SetTitle("Temp. Class")
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("KneadingLine")
                        .SetTitle("Kneading Line")
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("MaterialMakeUp")
                        .SetTitle("< Material Make Up >")
                        .SetWidth(180)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("MaterialMakeUp")
                        .SetTitle("%")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("MilingFlag1")
                        .SetTitle("Crush(1)")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("MilingFlag2")
                        .SetTitle("Crush(2)")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F03_ContainerType")
                        .SetTitle("Container Type")
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("EquilibriumTime")
                        .SetTitle("Temp. Return Time")
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F03_MixMode")
                        .SetTitle("MixMode")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F03_Point")
                        .SetTitle("Factor")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                );
        }

        /// <summary>
        ///     Initiate products grid view.
        /// </summary>
        /// <returns></returns>
        private Grid GenerateGridProduct()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("ProductGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSearchUrl(urlHelper.Action("SearchByProduct", "MasterList", new {Area = "ProductionPlanning"}))
                .SetDefaultSorting("F09_ProductCode", SortOrder.Asc)
                .SetFields(
                    new Field("F09_ProductCode")
                        .SetTitle("Product Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F09_ProductDesp")
                        .SetTitle("Product Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F09_Unit")
                        .SetTitle("Unit")
                        .SetWidth(100),
                    new Field("TabletSize")
                        .SetTitle("Tablet Size")
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F09_TabletType")
                        .SetTitle("Tablet Type")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F09_TabletAmount")
                        .SetTitle("Tablet Qty.")
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.displayn"),
                    new Field("F09_KneadingTime")
                        .SetTitle("Kneading Time")
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F09_PackingUnit")
                        .SetTitle("Pack. Unit")
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("F09_Factor")
                        .SetTitle("Factor")
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F09_NeedTime")
                        .SetTitle("Need time")
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("F09_StdStkMtn")
                        .SetTitle("St. Period")
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("F09_ValidPeriod")
                        .SetTitle("Shelf Life")
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("F09_YieldRate")
                        .SetTitle("Yield Rate")
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("F09_PreProductCode")
                        .SetTitle("Pre-Product  Code")
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F09_EndUserCode")
                        .SetTitle("End User Code")
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("InnerLabelReq")
                        .SetTitle("Inner Label Req.")
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("Temperature")
                        .SetTitle("Low Temp. Class")
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                );
        }

        #endregion
    }
}