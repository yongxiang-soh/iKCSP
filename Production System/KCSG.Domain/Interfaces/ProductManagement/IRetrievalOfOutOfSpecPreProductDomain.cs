using System.Collections.Generic;
using System.Threading.Tasks;
using KCSG.Core.Models;
using KCSG.Domain.Models;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.ProductManagement
{
    public interface IRetrievalOfOutOfSpecPreProductDomain
    {
        ResponseResult<GridResponse<TempTableItem>> SearchAsync(string[] status, GridSettings gridSettings);

        /// <summary>
        ///     Storage or retrieval pre-product.
        /// </summary>
        /// <param name="shelfNo"></param>
        /// <param name="terminerNo"></param>
        /// <param name="shelfStatus"></param>
        /// <param name="emptyPallet"></param>
        /// <param name="storage"></param>
        /// <returns></returns>
        ResponseResult StorageOrRetrieval(string shelfNo, string terminerNo, string shelfStatus, bool emptyPallet, bool storage);

        /// <summary>
        /// Respond message from C3.
        /// </summary>
        /// <param name="shelfNo"></param>
        /// <param name="terminalNo"></param>
        /// <param name="shelftStatus"></param>
        /// <returns></returns>
        List<ThirdCommunicationResponse> ResponeMessageC3(string shelfNo, string terminalNo, string shelftStatus);

        bool CheckDeviceStatus(string deviceCode);
    }
}