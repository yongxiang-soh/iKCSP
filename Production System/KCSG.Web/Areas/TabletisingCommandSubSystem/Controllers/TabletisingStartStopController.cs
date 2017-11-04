using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using AutoMapper;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Interfaces.TabletisingCommandSubSystem;
using KCSG.Domain.Models;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.TabletisingCommandSubSystem.Models.TabletisingStartStop;
using KCSG.Web.Areas.TabletisingCommandSubSystem.Models;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.TabletisingCommandSubSystem.Controllers
{
    [MvcAuthorize("TCMD021F")]
    public class TabletisingStartStopController : KCSG.Web.Controllers.BaseController
    {
        #region Domain Declaration

        private readonly ITabletisingStartStopDomain _tabletisingStartStopDomain;
        private readonly IIdentityService _identityService;

        #endregion

        #region Constructor

        public TabletisingStartStopController(ITabletisingStartStopDomain tabletisingStartStopDomain, IIdentityService identityService)
        {
            _tabletisingStartStopDomain = tabletisingStartStopDomain;
            _identityService = identityService;
        }

        #endregion

        //
        // GET: /TabletisingCommandSubSystem/TabletisingStartStop/
        public ActionResult Index()
        {
            var model = new TabletisingStartStopViewModel()
            {
                GridTabletising = GenerateGridTabletising(),
                GridTabletisingSelected = GenerateGridTabletisingSelected()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ShowTabletising(TabletisingStartStopViewModel model, GridSettings gridSettings)
        {
            var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F41_KndCmdNo";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            var result = _tabletisingStartStopDomain.SearchCriteria(gridSettings,terminalNo);
            if (!result.IsSuccess)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Selected(string cmdno, string lotno, GridSettings gridSettings)
        {
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F56_AddDate";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            var result = _tabletisingStartStopDomain.Selected(cmdno.Trim(), lotno.Trim(), gridSettings);
            if (!result.IsSuccess)
            {
                return Json(new {Message = TabletisingResources.MSG20}, JsonRequestBehavior.AllowGet);
            }
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CheckStatus(string cmdNo, string lotNo)
        {
            var checkStatus = _tabletisingStartStopDomain.CheckedStatus(cmdNo, lotNo);
            if (checkStatus)
            {
                return Json(new {Success = true});
            }

            return Json(new {Success = false});
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult TimeJob(string cmdNo, string productCode, string lotNo, string lowerLotNo)
        {
            var result = _tabletisingStartStopDomain.TimeJob(cmdNo, productCode, lotNo, lowerLotNo);
            if (result.IsSuccess)
            {
                return Json(new {Success = true,Message=result.ErrorMessages}, JsonRequestBehavior.AllowGet);
            }
            return Json(new {Success = false, JsonRequestBehavior.AllowGet});
        }

        #region Private Method

        private Grid GenerateGridTabletising()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("TabletisingGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSorting(true)
                .SetSelected(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSearchUrl(urlHelper.Action("ShowTabletising", "TabletisingStartStop",
                    new {Area = "TabletisingCommandSubSystem"}))
                .SetDefaultSorting("F41_KndCmdNo", SortOrder.Asc)
                .SetFields(
                    new Field("F41_PreproductCode")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F41_KndCmdNo")
                        .SetTitle("Command No.")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(120),
                    new Field("F41_PreproductCode")
                        .SetTitle("Pre-Product Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("PreProductName")
                        .SetTitle("Pre-Product Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F41_PrePdtLotNo")
                        .SetTitle("Lot No.")
                        .SetWidth(120)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("Status")
                        .SetTitle("Status")
                        .SetSorting(false)
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F49_ContainerCode")
                        .SetTitle("Container Code")
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("Quantity")
                        .SetTitle("Quantity")
                        .SetSorting(false)
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("RetrievalDate")
                        .SetTitle("Retrieval Date")
                        .SetSorting(false)
                        .SetWidth(140)
                        .SetItemTemplate("gridHelper.displayDateTime"),
                    new Field("TmpReturnTime")
                        .SetTitle("Tmp Return Time")
                        .SetSorting(false)
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.displayDateTime"),
                    new Field("PassedTime")
                        .SetTitle("Passed Time")
                        .SetSorting(false)
                        .SetWidth(150)
                        .SetItemTemplate("gridHelper.generateNameColumnFormat"),
                    new Field("F41_TabletLine")
                        .SetVisible(false)
                );
        }

        private Grid GenerateGridTabletisingSelected()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("TabletisingSelectGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSorting(true)
                .SetSelected(true)
                .SetPaging(true)
                .SetPageSize(30)
                .OnDataLoaded("CheckNumberRecord")
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSearchUrl(urlHelper.Action("Selected", "TabletisingStartStop",
                    new {Area = "TabletisingCommandSubSystem"}))
                .SetDefaultSorting("F56_ProductCode", SortOrder.Asc)
                .SetFields(
                    new Field("F56_ProductCode")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F56_ProductCode")
                        .SetTitle("Product Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("ProductName")
                        .SetTitle("Product Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(150),
                    new Field("F56_ProductLotNo")
                        .SetTitle("Lot No.")
                        .SetSorting(false)
                        .SetWidth(120)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F56_TbtCmdAmt")
                        .SetTitle("Cmd Qty").SetSorting(false)
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumberFormat"),
                    new Field("PackUnit")
                        .SetTitle("Pack Unit").SetSorting(false)
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("F56_TbtCmdEndPackAmt")
                        .SetTitle("Packages")
                        .SetSorting(false)
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("F56_TbtCmdEndFrtAmt")
                        .SetTitle("Fraction")
                        .SetSorting(false)
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumberFormat"),
                    new Field("Status")
                        .SetTitle("Status")
                        .SetSorting(false)
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                   new Field("F56_PrePdtLotNo")
                        .SetVisible(false)
                );
        }

        #endregion

        #region UC6 Container set

        [HttpPost]
        public ActionResult ContainerSet(string cmdNo, string lotNo)
        {
            var isChecked = _tabletisingStartStopDomain.ContainerSet(cmdNo, lotNo);
            if (isChecked)
            {
                return Json(new {Success = true});
            }
            return Json(new {Success = false, Message = Resources.TabletisingResources.MSG32});
        }

        #endregion

        #region UC9

        [HttpPost]
        public JsonResult Start(string commandNo, string lotNo, string preProductCode,string productCode)
        {
            var isEnd = _tabletisingStartStopDomain.Start(commandNo.Trim(), lotNo.Trim(), preProductCode.Trim(),productCode.Trim());
            return Json(new { Success = isEnd});
            //return Json(new {Success = isEnd, Message = isEnd ? TabletisingResources.MSG28 : ""});
        }

        #endregion

        #region Uc10

        public ActionResult End(string cmdNo, string lotNo, string productCode, string productName, int packageValue,
            double fraction, double packUnit, string preProductLotNo)
        {
            var model = new EndTabletisingViewModel()
            {
                LotNo = lotNo,
                ProductCode = productCode,
                ProductName = productName,
                CommandNo = cmdNo,
                //Package = packageValue,
                //Fraction = fraction,
                PackingUnit = packUnit,
                PreProductLotNo = preProductLotNo
            };
            return PartialView("_PartialEndTabletising", model);
        }

        [HttpPost]
        public JsonResult End(EndTabletisingViewModel model)
        {
            var item = Mapper.Map<EndTabletisingItem>(model);
            var isEnd = _tabletisingStartStopDomain.End(item);
            return Json(new {Success = isEnd, Message = isEnd ? TabletisingResources.MSG28 : ""},
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ValidationForEndButton(string cmdNo)
        {
            var checkValidation = _tabletisingStartStopDomain.ValidationForEndButton(cmdNo);
            if (checkValidation)
            {
                return Json(new {Success = true});
            }
            else
            {
                return Json(new {Success = false});
            }
        }

        #endregion
    }
}