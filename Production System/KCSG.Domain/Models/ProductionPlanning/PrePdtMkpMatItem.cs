using KCSG.Core.Constants;
using KCSG.Data.DataModel;
using System;
using System.Linq;
using KCSG.Core.Helper;

namespace KCSG.Domain.Models.ProductionPlanning
{
    public class PrePdtMkpMatItem : TM02_PrePdtMkp
    {
        public string F01_MaterialDsp { get; set; }

        public string Additive
        {
            get
            {
                return Enum.GetName(typeof(Constants.Additive), Convert.ToInt32(this.F02_Addtive));
            }
        }

        public string Method
        {
            get
            {
                return Enum.GetName(typeof(Constants.WeighingMethod), Convert.ToInt32(this.F02_WeighingMethod));
            }
        }

        public string Crushing1
        {
            get
            {
                return EnumsHelper.GetEnumDescription((Constants.Crushing)this.F02_MilingFlag1);
                 
            }
        }

        public string Crushing2
        {
            get
            {
                return EnumsHelper.GetEnumDescription((Constants.Crushing)this.F02_MilingFlag2);

            }
        }

        public string LayinAmount
        {
            get { return F02_3FLayinAmount.ToString("0.00"); }
        }
    }
}
