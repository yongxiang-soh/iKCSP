using System.ComponentModel.DataAnnotations;

namespace KCSG.Domain.Models.ProductManagement
{
    public class AssignProductShippingViewModel
    {
        public string ShippingNo { get; set; }

        public string ProductCode { get; set; }

        public string ProductLotNo { get; set; }

        public double ShippingQuantity { get; set; }

        public double ShippedAmount { get; set; }
        
    }
}