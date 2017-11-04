using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KCSG.Domain.Models.Tabletising
{
    public class InitiateTabletisingCommandViewModel
    {
        /// <summary>
        /// Kneading command number (F42_KndNo)
        /// </summary>
        [Required]
        public string KneadingNo { get; set; }

        /// <summary>
        /// Pre-product code (F42_PreProductCode)
        /// </summary>
        [Required]
        public string PreproductCode { get; set; }

        /// <summary>
        /// Lot number (F39_PrePdtLotAmt)
        /// </summary>
        [Required]
        public string LotNo { get; set; }
        
        public double Remaining { get; set; }

        /// <summary>
        /// Items on the lower table.
        /// </summary>
        [Required]
        public List<ProductLowerTable> Items { get; set; } 
    }
}