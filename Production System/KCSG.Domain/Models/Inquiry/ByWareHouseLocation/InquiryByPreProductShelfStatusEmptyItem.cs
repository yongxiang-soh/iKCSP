using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.Inquiry.ByWareHouseLocation
{
    public class InquiryByPreProductShelfStatusEmptyItem
    {
        public string ShelfNo { get; set; }
        public string ShelfStatus { get; set; }
        public string ContainerType { get; set; }
        public string ContainerName { get; set; }
        public string ContainerNo { get; set; }
        public string Row { get; set; }
        public string Bay { get; set; }
        public string Level { get; set; }
    }
}
