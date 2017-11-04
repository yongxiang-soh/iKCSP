using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.Inquiry.ByWareHouseLocation
{
    public class InquiryByProductShelfStatusExternalPreProductItem
    {
        public string ShelfNo { get; set; }
        public string ShelfStatus { get; set; }
        public string PreProductCode { get; set; }
        public string PreProductName { get; set; }
        public string PreProductLotNo { get; set; }
        public string PalletNo { get; set; }
        public double Amount { get; set; }
        public string KneadingCommandNo { get; set; }
        public int PalletSeqNo { get; set; }
        public DateTime StorageDate { get; set; }
        public string Row { get; set; }
        public string Bay { get; set; }
        public string Level { get; set; }
    }
}
