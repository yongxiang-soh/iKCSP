using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.Inquiry.ByWareHouseLocation
{
    public class InquiryBySupplierPalletItem
    {
        public string ShelfNo { get; set; }
        public string ShelfStatus { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public int MaxPallet { get; set; }
        public int StockedPallet { get; set; }
        public string Row { get; set; }
        public string Bay { get; set; }
        public string Level { get; set; }
    }
}
