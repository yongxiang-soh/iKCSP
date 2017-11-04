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
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.Inquiry.ByExternalPre_ProductName;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.Inquiry.ViewModels.InquiryByExternalPreProductName;
using KCSG.Web.Areas.Inquiry.ViewModels.InquiryBySupplierName;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.Inquiry.Controllers
{
    [MvcAuthorize("TCFC015F")]
    public class InquiryBySupplierNameController : KCSG.Web.Controllers.BaseController
    {
        #region Constructors
        /// <summary>
        ///     Initiate controller with dependency injections.
        /// </summary>
        /// <param name="inputOfKneadingCommandDomain"></param>
        /// <param name="exportReportDomain"></param>
        public InquiryBySupplierNameController(IInquiryBySupplierNameDomain inquiryBySupplierNameDomain,
            IExportReportDomain exportReportDomain, IConfigurationService configurationService)
        {
            _inquiryBySupplierNameDomain = inquiryBySupplierNameDomain;
            _exportReportDomain = exportReportDomain;
            _configurationService = configurationService;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Domain which handles input of kneading command function businesses
        /// </summary>
        private readonly IInquiryBySupplierNameDomain _inquiryBySupplierNameDomain;

        /// <summary>
        ///     Domain which provides functions to export reports.
        /// </summary>
        private readonly IExportReportDomain _exportReportDomain;

        private readonly IConfigurationService _configurationService;

        #endregion
        // GET: Inquiry/InquiryBySupplierName
        public ActionResult Index()
        {
            ViewBag.ScreenId = "TCFC015F";
            var model = new InquiryBySupplierNameSearchViewModel()
            {
                DateTime = DateTime.Now,
                Grid = GenerateGrid()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SearchBySupplierCode(string supplierCode, GridSettings gridSettings)
        {

            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "MaterialCode";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            if (string.IsNullOrEmpty(supplierCode))
            {
                //return Json(new { Success = false, Message = ProductManagementResources.MSG37 });
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            //if (!model.ma.HasValue)
            //{
            //    return Json(new GridResponse<MaterialShelfStatusItem>(new List<MaterialShelfStatusItem>(), 0), JsonRequestBehavior.AllowGet);
            //}
            //var dateCurrent = DateTime.Now;
            //var date = dateCurrent.AddDays(model.Within ?? 0);
            double total = 0;
            var result = _inquiryBySupplierNameDomain.SearchCriteria(supplierCode, gridSettings, out total);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);



            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchBySupplierCodeTotal(string supplierCode, GridSettings gridSettings)
        {
            double total;
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "MaterialCode";
                gridSettings.SortOrder = SortOrder.Asc;
            }

            var result = _inquiryBySupplierNameDomain.SearchCriteria(supplierCode, gridSettings, out total);

            return Json(new { data = total }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> ExportSupplierName(string status)
        {
            try
            {
                // Request parameters are invalid.
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                // Find the list of records                                 
                var exportSupplierNameAbsolutePath = "";
                var render = string.Empty;
                // check status
                if (status == "0")
                {
                    var printingNormalResult = new
                    {
                        normal = await _inquiryBySupplierNameDomain.SearchRecordsForPrintingNormal(),
                        companyName = _configurationService.CompanyName,
                        datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                    };
                    // Find the template file.
                    exportSupplierNameAbsolutePath =
                    Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.InquiryBySupplierNameNormal));
                    // Read the template.
                    var printingTemplateNormal = await _exportReportDomain.ReadTemplateFileAsync(exportSupplierNameAbsolutePath);

                  

                    // Export the template.
                    render = await _exportReportDomain.ExportToFlatFileAsync(printingNormalResult, printingTemplateNormal);

                    return Json(new
                    {
                        render
                    });
                }
                else if (status == "1")
                {
                    var printingBailmentResult = new
                    {
                        bailment = await _inquiryBySupplierNameDomain.SearchRecordsForPrintingBailment(),
                        companyName = _configurationService.CompanyName,
                        datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                    };
                    // Find the template file.
                    exportSupplierNameAbsolutePath =
                    Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.InquiryBySupplierNameBailment));
                    // Read the template.
                    var printingBailmentTemplate = await _exportReportDomain.ReadTemplateFileAsync(exportSupplierNameAbsolutePath);

               
                    // Export the template.
                    render = await _exportReportDomain.ExportToFlatFileAsync(printingBailmentResult, printingBailmentTemplate);

                    return Json(new
                    {
                        render
                    });
                }
                var printingAllResult = new
                {
                    normal = await _inquiryBySupplierNameDomain.SearchRecordsForPrintingNormal(),
                    bailment = await _inquiryBySupplierNameDomain.SearchRecordsForPrintingBailment(),
                    companyName = _configurationService.CompanyName,
                    datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                };

                // Find the template file.
                exportSupplierNameAbsolutePath =
                Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.InquiryBySupplierNameAll));

                // Read the template.
                var printingAllResultTemplate = await _exportReportDomain.ReadTemplateFileAsync(exportSupplierNameAbsolutePath);

                // Export the template.
                render = await _exportReportDomain.ExportToFlatFileAsync(printingAllResult, printingAllResultTemplate);

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

        public string GetSupplierCode(string supplierCode)
        {
            var result = _inquiryBySupplierNameDomain.GetById(supplierCode);
            //return Json(new { result }, JsonRequestBehavior.AllowGet);
            if (string.IsNullOrEmpty(result))
            {
                //return Resources.ProductManagementResources.MSG1 ;
                return "";
            }
            return result;
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
                .SetSearchUrl(Url.Action("SearchBySupplierCode", "InquiryBySupplierName",
                    new { Area = "Inquiry" }))
                .SetDefaultSorting("MaterialCode", SortOrder.Asc)
                .SetFields(
                    new Field("MaterialCode")
                        .SetWidth(50)
                        .SetTitle("Material Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("MaterialName")
                        .SetTitle("Material Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100)
                        .SetSorting(false),
                    new Field("F33_MaterialLotNo")
                        .SetTitle("LotNo")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100)
                        .SetSorting(false),
                    new Field("ShelfNo1")
                        .SetTitle("Shelf No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100)
                        .SetSorting(false),
                    new Field("F33_Amount")
                        .SetTitle("Quantity")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetWidth(100)
                        .SetSorting(false)
                );
        }
        #endregion
    }
}