using System.Collections.Generic;
using KCSG.Core.Models;
using KCSG.Domain.Models;
using KCSG.Domain.Models.MaterialManagement;

namespace KCSG.Domain.Interfaces.MaterialManagement
{
    public interface IRetrievalOfWarehousePalletDomain
    {
        int GetPossibleQuantity();
        ResponseResult Retrieval(RetrievalOfWarehousePalletItem item,string terminalNo);
        IList<FirstCommunicationResponse> RetrieveWarehousePalletMessageC1Reply(string terminalNo);
    }
}
