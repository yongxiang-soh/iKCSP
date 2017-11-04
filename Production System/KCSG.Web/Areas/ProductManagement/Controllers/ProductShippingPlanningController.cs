using System;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Drawing.Charts;
using KCSG.Domain.Interfaces.ProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.ProductManagement.ViewModels.ProductShippingPlanning;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.ProductManagement.Controllers
{
    [MvcAuthorize("TCPR031F")]
    public class ProductShippingPlanningController : KCSG.Web.Controllers.BaseController
    {
        // GET: ProductManagement/ProductShippingPlanning

        #region Controller Constructor

        public ProductShippingPlanningController(IProductShippingPlanningDomain prodShipDomain,
            IIdentityService identityService)
        {
            _prodShippingPlanningDomain = prodShipDomain;
            _identityService = identityService;
        }

        #endregion

        public ActionResult Index()
        {
            ViewBag.ScreenId = "TCPR031F";
            var model = new ProductShippingPlanningSearchViewModel
            {
                Grid = GenerateGrid()
            };
            return View(model);
        }

        //public ActionResult Edit(string prodShipCode, string proCode, string userCode)
        public ActionResult Edit(string prodShipCode, string proCode, string userCode, string productLotNo,
            string reqShippingQty)
        {
            try
            {
                var shippingNo = _prodShippingPlanningDomain.GenShippingNo();
                //var productShelf = _prodShippingPlanningDomain.CheckProductShelfStatus(proCode, productLotNo);
                //var ReqShippingQty = _prodShippingPlanningDomain.CheckReqShippingQty(proCode, Convert.ToDouble(reqShippingQty));

                var model = new ProductShippingPlanningViewModel
                {
                    IsCreate = true,
                    F44_ShipCommandNo = shippingNo
                    //F44_ProductLotNo = productShelf,
                    //F44_ShpRqtAmt = ReqShippingQty
                    //F39_Status = Constants.Status.Yet.ToString(),
                    //F39_KndEptBgnDate = DateTime.Now.ToString("MM/yyyy")
                };
                if (!string.IsNullOrEmpty(prodShipCode))
                {
                    var entity = _prodShippingPlanningDomain.GetById(prodShipCode, proCode, userCode);
                    if (entity != null)
                    {
                        model = Mapper.Map<ProductShippingPlanningViewModel>(entity);
                        //model.F39_Status = Enum.GetName(typeof(Constants.Status), ConvertHelper.ToInteger(model.F39_Status));
                        model.IsCreate = false;
                    }
                }
                return PartialView("_PartialViewEditProductShippingPlan", model);
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ProductShippingPlanningViewModel model)
        {
            try
            {
                var item = Mapper.Map<ProductShippingPlanningItem>(model);
                var resultShippingPlanning = _prodShippingPlanningDomain.CheckReqShippingQty(item.F44_ProductCode,item.F44_ShpRqtAmt);
                model.F44_ShpRqtAmt = resultShippingPlanning.F44_ShpRqtAmt.ToString(CultureInfo.InvariantCulture);

                //var result = _prodShippingPlanningDomain.CheckProductShelfStatus(model.F44_ProductCode,
                //    model.F44_ProductLotNo);
                //if (result == false)
                //{
                //    return Json(new { Success = false, Message = "MSG1" }, JsonRequestBehavior.AllowGet);
                //}

                item.F44_ShpRqtAmt = Convert.ToDouble(model.F44_ShpRqtAmt);
                var isSuccess = _prodShippingPlanningDomain.CreateOrUpdate(item);
                if (!isSuccess.IsSuccess)
                    return Json(new {Success = false, Message = isSuccess.ErrorMessages}, JsonRequestBehavior.AllowGet);
                return Json(
                    new {Success = true, Message = model.IsCreate ? MessageResource.MSG6 : MessageResource.MSG9},
                    JsonRequestBehavior.AllowGet);
                ;
            }
            catch (DbUpdateException e)
            {
                var sb = new StringBuilder();
                sb.AppendLine("DbUpdateException error details - {e?.InnerException?.InnerException?.Message}");

                foreach (var eve in e.Entries)
                    sb.AppendLine("Entity of type {eve.Entity.GetType().Name} in state {eve.State} could not be updated");

                Console.WriteLine(sb.ToString());
                throw;
            }
            catch (Exception ex)
            {
                return Json(new {Success = false, ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Delete(string prodCode)
        {
            try
            {
                //var dateDelete = ConvertHelper.ConvertToDateTimeFull(date);
                var check = _prodShippingPlanningDomain.Delete(prodCode);
                if (check)
                    return Json(new { Success = true, Message = MessageResource.MSG10 },
                        JsonRequestBehavior.AllowGet);
                return Json(new { Success = false, Message = MessageResource.MSG51 }, JsonRequestBehavior.AllowGet);
                //return Json(new { Success = true, Message = Resources.MessageResource.MSG10 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, MessageResource.MSG51 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SearchByShipNo(ProductShippingPlanningSearchViewModel model, GridSettings gridSettings)
        {
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F44_ShipCommandNo";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            var result = _prodShippingPlanningDomain.SearchCriteria(model.ShippingNo, gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);

            var response = Mapper.Map<GridResponse<ProductShippingPlanningItem>>(result.Data);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public bool TranferInterFloor(string proCode, string productLotNo)
        //{
        //    var result = true;
        //    var productShelf = _prodShippingPlanningDomain.CheckProductShelfStatus(proCode, productLotNo);
        //    var model = new ProductShippingPlanningViewModel
        //    {
        //        F44_ProductLotNo = productShelf
        //    };
        //    if (model.F44_ProductLotNo == "")
        //        result = false;
        //    return result;
        //}

        [AllowAnonymous]
        [HttpOptions]
        public JsonResult CheckExistCode(string f44_ProductCode, string f44_ProductLotNo, bool isCreate)
        {
            if (!isCreate || f44_ProductLotNo == "") return Json(true, JsonRequestBehavior.AllowGet);
            var result = _prodShippingPlanningDomain.CheckProductShelfStatus(f44_ProductCode, f44_ProductLotNo);
            //if (result == "")
            //{
            //    return Json(false, JsonRequestBehavior.AllowGet);
            //}
            return Json(result, JsonRequestBehavior.AllowGet);
        }
                
        [AllowAnonymous]
        [HttpOptions]

        public JsonResult CheckReqShippingQty(string f44_ProductCode, double f44_ShpRqtAmt)
        {
            var result = _prodShippingPlanningDomain.CheckReqShippingQty(f44_ProductCode, f44_ShpRqtAmt);            
            bool success = result.Mode.All(char.IsDigit);
            return Json(success, JsonRequestBehavior.AllowGet);
        }


        #region Private Methods

        private Grid GenerateGrid()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("Grid")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSearchUrl(urlHelper.Action("SearchByShipNo", "ProductShippingPlanning",
                    new {Area = "ProductManagement"}))
                .SetDefaultSorting("F44_ShipCommandNo", SortOrder.Asc)
                .SetFields(
                    new Field("F44_ShipCommandNo")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F44_ShipCommandNo")
                        .SetWidth(80).SetTitle("Shipping No.")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("F44_ProductCode")
                        .SetWidth(80)
                        .SetTitle("Product Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("F09_ProductDesp")
                        .SetTitle("Product Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("F44_ProductLotNo")
                        .SetTitle("Product LotNo")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F44_ShpRqtAmt")
                        .SetTitle("Req Shipping Qty")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetWidth(100),
                    new Field("F44_DeliveryDate")
                        .SetTitle("Delivery Date")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayDateFormat"),
                    new Field("F44_EndUserCode")
                        .SetTitle("End User Code")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F10_EndUserName")
                        .SetTitle("End User Name")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                );
        }

        #endregion

        #region Domain Declaration

        private readonly IProductShippingPlanningDomain _prodShippingPlanningDomain;

        private readonly IIdentityService _identityService;

        #endregion
    }
}