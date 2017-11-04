using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.ProductionPlanning;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.MasterList;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.MaterialRequirementList;
using KCSG.Web.Attributes;

namespace KCSG.Web.Areas.ProductionPlanning.Controllers
{
    [MvcAuthorize("TCPP081F")]
    public class MaterialRequirementListController : KCSG.Web.Controllers.BaseController
    {
        private readonly IMaterialRequirementListDomain _materialRequirementListDomain;
        private IPreProductDomain _preProductDomain;
        private IProductDomain _productDomain;
        private readonly IExportReportDomain _exportReportDomain;
        public MaterialRequirementListController(IMaterialRequirementListDomain iMaterialRequirementListDomain,
            IProductDomain iProductDomain, IPreProductDomain iPreProductDomain,IExportReportDomain iExportReportDomain)
        {
            _materialRequirementListDomain = iMaterialRequirementListDomain;
            _productDomain = iProductDomain;
            _exportReportDomain = iExportReportDomain;
            _preProductDomain = iPreProductDomain;
        }

        //
        // GET: /Master/MaterialRequirementList/
        public ActionResult Index()
        {
            var model = new MaterialReqListSearchViewModel
            {
                Grid = GenerateGrid(),
                DefaultGrid = GenerateDefaultGrid(),
                DateSearch =  DateTime.Now.ToString("MM/yyyy")
            };
            return View(model);
        }

       
        public ActionResult SearchByMaterial(MaterialReqListSearchViewModel model, GridSettings gridSettings)

        {
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortOrder = SortOrder.Asc;
                gridSettings.SortField = "F01_MaterialCode";
            }
            var result = _materialRequirementListDomain.SearchCriteria(ConvertHelper.ConvertToDateTimeFull("01/"+model.DateSearch), gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);

            var response = Mapper.Map<GridResponse<object>>(result.Data);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchDefault(GridSettings gridSettings)

        {
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortOrder = SortOrder.Asc;
                gridSettings.SortField = "F01_MaterialCode";
            }
            var result = _materialRequirementListDomain.Search( gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);

            var response = Mapper.Map<GridResponse<object>>(result.Data);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Print(MaterialReqListSearchViewModel model)
        {
            // Parameters are invalid.
            if (!ModelState.IsValid)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            string renderedData;

            // Base on what user selects, print the related.
            
            // Find the template absolute path.
            var exportPreProductAbsolutePath =
                Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.MaterialRequirementList));

            // Read the template.
            var masterListPreProductTemplate =
               await _exportReportDomain.ReadTemplateFileAsync(exportPreProductAbsolutePath);

            // Find all records.
            var result = _materialRequirementListDomain.SearchCriteria(ConvertHelper.ConvertToDateTimeFull("01/" + model.DateSearch), null);
            var lstPreProduct = new
            {
                Data = new
                {
                    result.Data.data,
                    currentDate = DateTime.Now.ToString("dd/MM/yyyy"),
                    yearMonth = model.DateSearch,
                    companyName = WebConfigurationManager.AppSettings["CompanyName"]
                }

            };
            // Render template
            renderedData =
                await _exportReportDomain.ExportToFlatFileAsync(lstPreProduct.Data,
                    masterListPreProductTemplate);

            return Json(new
            {
                render = renderedData,

            });
        }

        #region Private Methods
        private Grid GenerateDefaultGrid()
        {
            return new Grid("DefaultGrid")
               .SetMode(GridMode.Listing)
               .SetWidth("100%")
               .SetSorting(true)
               .SetPaging(true)
               .SetPageSize(30)
               .SetPageLoading(true)
               .SetAutoload(true)
               .SetSearchUrl(Url.Action("SearchDefault", "MaterialRequirementList",
                   new { Area = "ProductionPlanning" }))
               .SetDefaultSorting("F01_MaterialCode", SortOrder.Asc)
               .SetFields(
                   new Field("F01_MaterialCode")
                       .SetTitle("Material Code")
                       .SetItemTemplate("gridHelper.generateNameColumn")
                       .SetWidth(100),
                   new Field("F01_MaterialDsp")
                       .SetTitle("Material Name")
                       .SetItemTemplate("gridHelper.generateNameColumn")
                       .SetWidth(100),
                   new Field("F31_Amount")
                       .SetTitle("Material Requirement Qty")
                       .SetItemTemplate("gridHelper.displayNumber")
                       .SetWidth(100),
                   new Field("unit")
                       .SetTitle("Unit")
                       .SetWidth(100)
                       .SetItemTemplate("gridHelper.generateNameColumn")
               );
        }

        private Grid GenerateGrid()
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
                .SetSearchUrl(urlHelper.Action("SearchByMaterial", "MaterialRequirementList",
                    new {Area = "ProductionPlanning"}))
                .SetDefaultSorting("F01_MaterialCode", SortOrder.Asc)
                .SetFields(
                    new Field("F01_MaterialCode")
                        .SetTitle("Material Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("F01_MaterialDsp")
                        .SetTitle("Material Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("totalAmount")
                        .SetTitle("Material Requirement Qty")
                        .SetItemTemplate("gridHelper.displayNumber")
                        .SetWidth(100),
                    new Field("unit")
                        .SetTitle("Unit")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                );
        }

        #endregion
    }
}