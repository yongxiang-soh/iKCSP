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
    [MvcAuthorize("TCFC081F")]
    public class InquiryKneadingLotNoController : KCSG.Web.Controllers.BaseController
    {
        #region Properties

        /// <summary>
        ///     Domain which provides access to database repositories.
        /// </summary>
        private readonly IInquiryKneadingLotNoDomain _KneadingLotNoDomain;

        /// <summary>
        /// Service which is used for handling Identity from request.
        /// </summary>
        private readonly IIdentityService _identityService;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initialize controller with dependency injections.
        /// </summary>
        /// <param name="KneadingLotNoDomain"></param>
        /// <param name="identityService"></param>
        public InquiryKneadingLotNoController(IInquiryKneadingLotNoDomain KneadingLotNoDomain, IIdentityService identityService)
        {
            _KneadingLotNoDomain = KneadingLotNoDomain;
            _identityService = identityService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     This function is for rendering index page of Kneading Start/End Control page.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string preLotNo,string preCode, string preName)
        {
            var KneadingLotNoViewModel = new KneadingCommandControlViewModel
            {
                Grid = GenerateGrid(),
                PreProLotNo = preLotNo != null ? preLotNo.Trim() : string.Empty,
                PreProductCode = preCode != null ? preCode.Trim() : string.Empty,
                PreProductName = preName != null ? preName.Trim() : string.Empty
            };

            return View(KneadingLotNoViewModel);
        }

        /// <summary>
        ///     Load kneading commands from database and display 'em to screen.
        /// </summary>
        /// <param name="gridSettings"></param>
        /// <param name="KneadingLotLine"></param>
        /// <returns></returns>
        [HttpPost]

        public ActionResult SearchByCommandCode(string preCode, string preLotNo,  GridSettings gridSettings)
        {

            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F56_PrePdtLotNo";
                gridSettings.SortOrder = SortOrder.Asc;
            }

            var result = _KneadingLotNoDomain.SearchCriteria(preCode, preLotNo, gridSettings);
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
                .SetSearchUrl(Url.Action("SearchByCommandCode", "InquiryKneadingLotNo",
                    new { Area = "Inquiry" }))
                .SetDefaultSorting("F56_PrePdtLotNo", SortOrder.Asc)
                .SetFields(
                    new Field("F56_PrePdtLotNo")
                        .SetWidth(15)
                        .SetTitle("Pre-Product LotNo")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                   new Field("F56_ProductCode")
                        .SetWidth(20)
                        .SetTitle("Product Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("F09_ProductDesp")
                        .SetTitle("Product Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(20)
                        .SetSorting(false),
                    new Field("F56_ProductLotNo")
                        .SetTitle("Product LotNo")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(20)
                        .SetSorting(false),
                    new Field("F56_TbtCmdEndAmt")
                        .SetTitle("Tableted Quantity")
                        .SetItemTemplate("gridHelper.displayNumber")
                        .SetWidth(20)
                        .SetSorting(false),
                    new Field("F56_CertificationDate")
                        .SetTitle("Certification Date")
                        .SetItemTemplate("gridHelper.displayDate")
                        .SetWidth(15)
                        .SetSorting(false)
                );
        }
        #endregion
    }
}