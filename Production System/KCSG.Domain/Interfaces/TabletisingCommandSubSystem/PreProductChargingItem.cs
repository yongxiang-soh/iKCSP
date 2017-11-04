using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;

namespace KCSG.Domain.Interfaces.TabletisingCommandSubSystem
{
    public class PreProductChargingItem
    {
        public string KneadingCmdNo { get; set; }
        public string PreProductCode { get; set; }
        public string PreProductName { get; set; }
        public string PreProductLotNo { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductLotNo { get; set; }
        public string ContainerCode1 { get; set; }
        public string ContainerCode2 { get; set; }
        //public Constants.LockStatus LockStatus { get; set; }
        public bool IsError { get; set; }
        public string ErrorCode { get; set; }
        
    }
}
