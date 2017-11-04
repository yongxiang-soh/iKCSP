namespace KCSG.Domain.Models.ProductManagement
{
    public class ProductShippingCommandItem
    {
        public string F44_ShipCommandNo { get; set; }

        public string F44_ProductCode { get; set; }

        public string F09_ProductDesp { get; set; }

        public string F44_ProductLotNo { get; set; }
        
        public double F44_ShpRqtAmt { get; set; }

        public double F44_ShippedAmount { get; set; }

        public string ShippingQuantity
        {
            get { return F44_ShpRqtAmt.ToString("###,###.00"); }
        }

        public string ShippedAmount
        {
            get { return F44_ShippedAmount.ToString("###,###.00"); }
        }

        public string ShelfNo { get; set; }
    }
}