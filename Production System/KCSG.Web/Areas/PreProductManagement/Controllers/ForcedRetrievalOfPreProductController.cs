using System.Linq;
using System.Net;
using System.Web.Mvc;
using KCSG.Domain.Interfaces.PreProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.PreProductManagement.ViewModels;
using KCSG.Web.Attributes;
using KCSG.Web.ViewModels;
using System.Collections.Generic;
using KCSG.Domain.Interfaces;
using Resources;

namespace KCSG.Web.Areas.PreProductManagement.Controllers
{
    [MvcAuthorize("TCIP061F")]
    public class ForcedRetrievalOfPreProductController : KCSG.Web.Controllers.BaseController
    {
        #region Constructor

        public ForcedRetrievalOfPreProductController(
            IForcedRetrievalOfPreProductDomain forcedRetrievalOfPreProductDomain, IIdentityService identityDomain,
            IRetrievalOfPreProductDomain retrievalOfPreProductDomain, IConfigurationService configurationService,
            ICommonDomain commonDomain)
        {
            _forcedRetrievalOfPreProductDomain = forcedRetrievalOfPreProductDomain;
            _retrievalOfPreProductDomain = retrievalOfPreProductDomain;
            _identityService = identityDomain;
            _configurationService = configurationService;
            _commonDomain = commonDomain;
        }

        #endregion

        #region Properties

        private readonly IConfigurationService _configurationService;
        private readonly ICommonDomain _commonDomain;

        /// <summary>
        ///     Domain which handles business of pre-product.
        /// </summary>
        private readonly IForcedRetrievalOfPreProductDomain _forcedRetrievalOfPreProductDomain;

        /// <summary>
        ///     Service which handles identity in HttpContext.
        /// </summary>
        private readonly IIdentityService _identityService;

        private readonly IRetrievalOfPreProductDomain _retrievalOfPreProductDomain;

        #endregion

        #region Methods

        /// <summary>
        ///     Render index page.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = new RetrieveForcedRetrievalPreProductViewModel
            {
                GroupNameList = GetListGroupName(),
                Grid = GenerateGrid()
            };
            return View(model);
        }


        /// <summary>
        ///     Search pre-products list which need to be retrieved.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="gridSettings"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SearchByName(string groupNameValue, GridSettings gridSettings)
        {
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F42_KndCmdNo";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            var result = _forcedRetrievalOfPreProductDomain.SearchCriteria(gridSettings, groupNameValue);

            if (!result.IsSuccess)
                return Json(null);

            return Json(result.Data);
        }

        /// <summary>
        ///     Retrieve pre-products.
        /// </summary>
        /// <param name="commandNo"></param>
        /// <param name="lotNo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(string commandNo, string lotNo, string containerCode, string containerNo,
            string shelfNo, string groupName)
        {
            var deviceCode = _configurationService.PreProductDeviceCode;


            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            var checkConveyorCode = _commonDomain.CheckStatusAndNumberRecordOfConveyor(terminalNo);
            if (!checkConveyorCode)
            {
                return Json(new
                {
                    Success = false,
                    Message = PreProductManagementResources.MSG13
                }, JsonRequestBehavior.AllowGet);
            }

            var checkedDeviceStatus = _commonDomain.CheckStatusOfDeviceRecordForProductManagement(deviceCode);
            if (!checkedDeviceStatus)
            {
                return Json(new
                {
                    Success = false,
                    Message = PreProductManagementResources.MSG14
                }, JsonRequestBehavior.AllowGet);
            }

            //var result = _retrievalOfPreProductDomain.CheckConveyor(terminalNo);
            //BR 6 Checking Status of Conveyor and Pre-product Warehouse
            //if (!result.IsSuccess)
            //{
            //    httpResponseMessage.HttpStatusCode = HttpStatusCode.InternalServerError;
            //    httpResponseMessage.Message = result.ErrorMessages.Equals("MSG13")
            //        ? Resources.PreProductManagementResources.MSG13
            //        : Resources.PreProductManagementResources.MSG1;
            //    return Json(httpResponseMessage);
            //}
            //get all item same lotno to process

            //var allitem = _forcedRetrievalOfPreProductDomain.Getallitem(lotNo, groupName);
            //if (allitem.Any())
            //{
            //    foreach (var item in allitem)
            //    {
            var updateitem = _forcedRetrievalOfPreProductDomain.Edit(commandNo, lotNo, terminalNo, containerCode,
                containerNo, shelfNo);
            if (!updateitem.Success)
            {
                return Json(new {Success = false, Message = updateitem.MessageError},
                    JsonRequestBehavior.AllowGet);
            }
            //    }
            //}

            return Json(new {Success = true, Message = HttpStatusCode.OK},
                JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Handle response message sent back from communication server.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ReplySecondCommunication(ForceRetrievalResponseMessageViewModel parameters)
        {
            // Parameters haven't been initialized.
            if (parameters == null)
            {
                parameters = new ForceRetrievalResponseMessageViewModel();
                TryValidateModel(parameters);
            }

            // Request parameters are invalid.
            if (!ModelState.IsValid)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Find identity from http context.
            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            var item = _forcedRetrievalOfPreProductDomain.ForcedRetrievalMessageC2Reply(parameters.PreProductCode,
                parameters.ShelfNo,
                parameters.CommandNo, parameters.CommandLotNo, terminalNo, parameters.isNotCommand);

            return Json(item);
        }

        /// <summary>
        ///     Initiate grid of force retrieval pre-product.
        /// </summary>
        /// <returns></returns>
        private Grid GenerateGrid()
        {
            return new Grid("ForcedRetrievalGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(10)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSelected(true)
                .SetSearchUrl(Url.Action("SearchByName", "ForcedRetrievalOfPreProduct",
                    new {Area = "PreProductManagement"}))
                .SetDefaultSorting("F42_KndCmdNo", SortOrder.Asc)
                .SetFields(
                    new Field("F42_KndCmdNo")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F42_KndCmdNo")
                        .SetTitle("Knd Cmd No.")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F42_PreProductCode")
                        .SetTitle("Pre-product Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("PreProductName")
                        .SetTitle("Pre-product Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("F42_PrePdtLotNo")
                        .SetTitle("Lot No.")
                        .SetWidth(120)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                );
        }


        public static List<SelectListItem> GetListGroupName()
        {
            var dataList = new List<SelectListItem>
            {
                new SelectListItem()
                {
                    Value = "0",
                    Text = "Not Command"
                },
                new SelectListItem()
                {
                    Value = "1",
                    Text = " Tabletised"
                }
            };

            return dataList;
        }

        #endregion
    }
}