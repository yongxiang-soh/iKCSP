using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Resources;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.MaterialManagement.ViewModels.RetrieveOfMaterial;
using KCSG.Web.Attributes;
using KCSG.Web.ViewModels;
using log4net;

namespace KCSG.Web.Areas.MaterialManagement.Controllers
{
    [MvcAuthorize("TCRM071F")]
    public class RetrieveOfMaterialController : KCSG.Web.Controllers.BaseController
    {
        #region Controller Constructor

        /// <summary>
        /// Initiate controller with dependency injections.
        /// </summary>
        /// <param name="retrieveOfMaterialDomain"></param>
        /// <param name="commonDomain"></param>
        /// <param name="identityService"></param>
        /// <param name="log"></param>
        public RetrieveOfMaterialController(
            IRetrieveOfMaterialDomain retrieveOfMaterialDomain,
            ICommonDomain commonDomain,
            IIdentityService identityService,
            IConfigurationService configurationService,
            IStorageOfWarehousePalletDomain storageOfWarehousePalletDomain,
            IRestorageOfMaterialDomain restorageOfMaterialDomain,
        ILog log)
        {
            _retrieveOfMaterialDomain = retrieveOfMaterialDomain;
            _commonDomain = commonDomain;
            _identityService = identityService;
            _log = log;
            _configurationService = configurationService;
            this._storageOfWarehousePalletDomain = storageOfWarehousePalletDomain;
            this._restorageOfMaterialDomain = restorageOfMaterialDomain;

        }

        #endregion

        #region Methods

        /// <summary>
        /// Render index page.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var retrievalOfMaterialViewModel = new RetrieveOfMaterialViewModel()
            {
                PalletsGrid = InitiateGrid()
            };
            return View(retrievalOfMaterialViewModel);
        }

        /// <summary>
        /// Retrieve or assign pallet.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> RetrieveOrAssignPallet(RetrieveOfMaterialViewModel item)
        {

            // Request parameters haven't been initialized.
            if (item == null)
            {
                item = new RetrieveOfMaterialViewModel();
                TryValidateModel(item);
            }


            // Request parameters are invalid.
            if (!ModelState.IsValid)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Find current terminal number of request.
            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);

            var result =
                await
                    _retrieveOfMaterialDomain.RetrieveOrAssignPallet(item.MaterialCode.Trim(),
                        item.RequestedRetrievalQuantity,
                        terminalNo);

            return Json(new
            {
                result
            });

        }

        /// <summary>
        /// Unassign a list of pallets with specific conditions.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> UnassignPallets(RetrieveOfMaterialViewModel item)
        {
            try
            {
                // Request parameters haven't been initialized.
                if (item == null)
                {
                    item = new RetrieveOfMaterialViewModel();
                    TryValidateModel(item);
                }

                // Request parameters are invalid.
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                // Find terminal number of current request.
                var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);

                await _retrieveOfMaterialDomain.UnassignPalletsList(item.MaterialCode.Trim(), terminalNo);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Unassign a pallet with specific conditions.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> UnassignPallet(UnassignPalletsViewModel item)
        {
            try
            {
                // Request parameters haven't been initialized.
                if (item == null)
                {
                    item = new UnassignPalletsViewModel();
                    TryValidateModel(item);
                }

                // Request parameters are invalid.
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                await _retrieveOfMaterialDomain.UnassignSpecificPallet(item.ShelfRow, item.ShelfBay, item.ShelfLevel);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// After a specific pallet has been unassigned, tally value should be re-calculated.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<ActionResult> CalculateTally(RetrieveOfMaterialViewModel parameters)
        {
            try
            {
                // Parameters haven't been initialized.
                if (parameters == null)
                {
                    parameters = new RetrieveOfMaterialViewModel();
                    TryValidateModel(parameters);
                }

                // Request parameters are invalid.
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                // Find list of pallets by using specific conditions.
                var result =
                    await
                        _retrieveOfMaterialDomain.ReCalculateTally(parameters.MaterialCode.Trim());

                return Json(new
                {
                    tally = result
                });
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Find pallets by using specific conditions.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="gridSettings"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> FindPallets(RetrieveOfMaterialViewModel parameters, GridSettings gridSettings)
        {
            try
            {
                // Parameters haven't been initialized.
                if (parameters == null)
                {
                    parameters = new RetrieveOfMaterialViewModel();
                    TryValidateModel(parameters);
                }

                // Request parameters are invalid.
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                // Find current terminal number of pallet.
                var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);

                // Find list of pallets by using specific conditions.
                var result =
                    await
                        _retrieveOfMaterialDomain.FindPalletsList(parameters.MaterialCode.Trim(),
                            parameters.RequestedRetrievalQuantity, terminalNo, gridSettings);

                return Json(result.Data);
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        ///// <summary>
        ///// Find pallets detail by using pallet no as search condition.
        ///// </summary>
        ///// <param name="item"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<ActionResult> FindPalletDetails(FindPalletDetailViewModel item)
        //{
        //    if (item == null)
        //    {
        //        item = new FindPalletDetailViewModel();
        //        TryValidateModel(item);
        //    }

        //    if (!ModelState.IsValid)
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        //    var details = await _retrieveOfMaterialDomain.FindPalletDetails(item.PalletNo.Trim());
        //    return Json(new
        //    {
        //        results = details
        //    });
        //}

        /// <summary>
        /// Retrieve specific pallets.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> RetrievePallets(RetrieveOfMaterialViewModel parameters)
        {
            try
            {

                if (parameters == null)
                {
                    parameters = new RetrieveOfMaterialViewModel();
                    TryValidateModel(parameters);
                }

                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                

                // Initiate message response.
                var messageResponse = new MessageResponseViewModel();

                // Find current terminal no.
                var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);

                // Check the conveyor code.
                if (!_storageOfWarehousePalletDomain.CheckedRecordInTM05(terminalNo))
                {
                    messageResponse.HttpStatusCode = HttpStatusCode.NotFound;
                    messageResponse.Message = HttpMessages.InvalidConveyorStatus;
                    return Json(messageResponse);
                }

                // Find device.
                if (!_restorageOfMaterialDomain.IsValidDevice())
                {
                    messageResponse.HttpStatusCode = HttpStatusCode.NotFound;
                    messageResponse.Message = HttpMessages.InvalidDeviceAvailability;
                    return Json(messageResponse);
                }

                // Retrieve pallets.
                await
                    _retrieveOfMaterialDomain.RetrieveMaterial(parameters.MaterialCode.Trim(),
                        parameters.RequestedRetrievalQuantity, terminalNo);

                messageResponse.HttpStatusCode = HttpStatusCode.OK;
                return Json(messageResponse);
            }
            catch (Exception exception)
            {
                _log.Error(exception.Message, exception);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }


        public ActionResult CheckValidate()
        {
            var deviceCode = _configurationService.MaterialDeviceCode;
            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            var checkConveryorStatus = _commonDomain.CheckStatusAndNumberRecordOfConveyor(terminalNo);
            if (!checkConveryorStatus)
                return Json(new { Succes = false, Message = Resources.MaterialResource.MSG15 });

            var checkDeviceStatus = _commonDomain.CheckStatusOfDeviceRecord(deviceCode);
            if (!checkDeviceStatus)
                return Json(new { Success = false, Message = Resources.MaterialResource.MSG16 });
            return Json(new { Success = true });
        }

        /// <summary>
        /// Post Retrieve Material when C1 return notification
        ///     Refer Br36 - SRS 1.1 for more infomation
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> PostRetrieveMaterial(PostResultProceedViewModel parameters)
        {
            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            var proceededRecords =    await _retrieveOfMaterialDomain.PostRetrieveMaterial(terminalNo, parameters.MaterialCode);
            return Json(proceededRecords);
        }

        public async Task<ActionResult> CheckAssignPallet(string materialCode)
        {
            var result = await _retrieveOfMaterialDomain.CheckAssignPallet(materialCode);
            if (result)
            {
                var tally = await _retrieveOfMaterialDomain.ReCalculateTally(materialCode);
                return Json(new {Checked = result, tally},JsonRequestBehavior.AllowGet);
            }
            return Json(new { Checked = false, tally = 0 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReStorage(RetrieveOfMaterialViewModel model)
        {
            return RedirectToAction("Index", "RestorageOfMaterial", new { showInRetrivel = true});
        }

        /// <summary>
        /// Initiate grid of pallets.
        /// </summary>
        /// <returns></returns>
        private Grid InitiateGrid()
        {
            return new Grid("DetailPallet")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSearchUrl(Url.Action("FindPallets", "RetrieveOfMaterial", new { Area = "MaterialManagement" }))
                .SetDefaultSorting("F33_MaterialCode", SortOrder.Asc)
                .OnDataLoaded("OnDetailPalletGridInitiated")
                .SetFields(
                new Field("ShelfNo")
                        .SetWidth(10)
                        .SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("ShelfNo")
                        .SetWidth(50)
                        .SetTitle("Shelf No. ")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("F31_PalletNo")
                        .SetWidth(50)
                        .SetTitle("Pallet No.")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("F31_Amount")
                        .SetTitle("Shelf Total")
                        .SetItemTemplate("gridHelper.displayNumber")
                        .SetWidth(50)
                );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Domain which handles businesses of material.
        /// </summary>
        private readonly IRetrieveOfMaterialDomain _retrieveOfMaterialDomain;

        /// <summary>
        /// Service which handles user information from identity.
        /// </summary>
        private readonly IIdentityService _identityService;

        /// <summary>
        /// Domain which is for handling common tasks.
        /// </summary>
        private readonly ICommonDomain _commonDomain;

        /// <summary>
        /// Instance which is used for logging task.
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// Get deviceCode
        /// </summary>
        private readonly IConfigurationService _configurationService;

        private readonly IStorageOfWarehousePalletDomain _storageOfWarehousePalletDomain;
        private readonly IRestorageOfMaterialDomain _restorageOfMaterialDomain;

        #endregion
    }
}