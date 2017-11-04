using System;
using System.Data.Entity.Validation;
using System.Web.Mvc;
using AutoMapper;
using KCSG.Data.DataModel;
using KCSG.Domain.Interfaces.ProductionPlanning;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.PckMtr;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.Product;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.ProductionPlanning.Controllers
{
    [MvcAuthorize("TCPP031F")]
    public class ProductController : KCSG.Web.Controllers.BaseController
    {
        #region Controller Constructor

        public ProductController(IProductDomain productDomain, IPckMtrDomain pckMtrDomain,
            ISubMaterialDomain subMaterialDomain)
        {
            _productDomain = productDomain;
            _pckMtrDomain = pckMtrDomain;
            _subMaterialDomain = subMaterialDomain;
        }

        #endregion

        //
        // GET: /Master/Product/
        public ActionResult Index()
        {
            var model = new ProductSearchViewModel
            {
                Grid = GenerateGrid()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SearchByName(ProductSearchViewModel model, GridSettings gridSettings)
        {
            var result = _productDomain.SearchCriteria(model.ProductCode, gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);

            var response = Mapper.Map<GridResponse<TM09_Product>>(result.Data);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(string id)
        {
            var model = new ProductViewModel
            {
                IsCreate = true,
                GridSupMaterial = GridSupMaterial()
                // F09_TabletSize = "0",
                // F09_TabletSize2 = "0.00",
            };
            if (!string.IsNullOrEmpty(id))
            {
                var entity = _productDomain.GetById(id.Trim());
                if (entity != null)
                {
                    model = Mapper.Map<ProductViewModel>(entity);
                    model.IsCreate = false;
                    model.GridSupMaterial = GridSupMaterial();
                }
            }
            return PartialView("Product/_PartialViewEditProduct", model);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Edit(ProductViewModel model)
        {
            try
            {
                var item = Mapper.Map<ProductItem>(model);
                var isSuccess = _productDomain.CreateOrUpdate(item);
                if (!isSuccess.IsSuccess)
                    return Json(new {Success = false, Message = isSuccess.ErrorMessages}, JsonRequestBehavior.AllowGet);
                return Json(
                    new {Success = true, Message = model.IsCreate ? MessageResource.MSG6 : MessageResource.MSG9},
                    JsonRequestBehavior.AllowGet);
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                }
                throw;
            }
            catch (Exception ex)
            {
                return Json(new {Success = false, ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Delete(string lstCode)
        {
            try
            {
                _productDomain.Delete(lstCode);
                return Json(new {Success = true, Message = MessageResource.MSG10}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {Success = false, ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        [HttpOptions]
        public JsonResult CheckExistCode(string F09_ProductCode, bool isCreate)
        {
            if (!isCreate)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            var result = !_productDomain.CheckUnique(F09_ProductCode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpOptions]
        public JsonResult CheckPackingUnit(double? f09_packingunit, string f09_Unit)
        {
            if (f09_Unit.Equals("K") && f09_packingunit.HasValue && f09_packingunit.Value <= 0)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        #region Domain Declaration

        private readonly IProductDomain _productDomain;
        private readonly IPckMtrDomain _pckMtrDomain;
        private readonly ISubMaterialDomain _subMaterialDomain;

        #endregion

        #region Sub Material Event

        public ActionResult CreateSub(string productCode, string productName, string subMaterialCode)
        {
            var model = new PckMtrViewModel {IsCreate = true, F11_ProductCode = productCode, ProductName = productName};
            if (!string.IsNullOrEmpty(subMaterialCode))
            {
                var pkmtr = _pckMtrDomain.GetPckMtr(productCode, subMaterialCode);
                model = Mapper.Map<PckMtrViewModel>(pkmtr);
                model.SubMaterialName = _subMaterialDomain.GetById(subMaterialCode).F15_MaterialDsp;
                model.IsCreate = false;
            }
            return PartialView("Product/_PartialViewAddSup", model);
        }

        [HttpPost]
        public ActionResult CreateSub(PckMtrViewModel model)
        {
            try
            {
                var pckMtr = Mapper.Map<PckMtrItem>(model);
                _pckMtrDomain.CreateOrUpdate(pckMtr);
                return Json(
                    new {Success = true, Message = model.IsCreate ? MessageResource.MSG6 : MessageResource.MSG9},
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {Success = false, ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteSubMaterial(string productCode, string subMaterialCode)
        {
            try
            {
                var result = _pckMtrDomain.Delete(productCode, subMaterialCode);
                if (result)
                {
                    return Json(new {Success = true, Message = MessageResource.MSG10},
                        JsonRequestBehavior.AllowGet);
                }
                return Json(new {Success = false},
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {Success = false, ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }
      
        public JsonResult GetSubMaterialName(string subMaterialCode)
        {
            var result = _subMaterialDomain.GetById(subMaterialCode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchSupMaterial(string productCode, GridSettings gridSettings)

        {
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.PageSize = 10;
                gridSettings.SortField = "F11_SubMaterialCode";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            var result = _pckMtrDomain.SearchCriteria(productCode, gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);

            var response = Mapper.Map<GridResponse<TM11_PckMtr>>(result.Data);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpOptions]
        public JsonResult CheckExistSubCode(string f11_SubMaterialCode, string f11_ProductCode, bool isCreate)
        {
            if (!isCreate)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            var result = !_pckMtrDomain.CheckUnique(f11_SubMaterialCode, f11_ProductCode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Private Methods

        private Grid GenerateGrid()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("ProductGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSearchUrl(urlHelper.Action("SearchByName", "Product", new {Area = "ProductionPlanning"}))
                .SetDefaultSorting("F09_ProductCode", SortOrder.Asc)
                .SetFields(
                    new Field("F09_ProductCode")
                        .SetWidth(20)
                        .SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F09_ProductCode")
                        .SetTitle("Product Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("F09_PreProductCode")
                        .SetTitle("Pre-Product  Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("F09_ProductDesp")
                        .SetTitle("Product Name")
                        .SetWidth(100),
                    new Field("F09_TabletSize")
                        .SetTitle("Diameter")
                        .SetWidth(50)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("F09_TabletSize2")
                        .SetTitle("Weight")
                        .SetWidth(50)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("F09_TabletType")
                        .SetTitle("Type")
                        .SetWidth(50)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                );
        }

        private Grid GridSupMaterial()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("SupMaterialGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(10)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSearchUrl(urlHelper.Action("SearchSupMaterial", "Product", new {Area = "ProductionPlanning"}))
                .SetDefaultSorting("F11_ProductCode", SortOrder.Asc)
                .SetFields(
                    new Field("F11_SubMaterialCode")
                        .SetTitle(" ")
                        .SetWidth(20)
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F11_SubMaterialCode")
                        .SetTitle("Sup. Material Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("F11_Amount")
                        .SetTitle("Quantity")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("F11_Unit")
                        .SetTitle("Unit")
                        .SetWidth(100)
                );
        }

        #endregion
    }
}