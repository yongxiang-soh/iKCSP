using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.Inquiry
{
    public class FindPrintMaterialNameItem
    {
        public string MaterialCode { get; set; }

        public string MaterialName { get; set; }        

        public string LotNo { get; set; }

        public string PalletNo { get; set; }

        public string RBL { get; set; }

        public double Quantity { get; set; }
        public string QuantityString
        {
            get { return String.Format("{0:#,##0.00}", Quantity); }
        }

    }
}
