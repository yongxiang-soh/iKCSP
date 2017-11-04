using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces.ProductionPlanning;
using KCSG.Domain.Interfaces.ProductManagement;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.ProductManagement.ViewModels.OutOfPlanProduct;
using KCSG.Web.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Resources;

namespace KCSG.Web.Areas.ProductManagement.Controllers
{
    [MvcAuthorize("TCPR141F")]
    public class OutOfPlanProductController : KCSG.Web.Controllers.BaseController
    {
        #region Domain Declaration

        private readonly IOutOfPlanProductDomain _outOfPlanProductDomain;
        private readonly IProductDomain _productDomain;
        

        #endregion

        #region Constructor

        public OutOfPlanProductController(IOutOfPlanProductDomain outOfPlanProductDomain, IProductDomain productDomain)
        {
            this._outOfPlanProductDomain = outOfPlanProductDomain;
            this._productDomain = productDomain;
        }

        #endregion

        #region Methods

        // GET: ProductManagement/OutOfPlanOfProduct
        public ActionResult Index()
        {
            ViewBag.ScreenId = Constants.PictureNo.TCPR141F;
            var model = new OutOfPlanProductViewModel
            {
                GridOutOfPlanProduct = GenerateGridOutOfPlanProduct()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SearchByCode(string productCode, GridSettings gridSettings)
        {
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F58_ProductCode";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            var result = _outOfPlanProductDomain.FindOutOfPlanProduct(productCode, gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }

        private Grid GenerateGridOutOfPlanProduct()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);
            return new Grid("OutOfPlanProductGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSorting(true)
                .SetSelected(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetDefaultSorting("F58_ProductCode", SortOrder.Asc)
                .SetSearchUrl(urlHelper.Action("SearchByCode", "OutOfPlanProduct",
                    new {Area = "ProductManagement"}))
                .SetFields(
                    new Field("F58_ProductCode")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F58_ProductCode")
                        .SetTitle("Product Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("ProductName")
                        .SetTitle("Product Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100)
                    .SetSorting(false),
                    new Field("F58_PrePdtLotNo")
                        .SetTitle("PrePdt LotNo")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("F58_ProductLotNo")
                        .SetTitle("Lot No.")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("F58_TbtCmdEndPackAmt")
                        .SetTitle("Pack Qty")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("F58_TbtCmdEndFrtAmt")
                        .SetTitle("Fraction")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumberFormat"),
                    new Field("F58_TbtEndDate")
                        .SetTitle("Tabletising End Date")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayDateFormat")
                );
        }

        public ActionResult Edit(string productCode,string prePdtLotNo)
        {
            var model = new OutOfPlanProductViewModel { IsCreate = true, F58_TbtEndDateString = DateTime.Now.ToString("dd/MM/yyyy") };
            if (!string.IsNullOrEmpty(productCode))
            {
                var entity = _outOfPlanProductDomain.GetByProductsCode(productCode, prePdtLotNo);
                if (entity != null)
                {
                    model = Mapper.Map<OutOfPlanProductViewModel>(entity);
                    model.IsCreate = false;
                    //string datetime = model.TableEnDate.ToString("dd/MM/yyyy");
                    //model.TableEnDate = Convert.ToDateTime(datetime);
                }
            }
            return PartialView("OutOfPlanProduct/_PartialViewEditOutOfPlanProduct", model);
        }

        [HttpPost]
        [ValidateInput(true)]
        public JsonResult Edit(OutOfPlanProductViewModel model)
        {
           // var products = _productDomain.GetByCode(model.ProductCode);
            //if (model.Fraction > products.F09_PackingUnit)
            //    return Json(new { Success = false, Message = ProductManagementResources.MSG15 },
            //               JsonRequestBehavior.AllowGet);

           // if (ModelState.IsValid)
                try
                {
                    var item = Mapper.Map<OutOfPlanProductItem>(model);
                    var isSuccess = _outOfPlanProductDomain.CreateOrUpdate(item,model.IsCreate);
                    if (!isSuccess.IsSuccess)
                        return Json(new { Success = false, Message = isSuccess.ErrorMessages },
                            JsonRequestBehavior.AllowGet);
                    return
                        Json(
                            new { Success = true, Message = model.IsCreate ? MessageResource.MSG6 : MessageResource.MSG9 },
                            JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { Success = false, ex.Message }, JsonRequestBehavior.AllowGet);
                }
            //return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(string productCode,string prepdtlotno)
        {
            try
            {
                var result = _outOfPlanProductDomain.Delete(productCode, prepdtlotno);
                if (result)
                    return Json(new { Success = true, Message = MessageResource.MSG10 },
                        JsonRequestBehavior.AllowGet);
                return Json(new { Success = false, Message = MaterialResource.MSG1 },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        [HttpOptions]
        public ActionResult CheckDuplicate(string productcode, string prePdtLotNo, bool isCreate)
        {
            if (!isCreate) { return Json(true, JsonRequestBehavior.AllowGet); }
            var result = !_outOfPlanProductDomain.CheckUnique(productcode, prePdtLotNo);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}