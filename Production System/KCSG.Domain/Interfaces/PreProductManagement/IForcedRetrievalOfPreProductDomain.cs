using System.Collections.Generic;
using KCSG.Core.Models;
using KCSG.Domain.Domains.PreProductManagement;
using KCSG.Domain.Models;
using KCSG.Domain.Models.PreProductManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.PreProductManagement
{
    public interface IForcedRetrievalOfPreProductDomain
    {
        ResponseResult<GridResponse<ForcedRetrievalOfPreProductItem>> SearchCriteria(GridSettings gridSettings,
            string groupNameValue);

        List<ForcedRetrievalOfPreProductItem> Getallitem(string lotno, string groupNameValue);
        ResultSuccess Edit(string commandNo, string lotNo, string terminalNo,string containercode,string containerno,string shelfno);

        /// <summary>
        /// Respond messages sent back from C2 in Forced retrieval.
        /// </summary>
        /// <param name="preProductCode"></param>
        /// <param name="shelfNo"></param>
        /// <param name="cmdNo"></param>
        /// <param name="lotNo"></param>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        List<SecondCommunicationResponse> ForcedRetrievalMessageC2Reply(string preProductCode, string shelfNo,
            string cmdNo, string lotNo,
            string terminalNo, bool isNotCommand);
    }
}