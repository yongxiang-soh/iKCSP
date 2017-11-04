using System.ComponentModel.DataAnnotations;
using KCSG.jsGrid.MVC;
using Resources;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.ProductShippingCommand
{
    public class ProductShippingCommandDetailViewModel
    {
        [MaxLength(2)]
        public string ShelfRow { get; set; }
        
        [MaxLength(2)]
        public string ShelfBay { get; set; }
        
        [MaxLength(2)]
        public string ShelfLevel { get; set; }
        public string ShelfNo { get; set; }
        
        public string PalletNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string ProductLotNo { get; set; }
        public string ProductCode { get; set; }
        public string RequestAmount { get; set; }

        public string AssignQuantity { get; set; }
        
        public GridSettings GridSettings { get; set; } 
    }
}