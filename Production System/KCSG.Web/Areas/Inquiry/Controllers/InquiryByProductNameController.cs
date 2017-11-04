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
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.Inquiry.ViewModels.InquiryByProductName;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.Inquiry.Controllers
{
    [MvcAuthorize(new []{ "TCFC014F" , "TCSS018F" })]
    public class InquiryByProductNameController : KCSG.Web.Controllers.BaseController
    {
        #region Constructors
        /// <summary>
        ///     Initiate controller with dependency injections.
        /// </summary>
        /// <param name="inputOfKneadingCommandDomain"></param>
        /// <param name="exportReportDomain"></param>
        public InquiryByProductNameController(IInquiryByProductNameDomain inquiryPreProductNameDomain,
            IExportReportDomain exportReportDomain, IConfigurationService configurationService)
        {
            _inquiryByProductNameDomain = inquiryPreProductNameDomain;
            _exportReportDomain = exportReportDomain;
            _configurationService = configurationService;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Domain which handles input of kneading command function businesses
        /// </summary>
        private readonly IInquiryByProductNameDomain _inquiryByProductNameDomain;

        /// <summary>
        ///     Domain which provides functions to export reports.
        /// </summary>
        private readonly IExportReportDomain _exportReportDomain;

        private readonly IConfigurationService _configurationService;

        #endregion
        // GET: Inquiry/InquiryByProductName
        public ActionResult Index()
        {
            ViewBag.ScreenId = "TCFC014F";
            var model = new InquiryByProductNameSearchViewModel()
            {
                DateTime = DateTime.Now,
                Grid = GenerateGrid()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SearchByProductCode(string productCode, GridSettings gridSettings)
        {
            double total;
            double deliveryTotal;
            double cerTotal;
            double nonCerTotal;
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "ProductLotNo";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            if (string.IsNullOrEmpty(productCode))
            {
                //return Json(new { Success = false, Message = ProductManagementResources.MSG37 });
                return Json(null, JsonRequestBehavior.AllowGet);
            }                        
            var result = _inquiryByProductNameDomain.SearchCriteria(productCode, gridSettings, out total, out deliveryTotal, out cerTotal, out nonCerTotal);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);



            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }

        public class TotalProducts
        {
            public double Total { get; set; }
            public double DeliveryTotal { get; set; }
            public double DerTotal { get; set; }
            public double NonCerTotal { get; set; }
        }
        public ActionResult SearchByExtPreProductCodeTotal(string productCode, GridSettings gridSettings)
        {
            double total;
            double deliveryTotal;
            double cerTotal;
            double nonCerTotal;
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "ProductLotNo";
                gridSettings.SortOrder = SortOrder.Asc;
            }

            var result = _inquiryByProductNameDomain.SearchCriteria(productCode, gridSettings, out total, out deliveryTotal, out cerTotal, out nonCerTotal);
            var totalProducts=new TotalProducts()
            {
                Total=total,
                DeliveryTotal=deliveryTotal,
                DerTotal = cerTotal,
                NonCerTotal = nonCerTotal
            };
            return Json(new { data = totalProducts }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> ExportExtProductName(string status,int type)
        {
            try
            {
                // Request parameters are invalid.
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                // Find the list of records                                 
                var exportProductNameAbsolutePath = "";
                var render = string.Empty;
                // check status
                if (status == "0")
                {
                    var printingCertifiedResult = new
                    {
                        Certified = await _inquiryByProductNameDomain.SearchRecordsForPrintingCertified(),
                        companyName = _configurationService.CompanyName,
                        datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                    };
                    
                    // Find the template file.
                    exportProductNameAbsolutePath =
                    Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.InquiryByProductNameCertified));
                    
                    // Read the template.
                    var printingTemplateCertified = await _exportReportDomain.ReadTemplateFileAsync(exportProductNameAbsolutePath);                    

                    // Export the template.
                    render = await _exportReportDomain.ExportToFlatFileAsync(printingCertifiedResult, printingTemplateCertified);

                    return Json(new
                    {
                        render
                    });
                }
                else if (status == "1")
                {
                    var printingNotCertifiedResult = new
                    {
                        notCertified = await _inquiryByProductNameDomain.SearchRecordsForPrintingNotCertified(),
                        companyName = _configurationService.CompanyName,
                        datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                    };

                    // Find the template file.
                    exportProductNameAbsolutePath =
                    Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.InquiryByProductNameNotCertified));
                    // Read the template.
                    var printingNotCertifiedTemplate = await _exportReportDomain.ReadTemplateFileAsync(exportProductNameAbsolutePath);                   

                    // Export the template.
                    render = await _exportReportDomain.ExportToFlatFileAsync(printingNotCertifiedResult, printingNotCertifiedTemplate);

                    return Json(new
                    {
                        render
                    });
                }
                var printingAllResult = new
                {
                    Certified = await _inquiryByProductNameDomain.SearchRecordsForPrintingCertified(),
                    NotCertified = await _inquiryByProductNameDomain.SearchRecordsForPrintingNotCertified(),
                    companyName = _configurationService.CompanyName,
                    datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                };

                //#if DEBUG

                //                var externals = printingAllResult.external;
                //                var normals = printingAllResult.normal;
                //#endif
                // Find the template file.
                exportProductNameAbsolutePath =
                Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.InquiryByProductNameAll));

                // Read the template.
                var printingAllResultTemplate = await _exportReportDomain.ReadTemplateFileAsync(exportProductNameAbsolutePath);

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

        [HttpPost]
        public async Task<ActionResult> ExportExtProductNameReport(string status, int type)
        {
            try
            {
                // Request parameters are invalid.
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                // Find the list of records                                 
                var exportProductNameAbsolutePath = "";
                var render = string.Empty;
                // check status
                if (status == "0")
                {
                    var printingCertifiedResult = new
                    {
                        Certified = await _inquiryByProductNameDomain.SearchRecordsForPrintingCertified(),
                        companyName = _configurationService.CompanyName,
                        datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                    };

                    // Find the template file.
                    exportProductNameAbsolutePath =
                    Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.InquiryByProductNameCertifiedReport));

                    // Read the template.
                    var printingTemplateCertified = await _exportReportDomain.ReadTemplateFileAsync(exportProductNameAbsolutePath);

                    // Export the template.
                    render = await _exportReportDomain.ExportToFlatFileAsync(printingCertifiedResult, printingTemplateCertified);

                    return Json(new
                    {
                        render
                    });
                }
                else if (status == "1")
                {
                    var printingNotCertifiedResult = new
                    {
                        notCertified = await _inquiryByProductNameDomain.SearchRecordsForPrintingNotCertified(),
                        companyName = _configurationService.CompanyName,
                        datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                    };

                    // Find the template file.
                    exportProductNameAbsolutePath =
                    Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.ReportByProductNameCertifiedReport));
                    // Read the template.
                    var printingNotCertifiedTemplate = await _exportReportDomain.ReadTemplateFileAsync(exportProductNameAbsolutePath);

                    // Export the template.
                    render = await _exportReportDomain.ExportToFlatFileAsync(printingNotCertifiedResult, printingNotCertifiedTemplate);

                    return Json(new
                    {
                        render
                    });
                }
                var printingAllResult = new
                {
                    Certified = await _inquiryByProductNameDomain.SearchRecordsForPrintingCertified(),
                    NotCertified = await _inquiryByProductNameDomain.SearchRecordsForPrintingNotCertified(),
                    companyName = _configurationService.CompanyName,
                    datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                };

                //#if DEBUG

                //                var externals = printingAllResult.external;
                //                var normals = printingAllResult.normal;
                //#endif
                // Find the template file.
                exportProductNameAbsolutePath =
                Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.InquiryByProductNameAll));

                // Read the template.
                var printingAllResultTemplate = await _exportReportDomain.ReadTemplateFileAsync(exportProductNameAbsolutePath);

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

        public string GetProductCode(string productCode)
        {
            var result = _inquiryByProductNameDomain.GetById(productCode);
            //return Json(new { result }, JsonRequestBehavior.AllowGet);
            if (string.IsNullOrEmpty(result))
            {
                //return Resources.ProductManagementResources.MSG1 ;
                return "";
            }
            return result;
            return string.Empty;
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
                .SetSearchUrl(Url.Action("SearchByProductCode", "InquiryByProductName",
                    new { Area = "Inquiry" }))
                .SetDefaultSorting("ProductLotNo", SortOrder.Asc)
                .SetFields(
                    new Field("ProductLotNo")
                        .SetWidth(50)
                        .SetTitle("LotNo")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("ShelfNo1")
                        .SetTitle("Shelf No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100)
                        .SetSorting(false),
                    new Field("PreProdLotNo")
                        .SetTitle("Pre-prod LotNo")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100)
                        .SetSorting(false),
                    new Field("Amount")
                        .SetTitle("Quantity")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetWidth(100)
                        .SetSorting(false),
                    new Field("CerfDate")
                        .SetTitle("Certification Date")
                        .SetItemTemplate("gridHelper.displayDate")
                        .SetWidth(100)
                        .SetSorting(false),
                    new Field("Flag")
                        .SetTitle("Certification Flag")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100)
                        .SetSorting(false)
                );
        }
        #endregion
    }
}