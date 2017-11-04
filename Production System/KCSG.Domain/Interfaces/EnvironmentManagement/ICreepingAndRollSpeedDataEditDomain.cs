using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models.EnvironmentManagement;

namespace KCSG.Domain.Interfaces.EnvironmentManagement
{
    public interface ICreepingAndRollSpeedDataEditDomain
    {
        ResponseResult<CreepingAndRollSpeedDurationItem> Search(DateTime environmentDate, Constants.RollMachine machine,
            Constants.EnvMode mode);

        void Edit(int id1, int id2, int id3, string environmentDate, string time, double leftCreeping,
            double rightCreeping, double rollSpeed);

        Tuple<Te83_Env_Else, Te83_Env_Else, Te83_Env_Else> ChangeValueOfTime(string environmentDate, string time, int id1, int id2, int id3);
    }
}