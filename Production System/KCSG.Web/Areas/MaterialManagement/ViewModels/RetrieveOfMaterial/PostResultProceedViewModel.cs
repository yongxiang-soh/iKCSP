using System.ComponentModel.DataAnnotations;

namespace KCSG.Web.Areas.MaterialManagement.ViewModels.RetrieveOfMaterial
{
    public class PostResultProceedViewModel
    {
        [Required]
        public string MaterialCode { get; set; }
    }
}