using KCSG.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Interfaces.SystemManagement
{
    public interface IStartEndSystemDomain
    {
        ResponseResult EndSystem(Enum startOrEnd, string terminalNo);
        TM14_Device GetStatus();
    }
}
