using System.ComponentModel.DataAnnotations;
using Resources;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.StockTakingOfProduct
{
    public class FindStockTakingOfProductItem
    {
        [Required(ErrorMessageResourceType = typeof(ProductManagementResources), ErrorMessageResourceName = "MSG37")]
        [StringLength(2)]
        public string ShelfRowFrom { get; set; }

        [Required(ErrorMessageResourceType = typeof(ProductManagementResources), ErrorMessageResourceName = "MSG37")]
        [StringLength(2)]
        public string ShelfBayFrom { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(2)]
        public string ShelfLevelFrom { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(2)]
        public string ShelfRowTo { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(2)]
        public string ShelfBayTo { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(2)]
        public string ShelfLevelTo { get; set; }
    }
}