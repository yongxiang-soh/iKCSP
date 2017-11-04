using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Models;

namespace KCSG.Domain.Interfaces.SystemManagement
{
    public interface IDailyProcessDomain
    {
        ResponseResult DailyProcess(string terminalNo);
    }
}
