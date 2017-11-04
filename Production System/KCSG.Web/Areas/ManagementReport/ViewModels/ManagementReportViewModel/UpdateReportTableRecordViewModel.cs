using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Resources;

namespace KCSG.Web.Areas.ManagementReport.ViewModels.ManagementReportViewModel
{
    public class UpdateReportTableRecordViewModel
    {
        /// <summary>
        /// Table name which record will be updated into.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string Table { get; set; }

        /// <summary>
        /// Original data which is used for filtering record for updating.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public IDictionary<string, string> Original { get; set; }

        /// <summary>
        /// Data which will be updated to the original above.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public IDictionary<string, string> Target { get; set; }
    }
}