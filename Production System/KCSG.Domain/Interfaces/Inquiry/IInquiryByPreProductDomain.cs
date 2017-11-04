using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Models;
using KCSG.Domain.Models.PreProductManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.Inquiry
{
    public interface IInquiryByPreProductDomain
    {


        ResponseResult<GridResponse<SearchInquiryByPreProductCode>> SearchCriteria(string materialCode, string lotNo,
            GridSettings gridSettings, out double total);

    }
}
