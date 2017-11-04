namespace KCSG.Domain.Models.ProductManagement
{
    public class ExternalPreProductItem
    {
        /// <summary>
        /// Kneading command no.
        /// </summary>
        public string F41_KndCmdNo { get; set; }

        /// <summary>
        /// Pre-product code.
        /// </summary>
        public string F41_PreProductCode { get; set; }

        /// <summary>
        /// Pre-product name.
        /// </summary>
        public string F03_PreProductName { get; set; }

        /// <summary>
        /// Pre-product lot no.
        /// </summary>
        public string F41_PrePdtLotNo { get; set; }

        /// <summary>
        /// Throw amount.
        /// </summary>
        public double F42_ThrowAmount { get; set; }

        /// <summary>
        /// Table line.
        /// </summary>
        public string F41_TableLine { get; set; }

        /// <summary>
        /// Table cnt amount.
        /// </summary>
        public double F41_TblCntAmt { get; set; }
        
        public double F41_RtrEndCntAmt { get; set; }

        public double PalletSeqNo
        {
            get { return F41_TblCntAmt - F41_RtrEndCntAmt; }
        }

        public string Line
        {
            get
            {
                if (string.IsNullOrEmpty(F41_TableLine))
                    return "";

                var tableListingLine = F41_TableLine.Trim();

                
                var tableLineLength = tableListingLine.Length - 2;

                return tableListingLine.Substring(tableLineLength < 0 ? 0 : tableLineLength);
            }
        }
    }
}