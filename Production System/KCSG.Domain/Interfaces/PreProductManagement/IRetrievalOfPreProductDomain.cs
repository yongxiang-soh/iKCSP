using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Models;
using KCSG.Domain.Models;
using KCSG.Domain.Models.PreProductManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.PreProductManagement
{
    public interface IRetrievalOfPreProductDomain
    {
        ResponseResult<GridResponse<RetrievalOfPreProductItem>> SearchCriteria(GridSettings gridSettings);
        ResponseResult Edit(string commandNo, string preProductCode, string lotNo, string tabletisingLine,string terminalNo);

        /// <summary>
        /// Receive message from C2.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="kndCmdNo"></param>
        /// <param name="prepdtLotNo"></param>
        /// <param name="preProductCode"></param>
        /// <returns></returns>
        IList<SecondCommunicationResponse> ReceiveMessageFromC2(string terminalNo, string kndCmdNo, string prepdtLotNo, string preProductCode);

        ResponseResult CheckConveyor(string terminerNo);
    }
}
