using Resources;
using System.ComponentModel.DataAnnotations;

namespace KCSG.Web.Areas.PreProductManagement.ViewModels
{
    public class AnalyzeStockTakingForMovingViewModel
    {
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string ContainerCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string LchStatus { get; set; }
        public string PreProductCode { get; set; }

        /// <summary>
        /// Whether ok button is clicked.
        /// </summary>
        public bool OkClicked { get; set; }
    }
}