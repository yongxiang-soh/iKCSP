namespace KCSG.Domain.Models.Tabletising
{
    public class TabletisingKneadingCommandItem
    {
        /// <summary>
        /// Kneading command number (F42_KndNo)
        /// </summary>
        public string KneadingNo { get; set; }

        /// <summary>
        /// Pre-product code (F42_PreProductCode)
        /// </summary>
        public string PreproductCode { get; set; }

        /// <summary>
        /// Pre-product name (F03_PreProductName)
        /// </summary>
        public string PreproductName { get; set; }

        /// <summary>
        /// Lot number (F39_PrePdtLotAmt)
        /// </summary>
        public string  LotNo { get; set; }

        /// <summary>
        /// Lot quantity (F03_BatchLot)
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Status of kneading command (F42_Status)
        /// </summary>
        public string Status { get; set; }
    }
}