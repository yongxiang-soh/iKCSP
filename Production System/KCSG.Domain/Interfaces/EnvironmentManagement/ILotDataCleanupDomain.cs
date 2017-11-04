using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Interfaces.EnvironmentManagement
{
    public interface ILotDataCleanupDomain
    {
        int GetLots();
        IList<Te84_Env_Lot> GetListTe84EnvLots();
        int Testing(string stringNewCutOffDate, string stringNewCutOffTime);
        bool Delete(string stringNewCutOffDate, string stringNewCutOffTime);
    }
}
