using System.ComponentModel.DataAnnotations;
using KCSG.jsGrid.MVC;
using Resources;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.StockTakingOfProduct
{
    public class StockTakingOfProductViewModel
    {
        /// <summary>
        /// Grid which display stock-taking of products list.
        /// </summary>
        public Grid StockTakingOfProductGrid { get; set; }

        [Required(ErrorMessageResourceName = "MSG37", ErrorMessageResourceType = typeof(ProductManagementResources))]
        [StringLength(8)]
        [Display(Name = @"Shelf No.")]
        public string ShelfNoFrom { get; set; }

        [Required(ErrorMessageResourceName = "MSG37", ErrorMessageResourceType = typeof(ProductManagementResources))]
        [StringLength(8)]
        public string ShelfNoTo { get; set; }
    }
}