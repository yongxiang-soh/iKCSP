using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Domain.Models;

using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.CommunicationManagement
{
    public interface ICommunication1Domain
    {
        ResponseResult<GridResponse<HistoryItem>> GetHistory(DateTime? date, string terminal, GridSettings gridSettings);
        ResponseResult<GridResponse<QueueItem>> GetQueue(DateTime? date, string terminal, GridSettings gridSettings);
        bool SetDeviceStatus(bool online,string deviceCode);
        ResponseResult DeleteMaterialWarehouseCommand(string commandNo, string cmdSeqNo);
        bool DeleteMaterialWarehouseHistories();
        bool EndMaterialWarehouseCommand(string commandNo, string cmdSeqNo);
        bool CancelMaterialWarehouseCommand(string commandNo, string cmdSeqNo);

        /// <summary>
        /// Proceed data of automated warehouse.
        /// </summary>
        /// <param name="isAutomatedWarehouse"></param>
        /// <param name="deviceCode"></param>
        /// <returns></returns>
        bool ProcessData(bool isAutomatedWarehouse,string deviceCode);

        string GetDeviceStatus(string deviceCode);
    }
}
