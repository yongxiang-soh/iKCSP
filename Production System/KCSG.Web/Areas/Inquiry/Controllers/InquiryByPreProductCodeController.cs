using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Data.DataModel;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.Inquiry;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.Domain.Interfaces.Inquiry;
using KCSG.Domain.Models.Inquiry;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.Inquiry.ViewModels.InquiryByMaterialName;
using KCSG.Web.Areas.Inquiry.ViewModels.InquiryByPreProductCode;
using KCSG.Web.Attributes;

namespace KCSG.Web.Areas.Inquiry.Controllers
{
    [MvcAuthorize(new []{ "TCFC041F", "TCSS017F" })]
    public class InquiryByPreProductCodeController : KCSG.Web.Controllers.BaseController
    {

        #region Properties

        /// <summary>
        ///     Domain which handles input of kneading command function businesses
        /// </summary>
        private readonly IInquiryByPreProductDomain _inquiryByPreProductCodeDomain;

        /// <summary>
        ///     Domain which provides functions to export reports.
        /// </summary>
        //private readonly IExportReportDomain _exportReportDomain;

        #endregion

        #region Constructors
        /// <summary>
        ///     Initiate controller with dependency injections.
        /// </summary>

        public InquiryByPreProductCodeController(IInquiryByPreProductDomain inquiryByPreProductCodeDomain,
            IExportReportDomain exportReportDomain)
        {
            _inquiryByPreProductCodeDomain = inquiryByPreProductCodeDomain;
            //_exportReportDomain = exportReportDomain;
        }

        #endregion

        // GET: Inquiry/InquiryByPreProductCode
        public ActionResult Index()
        {
            ViewBag.ScreenId = "TCFC041F";
            var model = new InquiryByPreProductCodeSearchViewModel()
            {
                Grid = GenerateGrid()
            };
            return View(model);
        }

        public ActionResult SearchByPreProductCode(string materialCode, string lotNo, GridSettings gridSettings)
        {

            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F43_MaterialCode";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            //if (!model.ma.HasValue)
            //{
            //    return Json(new GridResponse<MaterialShelfStatusItem>(new List<MaterialShelfStatusItem>(), 0), JsonRequestBehavior.AllowGet);
            //}
            //var dateCurrent = DateTime.Now;
            //var date = dateCurrent.AddDays(model.Within ?? 0);
            double total;
            var result = _inquiryByPreProductCodeDomain.SearchCriteria(materialCode,lotNo, gridSettings, out total);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);



            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchByPreProductCodeTotal(string materialCode, string lotNo, GridSettings gridSettings)
        {
            double total;
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F43_MaterialCode";
                gridSettings.SortOrder = SortOrder.Asc;
            }

            var result = _inquiryByPreProductCodeDomain.SearchCriteria(materialCode, lotNo, gridSettings, out total);

            return Json(new { data = total.ToString("F") }, JsonRequestBehavior.AllowGet);
        }



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
                .OnDataLoaded("granTotalLoaded")
                .SetSearchUrl(Url.Action("SearchByPreProductCode", "InquiryByPreProductCode",
                    new { Area = "Inquiry" }))
                .SetDefaultSorting("F43_MaterialCode", SortOrder.Asc)
                .SetFields(
                    new Field("F43_MaterialCode")
                        .SetWidth(100)
                        .SetTitle("Material Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                   new Field("F01_Materialdsp")
                        .SetWidth(100)
                        .SetTitle("Material Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("F43_MaterialLotNo")
                        .SetTitle("Lot No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(50)
                        .SetSorting(false),
                    new Field("F43_LayinginAmount")
                        .SetTitle("Quantity")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetWidth(80)
                        .SetSorting(false)
                //new Field("CommandSequenceNo")
                //    .SetTitle("Sequence No")
                //    .SetWidth(100)
                //    .SetItemTemplate("gridHelper.generateNameColumn")
                );
        }
        #endregion
    }
}