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
    public class ProductCertificationOutOfPlanItem:TX58_OutPlanPdt
    {
        public string ProductName { get; set; }
        public bool IsCreate { get; set; }
        public string F09_ProductDesp { get; set; }

        public string OutofSpecFlag
        {
            get
            {
                return
                    EnumsHelper.GetEnumDescription(
                        (Constants.F56_CertificationFlag)Int32.Parse(this.F58_CertificationFlag));
            }
        }
    }
}
