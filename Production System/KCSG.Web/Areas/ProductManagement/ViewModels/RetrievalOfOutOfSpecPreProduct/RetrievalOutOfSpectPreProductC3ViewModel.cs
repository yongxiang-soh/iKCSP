using System.ComponentModel.DataAnnotations;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.RetrievalOfOutOfSpecPreProduct
{
    public class RetrievalOutOfSpectPreProductC3ViewModel
    {
        /// <summary>
        /// Shelf status of pre-product.
        /// </summary>
        [Required]
        public string ShelfStatus { get; set; }

        /// <summary>
        /// Number of selected shelf.
        /// </summary>
        [Required]
        public string ShelfNo { get; set; }
    }
}