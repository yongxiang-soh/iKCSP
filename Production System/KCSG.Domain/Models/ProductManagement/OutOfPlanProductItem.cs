using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Models.ProductManagement
{
    public class OutOfPlanProductItem : TX58_OutPlanPdt
    {
        public string ProductName { get; set; }
        public double PackingUnit { get; set; }
        public double Total { get { return F58_TbtCmdEndAmt - F58_StorageAmt; } }
        public string OutofSpecFlag
        {
            get
            {
                if (this.F58_CertificationFlag != null)
                {
                    return
                    EnumsHelper.GetEnumDescription(
                        (Constants.F56_CertificationFlag)Int32.Parse(this.F58_CertificationFlag));
                }
                return null;

            }
        }
        public string F58_TbtEndDateString { get; set; }
    }
}
