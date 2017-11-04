using System.ComponentModel.DataAnnotations;
using KCSG.jsGrid.MVC;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.ProductShippingCommand
{
    public class ExportProductShippingCommandViewModel
    {
        /// <summary>
        /// Code of product.
        /// </summary>
        public string ProductCode { get; set; }
        public string lstPalletNo { get; set; }
        public string ShelfNo { get; set; }

        /// <summary>
        /// Name of selected product.
        /// </summary>
        public string ProductName { get; set; }
        
        /// <summary>
        /// Product lot number.
        /// </summary>
        public string ProductLotNo { get; set; }

        /// <summary>
        /// Shipping command.
        /// </summary>
        [Required]
        public string ShippingNo { get; set; }

        /// <summary>
        /// Settings of grid.
        /// </summary>
        public GridSettings Settings { get; set; }
    }
}