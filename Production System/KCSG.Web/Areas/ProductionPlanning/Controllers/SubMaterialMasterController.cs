using System;
using System.Web.Mvc;
using AutoMapper;
using KCSG.Domain.Interfaces.ProductionPlanning;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.SubMaterialMasters;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.ProductionPlanning.Controllers
{
    [MvcAuthorize("TCPP034F")]
    public class SubMaterialMasterController : KCSG.Web.Controllers.BaseController
    {
        // GET: Master/SubMaterialMaster
        private readonly ISubMaterialDomain _subMaterialDomain;

        public SubMaterialMasterController(ISubMaterialDomain iSubMaterialDomain)
        {
            _subMaterialDomain = iSubMaterialDomain;
        }

        public ActionResult Index()
        {
            var model = new SubMaterialSearchViewModel
            {
                Grid = GenerateGrid()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Edit(SubMaterialViewModel model)
        {
            try
            {
                var item = Mapper.Map<SubMaterialItem>(model);
                var isSuccess = _subMaterialDomain.CreateOrUpdate(item);
                if (!isSuccess.IsSuccess)
                    return Json(new {Success = false, Message = isSuccess.ErrorMessages},
                        JsonRequestBehavior.AllowGet);
                return
                    Json(
                        new
                        {
                            Success = true,
                            Message = model.IsCreate ? MessageResource.MSG6 : MessageResource.MSG9
                        },
                        JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {Success = false, ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                _subMaterialDomain.Delete(id);
                return Json(new {Success = true, Message = MessageResource.MSG10}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {Success = false, ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Edit(string id)
        {
            var model = new SubMaterialViewModel {IsCreate = true};
            if (!string.IsNullOrEmpty(id))
            {
                var entity = _subMaterialDomain.GetById(id.Trim());
                if (entity != null)
                {
                    model = Mapper.Map<SubMaterialViewModel>(entity);
                    model.IsCreate = false;
                }
            }
            return PartialView("SubMaterialMaster/_PartialSubMaterial", model);
        }

        public ActionResult SearchByCode(SubMaterialSearchViewModel model, GridSettings gridSettings)
        {
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F15_SubMaterialCode";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            var result = _subMaterialDomain.SearchCriteria(model.MaterialCode, gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }

        #region Private Methods

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
                .SetAutoload(true)
                .SetSearchUrl(urlHelper.Action("SearchByCode", "SubMaterialMaster", new {Area = "ProductionPlanning"}))
                .SetDefaultSorting("F15_SubMaterialCode", SortOrder.Asc)
                .SetFields(
                    new Field("F15_SubMaterialCode")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F15_SubMaterialCode")
                        .SetTitle("Sup. Material Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("F15_MaterialDsp")
                        .SetTitle("Sup. Material Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("F15_Unit")
                        .SetTitle("Unit")
                        .SetWidth(100),
                    new Field("F15_PackingUnit")
                        .SetTitle("Pack Unit")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("Bail")
                        .SetTitle("Bail")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                );
        }

        #endregion

        [AllowAnonymous]
        [HttpOptions]
        public JsonResult CheckExistCode(string subMaterialCode, bool isCreate)
        {
            if (!isCreate) return Json(true, JsonRequestBehavior.AllowGet);
            var result = !_subMaterialDomain.CheckUnique(subMaterialCode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}