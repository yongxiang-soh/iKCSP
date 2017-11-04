using KCSG.jsGrid.MVC;
using System.ComponentModel.DataAnnotations;
using Resources;

namespace KCSG.Web.Areas.MaterialManagement.ViewModels.StockTakingOfMaterial
{
    public class StockTakingOfMaterialViewModel
    {
        [Required(ErrorMessageResourceName = "MSG2", ErrorMessageResourceType = typeof(MessageResource))] 
        [StringLength(8)]
        [Display(Name = @"Shelf No.")]
        public string ShelfNoFrom { get; set; }

        [Required(ErrorMessageResourceName = "MSG2", ErrorMessageResourceType = typeof(MessageResource))] 
        [StringLength(8)]
        public string ShelfNoTo { get; set; }

        public Grid Grid { get; set; }

        [StringLength(16)]
        [Display(Name = @"Material Lot No.")]
        public string MaterialLotNo01 { get; set; }

        [StringLength(16)]
        [Display(Name = @"Material Lot No.")]
        public string MaterialLotNo02 { get; set; }

        [StringLength(16)]
        [Display(Name = @"Material Lot No.")]
        public string MaterialLotNo03 { get; set; }

        [StringLength(16)]
        [Display(Name = @"Material Lot No.")]
        public string MaterialLotNo04 { get; set; }

        [StringLength(16)]
        [Display(Name = @"Material Lot No.")]
        public string MaterialLotNo05 { get; set; }

        [Display(Name = @"Quantity")]
        public double? Amount01 { get; set; }

        [Display(Name = @"Quantity")]
        public double? Amount02 { get; set; }

        [Display(Name = @"Quantity")]
        public double? Amount03 { get; set; }

        [Display(Name = @"Quantity")]
        public double? Amount04 { get; set; }

        [Display(Name = @"Quantity")]
        public double? Amount05 { get; set; }
    }
}