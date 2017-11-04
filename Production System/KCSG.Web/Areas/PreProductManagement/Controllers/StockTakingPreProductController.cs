using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.PreProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.PreProductManagement.ViewModels;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.PreProductManagement.Controllers
{
    [MvcAuthorize("TCIP041F")]
    public class StockTakingPreProductController : KCSG.Web.Controllers.BaseController
    {
        #region Constructors

        /// <summary>
        ///     Initiate controller with dependency injections.
        /// </summary>
        /// <param name="stockTakingPreProductDomain"></param>
        /// <param name="identityDomain"></param>
        public StockTakingPreProductController(IStockTakingPreProductDomain stockTakingPreProductDomain,
            IIdentityService identityDomain, IRetrievalOfPreProductDomain retrievalOfPreProductDomain, ICommonDomain commonDomain, IConfigurationService configurationService)
        {
            _stockTakingPreProductDomain = stockTakingPreProductDomain;
            _identityDomain = identityDomain;
            _retrievalOfPreProductDomain = retrievalOfPreProductDomain;
            _configurationService = configurationService;
            _commonDomain = commonDomain;
        }
        private readonly ICommonDomain _commonDomain;
        private readonly IConfigurationService _configurationService;
        #endregion

        /// <summary>
        ///     This function is for rendering index page.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var stockTakingPreProductViewModel = new StockTakingPreProductViewModel
            {
                StockTakingPreProductGrid = InitiateStockTakingPreProductGrid()
            };
            return View(stockTakingPreProductViewModel);
        }
        #region function in screen TCIP041F

        /// <summary>
        ///     Find stock taking pre-product by using specific conditions.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FindStockTakingPreProduct(GridSettings gridSettings,
            FindStockTakingPreProductViewModel condition)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                // Find the result and bind searched data to response.
                var result =
                    _stockTakingPreProductDomain.SearchStockTakingPreProduct(gridSettings,
                        condition.FromRow, condition.FromBay, condition.FromLevel,
                        condition.ToRow, condition.ToBay, condition.ToLevel);

                return Json(result.Data);
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        ///     This function is for retrieving stocking pre-products.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Retrieve(string preProductCode, string lsRow, string lsBay, string lsLevel,
            string containerCode, string containerNo, string updateDate)
        {
            var deviceCode = _configurationService.PreProductDeviceCode;
            DateTime udate = DateTime.Parse(updateDate);

            if (!ModelState.IsValid)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Find terminal no from identity.
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
            var checkConveyorCode = _commonDomain.CheckStatusAndNumberRecordOfConveyor(terminalNo);
            if (!checkConveyorCode)
            {
                return Json(new
                {
                    Success = false,
                    Message = PreProductManagementResources.MSG13
                });
            }

            var checkedDeviceStatus = _commonDomain.CheckStatusOfDeviceRecordForProductManagement(deviceCode);
            if (!checkedDeviceStatus)
                return Json(new { Success = false, Message = PreProductManagementResources.MSG14 });
            //var result = _retrievalOfPreProductDomain.CheckConveyor(terminalNo);
            ////BR 6 Checking Status of Conveyor and Pre-product Warehouse
            //if (!result.IsSuccess)
            //{
            //    return Json(new { Success = false, Message = result.ErrorMessages[0].Equals("MSG13") ? Resources.PreProductManagementResources.MSG13 : Resources.PreProductManagementResources.MSG14 }, JsonRequestBehavior.AllowGet);
            //}


            //    _stockTakingPreProductDomain.RetrieveStockTakingOfPreProduct(terminalNo, preProductCode,
            //        lsRow, lsBay, lsLevel, containerCode, containerNo,udate);

            //return Json(new
            //{
            //            Success = false,
            //            Message =
            //                result.ErrorMessages[0].Equals("MSG13")
            //                    ? Resources.PreProductManagementResources.MSG13
            //                    : Resources.PreProductManagementResources.MSG14
            //        }, JsonRequestBehavior.AllowGet);

            var resulttRetrive = _stockTakingPreProductDomain.RetrieveStockTakingOfPreProduct(terminalNo, preProductCode,
                lsRow, lsBay, lsLevel, containerCode, containerNo, udate);
            if (resulttRetrive.IsSuccess)
            {
                return Json(new { Success = resulttRetrive.IsSuccess, convcode = resulttRetrive.Data });
            }
            return Json(new
            {
                Success = resulttRetrive.IsSuccess,
                Message =
                    resulttRetrive.ErrorMessages[0] == "MSG13"
                        ? Resources.PreProductManagementResources.MSG13
                        : resulttRetrive.ErrorMessages[0] == "MSG14"
                            ? Resources.PreProductManagementResources.MSG14
                            : Resources.PreProductManagementResources.MSG28
            });
        }

        #endregion
        #region  function in screen TCIP043F

        /// <summary>
        ///     This function will be called when button OK is clicked on confirm panel
        ///     Refer 3.4.3	UC 14: Retrieve Pre-Product  Container (TCIP043F) for more information.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> RetrievePreProductContainerOk(RetrievePreProductContainerOkViewModel item)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                // Find terminal no from identity.
                var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);

                await
                    _stockTakingPreProductDomain.RetrievePreProductContainerOk(item.Row, item.Bay, item.Level,
                        item.ContainerCode, item.ContainerNo, terminalNo, item.ContainerType);

                return Json(new { Success = true });
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        ///     This function will be called when button Ng is clicked on confirm panel
        ///     Refer BR 43 : Re-storing data for [NG] button rules
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> RetrievePreProductContainerNg(RetrievePreProductContainerOkViewModel item)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                // Find terminal no from identity.
                var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);

                await
                    _stockTakingPreProductDomain.RetrievePreProductContainerNg(item.Row, item.Bay, item.Level,
                        item.ContainerCode, item.ContainerNo, terminalNo);

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }
        #endregion
        #region Receiving Message From C2

        /// <summary>
        /// Please refer UC 15 - SRS 1.0 Sign off.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AnalyzeStockTakingForMoving(AnalyzeStockTakingForMovingViewModel parameters)
        {
            // Find the terminal which is currently sending request.
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
            var result = _stockTakingPreProductDomain.AnalyzeMessageForStoringMovingPreProduct(parameters.ContainerCode,
                parameters.LchStatus, terminalNo, parameters.OkClicked,parameters. PreProductCode);
            return Json(result);
        }

        /// <summary>
        /// Analyze stock taking preproduct as defined in UC-17
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ReceivingMessageFromC2(string preProductCode)
        {
            // Find terminal which is currently sending request to server.
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);

            var result = _stockTakingPreProductDomain.ReceivingMessageFromC2(terminalNo, preProductCode);

            return Json(result);
        }

        /// <summary>
        /// ReceiveMessageFromC2ServerForStoring - UC13
        /// </summary>
        /// <param name="shelfNo"></param>
        /// <param name="preProductCode"></param>
        /// <param name="containerCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ReceiveMessageFromC2ServerForStoring(string shelfNo, string preProductCode, string containerCode)
        {
            // Find terminal which is currently sending request to server.
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);

            var result = _stockTakingPreProductDomain.ReceiveMessageFromC2ServerForStoring(terminalNo, containerCode, preProductCode, shelfNo);

            return Json(result);
        }
        #endregion
        /// <summary>
        ///     Initiate grid of Stock taking pre-product grid information.
        /// </summary>
        /// <returns></returns>
        private Grid InitiateStockTakingPreProductGrid()
        {
            return new Grid("StockTakingPreProduct")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSearchUrl(Url.Action("FindStockTakingPreProduct", "StockTakingPreProduct",
                    new { Area = "PreProductManagement" }))
                .SetFields(
                    new Field("")
                        .SetWidth(20)
                        .SetTitle("")
                        .SetItemTemplate("gridHelper.generateRadiobox"),
                    new Field("ShelfNo")
                        .SetTitle("Shelf No.")
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F49_PreProductCode")
                        .SetWidth(100)
                        .SetTitle("Pre-product Code")
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F03_PreProductName")
                        .SetWidth(100)
                        .SetTitle("Pre-product Name")
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F49_PreProductLotNo")
                        .SetTitle("Lot No.")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100)
                        .SetSorting(false),
                    new Field("F49_Amount")
                        .SetTitle("Quantity")
                        .SetItemTemplate("gridHelper.displayNumber")
                        .SetWidth(100)
                        .SetSorting(false),
                    new Field("F49_ContainerCode")
                        .SetTitle("Container Code")
                        .SetWidth(100)
                        .SetSorting(false),
                    new Field("F37_ContainerType")
                        .SetTitle("Container Type")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                );
        }

        [HttpPost]
        public ActionResult CreateAndUpdate(StockTakingPreProductViewModel model)
        {
            //This feed data for funciton only. Business rule not yet verified\

            DateTime udate = new DateTime();
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
            //if (ModelState.IsValid)
            //{
                var result = _stockTakingPreProductDomain.CreateAndUpdate(model.PreProductCode, model.ShelfNo,
                    model.ContainerCode, model.Quantity, model.LotNo, terminalNo, udate, model.ContainerType);
                if (result.IsSuccess)
                    return Json(new { Success = true });
                return Json(new { Success = false });
            //}
            //return Json(new { Success = false });
        }

        #region Properties

        /// <summary>
        ///     Domain which handles Pre-Product  operations.
        /// </summary>
        private readonly IStockTakingPreProductDomain _stockTakingPreProductDomain;

        /// <summary>
        ///     Domain which provides function to analyze claim identity.
        /// </summary>
        private readonly IIdentityService _identityDomain;

        private readonly IRetrievalOfPreProductDomain _retrievalOfPreProductDomain;

        #endregion
    }
}