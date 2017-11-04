using System.ComponentModel.DataAnnotations;

namespace KCSG.Domain.Models.Tabletising
{
    public class ProductLowerTable
    {
        [Required]
        public string ProductCode { get; set; }

        [Required]
        public string ProductName { get; set; }

        public double TabletisingQuantity { get; set; }

        public double? UsedPreProduct { get; set; }


        /// <summary>
        /// Lot number
        /// </summary>
        [Required]
        public string LotNo { get; set; }

        /// <summary>
        /// yieldrate
        /// </summary>
        public double Yieldrate { get; set; }
    }
}