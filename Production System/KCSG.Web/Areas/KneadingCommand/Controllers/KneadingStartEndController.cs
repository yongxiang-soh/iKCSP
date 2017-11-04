using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces.KneadingCommand;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.KneadingCommand;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.KneadingCommand.ViewModels;
using KCSG.Web.Areas.KneadingCommand.ViewModels.KneadingStartEnd;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.KneadingCommand.Controllers
{
    [MvcAuthorize("TCPS041F")]
    public class KneadingStartEndController : KCSG.Web.Controllers.BaseController
    {
        #region Properties

        /// <summary>
        ///     Domain which provides access to database repositories.
        /// </summary>
        private readonly IKneadingStartEndControlDomain _kneadingStartEndControlDomain;

        /// <summary>
        /// Service which is used for handling Identity from request.
        /// </summary>
        private readonly IIdentityService _identityService;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initialize controller with dependency injections.
        /// </summary>
        /// <param name="kneadingStartEndControlDomain"></param>
        /// <param name="identityService"></param>
        public KneadingStartEndController(IKneadingStartEndControlDomain kneadingStartEndControlDomain, IIdentityService identityService)
        {
            _kneadingStartEndControlDomain = kneadingStartEndControlDomain;
            _identityService = identityService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     This function is for rendering index page of Kneading Start/End Control page.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var kneadingStartEndControlViewModel = new KneadingCommandControlViewModel
            {
                Grid = InitializeKneadingCommandsGrid()
            };

            return View(kneadingStartEndControlViewModel);
        }

        /// <summary>
        ///     Load kneading commands from database and display 'em to screen.
        /// </summary>
        /// <param name="gridSettings"></param>
        /// <param name="kneadingCommandLine"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Load(GridSettings gridSettings, Constants.KndLine kneadingCommandLine = Constants.KndLine.Conventional)
        {
            try
            {
                // Request parameters to server are invalid.
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(FindValidationErrors(ModelState));
                }

                var loadKneadingCommandResult = await _kneadingStartEndControlDomain.LoadKneadingCommands(gridSettings, kneadingCommandLine);
                return Json(loadKneadingCommandResult.Data);
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// This function is for interupting kneading commands.
        /// </summary>
        /// <param name="kneadingCommands"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Interrupt(FindKneadingCommandItem[]kneadingCommands)
        {
            try
            {
                // Request parameters to server are invalid.
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = (int) HttpStatusCode.BadRequest;
                    return Json(FindValidationErrors(ModelState));
                }
                
                // Find the terminal number in request.
                var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);

                await _kneadingStartEndControlDomain.InterruptKneadingCommand(terminalNo, kneadingCommands);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Start kneading command.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Start(StartKneadingCommandViewModel item)
        {
            try
            {
                // Request parameters to server are invalid.
                if (!ModelState.IsValid)
                {
                    Response.StatusCode = (int) HttpStatusCode.BadRequest;
                    return Json(FindValidationErrors(ModelState));
                }

                // Find terminal number from request.
                var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);

                // Trim item information.
                foreach (var kneadingCommand in item.KneadingCommands)
                {
                    if (!string.IsNullOrEmpty(kneadingCommand.F03_PreProductName))
                        kneadingCommand.F03_PreProductName = kneadingCommand.F03_PreProductName.Trim();

                    if (!string.IsNullOrEmpty(kneadingCommand.F39_ColorClass))
                        kneadingCommand.F39_ColorClass = kneadingCommand.F39_ColorClass.Trim();

                    if (!string.IsNullOrEmpty(kneadingCommand.F42_KndCmdNo))
                        kneadingCommand.F42_KndCmdNo = kneadingCommand.F42_KndCmdNo.Trim();

                    if (!string.IsNullOrEmpty(kneadingCommand.F42_PreProductCode))
                        kneadingCommand.F42_PreProductCode = kneadingCommand.F42_PreProductCode.Trim();

                    if (!string.IsNullOrEmpty(kneadingCommand.KneadingStatus))
                        kneadingCommand.KneadingStatus = kneadingCommand.KneadingStatus.Trim();

                    if (!string.IsNullOrEmpty(kneadingCommand.LotNo))
                        kneadingCommand.LotNo = kneadingCommand.LotNo.Trim();

                    if (!string.IsNullOrEmpty(kneadingCommand.ProductStatus))
                        kneadingCommand.ProductStatus = kneadingCommand.ProductStatus.Trim();
                }
                await
                    _kneadingStartEndControlDomain.StartKneadingCommand(terminalNo, item.KneadingCommands,
                        item.KneadingLine);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                if (exception.Message.Equals("MSG12", StringComparison.InvariantCultureIgnoreCase))
                {
                    Response.Headers["x-process-error"] = MessageResource.MSG12;
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Stop kneading command process execution.
        /// </summary>
        /// <param name="kneadingCommand"></param>
        /// <returns></returns>
        public ActionResult Stop(FindKneadingCommandItem kneadingCommand)
        {
            try
            {
                if (kneadingCommand == null)
                {
                    kneadingCommand = new FindKneadingCommandItem();
                    TryValidateModel(kneadingCommand);
                }

                if (!ModelState.IsValid)
                {
                    Response.StatusCode = (int) HttpStatusCode.BadRequest;
                    return Json(FindValidationErrors(ModelState));
                }

                // Trim everything.
                if (!string.IsNullOrEmpty(kneadingCommand.F03_PreProductName))
                    kneadingCommand.F03_PreProductName = kneadingCommand.F03_PreProductName.Trim();

                if (!string.IsNullOrEmpty(kneadingCommand.F39_ColorClass))
                kneadingCommand.F39_ColorClass = kneadingCommand.F39_ColorClass.Trim();

                if (!string.IsNullOrEmpty(kneadingCommand.F42_KndCmdNo))
                kneadingCommand.F42_KndCmdNo = kneadingCommand.F42_KndCmdNo.Trim();

                if (!string.IsNullOrEmpty(kneadingCommand.F42_PreProductCode))
                kneadingCommand.F42_PreProductCode = kneadingCommand.F42_PreProductCode.Trim();

                if (!string.IsNullOrEmpty(kneadingCommand.KneadingStatus))
                kneadingCommand.KneadingStatus = kneadingCommand.KneadingStatus.Trim();

                if (!string.IsNullOrEmpty(kneadingCommand.LotNo))
                kneadingCommand.LotNo = kneadingCommand.LotNo.Trim();

                if (!string.IsNullOrEmpty(kneadingCommand.ProductStatus))
                kneadingCommand.ProductStatus = kneadingCommand.ProductStatus.Trim();
                
                // Find terminal from request.
                var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
                _kneadingStartEndControlDomain.StopKneadingCommand(terminalNo, kneadingCommand);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                if (exception.Message.Equals("MSG12", StringComparison.InvariantCultureIgnoreCase))
                {
                    Response.Headers["x-process-error"] = MessageResource.MSG12;
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }
        
        #endregion

        #region Private Methods

        /// <summary>
        ///     Find validation errors and construct 'em as json object.
        /// </summary>
        /// <param name="modelStateDictionary"></param>
        /// <returns></returns>
        private IDictionary<string, string[]> FindValidationErrors(ModelStateDictionary modelStateDictionary)
        {
            return modelStateDictionary.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());
        }

        /// <summary>
        ///     This function is for generating a grid which contains kneading commands.
        /// </summary>
        /// <returns></returns>
        private Grid InitializeKneadingCommandsGrid()
        {
            return new Grid("KneadingStartEndControl")
                .SetMode(GridMode.Listing)
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetDefaultSorting("F42_KndCmdNo", SortOrder.Asc)
                .OnDataLoaded("kneadingCommandsLoaded")
                .SetSearchUrl(Url.Action("Load", "KneadingStartEnd", new { Area = "KneadingCommand" }))
                .SetFields(
                    new Field("F42_KndCmdNo")
                        .SetWidth(20)
                        .SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetEditing(false),
                    new Field("F42_KndCmdNo")
                        .SetTitle("CmdNo")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("F42_PreProductCode")
                        .SetWidth(100)
                        .SetTitle("Pre-Product  Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("F03_PreProductName")
                        .SetWidth(100)
                        .SetTitle("Pre-Product Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("ProductStatus")
                        .SetTitle("Product Status")
                        .SetItemTemplate("gridHelper.initiateProductStatus")
                        .SetWidth(100)
                        .SetSorting(false),
                    new Field("LotNo")
                        .SetTitle("LotNo")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100)
                        .SetSorting(false),
                    new Field("ColorClass")
                        .SetTitle("Colour")
                        .SetWidth(100)
                        .SetSorting(false),
                    new Field("KneadingStatus")
                        .SetTitle("Kneading Status")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateKneadingCommandStatus")
                        .SetSorting(false),
                    new Field("UpdatedDate1")
                        .SetTitle("UpdatedDate1")
                        .SetItemTemplate("gridHelper.displayDateTime")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetVisible(false),
                    new Field("UpdateDate2")
                        .SetTitle("UpdateDate2")
                        .SetItemTemplate("gridHelper.displayDateTime")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetVisible(false),
                    new Field("ProductionDate")
                        .SetTitle("ProductionDate")
                        .SetItemTemplate("gridHelper.displayDateTime")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetVisible(false),
                    new Field("F42_CommandSeqNo")
                        .SetTitle("F42_CommandSeqNo")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetVisible(false),
                    new Field("F42_LotSeqNo")
                        .SetTitle("F42_LotSeqNo")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetVisible(false)
                );
        }

        #endregion
    }
}