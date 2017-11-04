using System.ComponentModel.DataAnnotations;

namespace KCSG.Domain.Models.ProductManagement
{
    public class UnassignShippingCommandViewModel
    {
        /// <summary>
        /// Shipping number.
        /// </summary>
        [Required]
        public string ShippingNo { get; set; }
    }
}