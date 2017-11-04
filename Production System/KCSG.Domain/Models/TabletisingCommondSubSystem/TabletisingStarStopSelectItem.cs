using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Models.TabletisingCommondSubSystem
{
    public class TabletisingStarStopSelectItem : TX56_TbtPdt
    {
        public string ProductName { get; set; }
        public double PackUnit { get; set; }

        public string Status
        {
            get
            {
                return EnumsHelper.GetEnumDescription((Constants.TX56_Status)Int32.Parse(this.F56_Status));
            }
        }
    }
}