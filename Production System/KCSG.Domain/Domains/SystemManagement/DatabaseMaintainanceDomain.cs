using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Data.DatabaseContext;
using KCSG.Data.DataModel;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.SystemManagement;
using KCSG.Domain.Models;

namespace KCSG.Domain.Domains.SystemManagement
{
    public class DatabaseMaintainanceDomain : IDatabaseMaintainanceDomain
    {
        #region Properties

        /// <summary>
        /// Private instance which provides access to unit of work.
        /// </summary>
        private readonly IKCSGDbContext _kcsgDbContext;
        
        #endregion

        #region Constructors

        /// <summary>
        /// Initialize domain with unit of work.
        /// </summary>
        /// <param name="kcsgDbContext"></param>
        public DatabaseMaintainanceDomain(IKCSGDbContext kcsgDbContext)
        {
            _kcsgDbContext = kcsgDbContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Thí function
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="page"></param>
        /// <param name="records"></param>
        public async Task<FindRecordTableResult> FindTableRecords(string tableName, int page, int records)
        {
            // Table name is invalid.
            if (string.IsNullOrEmpty(tableName))
                return null;

            if (records < 1)
                records = 1;

            // Initialize database context.
            var dbContext = _kcsgDbContext.Context;
          
            // Record response intialization.
            var result = new FindRecordTableResult();
            result.Rows = new List<Dictionary<string, object>>();
            
            var sqlCommandResultFind = new StringBuilder(string.Format("SELECT * FROM {0} ", tableName))
                .Append("ORDER BY 1 ")
                .Append(string.Format("OFFSET {0} ROWS ", page * records))
                .Append(string.Format("FETCH NEXT {0} ROWS ONLY", records));

            using (var sqlCommand = dbContext.Database.Connection.CreateCommand())
            {
                dbContext.Database.Connection.Open();
                sqlCommand.CommandText = sqlCommandResultFind.ToString();
                using (var sqlCommandReader = sqlCommand.ExecuteReader())
                {
                    while (await sqlCommandReader.ReadAsync())
                    {
                        var sqlRow = new Dictionary<string, object>();
                        for (var i = 0; i < sqlCommandReader.FieldCount; i++)
                            sqlRow.Add(sqlCommandReader.GetName(i), sqlCommandReader.GetValue(i));

                        result.Rows.Add(sqlRow);
                    }
                }
            }

            // Count total records retrieved from database.
            result.Total = await dbContext.Database
                .SqlQuery<int>(string.Format("SELECT COUNT(*) FROM {0}", tableName))
                .FirstOrDefaultAsync();

            return result;
        }

        /// <summary>
        /// Insert a row into a specific table in database.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="insertTableRowModel"></param>
        /// <returns></returns>
        public async Task InsertTableRecords(string tableName, IList<InsertTableRowModel> insertTableRowModel)
        {
            try
            {
                // List of columns which record should be added into.
                var columnsList = string.Join(",", insertTableRowModel.Select(x => x.Column));
                var valuesList = string.Join(",", insertTableRowModel.Select(x =>
                {
                    if (x.Type.Equals("nvarchar"))
                        return string.Format("N'{0}'", x.Value);
                    return string.Format("'{0}'", x.Value);
                }));

                var sqlCommand = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", tableName, columnsList, valuesList);

                var records = await _kcsgDbContext.Context.Database
                    .ExecuteSqlCommandAsync(sqlCommand);
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert a row into a specific table in database.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public async Task<int> DeleteTableRecordAsync(string tableName, IDictionary<string, string> filters)
        {
            try
            {
                // Key-value pairs construction.
                var keyValuePairs = filters.Select(x => string.Format("{0}='{1}'", x.Key, x.Value));
                
                var sqlCommand = new StringBuilder(string.Format("DELETE FROM {0} ", tableName))
                    .Append(string.Format("WHERE {0} ", string.Join(" AND ", keyValuePairs)));

                var records = await _kcsgDbContext.Context.Database
                    .ExecuteSqlCommandAsync(sqlCommand.ToString());

                return records;
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        /// <summary>
        /// This function is for searching data in the targeted table and update the found one with defined data.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="filters"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public async Task<int> UpdateTableRecordAsync(string tableName, IDictionary<string, string> filters,
            IDictionary<string, string> target)
        {
            try
            {
                // This key-value collection is for setting data into database. SQL : SET a = x, b = y, c = z, ...
                var pairSet = target.Select(x => string.Format("{0}='{1}'", x.Key, target[x.Key]));

                // This key-value collection is for searching data in database. SQL : WHERE a = x, b = y, c = z, ...
                var pairCheck = filters.Select(x => string.Format("{0}='{1}'", x.Key, filters[x.Key]));

                var sqlCommand = new StringBuilder(string.Format("UPDATE {0} ", tableName))
                    .Append(string.Format("SET {0} ", string.Join(",", pairSet)))
                    .Append(string.Format("WHERE {0} ", string.Join(" AND ", pairCheck)));

                var records = await _kcsgDbContext.Context.Database
                    .ExecuteSqlCommandAsync(sqlCommand.ToString());

                return records;
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Find all columns in a specific database.
        /// </summary>
        /// <param name="tableName">Name of table whose columns should be detected</param>
        /// <returns></returns>
        public async Task<IList<SchemaColumn>> FindTableColumns(string tableName)
        {
            try
            {
                var sqlCommand = new StringBuilder("SELECT ")
                    .Append("COLUMN_NAME AS [Column],")
                    .Append("ORDINAL_POSITION AS [Position],")
                    .Append("IS_NULLABLE AS [IsNullable],")
                    .Append("CHARACTER_MAXIMUM_LENGTH AS [MaxLength],")
                    .Append("DATA_TYPE AS [Type] ")
                    .Append("FROM INFORMATION_SCHEMA.COLUMNS ")
                    .Append(string.Format("WHERE TABLE_NAME = N'{0}'", tableName))
                    .Append("ORDER BY Position ");

                return await _kcsgDbContext.Context.Database.
                    SqlQuery<SchemaColumn>(sqlCommand.ToString())
                    .ToListAsync();
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Find tables in a specific database.
        /// </summary>
        /// <returns></returns>
        public async Task<SchemaTable[]> FindTablesAsync()
        {
            var sqlCommand = new StringBuilder("SELECT ")
               .Append("TABLE_CATALOG AS [Catalog], ")
               .Append("TABLE_SCHEMA AS [Schema], ")
               .Append("TABLE_NAME AS [Name], ")
               .Append("TABLE_TYPE AS [Type] ")
               .Append("FROM INFORMATION_SCHEMA.TABLES ")
               .Append("WHERE TABLE_NAME != 'sysdiagrams' ");

            return await _kcsgDbContext.Context.Database.
                SqlQuery<SchemaTable>(sqlCommand.ToString())
                .ToArrayAsync();
        }

        /// <summary>
        /// Find primary keys list in a specific table. This is used for deleting a record in table.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public async Task<string[]> FindTablePrimaryKeysAsync(string table)
        {
            var sqlCommandText = new StringBuilder("SELECT Col.Column_Name AS[Columns] ")
                .Append("FROM ")
                .Append("INFORMATION_SCHEMA.TABLE_CONSTRAINTS Tab, ")
                .Append("INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE Col ")
                .Append("WHERE ")
                .Append("Col.Constraint_Name = Tab.Constraint_Name ")
                .Append("AND Col.Table_Name = Tab.Table_Name ")
                .Append("AND Constraint_Type = 'PRIMARY KEY' ")
                .Append(string.Format("AND Col.Table_Name = '{0}'", table));
            
            var primaryKeys = new List<string>();

            try
            {
                using (var sqlCommand = _kcsgDbContext.Context.Database.Connection.CreateCommand())
                {
                    _kcsgDbContext.Context.Database.Connection.Open();
                    sqlCommand.CommandText = sqlCommandText.ToString();
                    using (var sqlCommandReader = sqlCommand.ExecuteReader())
                    {
                        while (await sqlCommandReader.ReadAsync())
                            primaryKeys.Add(sqlCommandReader.GetString(0));
                    }
                }

                return primaryKeys.ToArray();
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Find column data type from string
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public Constants.SqlDataType FindColumnDataType(string dataType)
        {
            // Cast data type to lower string.
            var loweredCaseDataType = dataType.ToLower();

            if (loweredCaseDataType.Equals("datetime")
                || loweredCaseDataType.Equals("smalldatetime")
                || loweredCaseDataType.Equals("date")
                || loweredCaseDataType.Equals("time"))
                return Constants.SqlDataType.DateTime;

            if (loweredCaseDataType.Equals("char")
                || loweredCaseDataType.Equals("varchar")
                || loweredCaseDataType.Equals("varchar(max)")
                || loweredCaseDataType.Equals("text"))
                return Constants.SqlDataType.Text;

            if (loweredCaseDataType.Equals("nchar")
                || loweredCaseDataType.Equals("nvarchar")
                || loweredCaseDataType.Equals("nvarchar(max)")
                || loweredCaseDataType.Equals("ntext"))
                return Constants.SqlDataType.Unicode;

            return Constants.SqlDataType.Numeric;
        }
        
        #endregion
    }
}