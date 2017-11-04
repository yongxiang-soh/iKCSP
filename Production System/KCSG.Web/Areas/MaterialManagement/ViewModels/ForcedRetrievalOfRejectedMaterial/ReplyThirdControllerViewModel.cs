using System.ComponentModel.DataAnnotations;

namespace KCSG.Web.Areas.MaterialManagement.ViewModels.ForcedRetrievalOfRejectedMaterial
{
    public class ReplyThirdControllerViewModel
    {
        [Required]
        public string MaterialCode { get; set; }
    }
}