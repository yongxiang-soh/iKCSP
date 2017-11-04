using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.ProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Web.Areas.ProductManagement.ViewModels.StorageOfEmptyProduct;
using KCSG.Web.Attributes;
using Microsoft.AspNet.SignalR.Messaging;
using Resources;

namespace KCSG.Web.Areas.ProductManagement.Controllers
{
    [MvcAuthorize("TCPR121F")]
    public class StorageOfEmptyProductPalletController : KCSG.Web.Controllers.BaseController
    {
        #region Domain Declaration

        private readonly IStorageOfEmptyProductPalletDomain _storageOfEmptyProductPalletDomain;
        private readonly IIdentityService _identityServices;
        private readonly ICommonDomain _commonDomain;
        private readonly IConfigurationService _configurationService;
        
        #endregion
        #region Constructor

        public StorageOfEmptyProductPalletController(
            IStorageOfEmptyProductPalletDomain storageOfEmptyProductPalletDomain, IIdentityService identityServices, ICommonDomain commonDomain, IConfigurationService configurationService)
        {
            _storageOfEmptyProductPalletDomain = storageOfEmptyProductPalletDomain;
            _identityServices = identityServices;
            _commonDomain = commonDomain;
            _configurationService = configurationService;
        }

        #endregion


        #region Method
        //
        // GET: /ProductManagement/StorageOfEmptyProductPallet/
        public ActionResult Index()
        {
            var model =new  StorageOfEmptyProductViewModel();
            model.PalletNumber = 1;
            return View(model);
        }

        [HttpPost]
        public ActionResult CheckMaxiumNumberOfIpAddress(int palletNumber)
        {
            var result = _storageOfEmptyProductPalletDomain.CheckMaxiumNumberOfIpAddress();
            if (result == null)
            {
                return Json(new{Success=false,Message=ProductManagementResources.MSG38});
            }
            var isChecked = _storageOfEmptyProductPalletDomain.CheckPalletNumber(palletNumber);
            if (!isChecked)
            {
                return Json(new{Success=false,Message=ProductManagementResources.MSG39});

            }
            return Json(new{Success=true});

        }

        [HttpPost]
        public ActionResult StoreTheEmptyPallet(StorageOfEmptyProductViewModel model)
        {

            var deviceCode = _configurationService.ProductDeviceCode;
            var terminalNo = _identityServices.FindTerminalNo(HttpContext.User.Identity);
            var checkedConveyorStatus = _commonDomain.CheckStatusAndNumberRecordOfConveyor(terminalNo);

            if(!checkedConveyorStatus)
                return Json(new{Success=false,Message=ProductManagementResources.MSG8});

            var checkedDeviceStatus = _commonDomain.CheckStatusOfDeviceRecordForProductManagement(deviceCode);
            if(!checkedDeviceStatus)
                return Json(new{Success=false,Message=ProductManagementResources.MSG9});

            var checkedProductShelfStatus = _storageOfEmptyProductPalletDomain.CheckProductShelfStatus();
            if(!checkedProductShelfStatus)
                return Json(new { Success = false, Message = ProductManagementResources.MSG19});

            _storageOfEmptyProductPalletDomain.StoreTheEmptyPallet(model.PalletNumber, terminalNo);
            return Json(new {Success = true});
        }

        [HttpPost]
        public async Task<ActionResult> CheckStatus()
        {
            // Find terminal no from request.
            var terminalNo = _identityServices.FindTerminalNo(HttpContext.User.Identity);

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

        /// <summary>
        /// Reply message sent back from C3.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ReplyMessageC3()
        {
            // Find terminal no.
            var terminalNo = _identityServices.FindTerminalNo(HttpContext.User.Identity);

            var items = _storageOfEmptyProductPalletDomain.RespondingReplyFromC3(terminalNo);
            return Json(items);
        }
        #endregion
    }
}