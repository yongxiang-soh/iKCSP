using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper.Execution;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.ProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.ProductManagement.ViewModels;
using KCSG.Web.Areas.ProductManagement.ViewModels.StockTakingOfProduct;
using KCSG.Web.Attributes;
using KCSG.Web.Services;
using Resources;
using KCSG.Web.ViewModels;

namespace KCSG.Web.Areas.ProductManagement.Controllers
{
    [MvcAuthorize("TCPR061F")]
    public class StockTakingOfProductController : KCSG.Web.Controllers.BaseController
    {
        #region Properties

        /// <summary>
        /// Domain which handles business of Stock taking of product.
        /// </summary>
        private readonly IStockTakingOfProductDomain _stockTakingOfProductDomain;

        /// <summary>
        /// Domain whose functions are used globally.
        /// </summary>
        private readonly ICommonDomain _commonDomain;

        private readonly IConfigurationService _configurationService;

        /// <summary>
        /// Service which handles identity information from request.
        /// </summary>
        private readonly IIdentityService _identityService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initiate controller with dependency injection.
        /// </summary>
        /// <param name="stockTakingOfProductDomain"></param>
        /// <param name="commonDomain"></param>
        /// <param name="identityService"></param>
        public StockTakingOfProductController(
            IStockTakingOfProductDomain stockTakingOfProductDomain, ICommonDomain commonDomain,
            IIdentityService identityService,IConfigurationService iConfigurationService)
        {
            _stockTakingOfProductDomain = stockTakingOfProductDomain;
            _commonDomain = commonDomain;
            _identityService = identityService;
            _configurationService = iConfigurationService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Renders index view.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var stockTakingOfProduct = new StockTakingOfProductViewModel();
            stockTakingOfProduct.StockTakingOfProductGrid = InitiateStockTakingOfProductGrid();
            return View(stockTakingOfProduct);
        }

        /// <summary>
        /// Find stock taking of products list.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> FindStockTakingOfProduct(FindStockTakingOfProductViewModel parameters, GridSettings gridSettings)
        {
            try
            {
                // Parameters haven't been initialized.
                if (parameters == null)
                {
                    parameters = new FindStockTakingOfProductViewModel();
                    TryValidateModel(parameters);
                }

                // Model state is not valid.
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                // Find the list of stock-taking of products.
                var stockTakingOfProducts =
                    await _stockTakingOfProductDomain.FindStockTakingOfProductAsync(parameters.ShelfRowFrom,
                        parameters.ShelfBayFrom, parameters.ShelfLevelFrom,
                        parameters.ShelfRowTo, parameters.ShelfBayTo, parameters.ShelfLevelTo, gridSettings);

                return Json(stockTakingOfProducts.Data);
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Find product details
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> FindProductDetails(FindProductDetailViewModel parameters)
        {
            try
            {
                // Parameters haven't been initialized.
                if (parameters == null)
                {
                    parameters = new FindProductDetailViewModel();
                    TryValidateModel(parameters);
                }

                // Parameters are invalid.
                if (!ModelState.IsValid)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                // Truncate the input.
                parameters.PalletNo = parameters.PalletNo.Trim();

                var productsDetail = await _stockTakingOfProductDomain.FindProductDetailAsync(parameters.PalletNo);
                return Json(new
                {
                    productsDetail
                });
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Find product confirm details
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> FindProductConfirmDetails(FindProductConfirmDetailViewModel conditions)
        {
            if (conditions == null)
                return Json(null);

            if (!ModelState.IsValid)
                return Json(null);

            var items =
                await
                    _stockTakingOfProductDomain.FindProductConfirmDetails(conditions.Row, conditions.Bay,
                        conditions.Level, conditions.PalletNo);

            return Json(items);
        }

        /// <summary>
        /// This function is called when restorage button is clicked.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Restorage(RestorageProductViewModel restorageProductsList)
        {
            var messageResponseViewModel = new MessageResponseViewModel();
            try
            {
                if (restorageProductsList == null)
                {
                    restorageProductsList = new RestorageProductViewModel();
                    TryValidateModel(restorageProductsList);
                }

                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                // Find terminal no.
                var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);

                await
                    _stockTakingOfProductDomain.RestoreProductsAsync(terminalNo, restorageProductsList.PalletNo,
                        restorageProductsList.Items, restorageProductsList.Row, restorageProductsList.Bay,
                        restorageProductsList.Level);

                messageResponseViewModel.HttpStatusCode = HttpStatusCode.OK;
                messageResponseViewModel.Message = "";
                return Json(messageResponseViewModel);
            }
            catch (Exception exception)
            {
                messageResponseViewModel.HttpStatusCode = HttpStatusCode.InternalServerError;
                messageResponseViewModel.Message = exception.Message;
                return Json(messageResponseViewModel);
            }
        }
        /// <summary>
        /// Retrieve product details asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> RetrieveProductDetails(RetrieveProductDetailViewModel item)
        {
            try
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
                //TODO: Q&A Nov 24 2016

                var result = await _stockTakingOfProductDomain.RetrieveProductDetailsAsync(item.PalletNo, item.Row, item.Bay, item.Level,
                    terminalNo);

                if (!result.IsSuccess)
                {
                    return Json(new {Success = false, Message = ProductManagementResources.MSG47});
                }
                //return new HttpStatusCodeResult(HttpStatusCode.OK);
                //return Json(new
                //{
                //    //row = item.Row,
                //    //bay = item.Bay,
                //    //level = item.Level
                //    result,
                //    Success = true,
                //    Message = "Shelf No [" + result.Data.Row + result.Data.Bay + result.Data.Level + "] retrieving ..."
                //});
                return Json(new { Success = true, Message = "Shelf No [" + result.Data.Row + result.Data.Bay + result.Data.Level + "] retrieving ..." },
                        JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                if ("MSG08".Equals(exception.Message))
                {
                    Response.StatusCode = (int) HttpStatusCode.NotFound;
                    return Json(new
                    {
                        exception.Message
                    });
                }
                if ("MSG47".Equals(exception.Message))
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new
                    {
                        message = "MSG47"
                    });
                }
                if ("MSG9".Equals(exception.Message))
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new
                    {
                        exception.Message
                    });
                }

                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
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

        /// <summary>
        /// Handle data after messages from C3 sent back from service.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RespondRetrieve()
        {
            // Find terminal no.
            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            var items = _stockTakingOfProductDomain.RespondingReplyFromC3RetrieveProduct(terminalNo);

            return Json(items);
        }

        /// <summary>
        /// Handle data after messages from C3 sent back from service.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RespondRestore(StockTakingRespondRestoreViewModel parameters)
        {
            if (parameters == null)
                return Json(null);

            if (!ModelState.IsValid)
                return Json(null);

            // Find terminal no.
            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            var items = _stockTakingOfProductDomain.RespondingReplyFromC3RestoreProduct(terminalNo, parameters.Row, parameters.Bay, parameters.Level);

            return Json(items);
        }

        /// <summary>
        /// Initiate stock taking of product grid.
        /// </summary>
        /// <returns></returns>
        private Grid InitiateStockTakingOfProductGrid()
        {
            return new Grid("StockTakingOfProductGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(false)
                .OnDataLoaded("disabledSearch")
                .SetSearchUrl(Url.Action("FindStockTakingOfProduct", "StockTakingOfProduct",
                    new {Area = "ProductManagement"}))
                .SetDefaultSorting("F51_ShelfRow", SortOrder.Asc)
                .SetFields(

                    new Field("ShelfNo")
                        .SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetWidth(20)
                        .SetSorting(false),

                    new Field("ShelfNo")
                        .SetTitle("Shelf No.")
                        .SetItemTemplate("gridHelper.generateNameColumn"),

                    new Field("F51_PalletNo")
                        .SetTitle("Pallet No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                );

        }

        #endregion

    }
}