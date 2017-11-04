using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Domain.Models.EnvironmentManagement;

namespace KCSG.Domain.Interfaces.EnvironmentManagement
{
    public interface ICreepingAndRollSpeedDurationDomain
    {
        ResponseResult<CreepingAndRollSpeedDurationItem> Search(DateTime startDate, DateTime endDate, Constants.RollMachine machine,
            Constants.EnvMode mode);
    }
}
