using Resources;
using System.ComponentModel.DataAnnotations;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.StockTakingOfProduct
{
    public class RetrieveProductDetailViewModel
    {
        /// <summary>
        /// Pallet no
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string PalletNo { get; set; }

        /// <summary>
        /// Shelf row
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(2)]
        public string Row { get; set; }

        /// <summary>
        /// Shelf bay.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(2)]
        public string Bay { get; set; }

        /// <summary>
        /// Shelf level.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(2)]
        public string Level { get; set; }
    }
}