using System;
using System.Web.Mvc;
using AutoMapper;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.ProductionPlanning;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.Material;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.ProductionPlanning.Controllers
{
    [MvcAuthorize("TCPP011F")]
    public class MaterialController : BaseController
    {
        #region Controller Constructor

        public MaterialController(IMaterialDomain materialDomain, ISupplierDomain supplierDomain,
            ICommonSearchDomain commonSearchDomain)
            : base(supplierDomain, materialDomain)
        {
            _materialDomain = materialDomain;
            _supplierDomain = supplierDomain;
            _commonSearchDomain = commonSearchDomain;
        }

        #endregion

        //
        // GET: /Master/Material/

        public ActionResult Index()
        {
            var model = new MaterialSearchViewModel
            {
                Grid = GenerateGrid()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SearchByName(MaterialSearchViewModel model, GridSettings gridSettings)
        {
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F01_MaterialCode";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            var result = _materialDomain.SearchCriteria(model.MaterialCode, gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Edit(string id)
        {
            var model = new MaterialViewModel {IsCreate = true, F01_Unit = "K"};

            if (!string.IsNullOrEmpty(id))
            {
                var entity = _materialDomain.GetById(id.Trim());
                if (entity != null)
                {
                    model = Mapper.Map<MaterialViewModel>(entity);
                    model.IsCreate = false;
                }
            }
            return PartialView("Material/_PartialViewEditMaterial", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Edit(MaterialViewModel model)
        {
           
                try
                {
                    var item = Mapper.Map<MaterialItem>(model);
                    var isSuccess = _materialDomain.CreateOrUpdate(item);
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
           
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                _materialDomain.Delete(id);
                return Json(new {Success = true, Message = MessageResource.MSG10}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {Success = false, ex.Message}, JsonRequestBehavior.AllowGet);
            }
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
                .SetSearchUrl(urlHelper.Action("SearchByName", "Material", new {Area = "ProductionPlanning"}))
                .SetDefaultSorting("F01_MaterialCode", SortOrder.Asc)
                .SetFields(
                    new Field("F01_MaterialCode")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F01_MaterialCode")
                        .SetTitle("Material Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F01_MaterialDsp")
                        .SetTitle("Material Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("Unit")
                        .SetTitle("Unit")
                        .SetWidth(50),
                    new Field("F01_PackingUnit")
                        .SetTitle("Pack Unit")
                        .SetWidth(70)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("Liquid")
                        .SetTitle("Liq").SetSorting(false)
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("Bali")
                        .SetTitle("Bail")
                        .SetSorting(false)
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("Retrieval")
                        .SetTitle("Retr")
                        .SetSorting(false)
                        .SetWidth(80)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("Comms")
                        .SetTitle("Comms")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                );
        }

        #endregion

        public JsonResult GetMaterialCode(string searchText)
        {
            var result = _materialDomain.GetMaterials(searchText);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region Domain Declaration

        private readonly IMaterialDomain _materialDomain;
        private readonly ISupplierDomain _supplierDomain;
        private readonly ICommonSearchDomain _commonSearchDomain;

        #endregion
    }
}