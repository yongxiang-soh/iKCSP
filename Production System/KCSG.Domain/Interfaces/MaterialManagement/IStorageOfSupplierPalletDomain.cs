using System.Collections.Generic;
using KCSG.Core.Models;
using KCSG.Domain.Models;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.jsGrid.MVC;
using Microsoft.SqlServer.Server;

namespace KCSG.Domain.Interfaces.MaterialManagement
{
    public interface IStorageOfSupplierPalletDomain
    {
        ResponseResult<GridResponse<MaterialShelfStatusItem>> SearchCriteria(string supplierCode, int maxPallet, GridSettings gridSettings);

        ResponseResult CreateOrUpdate(string shelfRow, string shelfBay, string shelfLevel, string terminalNo);
        bool CheckDeviceStatus();
        bool CheckConveyorStatus(string terminalNo);
        IList<FirstCommunicationResponse> StorageOfSupplierPalletMessageC1Reply(string terminalNo);

        void DetailStorage(string shelfNo, string supplierCode, int stackedPallet,
            int inCrementPallet,string terminalNo);
        IList<FirstCommunicationResponse> DetailStorageOfSupplierPalletMessageC1Reply(string terminalNo);
    }
}
