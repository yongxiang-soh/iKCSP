using System.ComponentModel.DataAnnotations;

namespace KCSG.Web.Areas.MaterialManagement.ViewModels.StockTakingOfMaterial
{
    public class StockMaterialComResponseViewModel
    {
        [Required]
        public string MaterialCode { get; set; }
    }
}