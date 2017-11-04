using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Resources;

namespace KCSG.Web.Areas.SystemManagement.ViewModels
{
    public class DeleteTableRecordViewModel
    {
        /// <summary>
        /// Table whose record should be deleted.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string Table { get; set; }

        /// <summary>
        /// List of key-value in row (this is used for column primary keys searching)
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public Dictionary<string, string> Parameters { get; set; }
    }
}