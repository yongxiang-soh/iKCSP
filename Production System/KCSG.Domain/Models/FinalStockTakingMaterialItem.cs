using System.ComponentModel.DataAnnotations;

namespace KCSG.Domain.Models
{
    public class FinalStockTakingMaterialItem
    {
        //[Required(ErrorMessageResourceName = "MSG2", ErrorMessageResourceType = typeof(MessageResource))]
        [StringLength(16, ErrorMessage = "Material Lot No. (1) cannot be longer than 16 characters.")]
        [Display(Name = @"Material Lot No.")]
        public string MaterialLotNo { get; set; }

        [Display(Name = @"Pack Unit")]
        [Range(0.00, 999.00, ErrorMessage = "The inputted value of Pack Unit (1) is out of range.")]
        public double PackUnit { get; set; }

        // [Required(ErrorMessageResourceName = "MSG2", ErrorMessageResourceType = typeof(MessageResource))]
        [Display(Name = @"Pack Qty")]
        [Range(0, 999, ErrorMessage = "The inputted value of Pack Qty (1) is out of range.")]
        public int PackQuantity { get; set; }
        
        // [Required(ErrorMessageResourceName = "MSG2", ErrorMessageResourceType = typeof(MessageResource))]
        [Display(Name = @"Fraction")]
        [Range(0.00, 999.00, ErrorMessage = "The inputted value of Fraction (1) is out of range.")]
        public double Fraction { get; set; }

        [Display(Name = @"Total")]
        [Range(0.00, 999999.00, ErrorMessage = "The inputted value of Total (1) is out of range.")]
        public double Total { get; set; }
    }
}