using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.PreProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.PreProductManagement.ViewModels;
using KCSG.Web.Attributes;
using KCSG.Web.ViewModels;
using Resources;

namespace KCSG.Web.Areas.PreProductManagement.Controllers
{
    [MvcAuthorize("TCIP031F")]
    public class RetrievalOfPreProductController : KCSG.Web.Controllers.BaseController
    {
        #region Domain

        private readonly IRetrievalOfPreProductDomain _retrievalOfPreProductDomain;
        private readonly IIdentityService _identityDomain;
        private readonly ICommonDomain _commonDomain;
        private readonly IConfigurationService _configurationService;

        #endregion
        
        #region Constructor

        public RetrievalOfPreProductController(IRetrievalOfPreProductDomain retrievalOfPreProductDomain, ICommonDomain commonDomain, IIdentityService identityDomain, IConfigurationService configurationService)
        {
            this._retrievalOfPreProductDomain = retrievalOfPreProductDomain;
            _identityDomain = identityDomain;
            _commonDomain = commonDomain;
            _configurationService = configurationService;
        }

        #endregion
        //
        // GET: /PreProductManagement/RetrievalOfPreProduct/
        #region Methods

        public ActionResult Index()
        {
            var model = new RetrievalOfPreProductViewModel()
            {
                Grid = GenerateGrid()
            };
            return View(model);
        }

        public ActionResult ShowRetrievalOfPreProduct(GridSettings gridSettings)
        {
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F41_KndCmdNo";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            var result = _retrievalOfPreProductDomain.SearchCriteria(gridSettings);
            if (!result.IsSuccess)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }
        
        private Grid GenerateGrid()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("RetrievalGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSorting(true)
                .SetPaging(true)
                .SetSelected(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSearchUrl(urlHelper.Action("ShowRetrievalOfPreProduct", "RetrievalOfPreProduct", new { Area = "PreProductManagement" }))
                .SetDefaultSorting("F41_KndCmdNo", SortOrder.Asc)
                .SetFields(
                    new Field("F41_KndCmdNo")   
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F41_KndCmdNo")
                        .SetTitle("Command No.")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F41_PreproductCode")
                        .SetTitle("Pre-product Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("PreProductName")
                        .SetTitle("Pre-product Name")
                        .SetWidth(100),
                    new Field("F41_PrePdtLotNo")
                        .SetTitle("Lot No.")
                        .SetWidth(120)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("ThrowAmount")
                        .SetTitle("Quantity")
                        .SetSorting(false)
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumberFormat"),
                    new Field("Line")
                        .SetTitle("Line")
                        .SetSorting(false)
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("ContQuanty")
                        .SetTitle("Cont Qty")
                        .SetSorting(false)
                        .SetWidth(80)
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                );
        }
        #endregion
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(string commandNo, string preProductCode, string lotNo, string tabletisingLine)
        {
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);

            var result=_retrievalOfPreProductDomain.Edit(commandNo, preProductCode, lotNo, tabletisingLine, terminalNo);
            if (!result.IsSuccess)
            {
                return Json(new { Success = false,Message=PreProductManagementResources.MSG1 });
            }

            return Json(new {Success = true});

        }

        [HttpPost]
        public JsonResult CheckConveyor()
        {
            var terminerNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
            var deviceCode = _configurationService.PreProductDeviceCode;

            var checkedConveyorStatus = _commonDomain.CheckStatusAndNumberRecordOfConveyor(terminerNo);
            if (!checkedConveyorStatus)
                return Json(new { Success = false, Message = PreProductManagementResources.MSG13 });

            var checkedDeviceStatus = _commonDomain.CheckStatusOfDeviceRecordForProductManagement(deviceCode);
            if (!checkedDeviceStatus)
                return Json(new { Success = false, Message = PreProductManagementResources.MSG14 });
            return Json(new
            {
                Success=true
            });
        }

        [HttpPost]
        public ActionResult ReceiveMessageFromC2(string kndCmdNo,string prepdtLotNo,string preProductCode)
        {
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
            var response = _retrievalOfPreProductDomain.ReceiveMessageFromC2(terminalNo,kndCmdNo,prepdtLotNo,preProductCode);
            return Json(response);
        }
    }
}