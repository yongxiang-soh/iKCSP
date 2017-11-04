using System.ComponentModel.DataAnnotations;
using Resources;

namespace KCSG.Web.Areas.SystemManagement.ViewModels
{
    public class FindTableRecordsViewModel
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
        
        /// <summary>
        /// Total record should be shown in page.
        /// </summary>
        public int Records { get; set; }   
    }
}