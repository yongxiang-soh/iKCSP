namespace KCSG.Domain.Models.MaterialManagement
{
    public class RestorageMaterialItem
    {
        /// <summary>
        /// Material lot number.
        /// </summary>
        public string MaterialLotNo { get; set; }

        public double PackUnit { get; set; }

        /// <summary>
        /// Package quantity.
        /// </summary>
        public double PackQuantity { get; set; }

        /// <summary>
        /// Fraction of material.
        /// </summary>
        public double Fraction { get; set; }
    }
}