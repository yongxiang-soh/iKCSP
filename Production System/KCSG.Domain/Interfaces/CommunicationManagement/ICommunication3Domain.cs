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
   public interface ICommunication3Domain
    {
        ResponseResult<GridResponse<HistoryItem>> GetHistory(DateTime? date, string terminal, GridSettings gridSettings);
        ResponseResult<GridResponse<QueueItem>> GetQueue(DateTime? date, string terminal, GridSettings gridSettings);
        bool SetDeviceStatus(bool online,string deviceCode);
        bool RequestTimerStatus(bool set);
        bool UpdateConveyorStatus(string conveyorCode);
        ResponseResult DeleteProductWarehouseCommand(string commandNo, string cmdSeqNo);
        bool DeleteProductWarehouseHistories();
        bool EndProductWarehouseCommand(string commandNo, string cmdSeqNo);

       bool CancelProductWarehouseCommand(string commandNo, string cmdSeqNo, int nHowCancel,
           bool nConveyorErr = false);

        /// <summary>
        /// Proceed data of third controller.
        /// </summary>
        /// <param name="isAutomatedWarehouse"></param>
        /// <returns></returns>
        bool ProcessData(bool isAutomatedWarehouse,string deviceCode);

        string GetDeviceStatus(string deviceCode);
    }
}
