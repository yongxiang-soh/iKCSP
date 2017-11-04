using System.Collections.Generic;

namespace KCSG.Domain.Models
{
    /// <summary>
    /// This class is a wrapper which contain information responded back from service about database query command (SELECT * FROM ...)
    /// </summary>
    public class FindRecordTableResult
    {
        /// <summary>
        /// Result list responded back from service.
        /// </summary>
        public IList<Dictionary<string, object>> Rows { get; set; }

        /// <summary>
        /// Total records number which match with the search conditions.
        /// </summary>
        public int Total { get; set; }
    }
}