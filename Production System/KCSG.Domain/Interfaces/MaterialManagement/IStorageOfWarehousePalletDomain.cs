using System.Collections.Generic;
using KCSG.Core.Models;
using KCSG.Domain.Models;

namespace KCSG.Domain.Interfaces.MaterialManagement
{
    public interface IStorageOfWarehousePalletDomain
    {
        ResponseResult CreateOrUpdate(string terminalNo);
        bool CheckedRecordInTM05(string terminalNo);
        bool CheckStatusForTX31();
        bool CheckStatusForTX31_1();
        bool CheckedRecordInTM01(string materialCode);
        IList<FirstCommunicationResponse> PostRetrieveMaterial(string terminalNo);
    }
}
