using System;
using System.ComponentModel.DataAnnotations;
using Resources;

namespace KCSG.Web.Areas.ManagementReport.ViewModels.ManagementReportViewModel
{
    public class FindReportTableRecordsViewModel
    {
        /// <summary>
        /// Name of table whose records should be shown.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string Table { get; set; }
        
        /// <summary>
        /// Index of page.
        /// </summary>
        public int Page { get; set; }
        [Display(Name = @"Year-Month")]
        public string YearMonth { get; set; }
        /// <summary>
        /// Total record should be shown in page.
        /// </summary>
        public int Records { get; set; }   
    }
}