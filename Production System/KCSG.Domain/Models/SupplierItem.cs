using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Models
{
    public class SupplierItem:TM04_Supplier
    {
        public string F04_SupplierCode { get; set; }
        public string F04_SupplierName { get; set; }
    }
}
