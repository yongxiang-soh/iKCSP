using System.ComponentModel.DataAnnotations;

namespace KCSG.Web.Areas.MaterialManagement.ViewModels.StockTakingOfMaterial
{
    public class RequestRestoreMaterialViewModel
    {
        [Required]
        [StringLength(8, ErrorMessage = "Shelf No. cannot be longer than 3 characters.")]
        [Display(Name = @"Shelf No.")]
        public string ShelfNo { get; set; }
        
        [Required]
        [StringLength(12, ErrorMessage = "Material Code cannot be longer than 12 characters.")]
        [Display(Name = @"Material Code")]
        public string MaterialCode { get; set; }

        [Required]
        [StringLength(16, ErrorMessage = "Material Name cannot be longer than 16 characters.")]
        [Display(Name = @"Material Name")]
        public string MaterialDsp { get; set; }

        [Required]
        public string PalletNo { get; set; }
    }
}