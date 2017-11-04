using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Models;
using Nustache.Core;
using log4net;

namespace KCSG.Domain.Domains
{
    public class ExportReportDomain : IExportReportDomain
    {
        #region Properties

        /// <summary>
        ///     Unit of work which provides functions to access repositories.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Logging instance.
        /// </summary>
        private readonly ILog _log;

        #endregion

        #region Constructor

        /// <summary>
        ///     Inititate domain with dependency injection.
        /// </summary>
        /// <param name="unitOfWork"></param>
        public ExportReportDomain(IUnitOfWork unitOfWork, ILog log)
        {
            _unitOfWork = unitOfWork;
            _log = log;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     This function is for exporting data from a table to DataTable instance.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataTable ExportSqlToDataTable(string tableName)
        {
            // Instance which stores exported data.
            var dataTable = new DataTable();

            // Find database context.
            var context = _unitOfWork.Context.Context;

            using (var sqlCommand = context.Database.Connection.CreateCommand())
            {
                context.Database.Connection.Open();
                sqlCommand.CommandText = string.Format("SELECT * FROM {0}", tableName);

                using (var sqlCommandReader = sqlCommand.ExecuteReader())
                {
                    dataTable.Load(sqlCommandReader);
                }
            }

            return dataTable;
        }

        /// <summary>
        ///     Export DataTable instance to memory stream. This stream can be attached to response to be downloaded by client.
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public MemoryStream ExportDataTableToMemoryStream(DataTable dataTable)
        {
            var memoryStream = new MemoryStream();

            using (var spreadsheetDocument = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = spreadsheetDocument.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);

                var sheets = workbookPart.Workbook.AppendChild(new Sheets());
                var sheet = new Sheet { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };

                sheets.Append(sheet);

                var headerRow = new Row();

                // Columns initialization.
                var columns = new List<string>();
                foreach (DataColumn column in dataTable.Columns)
                {
                    columns.Add(column.ColumnName);

                    var cell = new Cell
                    {
                        DataType = CellValues.String,
                        CellValue = new CellValue(column.ColumnName)
                    };
                    headerRow.AppendChild(cell);
                }

                // Columns name should be placed first.
                sheetData.AppendChild(headerRow);

                // Row initialization.
                foreach (DataRow dsrow in dataTable.Rows)
                {
                    var row = new Row();
                    foreach (var col in columns)
                    {
                        var cell = new Cell
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(dsrow[col].ToString())
                        };
                        row.AppendChild(cell);
                    }
                    sheetData.AppendChild(row);
                }

                workbookPart.Workbook.Save();
            }

            return memoryStream;
        }

        /// <summary>
        ///     Convert a generic list to datatable instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public DataTable ExportListToDataTable<T>(IList<T> data)
        {
            var properties = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (var item in data)
            {
                var row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }

            return table;
        }

        /// <summary>
        ///     Convert a generic enumerable to datatable instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public DataTable ExportListToDataTable<T>(IEnumerable<T> data)
        {
            var properties = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (var item in data)
            {
                var row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }

            return table;
        }

        /// <summary>
        ///     Export data to flat file.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public async Task<string> ExportToFlatFileAsync(object data, string template)
        {
            return await Task.Run(() => { return Render.StringToString(template, data); });
        }

        /// <summary>
        ///     Read template file asynchronously.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<string> ReadTemplateFileAsync(string file)
        {
            if (!File.Exists(file))
                return string.Empty;

            return await Task.Run(() => { return File.ReadAllText(file); });
        }

        /// <summary>
        ///     Export a temporary file and send command to label printer.
        /// </summary>
        /// <param name="printerSetting"></param>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public void ExportLabelPrint(PrinterSetting printerSetting, string path, string data)
        {
            if (!Directory.Exists(path))
            {
                _log.Debug(string.Format("{0} doesn't exist. Initiate the folder", path));
                Directory.CreateDirectory(path);
            }
            // Generate temporary file.
            var fileName = string.Format("{0}.txt", Guid.NewGuid().ToString("D"));
            var file = Path.Combine(path, fileName);
            File.WriteAllText(file, data);

            var commandCopy = string.Format("copy /B \"{0}\" \"{1}\"", file, printerSetting.Ip);
            _log.Debug(string.Format("Executed command {0}", commandCopy));
            ExecuteCommand(commandCopy);

            //File.Delete(file);

        }

        /// <summary>
        ///     Execute command to console.
        /// </summary>
        /// <param name="command"></param>
        private void ExecuteCommand(string command)
        {
            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;

            // *** Redirect the output ***
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            process = Process.Start(processInfo);
            if (process == null)
                return;

            process.WaitForExit();
            process.Close();
        }

        #endregion
    }
}