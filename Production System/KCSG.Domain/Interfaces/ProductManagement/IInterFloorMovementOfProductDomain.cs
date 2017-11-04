using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Domain.Models;

namespace KCSG.Domain.Interfaces.ProductManagement
{
    public interface IInterFloorMovementOfProductDomain
    {
        bool CheckConveyorStatus();
        bool CheckDeviceStatus(string deviceCode);
        bool CheckConveyorCode(int from);
        void TranferInterFloor(int from,string terminalNo);

        /// <summary>
        /// Analyze data as messages sent back from C3.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        List<ThirdCommunicationResponse> Reply(string terminalNo);
    }
}
