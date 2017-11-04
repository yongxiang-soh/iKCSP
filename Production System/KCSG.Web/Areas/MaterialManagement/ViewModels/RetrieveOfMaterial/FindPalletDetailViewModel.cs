using System.ComponentModel.DataAnnotations;

namespace KCSG.Web.Areas.MaterialManagement.ViewModels.RetrieveOfMaterial
{
    public class FindPalletDetailViewModel
    {
        [Required]
        public string PalletNo { get; set; }
    }
}