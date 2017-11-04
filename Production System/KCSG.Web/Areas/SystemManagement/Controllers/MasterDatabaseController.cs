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
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.SystemManagement;
using KCSG.Domain.Models;
using KCSG.Web.Areas.SystemManagement.ViewModels;
using KCSG.Web.Attributes;

namespace KCSG.Web.Areas.SystemManagement.Controllers
{
    [MvcAuthorize("TCSC021F")]
    public class MasterDatabaseController : KCSG.Web.Controllers.BaseController
    {
        #region Constructors

        /// <summary>
        ///     Initialize controller with dependency injections.
        /// </summary>
        /// <param name="masterDatabaseDomain"></param>
        /// <param name="exportReportDomain"></param>
        public MasterDatabaseController(IDatabaseMaintainanceDomain masterDatabaseDomain,
            IExportReportDomain exportReportDomain)
        {
            _masterDatabaseDomain = masterDatabaseDomain;
            _exportReportDomain = exportReportDomain;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Domain which provides functions to access to database management.
        /// </summary>
        private readonly IDatabaseMaintainanceDomain _masterDatabaseDomain;

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
            return View();
        }

        /// <summary>
        ///     This function is for retrieving list of table in the current database.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> RetrieveTablesList()
        {
            try
            {
                var tablesList = await _masterDatabaseDomain.FindTablesAsync();
                Response.StatusCode = (int) HttpStatusCode.OK;
                return Json(tablesList);
            }
            catch (Exception exception)
            {
                Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                return Json(new
                {
                    exception.Message
                });
            }
        }

        /// <summary>
        ///     This function is for retrieving list of records of a specific table in the database.
        /// </summary>
        /// <param name="findTableRecordsViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> RetrieveTableRecords(FindTableRecordsViewModel findTableRecordsViewModel)
        {
            try
            {
                // Model hasn't been initialize. Initialize it and do validation.
                if (findTableRecordsViewModel == null)
                {
                    findTableRecordsViewModel = new FindTableRecordsViewModel();
                    ValidateModel(findTableRecordsViewModel);
                }

                // Request parameters are invalid. 
                if (!ModelState.IsValid)
                {
                    // Build validation messages and respond to client.
                    Response.StatusCode = (int) HttpStatusCode.BadRequest;
                    return Json(new
                    {
                        validationMessages = FindValidationMessages(ModelState)
                    });
                }

                var tableColumns = await _masterDatabaseDomain.FindTableColumns(findTableRecordsViewModel.Table);
                var tableRows =
                    await
                        _masterDatabaseDomain.FindTableRecords(findTableRecordsViewModel.Table,
                            findTableRecordsViewModel.Page,
                            findTableRecordsViewModel.Records);

                var tableViewModel = new
                {
                    Headers = tableColumns,
                    Body = tableRows
                };

                return Json(tableViewModel);
            }
            catch (Exception exception)
            {
                Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                return Json(new
                {
                    exception.Message
                });
            }
        }

        /// <summary>
        ///     This function is for inserting records into database system.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> InsertTableRecords(InsertTableRecordViewModel insertTableRecordViewModel)
        {
            try
            {
                #region Validation

                // Model hasn't been initialize. Initialize it and do validation.
                if (insertTableRecordViewModel == null)
                {
                    insertTableRecordViewModel = new InsertTableRecordViewModel();
                    ValidateModel(insertTableRecordViewModel);
                }

                var validationMessages = FindValidationMessages(ModelState);

                // Request parameters are invalid. 
                if (!ModelState.IsValid)
                {
                    // Build validation messages and respond to client.
                    Response.StatusCode = (int) HttpStatusCode.BadRequest;
                    return Json(new
                    {
                        validationMessages
                    });
                }

                // The second validation is about deeper property validation.
                validationMessages = FindValidationMessages(insertTableRecordViewModel.Row);
                if (IsValidationMessageAvailable(validationMessages))
                {
                    // Build validation messages and respond to client.
                    Response.StatusCode = (int) HttpStatusCode.BadRequest;
                    return Json(new
                    {
                        validationMessages
                    });
                }

                #endregion

                await
                    _masterDatabaseDomain.InsertTableRecords(insertTableRecordViewModel.Table,
                        insertTableRecordViewModel.Row);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                return Json(new
                {
                    exception.Message
                });
            }
        }

        /// <summary>
        ///     This function is for deleting a record in database system.
        /// </summary>
        /// <param name="deleteTableRecordViewModel"></param>
        /// <returns></returns>
        public async Task<ActionResult> DeleteTableRecord(DeleteTableRecordViewModel deleteTableRecordViewModel)
        {
            try
            {
                #region Validation

                // Model hasn't been initialize. Initialize it and do validation.
                if (deleteTableRecordViewModel == null)
                {
                    deleteTableRecordViewModel = new DeleteTableRecordViewModel();
                    ValidateModel(deleteTableRecordViewModel);
                }

                var validationMessages = FindValidationMessages(ModelState);
                // Request parameters are invalid. 
                if (!ModelState.IsValid)
                {
                    // Build validation messages and respond to client.
                    Response.StatusCode = (int) HttpStatusCode.BadRequest;
                    return Json(new
                    {
                        validationMessages
                    });
                }

                #endregion

                // Find the columns which contains primary key first.
                var primaryKeys =
                    await _masterDatabaseDomain.FindTablePrimaryKeysAsync(deleteTableRecordViewModel.Table);

                // Initialize a dictionary contains paramters which should be submitted to domain.
                IDictionary<string, string> parameters = new Dictionary<string, string>();

                // Primary keys are defined.
                if (primaryKeys.Length > 0)
                    foreach (var key in primaryKeys)
                        if (parameters.ContainsKey(key))
                            parameters[key] = deleteTableRecordViewModel.Parameters[key];
                        else
                            parameters.Add(key, deleteTableRecordViewModel.Parameters[key]);
                else
                    parameters = deleteTableRecordViewModel.Parameters;

                // Tell service to delete record from database.
                await _masterDatabaseDomain.DeleteTableRecordAsync(deleteTableRecordViewModel.Table, parameters);

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                return Json(new
                {
                    exception.Message
                });
            }
        }

        /// <summary>
        ///     This function is for exporting a target table to excel file and send the exported one to client.
        /// </summary>
        /// <param name="findTableRecordsViewModel">
        ///     This instance is for retrieving table name sent from client [page|record] are
        ///     useless
        /// </param>
        /// <returns></returns>
        public ActionResult ExportDatabaseToExcel(FindTableRecordsViewModel findTableRecordsViewModel)
        {
            // Set page & record to one to prevent validation errors on service.
            findTableRecordsViewModel.Page = 1;
            findTableRecordsViewModel.Records = 1;

            // Validate the model again.
            ValidateModel(findTableRecordsViewModel);

            // Model state is invalid.
            if (!ModelState.IsValid)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Export the target table to DataTable instance.
            var dataTable = _exportReportDomain.ExportSqlToDataTable(findTableRecordsViewModel.Table);

            // Memorizing data table instance by using MemoryStream.
            var memorizedDataTable = _exportReportDomain.ExportDataTableToMemoryStream(dataTable);

            // Export report to client download.
            ExportExcelFileToClientDownload(findTableRecordsViewModel.Table, memorizedDataTable, Response);

            // Export the datatable into an excel file.
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        /// <summary>
        ///     This function is for updating records into database system.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> UpdateTableRecord(UpdateTableRecordViewModel updateTableRecordViewModel)
        {
            try
            {
                // Model hasn't been initialize. Initialize it and do validation.
                if (updateTableRecordViewModel == null)
                {
                    updateTableRecordViewModel = new UpdateTableRecordViewModel();
                    ValidateModel(updateTableRecordViewModel);
                }

                // Request parameters are invalid. 
                if (!ModelState.IsValid)
                {
                    // Build validation messages and respond to client.
                    Response.StatusCode = (int) HttpStatusCode.BadRequest;
                    return Json(new
                    {
                        validationMessages = FindValidationMessages(ModelState)
                    });
                }

                // Find the columns which contains primary key first.
                var primaryKeys =
                    await _masterDatabaseDomain.FindTablePrimaryKeysAsync(updateTableRecordViewModel.Table);

                // Initialize a dictionary contains paramters which should be submitted to domain.
                IDictionary<string, string> parameters = new Dictionary<string, string>();

                // Primary keys are defined. Build a list of properties which are used for searching record in table.
                if (primaryKeys.Length > 0)
                    foreach (var key in primaryKeys)
                        if (parameters.ContainsKey(key))
                            parameters[key] = updateTableRecordViewModel.Original[key];
                        else
                            parameters.Add(key, updateTableRecordViewModel.Original[key]);
                else
                    parameters = updateTableRecordViewModel.Original;

                // Tell service to delete record from database.
                await
                    _masterDatabaseDomain.UpdateTableRecordAsync(updateTableRecordViewModel.Table, parameters,
                        updateTableRecordViewModel.Target);

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                return Json(new
                {
                    exception.Message
                });
            }
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

        /// <summary>
        ///     Export byte stream of excel file to client download.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="excel"></param>
        /// <param name="httpResponseBase"></param>
        private void ExportExcelFileToClientDownload(string fileName, MemoryStream excel,
            HttpResponseBase httpResponseBase)
        {
            httpResponseBase.ContentType = "application/octet-stream";
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(excel.GetBuffer())
            };

            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = string.Format("{0}.xlsx", fileName)
            };


            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.xlsx", fileName));

            // Seek to the beginning of stream
            excel.Seek(0, SeekOrigin.Begin);
            excel.WriteTo(Response.OutputStream);
            Response.Flush();
            Response.End();
        }

        #endregion

        #region Properties

        #endregion

        #region Constructor

        #endregion
    }
}