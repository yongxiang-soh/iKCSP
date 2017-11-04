using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.ProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Web.Areas.ProductManagement.ViewModels.RetrievalOfEmptyProductPallet;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.ProductManagement.Controllers
{
    [MvcAuthorize("TCPR131F")]
    public class RetrievalOfEmptyProductPalletController : KCSG.Web.Controllers.BaseController
    {
        #region Costructor

        /// <summary>
        ///     Initiate controller with IoC.
        /// </summary>
        /// <param name="retrievalOfProductPalletDomain"></param>
        /// <param name="identityService"></param>
        /// <param name="commonDomain"></param>
        /// <param name="configurationService"></param>
        public RetrievalOfEmptyProductPalletController(IRetrievalOfProductPalletDomain retrievalOfProductPalletDomain,
            IIdentityService identityService, ICommonDomain commonDomain, IConfigurationService configurationService)
        {
            _retrievalOfProductPalletDomain = retrievalOfProductPalletDomain;
            _identityService = identityService;
            _commonDomain = commonDomain;
            _configurationService = configurationService;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Domain which handles retrieval of product pallets function.
        /// </summary>
        private readonly IRetrievalOfProductPalletDomain _retrievalOfProductPalletDomain;

        /// <summary>
        ///     Service which handles identities business.
        /// </summary>
        private readonly IIdentityService _identityService;

        /// <summary>
        ///     Domain which handles common tasks.
        /// </summary>
        private readonly ICommonDomain _commonDomain;

        /// <summary>
        ///     Service which handles configurations.
        /// </summary>
        private readonly IConfigurationService _configurationService;

        #endregion

        #region Properties.

        /// <summary>
        ///     Renders index page.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = new RetrievalOfEmptyProductPalletViewModel();
            model.PossibleRetrievalQuantity = _retrievalOfProductPalletDomain.GetPossibleRetrievalQuantity();
            model.RequestedRetrievalQuantity = 1;
            return View(model);
        }

        [HttpPost]
        public ActionResult GetRequestedRetrievalQuantity()
        {
            var result = _retrievalOfProductPalletDomain.GetPossibleRetrievalQuantity();
            return Json(new
            {
                result
            });
        }

        /// <summary>
        ///     Retrieve the empty pallet function.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RetrieveTheEmptyPallet()
        {
            var deviceCode = _configurationService.ProductDeviceCode;
            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            try
            {
                var checkedConveyorStatus = _commonDomain.CheckStatusAndNumberRecordOfConveyor(terminalNo);
                if (!checkedConveyorStatus)
                    return Json(new {Success = false, Message = ProductManagementResources.MSG8});

                var checkedDeviceStatus = _commonDomain.CheckStatusOfDeviceRecordForProductManagement(deviceCode);
                if (!checkedDeviceStatus)
                    return Json(new {Success = false, Message = ProductManagementResources.MSG9});

                _retrievalOfProductPalletDomain.Retrieval(terminalNo);
                return Json(new { Success = true, Message = ProductManagementResources.MSG57 });
            }
            catch (Exception)
            {
                return Json(new {Success = false});
            }
        }

        /// <summary>
        /// Reply messages sent back from C3.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Reply()
        {
            // Find terminal.
            var terminal = _identityService.FindTerminalNo(HttpContext.User.Identity);
            var items = _retrievalOfProductPalletDomain.RespondingReplyFromC3(terminal);
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
    }
}