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
using KCSG.Web.Areas.Inquiry.ViewModels.InquiryKneadingCommandNo;
using KCSG.Web.Attributes;

namespace KCSG.Web.Areas.Inquiry.Controllers
{
    [MvcAuthorize("TCFC061F")]
    public class InquiryKneadingCommandNoController : KCSG.Web.Controllers.BaseController
    {
        #region Properties

        /// <summary>
        ///     Domain which provides access to database repositories.
        /// </summary>
        private readonly IInquiryKneadingCommandNoDomain _KneadingCommandNoDomain;

        /// <summary>
        /// Service which is used for handling Identity from request.
        /// </summary>
        private readonly IIdentityService _identityService;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initialize controller with dependency injections.
        /// </summary>
        /// <param name="KneadingCommandNoDomain"></param>
        /// <param name="identityService"></param>
        public InquiryKneadingCommandNoController(IInquiryKneadingCommandNoDomain KneadingCommandNoDomain, IIdentityService identityService)
        {
            _KneadingCommandNoDomain = KneadingCommandNoDomain;
            _identityService = identityService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     This function is for rendering index page of Kneading Start/End Control page.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string cmdNo)
        {
            var KneadingCommandNoViewModel = new KneadingCommandLineViewModel
            {
                Grid = GenerateGrid(),
                KneadingNo = cmdNo != null? cmdNo.Trim():string.Empty,
                //PreProductCode = preCode != null ? preCode.Trim() : string.Empty,
                //PreProductName = preName != null ? preName.Trim() : string.Empty,
            };

            return View(KneadingCommandNoViewModel);
        }

        public ActionResult GetCodeNamebycmNo(string cmdNo)
        {
            var result = _KneadingCommandNoDomain.GetCodeNamebycmNo(cmdNo);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Load kneading commands from database and display 'em to screen.
        /// </summary>
        /// <param name="gridSettings"></param>
        /// <param name="kneadingCommandLine"></param>
        /// <returns></returns>
        [HttpPost]

        public ActionResult SearchByCommandCode(string commandCode, GridSettings gridSettings)
        {
            if (string.IsNullOrEmpty(commandCode))
                return Json(null, JsonRequestBehavior.AllowGet);

            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F42_LotSeqNo";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            var result = _KneadingCommandNoDomain.SearchCriteria(commandCode, gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);



            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }


        #endregion



        #region Private Methods
        private Grid GenerateGrid()
        {
            return new Grid("Grid")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(false)
                .OnDataLoaded("")
                .SetSearchUrl(Url.Action("SearchByCommandCode", "InquiryKneadingCommandNo",
                    new { Area = "Inquiry" }))
                .SetDefaultSorting("F42_LotSeqNo", SortOrder.Asc)
                .SetFields(
                    new Field("F42_PrePdtLotNo")
                        .SetWidth(5)
                        .SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetEditing(false)
                        .SetSorting(false),
                    new Field("F42_PrePdtLotNo")
                        .SetWidth(15)
                        .SetTitle("Pre-prod LotNo")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                   new Field("F42_Status")
                        .SetWidth(20)
                        .SetTitle("Status")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("F42_KndBgnDate")
                        .SetTitle("Knd Cmd Date/Time")
                        .SetItemTemplate("gridHelper.displayDateTimeOnly")
                        .SetWidth(20)
                        .SetSorting(false),
                    new Field("F42_KndEndDate")
                        .SetTitle("Kneaded Date/Time")
                        .SetItemTemplate("gridHelper.displayDateTimeOnly")
                        .SetWidth(20)
                        .SetSorting(false),
                    new Field("F42_TrwEndDate")
                        .SetTitle("Stored Date/Time")
                        .SetItemTemplate("gridHelper.displayDateTimeOnly")
                        .SetWidth(20)
                        .SetSorting(false),
                    new Field("F42_ThrowAmount")
                        .SetTitle("Stored Quantity")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetWidth(15)
                        .SetSorting(false),
                    new Field("F42_StgCtnAmt")
                        .SetTitle("Container Num")
                        .SetItemTemplate("gridHelper.displayNumber")
                        .SetWidth(15)
                        .SetSorting(false)

                );
        }
        #endregion
    }
}