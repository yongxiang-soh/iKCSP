using System.ComponentModel.DataAnnotations;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.ForcedRetrievalOfProduct
{
    public class ForceRetrievalProductRespondViewModel
    {
        [Required]
        public string ProductCode { get; set; }
    }
}