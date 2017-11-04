using System;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.MaterialManagement.ViewModels.StorageOfSupplierPallet;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.MaterialManagement.Controllers
{
    [MvcAuthorize("TCRM121F")]
    public class StorageOfSupplierPalletController : KCSG.Web.Controllers.BaseController
    {
        #region Constructor

        public StorageOfSupplierPalletController(IStorageOfSupplierPalletDomain storageOfSupplierPalletDomain,
            IIdentityService identityDomain)
        {
            _storageOfSupplierPalletDomain = storageOfSupplierPalletDomain;
            _identityDomain = identityDomain;
        }

        #endregion

        //
        // GET: /MaterialManagement/StorageOfSupplierPallet/
        public ActionResult Index()
        {
            ViewBag.ScreenId = Constants.PictureNo.TCRM121F;
            var model = new StorageOfSupplierPalletSearchViewModel
            {
                Grid = GenerateGrid()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SearchByName(StorageOfSupplierPalletSearchViewModel model, GridSettings gridSettings)
        {
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F31_ShelfRow";
                gridSettings.SortOrder = SortOrder.Asc;
            }           
                var result = _storageOfSupplierPalletDomain.SearchCriteria(model.SupplierCode, model.MaxPallet, gridSettings);
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
                .SetWidth("auto")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSelected(true)
                .SetSearchUrl(urlHelper.Action("SearchByName", "StorageOfSupplierPallet",
                    new {Area = "MaterialManagement"}))
                .SetDefaultSorting("F31_ShelfRow", SortOrder.Asc)
                .SetFields(
                    new Field("ShelfNo")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("ShelfNo")
                        .SetTitle("Shelf No.")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("StockedPallet")
                        .SetTitle("Stocked Pallet")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100)
                );
        }

        #endregion

        public ActionResult Edit(string shelfRow, string shelfBay, string shelfLevel)
        {
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);

            var checkedDevice = _storageOfSupplierPalletDomain.CheckDeviceStatus();
            if (!checkedDevice)
                return Json(new {Success = false, Message = MaterialResource.MSG16}, JsonRequestBehavior.AllowGet); 
            var checkedConveyor = _storageOfSupplierPalletDomain.CheckConveyorStatus(terminalNo);
            if (!checkedConveyor)
                return Json(new {Success = false, Message = MaterialResource.MSG15}, JsonRequestBehavior.AllowGet);
            var result = _storageOfSupplierPalletDomain.CreateOrUpdate(shelfRow, shelfBay, shelfLevel, terminalNo);
            return Json(new { Success = true, Message = "Successfully" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult StorageOfSupplierPalletMessageC1Reply()
        {
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
            var proceededRecords = _storageOfSupplierPalletDomain.StorageOfSupplierPalletMessageC1Reply(terminalNo);
            return Json(proceededRecords);
        }

        [HttpPost]
        public JsonResult DetailStorage(string shelfNo,string supplierCode,int stackedPallet,int incrementOfPallet)
        {
            try
            {
                var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);

                var checkedDevice = _storageOfSupplierPalletDomain.CheckDeviceStatus();
                if (!checkedDevice)
                    return Json(new { Success = false, Message = MaterialResource.MSG16 }, JsonRequestBehavior.AllowGet);
                var checkedConveyor = _storageOfSupplierPalletDomain.CheckConveyorStatus(terminalNo);
                if (!checkedConveyor)
                    return Json(new { Success = false, Message = MaterialResource.MSG15 }, JsonRequestBehavior.AllowGet);
                
                _storageOfSupplierPalletDomain.DetailStorage(shelfNo, supplierCode, stackedPallet, incrementOfPallet, terminalNo);
                return Json(new {Success=true});
            }
            catch (Exception e)
            {
                return Json(new {Success = false, Message = e.Message});
            }
        }

        [HttpPost]
        public ActionResult DeatailStorageOfSupplierPalletMessageC1Reply()
        {
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
            var proceededRecords = _storageOfSupplierPalletDomain.DetailStorageOfSupplierPalletMessageC1Reply(terminalNo);
            return Json(proceededRecords);
        }

        #region Domain Declaration

        private readonly IStorageOfSupplierPalletDomain _storageOfSupplierPalletDomain;
        private readonly IIdentityService _identityDomain;

        #endregion
    }
}