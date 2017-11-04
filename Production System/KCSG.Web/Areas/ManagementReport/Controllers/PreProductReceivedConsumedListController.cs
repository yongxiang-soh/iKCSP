using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.ManagementReport;
using KCSG.Domain.Interfaces.SystemManagement;
using KCSG.Domain.Models;
using KCSG.jsGrid.MVC;
using KCSG.Web.Areas.ManagementReport.ViewModels.ManagementReportViewModel;
using KCSG.Web.Areas.SystemManagement.ViewModels;
using KCSG.Web.Attributes;

namespace KCSG.Web.Areas.ManagementReport.Controllers
{
    [MvcAuthorize("TCFC202F")]
    public class PreProductReceivedConsumedListController : KCSG.Web.Controllers.BaseController
    {
        #region Constructors

        /// <summary>
        ///     Initialize controller with dependency injections.
        /// </summary>
        /// <param name="PreProductReceivedConsumedListDomain"></param>
        /// <param name="exportReportDomain"></param>
        public PreProductReceivedConsumedListController(IManagementReportDomain PreProductReceivedConsumedListDomain,
            IExportReportDomain exportReportDomain)
        {
            _PreProductReceivedConsumedListDomain = PreProductReceivedConsumedListDomain;
            _exportReportDomain = exportReportDomain;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Domain which provides functions to access to database management.
        /// </summary>
        private readonly IManagementReportDomain _PreProductReceivedConsumedListDomain;

        /// <summary>
        ///     Domain which provides functions for data export.
        /// </summary>
        private readonly IExportReportDomain _exportReportDomain;

        #endregion

        #region Methods

        /// <summary>
        ///     This function is for rendering index page.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = new FindReportTableRecordsViewModel()
            {
                YearMonth = DateTime.Now.ToString("MM/yyyy")

            };
            return View(model);
        }


        [HttpPost]
        public async Task<ActionResult> RetrieveTableRecords(string yearmonth,int page)
        {
            try
            {
                var loadKneadingCommandResult = await _PreProductReceivedConsumedListDomain.LoadConsumerPreProducts(yearmonth,page);
                return Json(loadKneadingCommandResult.Data);
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public async Task<ActionResult> CalculateTableRecords(string yearmonth,string precode)
        {
            try
            {
                var loadKneadingCommandResult = await _PreProductReceivedConsumedListDomain.RecalculatePreProduct(yearmonth,precode);
                return Json(loadKneadingCommandResult.Data);
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }
        /// <summary>
        /// Update management report record.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> UpdateManagementReport(UpdateManagementReportViewModel parameters)
        {
            // Parameter hasn't been initialized.
            if (parameters == null)
            {
                parameters = new UpdateManagementReportViewModel();
                ValidateModel(parameters);
            }

            // Parameters are invalid.
            if (!ModelState.IsValid)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            // Paramenter valid
            var updateConsumerMaterial = _PreProductReceivedConsumedListDomain.UpdatePreProductConsumer(parameters.YearMonth, parameters.Recieved, parameters.Remain, parameters.Used, parameters.PreProductCode);
            return Json(updateConsumerMaterial);

        }

        public async Task<ActionResult> ExportConsumerMaterial(string yearmonth)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                // Find the list of records                                 
                string exportMaterialNameAbsolutePath = "";
                // check status

                var result =
                await _PreProductReceivedConsumedListDomain.PrintConsumerPreProducts(yearmonth);
                // Find the template file.
                exportMaterialNameAbsolutePath =
                Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.ConsumerPreProduct));
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

        [HttpPost]
        public async Task<ActionResult> Checkitemexist(string yearmonth)
        {

                var exist = _PreProductReceivedConsumedListDomain.CheckConsumerPreProducts(yearmonth);
                return Json(exist);

        }
        /// <summary>
        ///     Find validation messages from ModelStateDictionary and pair the key with values.
        /// </summary>
        /// <param name="modelStateDictionary"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> FindValidationMessages(ModelStateDictionary modelStateDictionary)
        {
            var validationMessages = modelStateDictionary.ToDictionary(x => x.Key,
                x => x.Value.Errors.Select(y => y.ErrorMessage).ToList());
            return validationMessages;
        }

        /// <summary>
        ///     Find validation messages from insert table row model and pair the key with its values.
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> FindValidationMessages(IList<InsertTableRowModel> cells)
        {
            // Initialize validation messages.
            var validationMessages = cells.ToDictionary(x => x.Column, x => new List<string>());
            foreach (var cell in cells)
                if (cell.IsNullable.Equals("NO", StringComparison.InvariantCultureIgnoreCase))
                    if (cell.Value == null)
                    {
                        validationMessages[cell.Column].Add("This field is required");
                    }

            return validationMessages;
        }

        /// <summary>
        ///     Check whether validation message available or not.
        /// </summary>
        /// <param name="validationMessages"></param>
        /// <returns></returns>
        private bool IsValidationMessageAvailable(Dictionary<string, List<string>> validationMessages)
        {
            return validationMessages.Sum(x => x.Value.Count) > 0;
        }


        #endregion

        #region Properties

        #endregion

        #region Constructor

        #endregion
    }
}