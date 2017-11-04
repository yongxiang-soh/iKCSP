using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KCSG.Domain.Models.Inquiry.ByExternalPre_ProductName
{
    public class PrintExternalPreProductNameGroup
    {
        public double PreProductTotal { get; set; }
        public string PreProductTotalString { get; set; }

        

        public IList<PrintExternalPreProductNameGroupItem> PrintExternalPreProductNameGroupItem { get; set; }

        public PrintExternalPreProductNameGroup()
        {
            PrintExternalPreProductNameGroupItem = new List<PrintExternalPreProductNameGroupItem>();
        }
    }
}
