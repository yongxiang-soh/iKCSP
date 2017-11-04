using System.ComponentModel.DataAnnotations;

namespace KCSG.Domain.Models
{
    public class RestoreMaterialViewModel
    {
        [StringLength(8, ErrorMessage = "Shelf No. cannot be longer than 3 characters.")]
        [Display(Name = @"Shelf No.")]
        public string ShelfNo { get; set; }

        public string PalletNo { get; set; }

        //[Required(ErrorMessageResourceName = "MSG2", ErrorMessageResourceType = typeof(MessageResource))]
        [StringLength(12, ErrorMessage = "Material Code cannot be longer than 12 characters.")]
        [Display(Name = @"Material Code")]
        public string MaterialCode { get; set; }

        [StringLength(16, ErrorMessage = "Material Name cannot be longer than 16 characters.")]
        [Display(Name = @"Material Name")]
        public string MaterialDsp { get; set; }

        /// <summary>
        /// List of material.
        /// </summary>
        [Required]
        public FinalStockTakingMaterialItem[] Materials { get; set; }

        [Display(Name = @"Grand Total")]
        //[Range(0.01, 999999.00, ErrorMessageResourceName = "MSG36", ErrorMessageResourceType = typeof(MessageResource))]
        public double GrandTotal { get; set; }

        //[StringLength(1)]
        public string UnitFlag { get; set; }
    }
}