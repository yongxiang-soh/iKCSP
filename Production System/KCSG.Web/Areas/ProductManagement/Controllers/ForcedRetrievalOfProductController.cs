using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.ProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.ProductManagement.ViewModels.ForcedRetrievalOfProduct;
using KCSG.Web.Attributes;
using Microsoft.AspNet.SignalR.Messaging;
using Resources;

namespace KCSG.Web.Areas.ProductManagement.Controllers
{
    [MvcAuthorize("TCPR051F")]
    public class ForcedRetrievalOfProductController : KCSG.Web.Controllers.BaseController
    {
        private readonly IForcedRetrievalOfProductDomain _forcedRetrievalOfProductDomain;
        private readonly IIdentityService _identityService;
        private readonly ICommonDomain _commonDomain;
        private readonly IConfigurationService _configurationService;

        #region Constructor

        public ForcedRetrievalOfProductController(IForcedRetrievalOfProductDomain forcedRetrievalOfProductDomain,
            IIdentityService identityService, IConfigurationService configurationService, ICommonDomain commonDomain)
        {
            _forcedRetrievalOfProductDomain = forcedRetrievalOfProductDomain;
            _identityService = identityService;
            _commonDomain = commonDomain;
            _configurationService = configurationService;
        }

        #endregion

        //
        // GET: /ProductManagement/ForcedRetrievalOfProduct/
        public ActionResult Index()
        {
            var model = new ForcedRetrievalOfProductViewModel();
            model.Grid = GenerateGrid();
            return View(model);
        }

        [HttpPost]
        public ActionResult CheckRecordExistsFromTX40(string productCode, string productLotNo)
        {
            var isChecked = _forcedRetrievalOfProductDomain.CheckRecordExistsFromTX40(productCode.Trim(),
                productLotNo.Trim());
            if (!isChecked)
                return Json(new {Success = false, Message = ProductManagementResources.MSG1});

            var isCheckRecordInTX40AndTX57 = _forcedRetrievalOfProductDomain.CheckRecordExistsFormTX40AndTX57(productCode.Trim(),
                productLotNo.Trim());
            if(!isCheckRecordInTX40AndTX57)
                return Json(new { Success = false, Message = ProductManagementResources.MSG1 });
            return Json(new {Success = true});
        }

        [HttpPost]
        public ActionResult CheckRecordExistsFromTX40ForButtonRetrieval(string productCode, string productLotNo)
        {
            var isChecked = _forcedRetrievalOfProductDomain.CheckRecordExistsFromTX40ForButtonRetrieval(productCode.Trim(),
                productLotNo.Trim());
            if (!isChecked)
                return Json(new { Success = false, Message = ProductManagementResources.MSG1 });
            else
                return Json(new { Success = true });
        }
        

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetData(string productCode, string productLotNo,bool isPallet,double requestRetrievalQuantity,
            GridSettings gridSettings)
        {
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F51_ShelfRow";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            var result = _forcedRetrievalOfProductDomain.GetData(productCode.Trim(), productLotNo.Trim(),isPallet,requestRetrievalQuantity,gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetTally(string productCode, string productLotNo, bool isPallet, double requestRetrievalQuantity)
        {
            var result = _forcedRetrievalOfProductDomain.GetTotalTally(productCode.Trim(), productLotNo.Trim(), isPallet,requestRetrievalQuantity);
            return Json(result);
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

        #region private Method

        private Grid GenerateGrid()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);
            return new Grid("TempTableGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSorting(true)
                .SetSelected(true)
                .SetPaging(true)
                .SetPageSize(30).OnDataLoaded("GetTally")
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSearchUrl(urlHelper.Action("GetData", "ForcedRetrievalOfProduct",
                    new {Area = "ProductManagement"}))
                .SetDefaultSorting("F51_ShelfRow", SortOrder.Asc)
                .SetFields(
                    new Field("ShelfNo")
                        .SetWidth(15).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("ShelfNo")
                        .SetTitle("Shelf No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("F51_PalletNo")
                        .SetTitle("Pallet No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100)
                        .SetSorting(false),
                   new Field("RemainingAmount")
                        .SetVisible(false)
                );
        }

        #endregion

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ProductDetails(string productCode, string palletNo)
        {
            var results = _forcedRetrievalOfProductDomain.ProductDetail(productCode.Trim(), palletNo.Trim());
            if (results != null)
                return Json(results);
            return Json(null);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult DeAssign(string shelfNo, string palletNo)
        {
            if (shelfNo != null)
            {
                try
                {
                    _forcedRetrievalOfProductDomain.DeasigningPallet(shelfNo, palletNo);
                    return Json(new {Success = true});
                }
                catch
                {
                    return Json(new{Success=false});
                }
            }
            return Json(new{Success=false});
        }

        [HttpPost]
        public ActionResult DeAllAssign(string productCode, string productLotNo, string lstPalletNo)
        {
            try
            {
                var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
                //_forcedRetrievalOfProductDomain.DeAssignAllAssignedPallet(lstPalletNo, productCode.Trim(), productLotNo.Trim(),
                //     terminalNo);
                _forcedRetrievalOfProductDomain.UnassignPallet(productCode, productLotNo);
                return Json(new {Success=true});
            }
            catch (Exception exception)
            {
                return Json(new {Success = false, Message = ProductManagementResources.MSG17});
            }
        }


        [HttpPost]
        public ActionResult Restorage(string productCode, string productLotNo)
        {
            var palletno = _forcedRetrievalOfProductDomain.GetPalletNo(productCode.Trim(), productLotNo.Trim());
            return Json(new {results = palletno});
        }

        [HttpPost]
        public ActionResult CheckConveyorStatusAndDeviceStatus()
        {
            var deviceCode = _configurationService.ProductDeviceCode;
            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            var checkedConveyorStatus = _commonDomain.CheckStatusAndNumberRecordOfConveyor(terminalNo);
            if (!checkedConveyorStatus)
                return Json(new {Success = false, Message = ProductManagementResources.MSG8});

            var checkedDeviceStatus = _commonDomain.CheckStatusOfDeviceRecordForProductManagement(deviceCode);
            if (!checkedDeviceStatus)
                return Json(new {Success = false, Message = ProductManagementResources.MSG9});

            return Json(new {Success = true});
        }

        [HttpPost]
        public ActionResult RetrieveProduct(IList<TempTableItem> lstGridItem) 
        {
            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            _forcedRetrievalOfProductDomain.RetrieveProduct(lstGridItem, terminalNo);
            return Json(new {Success = true});
        }

      
        /// <summary>
        /// Respond result comes back from C3.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RespondReplyFromC3(ForceRetrievalProductRespondViewModel parameter)
        {
            if (parameter == null)
            {
                parameter = new ForceRetrievalProductRespondViewModel();
                TryValidateModel(parameter);
            }

            if (!ModelState.IsValid)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            // Find terminal no.
            var terminal = _identityService.FindTerminalNo(HttpContext.User.Identity);
            var lst = _forcedRetrievalOfProductDomain.RespondingReplyFromC3(terminal, parameter.ProductCode);

            return Json(lst);
        }
    }
}