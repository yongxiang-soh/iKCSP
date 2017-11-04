using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.TabletisingCommondSubSystem
{
    public class SearchProductInformationItem
    {
        public string F09_ProductCode { get; set; }
        public string F09_ProductDesp { get; set; }

        public double CommandQty { get; set; }
        public double StorageQty { get; set; }
        public double Yieldrate { get; set; }
    }
}
