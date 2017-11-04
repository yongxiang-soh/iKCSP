using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Interfaces.EnvironmentManagement
{
    public interface IAvailabityDataEditDomain
    {
        void Edit(string status, string environmentDate, string time, int id);
        string GetStatusInTe82_Env_Aval(int id, string environmentDate, string time);
    }
}
