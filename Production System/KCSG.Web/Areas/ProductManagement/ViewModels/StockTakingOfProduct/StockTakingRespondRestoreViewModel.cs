using System.ComponentModel.DataAnnotations;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.StockTakingOfProduct
{
    public class StockTakingRespondRestoreViewModel
    {
        [Required]
        [StringLength(2)]
        public string Row { get; set; }

        [Required]
        [StringLength(2)]
        public string Bay { get; set; }

        [Required]
        [StringLength(2)]
        public string Level { get; set; }
    }
}