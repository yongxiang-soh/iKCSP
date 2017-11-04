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
using KCSG.Domain.Models.Inquiry.ByPreProductName;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.Inquiry.ViewModels.InquiryByMaterialName;
using KCSG.Web.Areas.Inquiry.ViewModels.InquiryByPreProductName;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.Inquiry.Controllers
{
    [MvcAuthorize(new []{ "TCFC012F" , "TCSS017F" })]
    public class InquiryByPreProductNameController : KCSG.Web.Controllers.BaseController
    {
        #region Constructors
        /// <summary>
        ///     Initiate controller with dependency injections.
        /// </summary>
        /// <param name="inputOfKneadingCommandDomain"></param>
        /// <param name="exportReportDomain"></param>
        public InquiryByPreProductNameController(IInquiryByPreProductNameDomain inquiryByPreProductNameDomain,
            IExportReportDomain exportReportDomain, IConfigurationService configurationService)
        {
            _inquiryByPreProductNameDomain = inquiryByPreProductNameDomain;
            _exportReportDomain = exportReportDomain;
            _configurationService = configurationService;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Domain which handles input of kneading command function businesses
        /// </summary>
        private readonly IInquiryByPreProductNameDomain _inquiryByPreProductNameDomain;

        /// <summary>
        ///     Domain which provides functions to export reports.
        /// </summary>
        private readonly IExportReportDomain _exportReportDomain;

        private readonly IConfigurationService _configurationService;

        #endregion

        // GET: Inquiry/InquiryByPreProductName
        public ActionResult Index()
        {
            ViewBag.ScreenId = "TCFC012F";
            var model = new InquiryByPreProductNameSearchViewModel()
            {
                DateTime = DateTime.Now,
                Grid = GenerateGrid()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SearchByPreProductCode(string preProductCode, GridSettings gridSettings)
        {

            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F49_PreProductLotNo";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            if (string.IsNullOrEmpty(preProductCode))
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
            var result = _inquiryByPreProductNameDomain.SearchCriteria(preProductCode, gridSettings, out total);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);



            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SearchByPreProductCodeTotal(string preProductCode, GridSettings gridSettings)
        {
            double total;
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F49_PreProductLotNo";
                gridSettings.SortOrder = SortOrder.Asc;
            }

            var result = _inquiryByPreProductNameDomain.SearchCriteria(preProductCode, gridSettings, out total);

            return Json(new { data = total }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> ExportPreProductName(string status)
        {
            try
            {                
                // Request parameters are invalid.
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                // Find the list of records                                 
                var exportPreProductNameAbsolutePath = "";
                var render = string.Empty;

                // check status
                if (status == "0")
                {
                    var printingNormalResult = new
                    {
                        normal = await _inquiryByPreProductNameDomain.SearchRecordsForPrintingNormal(),
                        companyName = _configurationService.CompanyName,
                        datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                    };
                    
                    // Find the template file.
                    exportPreProductNameAbsolutePath =
                    Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.InquiryByPreProductNameNormal));
                    // Read the template.
                    var printingTemplateNormal = await _exportReportDomain.ReadTemplateFileAsync(exportPreProductNameAbsolutePath);                   

                    // Export the template.
                    render = await _exportReportDomain.ExportToFlatFileAsync(printingNormalResult, printingTemplateNormal);

                    return Json(new
                    {
                        render
                    });
                }
                
                if (status == "1")
                {
                    var printingExternalResult = new
                    {
                        external = await _inquiryByPreProductNameDomain.SearchRecordsForPrintingExternal(),
                        companyName = _configurationService.CompanyName,
                        datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                    };
                    
                    // Find the template file.
                    exportPreProductNameAbsolutePath =
                    Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.InquiryByPreProductNameExternal));
                    // Read the template.
                    var printingExternalTemplate = await _exportReportDomain.ReadTemplateFileAsync(exportPreProductNameAbsolutePath);
                  
                    // Export the template.
                    render = await _exportReportDomain.ExportToFlatFileAsync(printingExternalResult, printingExternalTemplate);

                    return Json(new
                    {
                        render
                    });
                }

                var printingAllResult = new
                {
                    normal = await _inquiryByPreProductNameDomain.SearchRecordsForPrintingNormal(),
                    external = await _inquiryByPreProductNameDomain.SearchRecordsForPrintingExternal(),
                    companyName = _configurationService.CompanyName,
                    datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                };
                
                // Find the template file.
                exportPreProductNameAbsolutePath =
                Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.InquiryByPreProductNameAll));
                
                // Read the template.
                var printingAllResultTemplate = await _exportReportDomain.ReadTemplateFileAsync(exportPreProductNameAbsolutePath);

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

        public string GetPreProductCode(string preProductCode)
        {
            var result = _inquiryByPreProductNameDomain.GetById(preProductCode);
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
                .SetSearchUrl(Url.Action("SearchByPreProductCode", "InquiryByPreProductName",
                    new { Area = "Inquiry" }))
                .SetDefaultSorting("F49_PreProductLotNo", SortOrder.Asc)
                .SetFields(
                    new Field("F49_PreProductLotNo")
                        .SetWidth(50)
                        .SetTitle("Lot No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("ShelfNo")
                        .SetTitle("Shelf No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100)
                        .SetSorting(false),
                    new Field("F49_ContainerCode")
                        .SetTitle("Container Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100)
                        .SetSorting(false),               
                    new Field("F49_Amount")
                        .SetTitle("Quantity")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetWidth(100)  
                        .SetSorting(false)          
                );
        }
        #endregion
    }
}