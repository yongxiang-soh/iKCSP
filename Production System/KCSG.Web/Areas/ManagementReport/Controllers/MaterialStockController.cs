using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Data.DataModel;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.ManagementReport;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.ManagementReport.ViewModels.MaterialStock;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.ManagementReport.Controllers
{
    [MvcAuthorize(new []{ "TCFC011F", "TCSS016F", "TCFC083P", "TCSS017F", "TCSS018F", "TCFC091P", "TCFC092P", "TCFC093P", "TCFC094P", "TCFC111P" })]
    public class MaterialStockController : KCSG.Web.Controllers.BaseController
    {

        #region Constructors
        /// <summary>
        ///     Initiate controller with dependency injections.
        /// </summary>
        /// <param name="inputOfKneadingCommandDomain"></param>
        /// <param name="exportReportDomain"></param>
        public MaterialStockController(IMaterialStockDomain MaterialStockDomain,
            IExportReportDomain exportReportDomain)
        {
            _MaterialStockDomain = MaterialStockDomain;
            _exportReportDomain = exportReportDomain;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Domain which handles input of kneading command function businesses
        /// </summary>
        private readonly IMaterialStockDomain _MaterialStockDomain;

        /// <summary>
        ///     Domain which provides functions to export reports.
        /// </summary>
        private readonly IExportReportDomain _exportReportDomain;

        #endregion

        // GET: Inquiry/MaterialStock
        public ActionResult Index()
        {
            ViewBag.ScreenId = "TCFC011F";
            var model = new MaterialStockSearchViewModel()
            {
                DateTime = DateTime.Now.ToString("G")
            };
            return View(model);
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
                // check status
                if (status == "0")
                {
                    var result =
                    await _MaterialStockDomain.SearchRecordsForPrinting(status);
                    // Find the template file.
                    exportMaterialNameAbsolutePath =
                    Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.ReportByMaterialNameNormal));
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
                    var result =
                    await _MaterialStockDomain.SearchRecordsForPrinting(status);
                    // Find the template file.
                    exportMaterialNameAbsolutePath =
                    Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.ReportByMaterialNameBailment));
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
                    var result =
                    await _MaterialStockDomain.SearchRecordsForPrintingAll();
                    // Find the template file.
                    exportMaterialNameAbsolutePath =
                    Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.ReportByMaterialNameAll));
                    // Read the template.
                    var template = await _exportReportDomain.ReadTemplateFileAsync(exportMaterialNameAbsolutePath);

                    // Export the template.
                    var render = await _exportReportDomain.ExportToFlatFileAsync(result, template);

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
        // UC2 Management Report
        [HttpPost]
        public async Task<ActionResult> ExportSupplementaryMaterialName()
        {
            try
            {
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                // Find the list of records                                 
                string exportMaterialNameAbsolutePath = "";
                // check status

                    var result =
                    await _MaterialStockDomain.SearchSupplementaryRecordsForPrintingAll();
                    // Find the template file.
                    exportMaterialNameAbsolutePath =
                    Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.SupplementaryMaterialNameNormal));
                    // Read the template.
                    var template = await _exportReportDomain.ReadTemplateFileAsync(exportMaterialNameAbsolutePath);

                    // Export the template.
                    var render = await _exportReportDomain.ExportToFlatFileAsync(result, template);

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
        //UC5 Management Report
        [HttpPost]
        public async Task<ActionResult> ExportMetarialShelt()
        {
            try
            {
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                // Find the list of records                                 
                string exportMaterialNameAbsolutePath = "";
                // check status

                var result =
                await _MaterialStockDomain.SearchMetarialShelftForPrintingAll();
                // Find the template file.
                exportMaterialNameAbsolutePath =
                Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.MaterialSheif));
                // Read the template.
                var template = await _exportReportDomain.ReadTemplateFileAsync(exportMaterialNameAbsolutePath);

                // Export the template.
                var render = await _exportReportDomain.ExportToFlatFileAsync(result, template);

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
        // UC 6 Management Report

        [HttpPost]
        public async Task<ActionResult> ExportPreProductShelt()
        {
            try
            {
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                // Find the list of records                                 
                string exportMaterialNameAbsolutePath = "";
                // check status

                var result =
                await _MaterialStockDomain.SearchPreProductForPrint();
                // Find the template file.
                exportMaterialNameAbsolutePath =
                Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.PreProductShelfList));
                // Read the template.
                var template = await _exportReportDomain.ReadTemplateFileAsync(exportMaterialNameAbsolutePath);

                // Export the template.
                var render = await _exportReportDomain.ExportToFlatFileAsync(result, template);

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
        // UC 7 Management Report
        [HttpPost]
        public async Task<ActionResult> ExportExtPreProductShelt()
        {
            try
            {
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                // Find the list of records                                 
                string exportMaterialNameAbsolutePath = "";
                // check status

                var result =
                await _MaterialStockDomain.SearchExtPreProductForPrint();
                // Find the template file.
                exportMaterialNameAbsolutePath =
                Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.ExternalPreProductShelfList));
                // Read the template.
                var template = await _exportReportDomain.ReadTemplateFileAsync(exportMaterialNameAbsolutePath);

                // Export the template.
                var render = await _exportReportDomain.ExportToFlatFileAsync(result, template);

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
        // UC 8 Management Report
        [HttpPost]
        public async Task<ActionResult> ExportProductShelt()
        {
            try
            {
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                // Find the list of records                                 
                string exportMaterialNameAbsolutePath = "";
                // check status

                var result =
                await _MaterialStockDomain.SearchProductForPrint();
                // Find the template file.
                exportMaterialNameAbsolutePath =
                Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.ExternalProductShelfList));
                // Read the template.
                var template = await _exportReportDomain.ReadTemplateFileAsync(exportMaterialNameAbsolutePath);

                // Export the template.
                var render = await _exportReportDomain.ExportToFlatFileAsync(result, template);

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
        // UC 9 Management Report
        [HttpPost]
        public async Task<ActionResult> ExportMetarialPallet()
        {
            try
            {
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                // Find the list of records                                 
                string exportMaterialNameAbsolutePath = "";
                // check status

                var result =
                await _MaterialStockDomain.SearchMaterialPalletForPrint();
                // Find the template file.
                exportMaterialNameAbsolutePath =
                Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.MaterialPalletList));
                // Read the template.
                var template = await _exportReportDomain.ReadTemplateFileAsync(exportMaterialNameAbsolutePath);

                // Export the template.
                var render = await _exportReportDomain.ExportToFlatFileAsync(result, template);

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
        // UC 10 Management Report
        [HttpPost]
        public async Task<ActionResult> ExportPreproductContainer()
        {
            try
            {
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                // Find the list of records                                 
                string exportMaterialNameAbsolutePath = "";
                // check status

                var result =
                await _MaterialStockDomain.SearchPreProductContainerForPrint();
                // Find the template file.
                exportMaterialNameAbsolutePath =
                Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.PreProductContainerList));
                // Read the template.
                var template = await _exportReportDomain.ReadTemplateFileAsync(exportMaterialNameAbsolutePath);

                // Export the template.
                var render = await _exportReportDomain.ExportToFlatFileAsync(result, template);

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

        // UC 11 Management Report
        [HttpPost]
        public async Task<ActionResult> ExportProductPallet()
        {
            try
            {
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                // Find the list of records                                 
                string exportMaterialNameAbsolutePath = "";
                // check status

                var result =
                await _MaterialStockDomain.SearchProductPalletForPrint();
                // Find the template file.
                exportMaterialNameAbsolutePath =
                Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.ProductPalletList));
                // Read the template.
                var template = await _exportReportDomain.ReadTemplateFileAsync(exportMaterialNameAbsolutePath);

                // Export the template.
                var render = await _exportReportDomain.ExportToFlatFileAsync(result, template);

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


        public JsonResult GetMateriaCode(string materialCode)
        {
            var result = _MaterialStockDomain.GetById(materialCode);                 
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }


    }
}