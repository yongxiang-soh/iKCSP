using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Data.DataModel;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.ManagementReport;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.ManagementReport.ViewModels.ManagementReportViewModel;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.ManagementReport.Controllers
{
    [MvcAuthorize("TCFC161P")]
    public class ProductCertificationRecordController : KCSG.Web.Controllers.BaseController
    {

        #region Constructors
        /// <summary>
        ///     Initiate controller with dependency injections.
        /// </summary>
        /// <param name=""></param>
        /// <param name="exportReportDomain"></param>
        public ProductCertificationRecordController(IManagementReportDomain ManagementReportDomain,
            IExportReportDomain exportReportDomain)
        {
            _ManagementReportDomain = ManagementReportDomain;
            _exportReportDomain = exportReportDomain;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Domain which handles input of kneading command function businesses
        /// </summary>
        private readonly IManagementReportDomain _ManagementReportDomain;

        /// <summary>
        ///     Domain which provides functions to export reports.
        /// </summary>
        private readonly IExportReportDomain _exportReportDomain;

        #endregion

        // GET: Inquiry/MaterialStock
        public ActionResult Index()
        {
            var model = new ManagementReportViewModel()
            {
                From = DateTime.Now.ToString("G"),
                To = DateTime.Now.ToString("G")

            };
            return View(model);
        }

                [HttpPost]
        public async Task<ActionResult> ExportProductCertificationRecord(string from, string to)
        {
            try
            {                
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                
                // Find the list of records                                 
                string exportMaterialNameAbsolutePath = "";
                // check status

                    var result =
                    await _ManagementReportDomain.SearchProductCertificationRecord(from,to);
                    // Find the template file.
                    exportMaterialNameAbsolutePath =
                    Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.ProductCertificationRecord));
                    // Read the template.
                    var template = await _exportReportDomain.ReadTemplateFileAsync(exportMaterialNameAbsolutePath);

                    // Export the template.
                    var render = await _exportReportDomain.ExportToFlatFileAsync(result, template);

                var jsonResult = Json(new
                {
                    render
                }, "application/json");
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
                                                               
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [AllowAnonymous]
        [HttpOptions]
        public JsonResult CheckDate(string @from, string to = null)
        {
            var currentdate = DateTime.Now;
            var fromDate = !string.IsNullOrEmpty(from) ? ConvertHelper.ConvertToDateTimeFull(from) : new DateTime();
            var toDate = !string.IsNullOrEmpty(to) ? ConvertHelper.ConvertToDateTimeFull(to) : new DateTime();
            if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to) && (fromDate > toDate))
            {
                return Json(MessageResource.MSG57, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

    }
}