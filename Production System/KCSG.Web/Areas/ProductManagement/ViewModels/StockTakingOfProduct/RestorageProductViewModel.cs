using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using KCSG.Domain.Models.ProductManagement;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.StockTakingOfProduct
{
    public class RestorageProductViewModel
    {
        [Required]
        public string Row { get; set; }

        [Required]
        public string Bay { get; set; }

        [Required]
        public string Level { get; set; }

        [Required]
        public string PalletNo { get; set; }

        /// <summary>
        /// Items which should be restoraged.
        /// </summary>
        public IList<StockTakingOfProductConfirmItem> Items { get; set; }
    }
}