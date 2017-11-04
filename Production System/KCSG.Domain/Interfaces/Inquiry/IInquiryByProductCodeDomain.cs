using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Models;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.Inquiry
{
    public interface IInquiryByProductDomain
    {


        ResponseResult<GridResponse<StorageOfProductItem>> SearchCriteria(string productCode,
            GridSettings gridSettings);

        string Searchuser(string proCode, string proName);
    }
}
