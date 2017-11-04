using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Models;
using KCSG.Domain.Models;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.CommunicationManagement
{
     public interface ICommunication2Domain
    {
        ResponseResult<GridResponse<HistoryItem>> GetHistory(DateTime? date, string terminal, GridSettings gridSettings);
        ResponseResult<GridResponse<QueueItem>> GetQueue(DateTime? date, string terminal, GridSettings gridSettings);
        bool SetDeviceStatus(bool online,string deviceCode);
        bool RequestTimerStatus(bool set);
        bool UpdateConveyorStatus(string conveyorCode);
        ResponseResult DeletePreProductWarehouseCommand(string commandNo, string cmdSeqNo);
        bool DeletepreProductWarehouseHistories();
        bool EndPreProductWarehouseCommand(string commandNo, string cmdSeqNo,string cmd);

         bool CancelpreProductWarehouseCommand(string commandNo, string cmdSeqNo, int nHowCancel,
             bool nConveyorErr = false);

        /// <summary>
        /// Proceed data of the second controller.
        /// </summary>
        /// <returns></returns>
         bool ProcessData(string deviceCode);


        string GetDeviceStatus(string deviceCode);
    }
}
