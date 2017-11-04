using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using KCSG.Domain.Models;

namespace KCSG.Domain.Interfaces
{
    /// <summary>
    ///     A service which provides functions to export data to file.
    /// </summary>
    public interface IExportReportDomain
    {
        /// <summary>
        ///     This function is for exporting data from a table to DataTable instance.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        DataTable ExportSqlToDataTable(string tableName);

        /// <summary>
        ///     Export DataTable instance to memory stream. This stream can be attached to response to be downloaded by client.
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        MemoryStream ExportDataTableToMemoryStream(DataTable dataTable);

        /// <summary>
        ///     Export a generic list to data table instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        DataTable ExportListToDataTable<T>(IList<T> data);

        /// <summary>
        ///     Convert a generic enumerable to datatable instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        DataTable ExportListToDataTable<T>(IEnumerable<T> data);

        /// <summary>
        ///     Export data using template string.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        Task<string> ExportToFlatFileAsync(object data, string template);

        /// <summary>
        /// Read template file asynchronously.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Task<string> ReadTemplateFileAsync(string file);

        /// <summary>
        /// Export label print.
        /// </summary>
        /// <param name="printerSetting"></param>
        /// <param name="path"></param>
        /// <param name="data"></param>
        void ExportLabelPrint(PrinterSetting printerSetting, string path, string data);
    }
}