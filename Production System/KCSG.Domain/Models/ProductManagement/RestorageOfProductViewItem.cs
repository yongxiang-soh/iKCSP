using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Models.ProductManagement
{
    public class RestorageOfProductViewItem : TX40_PdtShfStk
    {
        public string ProductName { get; set; }
        public double PackUnit { get; set; }
        public int Remainder { get; set; }

        public double Fraction
        {
            get
            {
                if (PackUnit != 0)
                    return Math.Round((this.F40_Amount-this.F40_ShippedAmount) % PackUnit);
                return 0;
            }
        }

        public double Total
        {
            get { return Math.Round(Remainder * PackUnit + Fraction, 2); }
        }

        public string TabletingEndDate
        {
            get { return this.F40_TabletingEndDate.ToString("yyyy-MM-dd"); }
        }

        public string CertificationDate
        {
            get
            {
                return this.F40_CertificationDate.HasValue
                    ? this.F40_CertificationDate.Value.ToString("yyyy-MM-dd")
                    : null;
            }
        }

        public string AddDate
        {
            get { return this.F40_AddDate.ToString("yyyy-MM-dd"); }
        }
    }
}