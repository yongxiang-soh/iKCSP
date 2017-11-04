using System.ComponentModel.DataAnnotations;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.StockTakingOfProduct
{
    public class FindProductConfirmDetailViewModel
    {
        [Required]
        public string Row { get; set; }

        [Required]
        public string Bay { get; set; }

        [Required]
        public string Level { get; set; }

        [Required]
        public string PalletNo { get; set; }
    }
}