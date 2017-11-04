using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.Inquiry.ByProductName
{
    public class PrintProductNameItem
    {
        public string Page { get; set; }

        public string Datetime { get; set; }

        public string CompanyName { get; set; }

        public double GrandTotal { get; set; }

        public double CertifiedTotal { get; set; }
        public double NonCertifiedTotal { get; set; }
        public double DeliveryTotal { get; set; }

        public double Totalcer { get; set; }
        public double Totaluncer { get; set; }
        public IList<PrintProductNameGroupItem> PrintProductNameGroupItem { get; set; }

        public PrintProductNameItem()
        {
            PrintProductNameGroupItem = new List<PrintProductNameGroupItem>();
        }
    }
}
