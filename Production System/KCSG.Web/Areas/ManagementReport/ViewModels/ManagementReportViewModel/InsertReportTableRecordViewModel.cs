using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using KCSG.Domain.Models;
using Resources;

namespace KCSG.Web.Areas.ManagementReport.ViewModels.ManagementReportViewModel
{
    public class InsertReportTableRecordViewModel
    {
        /// <summary>
        /// Name of table which record should be inserted into.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string Table { get; set; }

        /// <summary>
        /// Record rows which should be inserted into table.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public IList<InsertTableRowModel> Row { get; set; }
    }
}