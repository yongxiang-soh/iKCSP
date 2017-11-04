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
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.MasterList;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.PreProductPlan;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.ProductionPlanning.Controllers
{
    [MvcAuthorize("TCPP071F")]
    public class PreProductPlanController : KCSG.Web.Controllers.BaseController
    {
        #region Controller Constructor
        
        public PreProductPlanController(IPreProductPlanDomain preProductPlanDomain, IPreProductDomain preProductDomain,ICommonDomain iCommonDomain,IExportReportDomain iExportReportDomain)
        {
            _preProductPlanDomain = preProductPlanDomain;
            _preProductDomain = preProductDomain;
            _commonDomain = iCommonDomain;
            _exportReportDomain = iExportReportDomain;
        }

        #endregion

        /// <summary>
        /// Renders pre-product search view.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = new PreProductPlanSearchViewModel
            {
                YearMonth = DateTime.Now.ToString("MM/yyyy"),
                Grid = GenerateGrid()
            };
            return View(model);
        }

        public ActionResult Edit(string date, string code)
        {
            var model = new PreProductPlanViewModel {IsCreate = true, F94_YearMonth = DateTime.Now.ToString("MM/yyyy")};
            if (!string.IsNullOrEmpty(code))
            {
                var entity = _preProductPlanDomain.GetById(date, code);
                if (entity != null)
                {
                    model = Mapper.Map<PreProductPlanViewModel>(entity);
                    var preProductItem = _preProductDomain.GetPreProduct(code);
                    if (preProductItem != null)
                        model.PreProductName = preProductItem.F03_PreProductName;

                    model.IsCreate = false;
                }
            }
            return PartialView("PreProductPlan/_PartialViewEditPreProductPlan", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(PreProductPlanViewModel model)
        {
            if (ModelState.IsValid)
                try
                {
                    var item = Mapper.Map<PreProductPlanItem>(model);
                    var isSuccess = _preProductPlanDomain.CreateOrUpdate(item);
                    if (!isSuccess.IsSuccess)
                        return Json(new {Success = false, Message = isSuccess.ErrorMessages},
                            JsonRequestBehavior.AllowGet);
                    return
                        Json(
                            new {Success = true, Message = model.IsCreate ? MessageResource.MSG6 : MessageResource.MSG9},
                            JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new {Success = false, ex.Message}, JsonRequestBehavior.AllowGet);
                }
            return Json(new {Success = false}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SearchByName(PreProductPlanSearchViewModel model, GridSettings gridSettings)
        {
            if (string.IsNullOrEmpty(gridSettings.SortField))
                gridSettings.SortField = "F94_YearMonth";
            var result = _preProductPlanDomain.SearchCriteria(model.YearMonth, gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);

            var response = Mapper.Map<GridResponse<PreProductPlanItem>>(result.Data);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Delete(string date, string code)
        {
            try
            {
                var mydate = ConvertHelper.ConvertToDateTimeFull(date);
                _preProductPlanDomain.Delete(mydate, code);
                return Json(new {Success = true, Message = MessageResource.MSG10}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {Success = false, ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Print(PreProductPlanSearchViewModel model)
        {
            try
            {
                // Parameter hasn't been initialized.

                // Parameters are invalid.
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                string renderedData;

                // Base on what user selects, print the related.


                // Find the template absolute path.
                var exportPreProductAbsolutePath =
                    Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.PreProductPlan));

                // Read the template.
                var masterListPreProductTemplate =
                    await _exportReportDomain.ReadTemplateFileAsync(exportPreProductAbsolutePath);

                // Find all records.
                var preProducts = _preProductPlanDomain.SearchCriteria(model.YearMonth, null);
                var lstPreProduct = new
                {
                    Data = new
                    {
                        preProducts.Data.data,
                        currentDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                        productMonth = model.YearMonth,
                        companyName= WebConfigurationManager.AppSettings["CompanyName"]
                    }

                };
                // Render template
                renderedData =
                    await
                        _exportReportDomain.ExportToFlatFileAsync(lstPreProduct.Data,
                            masterListPreProductTemplate);
                return Json(new
                {
                    render = renderedData,

                });
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        #region Private Methods

             private
             Grid GenerateGrid()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("PreProductPlanGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(true)
                .SetSelected(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSearchUrl(urlHelper.Action("SearchByName", "PreProductPlan", new {Area = "ProductionPlanning"}))
                .SetDefaultSorting("F94_PrepdtCode", SortOrder.Asc)
                .SetFields(
                    new Field("F94_YearMonth")
                        .SetTitle(" ")
                        .SetWidth(20)
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F94_PrepdtCode")
                        .SetTitle("Pre-Product Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("PreProductName")
                        .SetTitle("Pre-Product Name")
                        .SetWidth(100),
                    new Field("amount")
                        .SetTitle(" Production Qty (Kg)")
                        //.SetItemTemplate("gridHelper.displayNumber")
                        .SetWidth(100)
                );
        }

        #endregion

        [AllowAnonymous]
        [HttpOptions]
        public JsonResult CheckExistCode(string f94_prepdtcode, string f94_YearMonth, bool isCreate)
        {
            if (!isCreate) return Json(true, JsonRequestBehavior.AllowGet);
            var result =
                !_preProductPlanDomain.CheckUnique(f94_prepdtcode,
                    ConvertHelper.ConvertToDateTimeFull("01/" + f94_YearMonth));
            return Json(result, JsonRequestBehavior.AllowGet);
        } 



        #region Domain Declaration

        private readonly IPreProductPlanDomain _preProductPlanDomain;
        private readonly IPreProductDomain _preProductDomain;
        private readonly ICommonDomain _commonDomain;
        private readonly IExportReportDomain _exportReportDomain;

        #endregion
    }
}