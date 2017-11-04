using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.KneadingCommand;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.KneadingCommand.ViewModels.InputOfKneadingCommand;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.KneadingCommand.Controllers
{
    [MvcAuthorize("TCPS031F")]
    public class InputOfKneadingCommandController : KCSG.Web.Controllers.BaseController
    {
        #region Constructors

        /// <summary>
        ///     Initiate controller with dependency injections.
        /// </summary>
        /// <param name="inputOfKneadingCommandDomain"></param>
        /// <param name="exportReportDomain"></param>
        public InputOfKneadingCommandController(IInputOfKneadingCommandDomain inputOfKneadingCommandDomain,
            IExportReportDomain exportReportDomain)
        {
            _inputOfKneadingCommandDomain = inputOfKneadingCommandDomain;
            _exportReportDomain = exportReportDomain;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Domain which handles input of kneading command function businesses
        /// </summary>
        private readonly IInputOfKneadingCommandDomain _inputOfKneadingCommandDomain;

        /// <summary>
        ///     Domain which provides functions to export reports.
        /// </summary>
        private readonly IExportReportDomain _exportReportDomain;

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult Index()
        {
            var model = new InputOfKneadingCommandSearchViewModel
            {
                Grid = InitiateKneadingComandsList(),
                GridSelected = InitiateSelectedKneadingCommandsList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SearchByDate(InputOfKneadingCommandSearchViewModel model, GridSettings gridSettings)
        {

            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F39_KndEptBgnDate";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            if (!model.Within.HasValue)
                return Json(new GridResponse<PdtPlnItem>(new List<PdtPlnItem>(), 0), JsonRequestBehavior.AllowGet);
            

            var current = DateTime.Now;
            current = new DateTime(current.Year, current.Month, current.Day, 0, 0, 0);

            var future = current.AddDays(model.Within ?? 0);
            future = new DateTime(future.Year, future.Month, future.Day, 23, 59, 59);
            var result = _inputOfKneadingCommandDomain.SearchCriteria(current,future, model.KndLine, gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);


            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Print the list of input commands.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> ExportKneadingCommands(PrintInputKneadingCommandViewModel item)
        {
            try
            {
                // Request parameters haven't been initialized.
                if (item == null)
                {
                    item = new PrintInputKneadingCommandViewModel();
                    TryValidateModel(item);
                }

                // Request parameters are invalid.
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                // Find the list of records 
                var result =
                    await _inputOfKneadingCommandDomain.SearchRecordsForPrinting(item.PreProductCode, item.CommandNo);

                // Find the template file.
                var exportKneadingCommandAbsolutePath =
                    Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.InputKneadingCommands));

                // Read the template.
                var template = await _exportReportDomain.ReadTemplateFileAsync(exportKneadingCommandAbsolutePath);

                // Export the template.
                var render = await _exportReportDomain.ExportToFlatFileAsync(result, template);

                return Json(new
                {
                    render
                });
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        ///     Initiate kneading commands list which should be shown on the screen.
        /// </summary>
        /// <returns></returns>
        private Grid InitiateKneadingComandsList()
        {
            return new Grid("KneadingCommandGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSearchUrl(Url.Action("SearchByDate", "InputOfKneadingCommand", new { Area = "KneadingCommand" }))
                .SetDefaultSorting("F39_KndEptBgnDate", SortOrder.Asc)
                .SetFields(
                    new Field("F39_PreProductCode")
                        .SetWidth(20)
                        .SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F39_KndEptBgnDate")
                        .SetTitle("Production Date")
                        .SetItemTemplate("gridHelper.displayDate")
                        .SetWidth(100),
                    new Field("F39_PreProductCode")
                        .SetTitle("Pre-ProductCode")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("F39_PreProductName")
                        .SetTitle("Pre-Product Name")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("F39_PrePdtLotAmt")
                        .SetTitle("Lot Qty")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("Quantity")
                        .SetTitle("Quantity")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumber")
                );
        }

        /// <summary>
        ///     Initiate the selected kneading commands list.
        /// </summary>
        /// <returns></returns>
        private Grid InitiateSelectedKneadingCommandsList()
        {
            return new Grid("KneadingCommandGridSelected")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSearchUrl(Url.Action("SelectedKindingCommand", "InputOfKneadingCommand",
                    new { Area = "KneadingCommand" }))
                .SetDefaultSorting("F39_PreProductCode", SortOrder.Asc)
                .SetFields(
                    new Field("F39_KndEptBgnDate")
                        .SetWidth(20)
                        .SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F39_KndEptBgnDate")
                        .SetTitle("Production Date")
                        .SetItemTemplate("gridHelper.displayDate")
                        .SetWidth(100),
                    new Field("F39_PreProductCode")
                        .SetTitle("Pre-Product  Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("F39_PreProductName")
                        .SetTitle("Pre-Product Name")
                        .SetWidth(100),
                    new Field("F39_PrePdtLotAmt")
                        .SetTitle("Lot Qty")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("Quantity")
                        .SetTitle("Quantity")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("CmdNo")
                        .SetTitle("CmdNo")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("LotNo")
                        .SetTitle("LotNo")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("Status")
                        .SetTitle("Status")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                //new Field("CommandSequenceNo")
                //    .SetTitle("Sequence No")
                //    .SetWidth(100)
                //    .SetItemTemplate("gridHelper.generateNameColumn")
                );
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SelectedKindingCommand(string selectedValue, string date, string deleteCode,
            GridSettings gridSettings)
        {
            if (!string.IsNullOrEmpty(date) && !string.IsNullOrEmpty(deleteCode))
            {
                var deletion = _inputOfKneadingCommandDomain.DeleteKneadingCommand(date, deleteCode);
                if (deletion)
                    return Json(new { Success = true, Message = MessageResource.MSG10 });
                return Json(new { Success = false, Message = MessageResource.MSG6_Kneading });
            }

            if (string.IsNullOrEmpty(selectedValue))
                return Json(new GridResponse<PdtPlnItem>(new List<PdtPlnItem>(), 0), JsonRequestBehavior.AllowGet);
            var result = _inputOfKneadingCommandDomain.SearchCriteriaSelected(selectedValue, gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);

            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Initiate list of kneading commands.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateOrUpdate(InitializeKneadingCommandInputViewModel info)
        {
            try
            {
                _inputOfKneadingCommandDomain.CreateOrUpdate(info.SelectedValue, info.Within, info.LotQuantity);
                return Json(new { Success = true, Message = MessageResource.MSG10 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}