using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Domain.Domains.Inquiry;
using KCSG.Domain.Interfaces.KneadingCommand;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.KneadingCommand;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.KneadingCommand.ViewModels;
using KCSG.Web.Areas.KneadingCommand.ViewModels.KneadingStartEnd;
using KCSG.Web.Attributes;

namespace KCSG.Web.Areas.Inquiry.Controllers
{
    [MvcAuthorize("TCFC071F")]
    public class InquiryKneadingLineNoController : KCSG.Web.Controllers.BaseController
    {
        #region Properties

        /// <summary>
        ///     Domain which provides access to database repositories.
        /// </summary>
        private readonly IInquiryKneadingLineNoDomain _kneadingLineNoDomain;

        /// <summary>
        /// Service which is used for handling Identity from request.
        /// </summary>
        private readonly IIdentityService _identityService;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initialize controller with dependency injections.
        /// </summary>
        /// <param name="kneadingLineNoDomain"></param>
        /// <param name="identityService"></param>
        public InquiryKneadingLineNoController(IInquiryKneadingLineNoDomain kneadingLineNoDomain, IIdentityService identityService)
        {
            _kneadingLineNoDomain = kneadingLineNoDomain;
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
            var kneadingLineNoViewModel = new KneadingCommandControlViewModel
            {
                Grid = InitializeKneadingCommandsGrid()
            };

            return View(kneadingLineNoViewModel);
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

                var loadKneadingCommandResult = await _kneadingLineNoDomain.LoadKneadingCommands(gridSettings, kneadingCommandLine);
                return Json(loadKneadingCommandResult.Data);
            }
            catch (Exception exception)
            {
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
                .SetDefaultSorting("F39_KndCmdNo", SortOrder.Asc)
                .OnDataLoaded("kneadingCommandsLoaded")
                .SetSearchUrl(Url.Action("Load", "InquiryKneadingLineNo", new { Area = "Inquiry" }))
                .SetFields(
                    new Field("F39_KndCmdNo")
                        .SetWidth(20)
                        .SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetEditing(false),
                    new Field("F39_KndCmdNo")
                        .SetTitle("Command No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("F39_PreProductCode")
                        .SetWidth(100)
                        .SetTitle("Pre-Product Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("F03_PreProductName")
                        .SetWidth(100)
                        .SetTitle("Pre-Product Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("LotNo")
                        .SetTitle("Start LotNo")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100)
                        .SetSorting(false),
                    new Field("MaterialAmount")
                        .SetTitle("Lot Quantity")
                        .SetItemTemplate("gridHelper.displayNumber")
                        .SetWidth(100)
                        .SetSorting(false),
                    new Field("F39_Status")
                        .SetTitle("Status")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)

                );
        }

        #endregion
    }
}