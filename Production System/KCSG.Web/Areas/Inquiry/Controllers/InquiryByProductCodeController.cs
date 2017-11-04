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
using KCSG.Web.Areas.Inquiry.ViewModels.InquiryByProductCode;
using KCSG.Web.Attributes;

namespace KCSG.Web.Areas.Inquiry.Controllers
{
    [MvcAuthorize("TCFC051F")]
    public class InquiryByProductCodeController : KCSG.Web.Controllers.BaseController
    {

        #region Properties

        /// <summary>
        ///     Domain which handles input of kneading command function businesses
        /// </summary>
        private readonly IInquiryByProductDomain _inquiryByProductCodeDomain;

        /// <summary>
        ///     Domain which provides functions to export reports.
        /// </summary>
        //private readonly IExportReportDomain _exportReportDomain;

        #endregion

        #region Constructors
        /// <summary>
        ///     Initiate controller with dependency injections.
        /// </summary>

        public InquiryByProductCodeController(IInquiryByProductDomain inquiryByProductCodeDomain,
            IExportReportDomain exportReportDomain)
        {
            _inquiryByProductCodeDomain = inquiryByProductCodeDomain;
            //_exportReportDomain = exportReportDomain;
        }

        #endregion

        // GET: Inquiry/InquiryByPreProductCode
        public ActionResult Index()
        {
            ViewBag.ScreenId = "TCFC051F";
            var model = new InquiryByProductCodeSearchViewModel()
            {
                Grid = GenerateGrid()
            };
            return View(model);
        }

        public ActionResult SearchByProductCode(string productCode, GridSettings gridSettings)
        {

            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F56_KndCmdNo";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            //if (!model.ma.HasValue)
            //{
            //    return Json(new GridResponse<MaterialShelfStatusItem>(new List<MaterialShelfStatusItem>(), 0), JsonRequestBehavior.AllowGet);
            //}
            //var dateCurrent = DateTime.Now;
            //var date = dateCurrent.AddDays(model.Within ?? 0);
            double total;
            var result = _inquiryByProductCodeDomain.SearchCriteria(productCode, gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);



            return Json(result.Data, JsonRequestBehavior.AllowGet);
        } public ActionResult Searchuser(string proCode, string proName)
        {
            var result = _inquiryByProductCodeDomain.Searchuser(proCode.Trim(), proName.Trim());
            if (string.IsNullOrEmpty(result))
                return Json(null, JsonRequestBehavior.AllowGet);


            return Json(new { data = result }, JsonRequestBehavior.AllowGet);

        }

        //public ActionResult SearchByPreProductCodeTotal(string materialCode, string lotNo, GridSettings gridSettings)
        //{
        //    double total;
        //    if (string.IsNullOrEmpty(gridSettings.SortField))
        //    {
        //        gridSettings.SortField = "F56_KndCmdNo";
        //        gridSettings.SortOrder = SortOrder.Asc;
        //    }

        //    var result = _inquiryByProductCodeDomain.SearchCriteria(materialCode, gridSettings);

        //    return Json(new { data = total }, JsonRequestBehavior.AllowGet);
        //}



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
                .SetSearchUrl(Url.Action("SearchByProductCode", "InquiryByProductCode",
                    new { Area = "Inquiry" }))
                .SetDefaultSorting("F56_KndCmdNo", SortOrder.Asc)
                .SetFields(
                    new Field("F56_KndCmdNo")
                        .SetWidth(20)
                        .SetTitle("Command No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                   new Field("F56_ProductLotNo")
                        .SetWidth(20)
                        .SetTitle("Product LotNo")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("F56_Status")
                        .SetTitle("Status")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(15)
                        .SetSorting(false),
                    new Field("F56_TbtCmdAmt")
                        .SetTitle("Tablet Cmd Qty")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetWidth(20)
                        .SetSorting(false),
                    new Field("F56_TbtCmdEndAmt")
                        .SetTitle("Tabletised Qty")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetWidth(20)
                        .SetSorting(false),
                    new Field("F56_TbtBgnDate")
                        .SetTitle("Tablet Begin Date/Time")
                        .SetItemTemplate("gridHelper.displayDateTimeOnly")
                        .SetWidth(25)
                        .SetSorting(false),
                    new Field("F56_TbtEndDate")
                        .SetTitle("Tablet End Date/Time")
                        .SetItemTemplate("gridHelper.displayDateTimeOnly")
                        .SetWidth(25)
                        .SetSorting(false),
                    new Field("F56_AddDate")
                        .SetTitle("WareHouse Date/Time")
                        .SetItemTemplate("gridHelper.displayDateTimeOnly")
                        .SetWidth(25)
                        .SetSorting(false),
                    new Field("F56_CertificationDate")
                        .SetTitle("Certification Date")
                        .SetItemTemplate("gridHelper.displayDate")
                        .SetWidth(20)
                        .SetSorting(false),
                    new Field("F56_ShipDate")
                        .SetTitle("Delivered Date/Time")
                        .SetItemTemplate("gridHelper.displayDateTimeOnly")
                        .SetWidth(25)
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