using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.ProductManagement
{
    public class SelectingItem
    {
        public string CommandNo { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string PreProductLotNo { get; set; }
        public string LotNo { get; set; }
        public double PackQty { get; set; }
        public double Fraction { get; set; }
        public double PackUnit { get; set; }
        public string TabletingEndDate { get; set; }
        public bool IsDuplicate { get; set; }

    }
}
