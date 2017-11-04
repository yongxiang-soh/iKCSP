using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Models
{
   public class ConveyorItem:TM05_Conveyor
    {
       public string Status { get { return EnumsHelper.GetDescription<Constants.F05_StrRtrSts>( ConvertHelper.ToInteger(this.F05_StrRtrSts)); } }
    }
}
