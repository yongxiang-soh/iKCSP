using System;
using System.Web.Mvc;
using AutoMapper;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.MaterialManagement.ViewModels.StorageOfSupplementaryMaterial;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.MaterialManagement.Controllers
{
    [MvcAuthorize("TCRM061F")]
    public class StorageOfSupplementaryMaterialController : KCSG.Web.Controllers.BaseController
    {
        #region Domain Declaration

        private readonly IStorageOfSupplementaryMaterialDomain _storageOfSupplementaryMaterialDomain;

        #endregion

        #region Controller Constructor

        public StorageOfSupplementaryMaterialController(
            IStorageOfSupplementaryMaterialDomain storageOfSupplementaryMaterialDomain)
        {
            _storageOfSupplementaryMaterialDomain = storageOfSupplementaryMaterialDomain;
        }

        #endregion

        //
        // GET: /MaterialManagement/StorageOfSupplementaryMaterial/
        public ActionResult Index()
        {
            var model = new StorageOfSupplementMaterialSearchViewModel
            {
                GridSupplementaryMaterial = GenerateGrid()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SearchByName(StorageOfSupplementMaterialSearchViewModel model, GridSettings gridSettings)
        {
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F15_SubMaterialCode";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            var result = _storageOfSupplementaryMaterialDomain.SearchCriteria(model.SubMaterialCode, gridSettings);
            if (!result.IsSuccess)
                return Json(null);
            return Json(result.Data);
        }

        #region Private Methods

        private Grid GenerateGrid()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("MaterialGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSearchUrl(urlHelper.Action("SearchByName", "StorageOfSupplementaryMaterial",
                    new {Area = "MaterialManagement"}))
                .SetDefaultSorting("F15_SubMaterialCode", SortOrder.Asc)
                .SetFields(
                    new Field("F15_SubMaterialCode")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F15_SubMaterialCode")
                        .SetTitle("Sup.Mat.Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("F15_MaterialDsp")
                        .SetTitle("Sup.Material Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("F15_Unit")
                        .SetTitle("Unit")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("F15_PackingUnit")
                        .SetTitle("Pack Unit")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("Quantity")
                        .SetTitle("Quantity")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("Comment")
                        .SetTitle("Comment")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                );
        }

        #endregion

        public ActionResult Edit(string id, bool isStore)
        {
            var model = new StorageOfSupplementaryMaterialViewModel();
            if (!string.IsNullOrEmpty(id))
            {
                var entity = _storageOfSupplementaryMaterialDomain.GetById(id);
                if (entity != null)
                {
                    model = Mapper.Map<StorageOfSupplementaryMaterialViewModel>(entity);
                    model.IsStore = isStore;
                }
            }
            return PartialView("StorageOfSupplementaryMaterial/_PartialViewEditStorageOfSupplementaryMaterial", model);
        }

        [HttpPost]
        [ValidateInput(true)]
        public JsonResult Edit(StorageOfSupplementaryMaterialViewModel model)
        {
            if (ModelState.IsValid)
                try
                {
                    var item = Mapper.Map<StorageOfSupplementaryMaterialItem>(model);
                    var isSuccess = _storageOfSupplementaryMaterialDomain.CreateOrUpdate(item);
                    if (!isSuccess.IsSuccess)
                        return Json(new {Success = false, Message = isSuccess.ErrorMessages},
                            JsonRequestBehavior.AllowGet);
                    return Json(new {Success = true, Message = MessageResource.MSG6},
                        JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new {Success = false, ex.Message}, JsonRequestBehavior.AllowGet);
                }
            return Json(new {Success = false}, JsonRequestBehavior.AllowGet);
        }
    }
}