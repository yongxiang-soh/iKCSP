using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KCSG.Domain.Models.Inquiry.ByExternalPre_ProductName
{
    public class PrintExternalPreProductNameItem
    {
        public string Page { get; set; }

        public string Datetime { get; set; }

        public string CompanyName { get; set; }

        public double PreproductTotal { get; set; }
        public string PreproductTotalString { get; set; }

        public IList<PrintExternalPreProductNameGroup> PrintExternalPreProductNameGroup { get; set; }

        public PrintExternalPreProductNameItem()
        {
            PrintExternalPreProductNameGroup = new List<PrintExternalPreProductNameGroup>();
        }
    }
}
