using System;
using System.Web.Mvc;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces.ProductionPlanning;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.PreProduct;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.ProductionPlanning.Controllers
{
    [MvcAuthorize("TCPP021F")]
    public class PreProductController : KCSG.Web.Controllers.BaseController
    {
        #region Controller Constructor

        public PreProductController(IPreProductDomain preProductDomain, IPrePdtMkpDomain prePdtMkpDomain,
            IMaterialDomain materialDomain)
        {
            _preProductDomain = preProductDomain;
            _prePdtMkpDomain = prePdtMkpDomain;
            _materialDomain = materialDomain;
        }

        #endregion

        public ActionResult Index()
        {
            // Screen ID will be displayed on the browser tab.
            ViewBag.ScreenId = "TCPP021F";

            var model = new PreProductSearchViewModel
            {
                Grid = GenerateGrid()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SearchByCode(PreProductSearchViewModel model, GridSettings gridSettings)
        {
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F03_PreProductCode";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            var results = _preProductDomain.Search(model.PreProductCode, gridSettings);
            if (!results.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);
            return Json(results.Data, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpOptions]
        public JsonResult CheckPreProductCodeExistence(string f03_PreProductCode, bool isCreate)
        {
            if (!isCreate) return Json(true, JsonRequestBehavior.AllowGet);
            var result = !_preProductDomain.CheckUnique(f03_PreProductCode.Trim());
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(string preProductCode)
        {
            var model = new PreProductViewModel {IsCreate = true};
            if (string.IsNullOrEmpty(preProductCode))
            {
                model.F03_BatchLot = 1;
                model.F03_YieldRate = 100.0f;
                model.F03_ContainerType = "";
                model.F03_Point = "";
                model.F03_LotNoEnd = 99;
                model.MixDate1 = new TimeSpan(0, 0, 0);
                model.MixDate2 = new TimeSpan(0, 0, 0);
                model.MixDate3 = new TimeSpan(0, 0, 0);
            }
            else
            {
                var entity = _preProductDomain.GetById(preProductCode.Trim());
                if (entity != null)
                {
                    model = Mapper.Map<PreProductViewModel>(entity);

                    model.IsCreate = false;
                    model.Days = entity.F03_TmpRetTime.Value.Day != 31?entity.F03_TmpRetTime.Value.Day:(int?) null;
                }
            }
            model.Grid = GenerateGridPreproductMaterial(preProductCode);
            return PartialView("PreProduct/_PartialViewEditPreProduct", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(PreProductViewModel model)
        {
          //  if (ModelState.IsValid)
                try
                {
                    var item = Mapper.Map<PreProductItem>(model);
                    if (model.Days == 0||model.Days==null)
                        item.F03_TmpRetTime = new DateTime(1980,
                            1,
                            Constants.LastDayOfMonth,
                             model.TmpRetTime.Hours ,
                             model.TmpRetTime.Minutes ,
                            model.TmpRetTime.Milliseconds );
                    else
                        item.F03_TmpRetTime = new DateTime(1980,
                            1,
                            model.Days.Value,
                            model.TmpRetTime.Hours,
                            model.TmpRetTime.Minutes,
                             model.TmpRetTime.Milliseconds);
                    var isSuccess = _preProductDomain.CreateOrUpdate(item);
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
           // return Json(new {Success = false}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(string preProductCode)
        {
            try
            {
                _preProductDomain.Delete(preProductCode.Trim());
                return Json(new {Success = true, Message = "Delete selected item(s) successfully."},
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {Success = false, ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetPreProductMaterial(string preProductId, GridSettings gridSettings)
        {
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F02_LayinPriority";
                gridSettings.PageSize = 10;
                gridSettings.SortOrder = SortOrder.Asc;
            }
            if (!string.IsNullOrEmpty(preProductId))
            {
                var result = _prePdtMkpDomain.SearchByPreProductCode(preProductId, gridSettings);
                if (result.IsSuccess)
                    return Json(result.Data, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        #region Domain Declaration

        private readonly IPreProductDomain _preProductDomain;
        private readonly IPrePdtMkpDomain _prePdtMkpDomain;
        private readonly IMaterialDomain _materialDomain;

        #endregion

        #region Private Methods

        private Grid GenerateGrid()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);
            return new Grid("PreproductGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSearchUrl(urlHelper.Action("SearchByCode", "PreProduct", new {Area = "ProductionPlanning"}))
                .SetDefaultSorting("F03_PreProductCode", SortOrder.Asc)
                .SetFields(
                    new Field("F03_PreProductCode")
                        .SetWidth(50)
                        .SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox"),
                    new Field("F03_PreProductCode")
                        .SetTitle("Pre-Product  Code")
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                    ,
                    new Field("F03_PreProductName")
                        .SetTitle("Pre-Product Name")
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                    ,
                    new Field("F03_BatchLot")
                        .SetTitle("B/L")
                        .SetWidth(50)
                        .SetItemTemplate("gridHelper.displayNumber")
                        .SetSorting(false),
                    new Field("KneadingLine")
                        .SetTitle("K Line")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("T_Qty")
                        .SetTitle("T_Qty")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumber")
                        .SetSorting(false),
                    new Field("Eq_T")
                        .SetTitle("Eq_T")
                        .SetWidth(120)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("F03_YieldRate")
                        .SetTitle("Yield")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumber")
                        .SetSorting(false),
                    new Field("F03_ContainerType")
                        .SetTitle("C_T")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("LowTmpClass")
                        .SetTitle("Temp")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("F03_MixMode")
                        .SetTitle("Mixmode")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                );
        }

        private Grid GenerateGridPreproductMaterial(string parrentId)
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);
            return new Grid("PreproductMaterialGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetSelected(true)
                .SetPaging(false)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSearchUrl(urlHelper.Action("GetPreProductMaterial", "PreProduct",
                    new {Area = "ProductionPlanning", preProductId = parrentId}))
                .SetDefaultSorting("F02_ThrawSeqNo", SortOrder.Asc)
                .SetFields(
                    new Field("F02_MaterialCode")
                        .SetTitle(" ")
                        .SetWidth(50)
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F02_MaterialCode")
                        .SetTitle("Material Code")
                        .SetWidth(120)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("F01_MaterialDsp")
                        .SetTitle("Material Name")
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("F02_LayinPriority")
                        .SetTitle("C_Pri")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumber")
                        .SetSorting(false),
                    new Field("F02_ThrawSeqNo")
                        .SetTitle("C_Seq")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumber")
                        .SetSorting(false),
                    new Field("F02_PotSeqNo")
                        .SetTitle("P_Seq")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumber")
                        .SetSorting(false),
                    new Field("F02_MsrSeqNo")
                        .SetTitle("W_Seq")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumber")
                        .SetSorting(false),
                    new Field("LayinAmount")
                        .SetTitle("3F_Qty")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetSorting(false),
                    new Field("F02_4FLayinAmount")
                        .SetTitle("4F_Qty")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetSorting(false),
                    new Field("Method")
                        .SetTitle("Method")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("Additive")
                        .SetTitle("Additive")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("Crushing1")
                        .SetTitle("Crushing 1")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("Crushing2")
                        .SetTitle("Crushing 2")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("F02_LoadPosition")
                        .SetTitle("Load Position")
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                );
        }

        #endregion
    }
}