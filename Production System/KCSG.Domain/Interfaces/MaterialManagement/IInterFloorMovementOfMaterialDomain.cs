using System.Collections.Generic;
using System.Threading.Tasks;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Interfaces.MaterialManagement
{
    public interface IInterFloorMovementOfMaterialDomain
    {
        bool CheckedWarehouseStatus();
        bool CheckRecordTm05(string terminalNo);
        bool CheckConveyorStatus(string terminalNo);
        string CreateOrUpdate(int from,int to,string terminalNo);

        /// <summary>
        /// Complete inter-floor movement.
        /// </summary>
        Task<IList<TX34_MtrWhsCmd>> Complete(string terminalNo);
    }
}
