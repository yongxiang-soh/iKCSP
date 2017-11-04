using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models;
using KCSG.jsGrid.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Domain.Models.Inquiry.ByProductName;
using KCSG.Domain.Models.ProductManagement;

namespace KCSG.Domain.Interfaces.Inquiry
{
    public interface IInquiryByProductNameDomain
    {
        ResponseResult<GridResponse<ForceRetrievalOfProductItem>> SearchCriteria(string productCode,
            GridSettings gridSettings, out double total, out double deliveryTotal, out double cerTotal, out double nonCerTotal);

        /// <summary>
        /// This function is for exporting data from kneading command database to list.
        /// </summary>
        /// <returns></returns>
        Task<PrintProductNameItem> SearchRecordsForPrintingCertified();

        Task<PrintProductNameItem> SearchRecordsForPrintingNotCertified();

        //Task<PrintProductNameItem> SearchRecordsForPrintingAll();

        string GetById(string productCode);
    }
}
