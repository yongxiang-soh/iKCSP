using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Models.TabletisingCommondSubSystem
{
    public class TabletisingStartStopControlItem :TX41_TbtCmd
    {
        public string PreProductName { get; set; }
        public string F49_ContainerCode { get; set; }
        public double Quantity { get; set; }
        public DateTime? RetrievalDate { get; set; }
        public DateTime? TmpReturnTime { get; set; }
        public string PassedTime { get; set; }

        public string Status
        {
            get
            {
                return EnumsHelper.GetDescription<Constants.TX41_Status>(ConvertHelper.ToInteger(F41_Status));
            }
        }


    }
}
