using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.Inquiry.ByWareHouseLocation
{
    public class PreProductShelfStatusItem
    {
        public string Row { get; set; }
        public string Bay { get; set; }
        public string Level { get; set; }
        public string ShelfNo { get; set; }
        public string ContainerType { get; set; }
        public string ContainerName { get; set; }
        public string PreProductCode { get; set; }
        public string PreProductName { get; set; }
        public string PreProductLotNo { get; set; }
        public string ContainerCode { get; set; }
        public string ContainerNo { get; set; }
        public string KneadingCommandNo { get; set; }
        public string ShelfStatus { get; set; }
        public DateTime StorageDate { get; set; }
        public int ContainerSeqNo { get; set; }
        public double Amount { get; set; }
    }
}
