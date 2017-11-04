using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Domain.Models;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.Inquiry
{
    public interface IInquiryKneadingCommandNoDomain
    {
        /// <summary>
        /// Load kneading commands from database.
        /// </summary>
        /// <param name="gridSettings"></param>
        /// <param name="kneadingCommandline"></param>
        /// <returns></returns>
        ResponseResult<GridResponse<KneadingCommandItem>> SearchCriteria(string productCode,
            GridSettings gridSettings);

        InquiryKneadingCommandNoRestlt GetCodeNamebycmNo(string commandNo);

    }
}