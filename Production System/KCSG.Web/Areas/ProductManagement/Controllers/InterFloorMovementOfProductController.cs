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
using KCSG.Domain.Models.ProductManagement;
using KCSG.Web.Areas.ProductManagement.ViewModels.InterFloorMovementOfProduct;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.ProductManagement.Controllers
{
     [MvcAuthorize("TCPR081F")]
    public class InterFloorMovementOfProductController : KCSG.Web.Controllers.BaseController
    {
        private readonly IInterFloorMovementOfProductDomain _interFloorMovementOfProductDomain;
        private readonly IIdentityService _identityService;
        private readonly IConfigurationService _configurationService;
        private readonly ICommonDomain _commonDomain;
        #region Constructor

        public InterFloorMovementOfProductController(
            IInterFloorMovementOfProductDomain interFloorMovementOfProductDomain, IIdentityService identityService, IConfigurationService configurationService, ICommonDomain commonDomain)
        {
            _interFloorMovementOfProductDomain = interFloorMovementOfProductDomain;
            _identityService = identityService;
            _configurationService = configurationService;
            _commonDomain = commonDomain;
        }
        #endregion
        //
        // GET: /ProductManagement/InterFloorMovementOfProduct/
        public ActionResult Index()
        {
            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            
            var model =new InterFloorMovementOfProductViewModel();
            if (terminalNo == Constants.TerminalNo.A019)
            {
                model.From = 1;
                model.To = 2;
            }
            else
            {
                model.From = 2;
                model.To = 1;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult TranferInterFloor(InterFloorMovementOfProductViewModel model)
        {
            //var deviceCode = _configurationService.ProductDeviceCode;
            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            //var checkedConveyorStatus = _commonDomain.CheckStatusAndNumberRecordOfConveyor(terminalNo);
            ////var checkedConveyorStatus = _interFloorMovementOfProductDomain.CheckConveyorStatus();
            //if (!checkedConveyorStatus)
            //    return Json(new { Success = false, Message = ProductManagementResources.MSG8 });

            //var checkedDeviceStatus =_commonDomain.CheckStatusOfDeviceRecordForProductManagement(deviceCode);
            //if(!checkedDeviceStatus)
            //    return Json(new{Success=false,Message=ProductManagementResources.MSG9});

            //var checkedConveyorCode = _interFloorMovementOfProductDomain.CheckConveyorCode(model.From);
            //if (!checkedConveyorCode)
            //    return Json(new {Success = false, Message = ProductManagementResources.MSG8});

            _interFloorMovementOfProductDomain.TranferInterFloor(model.From, terminalNo);
            return Json(new{Success=true,Message=ProductManagementResources.MSG36});
        }

        /// <summary>
        /// Proceed data after messages having been sent from C3.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ProcessThirdCommunicationData()
        {
            // Find terminal no.
            var teminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            var items = _interFloorMovementOfProductDomain.Reply(teminalNo);

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
                //throw new Exception("MSG08");
                //return Json(new
                //{
                //    message = "MSG08"                        
                //});
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

    }
}