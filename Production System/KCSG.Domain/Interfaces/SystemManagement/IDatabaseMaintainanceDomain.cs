using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Domain.Models;

namespace KCSG.Domain.Interfaces.SystemManagement
{
    public interface IDatabaseMaintainanceDomain
    {
        /// <summary>
        /// Find records of a table by using specific conditions.
        /// </summary>
        /// <param name="tableName">Name of table in database.</param>
        /// <param name="page">Index of page which records should be shown (start from 0)</param>
        /// <param name="records">The number of records which are able to be shown on page.</param>
        Task<FindRecordTableResult> FindTableRecords(string tableName, int page, int records);

        /// <summary>
        /// Insert a row into data table.
        /// </summary>
        /// <param name="tableName">Name of table which data should be inserted into.</param>
        /// <param name="insertTableRowModels">Collection of columns of a data row.</param>
        Task InsertTableRecords(string tableName, IList<InsertTableRowModel> insertTableRowModels);

        /// <summary>
        /// Find all columns in a specific database.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        Task<IList<SchemaColumn>> FindTableColumns(string tableName);

        /// <summary>
        /// This function is for searching tables in database asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<SchemaTable[]> FindTablesAsync();

        /// <summary>
        /// Find a list of primary keys in a specific table.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        Task<string[]> FindTablePrimaryKeysAsync(string table);

        /// <summary>
        /// Find column data type from string to enum
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        Constants.SqlDataType FindColumnDataType(string dataType);

        /// <summary>
        /// Delete a record from database with a specific conditions.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        Task<int> DeleteTableRecordAsync(string tableName, IDictionary<string, string> filters);
        
        /// <summary>
        /// Searching data in the table and update it.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="filters"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        Task<int> UpdateTableRecordAsync(string tableName, IDictionary<string, string> filters,
            IDictionary<string, string> target);
    }
}