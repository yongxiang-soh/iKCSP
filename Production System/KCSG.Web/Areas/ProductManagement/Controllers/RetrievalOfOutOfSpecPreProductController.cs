using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Helpers;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Core.Resources;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.ProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.ProductManagement.ViewModels.RetrievalOfOutOfSpecPreProduct;
using KCSG.Web.Attributes;
using KCSG.Web.ViewModels;
using Resources;

namespace KCSG.Web.Areas.ProductManagement.Controllers
{
    [MvcAuthorize("TCPR111F")]
    public class RetrievalOfOutOfSpecPreProductController : KCSG.Web.Controllers.BaseController
    {
        #region Properties

        /// <summary>
        /// Domain which handles retrieval of out of spec pre-product.
        /// </summary>
        private readonly IRetrievalOfOutOfSpecPreProductDomain _retrievalOfOutOfSpecPreProductDomain;

        /// <summary>
        /// Service which handles identity.
        /// </summary>
        private readonly IIdentityService _identityService;

        private readonly IRetrieveSupplierPalletDomain _retrieveSupplierPalletDomain;

        private readonly IStorageOfWarehousePalletDomain _storageOfWarehousePalletDomain;
        private readonly ICommonDomain _commonDomain;
        private readonly IConfigurationService _configurationService;
        
        #endregion

        #region Constructors

        /// <summary>
        /// Initiate controller with IoC
        /// </summary>
        /// <param name="retrievalOfOutOfSpecPreProductDomain"></param>
        /// <param name="service"></param>
        public RetrievalOfOutOfSpecPreProductController(IRetrievalOfOutOfSpecPreProductDomain retrievalOfOutOfSpecPreProductDomain,
            IIdentityService service, IRetrieveSupplierPalletDomain retrieveSupplierPalletDomain, IStorageOfWarehousePalletDomain storageOfWarehousePalletDomain,
            ICommonDomain commonDomain, IConfigurationService iConfigurationService)
        {
            _identityService = service;
            _retrievalOfOutOfSpecPreProductDomain = retrievalOfOutOfSpecPreProductDomain;
            _retrieveSupplierPalletDomain = retrieveSupplierPalletDomain;
            _storageOfWarehousePalletDomain = storageOfWarehousePalletDomain;
            _commonDomain = commonDomain;
            _configurationService = iConfigurationService;

        }

        #endregion

        #region Methods

        // GET: ProductManagement/RetrievalOfOutOfSpecPreProduct
        public ActionResult Index()
        {
            var model = new RetrievalOfOutOfSpecPreProductViewModel() { Grid = GenerateGrid() };
            return View(model);
        }

        /// <summary>
        /// Storage pre-product.
        /// </summary>
        /// <param name="shelfNo"></param>
        /// <param name="shelfStatus"></param>
        /// <param name="emptyPallet"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Storage(string shelfNo, string shelfStatus, bool emptyPallet)
        {
            
                // Find terminal name which sent the current request.
                var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);

                var checkConveyorStatus = _storageOfWarehousePalletDomain.CheckedRecordInTM05(terminalNo);
                if (!checkConveyorStatus)
                    return Json(new { Success = false, Message = MaterialResource.MSG15 });

                // Check whether device is valid or not.
                var offlineStatus = Constants.F14_DeviceStatus.Offline.ToString("D");
                var errorStatus = Constants.F14_DeviceStatus.Error.ToString("D");
                var usepermitClass = Constants.F14_UsePermitClass.Prohibited.ToString("D");
                var device = await _commonDomain.FindDeviceAvailabilityAsync(_configurationService.ProductDeviceCode);
                if (device == null)
                {
                    return Json(new { Success = false, Message = ProductManagementResources.MSG9 });
                }
                if (device.F14_DeviceStatus.Equals(offlineStatus) || device.F14_DeviceStatus.Equals(errorStatus) || device.F14_UsePermitClass.Equals(usepermitClass))
                {
                    return Json(new { Success = false, Message = ProductManagementResources.MSG9 });
                }

             
                _retrievalOfOutOfSpecPreProductDomain.StorageOrRetrieval(shelfNo, terminalNo, shelfStatus, emptyPallet, true);
                return Json(new { Success = true});
            
        }

        [HttpPost]
        public ActionResult Retrieval(string shelfNo, string shelfStatus, bool emptyPallet)
        {
           
                // Find terminal name which sent the current request.
                var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
              

                var checkConveyorStatus = _storageOfWarehousePalletDomain.CheckedRecordInTM05(terminalNo);
                if (!checkConveyorStatus)
                    return Json(new { Success = false, Message = MaterialResource.MSG15 });

                // Check whether device is valid or not.
              
                var device = _retrievalOfOutOfSpecPreProductDomain.CheckDeviceStatus(_configurationService.ProductDeviceCode);
               if(!device){
                    return Json(new { Success = false, Message = ProductManagementResources.MSG9 });
                }

                _retrievalOfOutOfSpecPreProductDomain.StorageOrRetrieval(shelfNo, terminalNo, shelfStatus, emptyPallet, false);
                return Json(new { Success = true});
            
        }

        /// <summary>
        /// Check whether terminal can do storage or not.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult IsStoragable()
        {
            // Find terminal which sends the current request.
            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);


            var checkConveyorStatus = _storageOfWarehousePalletDomain.CheckedRecordInTM05(terminalNo);
            if (!checkConveyorStatus)
                return Json(new {Success = false, Message = MaterialResource.MSG15});

            // Check whether device is valid or not.
            var isValidDevice = _retrieveSupplierPalletDomain.IsValidDevice();
            if (!isValidDevice)
            {
                // The message can be confused with the validation error messages because of status 400.
                // Use this custom header to clarify it.
                //Response.Headers.Add("x-process-error", MessageResource.MSG16);

                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return Json(new {Success = false, Message = MaterialResource.MSG16});
            }
            // TODO: _communicationService.FindConveyorCodeValidity(terminalNo);
            return Json(new {Success = true});

        }

        /// <summary>
        /// Respond messages sent from C3.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RespondMessageC3(RetrievalOutOfSpectPreProductC3ViewModel item)
        {
            if (item == null)
            {
                item = new RetrievalOutOfSpectPreProductC3ViewModel();
                TryValidateModel(item);
            }

            // Parameter is not valid.
            if (!ModelState.IsValid)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            var items = _retrievalOfOutOfSpecPreProductDomain.ResponeMessageC3(item.ShelfNo, terminalNo, item.ShelfStatus);

            return Json(items);
        }

        [HttpPost]
        public async Task<ActionResult> CheckStatus()
        {
            // Find terminal no from request.
            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);

            // Check whether this terminal has a valid conveyor or not.
            //•	Retrieve [f05_strrtrsts] in TM05_CONVEYOR, where [f05_terminalno] = Terminal No. which is retrieved from section “server” in “toshiba.ini” configurable file.
            var conveyor = await _commonDomain.FindConveyorCodeAsync(terminalNo);

            // If retrieval is failed or retrieved status is “Error” (or 9), system shows the message MSG 8, stop the use case.
            if (conveyor == null)
                return Json(new { Success = false, Message = ProductManagementResources.MSG8 });

            // •	Retrieve [f14_devicestatus] and [f14_usepermitclass] from TM14_DEVICE, where [f14_devicecode] = Product Device Code under “Device” section of “toshiba.ini” configurable file.
            //If retrieval is failed OR retrieved status is “Error” (or 2) OR “Offline” (or 1) OR retrieved permit is “Prohibited” (or 1), system shows the message MSG 9, stop the use case


            var offlineStatus = Constants.F14_DeviceStatus.Offline.ToString("D");
            var errorStatus = Constants.F14_DeviceStatus.Error.ToString("D");
            var usepermitClass = Constants.F14_UsePermitClass.Prohibited.ToString("D");
            var device = await _commonDomain.FindDeviceAvailabilityAsync(_configurationService.ProductDeviceCode);
            if (device == null)
            {
                return Json(new { Success = false, Message = ProductManagementResources.MSG9 });
            }
            if (device.F14_DeviceStatus.Equals(offlineStatus) || device.F14_DeviceStatus.Equals(errorStatus) || device.F14_UsePermitClass.Equals(usepermitClass))
            {
                return Json(new { Success = false, Message = ProductManagementResources.MSG9 });
            }
            return Json(new { Success = true });


        }
            
        #endregion

        #region Private Methods


        public ActionResult Search(string status, GridSettings gridSettings)
        {
            try
            {

                if (string.IsNullOrEmpty(gridSettings.SortField))
                {
                    gridSettings.SortField = "F51_ShelfStatus";
                    gridSettings.SortOrder = SortOrder.Asc;
                }
                string[] lstStatus = status == "0" ? new[] { status } : new[] { "1", "3" };
                var result = _retrievalOfOutOfSpecPreProductDomain.SearchAsync(lstStatus, gridSettings);
                return Json(result.Data);
            }
            catch (Exception)
            {
                return Json(null);
            }

        }
        private Grid GenerateGrid()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("PalletGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSearchUrl(urlHelper.Action("Search", "RetrievalOfOutOfSpecPreProduct",
                    new { Area = "ProductManagement", status = "0" }))
                .SetDefaultSorting("F51_ShelfStatus", SortOrder.Asc)
                .SetFields(
                    new Field("ShelfNo")
                        .SetWidth(5).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("ShelfNo")
                        .SetWidth(80)
                        .SetTitle("Shelf No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("Status")
                        .SetTitle("Shelf Status")
                        .SetItemTemplate("gridHelper.findProductPalletShelfStatus")
                        .SetWidth(100));
        }
        #endregion
    }
}