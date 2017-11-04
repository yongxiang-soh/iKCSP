using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.ProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.ProductManagement.ViewModels;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.ProductManagement.Controllers
{
    [MvcAuthorize("TCPR011F")]
    public class StorageOfProductController : KCSG.Web.Controllers.BaseController
    {
        #region Properties
        private readonly IStorageOfProductDomain _storageOfProductDomain;
        private readonly IIdentityService _identityService;
        private readonly ICommonDomain _commonDomain;
        private readonly IConfigurationService _configurationService;
        #endregion

        #region Constructor

        public StorageOfProductController(IStorageOfProductDomain storageOfProductDomain, IIdentityService identityService, ICommonDomain commonDomain, IConfigurationService configurationService)
        {
            _storageOfProductDomain = storageOfProductDomain;
            _identityService = identityService;
            _commonDomain = commonDomain;
            _configurationService = configurationService;

        }
        #endregion
        //
        // GET: /ProductManagement/StorageOfProduct/
        public ActionResult Index()
        {
            var model = new StorageOfProductSelectedViewModel()
            {
                GridNormal = GenerateNormalGrid(),
                GridOutOfPlan = GenerateOutOfPlanGrid()
            };

            return View(model);
        }

        public ActionResult Selected(string lstValue, int status)
        {
            var details = _storageOfProductDomain.GetSelected(lstValue, status);
            
            return Json(new
            {
                results = details
            });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SearchByName(StorageOfProductSelectedViewModel model, GridSettings gridSettings)
        {
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F56_KndCmdNo";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            var result = _storageOfProductDomain.SearchCriteria(gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ShowData(StorageOfProductSelectedViewModel model, GridSettings gridSettings)
        {

            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F58_ProductCode";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            var result = _storageOfProductDomain.ShowData(gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);
            return Json(result.Data, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult CheckStatus(string palletNo)
        {
            var deviceCode = _configurationService.ProductDeviceCode;
            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            var checkConveyorStatus = _commonDomain.CheckStatusAndNumberRecordOfConveyor(terminalNo);
            if (!checkConveyorStatus)
            {
                return Json(new {Success = false, Message = ProductManagementResources.MSG8,Message2=ProductManagementResources.MSG10});
            }

            var checkedDeviceStatus = _commonDomain.CheckStatusOfDeviceRecordForProductManagement(deviceCode);
            if(!checkedDeviceStatus)
                return Json(new { Success = false, Message = ProductManagementResources.MSG9, Message2 = ProductManagementResources.MSG10 });

            var checkPalletNo = _storageOfProductDomain.CheckedPalletNo(palletNo);
            if (checkPalletNo)
                return Json(new { Success = false, Message = ProductManagementResources.MSG11 });
            var checkedOutSidePreProductStock = _storageOfProductDomain.CheckOutSidePrePdtStk(palletNo);
            if (checkedOutSidePreProductStock)
                return Json(new { Success = false, Message = ProductManagementResources.MSG11 });
            //var totalAmount = _storageOfProductDomain.CheckTotalAmountOfTX40(palletNo);
            return Json(new{Success=true});


        }

        [HttpPost]
        public ActionResult GetTotalAmountFromTx40(string palletNo)
        {
            var totalAmount = _storageOfProductDomain.GetTotalAmountOfTX40(palletNo);
            return Json(new
            {
                totalAmount
            });
        }

        //[HttpPost]
        //public ActionResult CheckOutSidePreProductStock(string palletNo)
        //{
        //    var isChecked = _storageOfProductDomain.CheckOutSidePrePdtStk(palletNo);
        //    if (isChecked)
        //        return Json(new { Success = false, Message = ProductManagementResources.MSG11 });
        //    return Json(new { Success = true });
        //}
        [HttpPost]
        public ActionResult StoreProduct(StorageOfProductSelectedViewModel model)
        {
            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            var item = Mapper.Map<StoreProductItem>(model);
            var result = _storageOfProductDomain.UpdaDateCreateAndDelete(item,terminalNo);
            return Json(new { Success = true, Message = result.ErrorMessages});
        }

        /// <summary>
        /// Proceed data after messages having been sent from C3.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ProcessThirdCommunicationData(StoreProductItem item)
        {
            // Parameters are not valid.
            if (item == null)
            {
                item = new StoreProductItem();
                TryValidateModel(item);
            }    

            if (!ModelState.IsValid)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Find terminal no.
            var teminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            var result = _storageOfProductDomain.ProcessDataReceiveMessageForC3(teminalNo, item);

              return Json(result);
        }

        //[HttpPost]
        //public ActionResult StoreProduct(StorageOfProductSelectedViewModel model,string status, string palletNo, string kndCmdNo, string productCode, string preProductLotNo, int packQty, double fraction, double packUnit)
        //{
        //    var result = _storageOfProductDomain.UpdaDateCreateAndDelete(status, palletNo, kndCmdNo, productCode, preProductLotNo, packQty, fraction, packUnit);
        //    if (result.IsSuccess)
        //        return Json(new { Success = true });
        //    return Json(new{Success=false});
        //}

        private Grid GenerateNormalGrid()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);
            return new Grid("NormalGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSorting(true)
                .SetSelected(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSearchUrl(urlHelper.Action("SearchByName", "StorageOfProduct",
                    new { Area = "ProductManagement" }))
                .SetDefaultSorting("F56_KndCmdNo", SortOrder.Asc)
                .SetFields(
                    new Field("F56_KndCmdNo")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F56_KndCmdNo")
                        .SetTitle("Cmd No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("F56_ProductCode")
                        .SetTitle("Product Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("ProductName")
                        .SetTitle("Product Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("F56_PrePdtLotNo")
                        .SetTitle("Prepdt LotNo")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("F56_ProductLotNo")
                        .SetTitle("Lot No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("PackingUnit")
                        .SetTitle("Pack Unit")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("Total")
                        .SetTitle("Total")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumberFormat"),
                    new Field("OutofSpecFlag")
                        .SetTitle("Out of Spec Flag")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.initOutOfSpecStatus")
                );
        }
        private Grid GenerateOutOfPlanGrid()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);
            return new Grid("OutOfPlanGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSorting(true)
                .SetSelected(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSearchUrl(urlHelper.Action("ShowData", "StorageOfProduct",
                    new { Area = "ProductManagement" }))
                .SetDefaultSorting("F58_ProductCode", SortOrder.Asc)
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
                        .SetWidth(100),
                    new Field("F58_PrePdtLotNo")
                        .SetTitle("Prepdt LotNo")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("F58_ProductLotNo")
                        .SetTitle("Lot No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("PackingUnit")
                        .SetTitle("Pack Unit")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("Total")
                        .SetTitle("Total")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumberFormat"),
                    new Field("OutofSpecFlag")
                        .SetTitle("OutofSpec Flag")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                );
        }
    }
}