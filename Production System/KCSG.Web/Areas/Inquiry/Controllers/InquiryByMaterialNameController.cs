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
using KCSG.Domain.Models.Inquiry;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.Inquiry.ViewModels.InquiryByMaterialName;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.Inquiry.Controllers
{
    [MvcAuthorize("TCFC011F")]
    public class InquiryByMaterialNameController : KCSG.Web.Controllers.BaseController
    {

        #region Constructors
        /// <summary>
        ///     Initiate controller with dependency injections.
        /// </summary>
        /// <param name="inputOfKneadingCommandDomain"></param>
        /// <param name="exportReportDomain"></param>
        public InquiryByMaterialNameController(IInquiryByMaterialNameDomain inquiryByMaterialNameDomain,
            IExportReportDomain exportReportDomain, IConfigurationService configurationService)
        {
            _inquiryByMaterialNameDomain = inquiryByMaterialNameDomain;
            _exportReportDomain = exportReportDomain;
            _configurationService = configurationService;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Domain which handles input of kneading command function businesses
        /// </summary>
        private readonly IInquiryByMaterialNameDomain _inquiryByMaterialNameDomain;

        /// <summary>
        ///     Domain which provides functions to export reports.
        /// </summary>
        private readonly IExportReportDomain _exportReportDomain;

        private readonly IConfigurationService _configurationService;

        #endregion

        // GET: Inquiry/InquiryByMaterialName
        public ActionResult Index()
        {
            ViewBag.ScreenId = "TCFC011F";
            var model = new InquiryByMaterialNameSearchViewModel()
            {
               // DateTime1 = DateTime.Now.ToString("G"),
                DateTime1 = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                Grid = GenerateGrid()
                
            };
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SearchByMaterialCode(string materialCode, GridSettings gridSettings)
        {

            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F33_MaterialLotNo";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            if (string.IsNullOrEmpty(materialCode))
            {
                //return Json(new { Success = false, Message = ProductManagementResources.MSG37 });
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            double total;
            var result = _inquiryByMaterialNameDomain.SearchCriteria(materialCode, gridSettings,out total);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);



            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SearchByMaterialCodeTotal(string materialCode, GridSettings gridSettings)
        {
            double total;
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F33_MaterialLotNo";
                gridSettings.SortOrder = SortOrder.Asc;
            }

            var result = _inquiryByMaterialNameDomain.SearchCriteria(materialCode, gridSettings,out total);

            return Json(new { data = total }, JsonRequestBehavior.AllowGet); 
        }

        [HttpPost]
        public async Task<ActionResult> ExportMaterialName(string status)
        {
            try
            {                
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                
                // Find the list of records                                 
                string exportMaterialNameAbsolutePath = "";
                //DateTime saveUtcNow = DateTime.UtcNow;
                
                // check status
                if (status == "0")
                {
                    var result = new
                    {
                        normal = await _inquiryByMaterialNameDomain.SearchRecordsForPrinting(),
                        companyName = _configurationService.CompanyName,
                        datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                    };
                    
                    // Find the template file.
                    exportMaterialNameAbsolutePath =
                    Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.InquiryByMaterialNameNormal));
                    // Read the template.
                    var template = await _exportReportDomain.ReadTemplateFileAsync(exportMaterialNameAbsolutePath);                   
                    
                    // Export the template.
                    var render = await _exportReportDomain.ExportToFlatFileAsync(result, template);

                    return Json(new
                    {
                        render
                    });
                }
                else if (status == "1")
                {
                    var result = new
                    {
                        external = await _inquiryByMaterialNameDomain.SearchRecordsForPrintingBailment(),
                        companyName = _configurationService.CompanyName,
                        datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                    };
                    
                    // Find the template file.
                    exportMaterialNameAbsolutePath =
                    Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.InquiryByMaterialNameBailment));
                    // Read the template.
                    var template = await _exportReportDomain.ReadTemplateFileAsync(exportMaterialNameAbsolutePath);                                     

                    // Export the template.
                    var render = await _exportReportDomain.ExportToFlatFileAsync(result, template);

                    return Json(new
                    {
                        render
                    });
                }
                else
                {
                    var printingAllResult = new
                    {
                        normal = await _inquiryByMaterialNameDomain.SearchRecordsForPrinting(),
                        external = await _inquiryByMaterialNameDomain.SearchRecordsForPrintingBailment(),
                        companyName = _configurationService.CompanyName,
                        datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                    };
                    // Find the template file.
                    exportMaterialNameAbsolutePath =
                    Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.InquiryByMaterialNameAll));
                    // Read the template.
                    var template = await _exportReportDomain.ReadTemplateFileAsync(exportMaterialNameAbsolutePath);                    

                    // Export the template.
                    var render = await _exportReportDomain.ExportToFlatFileAsync(printingAllResult, template);

                    return Json(new
                    {
                        render
                    });
                }                                                
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        public JsonResult GetMateriaCode(string materialCode)
        {
            var result = _inquiryByMaterialNameDomain.GetById(materialCode);                 
            return Json(new { result }, JsonRequestBehavior.AllowGet);
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
                .SetSearchUrl(Url.Action("SearchByMaterialCode", "InquiryByMaterialName",
                    new { Area = "Inquiry" }))
                .SetDefaultSorting("F33_MaterialLotNo", SortOrder.Asc)
                .SetFields(
                    new Field("F33_MaterialLotNo")
                        .SetWidth(50)
                        .SetTitle("Lot No.")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("ShelfNo1")
                        .SetWidth(100)
                        .SetTitle("Shelf No.")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),                        
                    new Field("F33_Amount")
                        .SetTitle("Quantity")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetWidth(100)
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