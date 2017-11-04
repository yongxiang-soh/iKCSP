using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.RightsManagement;
using KCSG.Core.Constants;

namespace KCSG.Domain.Models.PreProductManagement
{
    public class StorageOfPreProductItem
    {
        /// <summary>
        /// Kneading line selection.
        /// </summary>
        [DisplayName(@"Line")]
        public Constants.KndLine KneadingLine { get; set; }

        /// <summary>
        /// Colour pre-product.
        /// </summary>
        [DisplayName(@"Colour")]
        [StringLength(6)]
        public string Colour { get; set; }

        /// <summary>
        /// Temperature of pre-product.
        /// </summary>
        [DisplayName(@"Temperature")]
        [StringLength(6)]
        public string Temperature { get; set; }

        /// <summary>
        /// Command number.
        /// </summary>
        [DisplayName(@"Command No.")]
        [StringLength(6)]
        public string CommandNo { get; set; }

        /// <summary>
        /// Pre-product code.
        /// </summary>
        [DisplayName(@"Pre-product Code")]
        [StringLength(12)]
        public string PreProductCode { get; set; }

        /// <summary>
        /// Pre-product name.
        /// </summary>
        [DisplayName(@"Pre-product Name")]
        [StringLength(15)]
        public string PreProductName { get; set; }

        /// <summary>
        /// Container number.
        /// </summary>
        [DisplayName(@"Container No.")]
        [StringLength(3)]
        public string ContainerNo { get; set; }

        /// <summary>
        /// Container type.
        /// </summary>
        [DisplayName(@"Container Type")]
        [StringLength(2)]
        public string ContainerType { get; set; }

        /// <summary>
        /// Container code.
        /// </summary>
        [DisplayName(@"Container Code")]
        [StringLength(12)]
        public string ContainerCode { get; set; }

        /// <summary>
        /// Pre-product lot number.
        /// </summary>
        [DisplayName(@"Pre-product Lot No.")]
        [StringLength(10)]
        public string PreProductLotNo { get; set; }

        /// <summary>
        /// Storaged Container Quantity.
        /// </summary>
        [DisplayName(@"Storage Container")]
        public int StoragedContainerQuantity { get; set; }

        /// <summary>
        /// Check status button OK
        /// </summary>
        public bool IsOK { get; set; }

        public double Quantity { get; set; }
    }
}