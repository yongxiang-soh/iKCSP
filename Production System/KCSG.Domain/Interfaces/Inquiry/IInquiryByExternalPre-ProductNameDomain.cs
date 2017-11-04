using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models;
using KCSG.jsGrid.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Domain.Models.Inquiry.ByExternalPre_ProductName;
using KCSG.Domain.Models.ProductManagement;

namespace KCSG.Domain.Interfaces.Inquiry
{
    public interface IInquiryByExternalPreProductNameDomain
    {

        ResponseResult<GridResponse<StockTakingOfProductItem>> SearchCriteria(string materialCode,
            GridSettings gridSettings, out double total);

        /// <summary>
        /// This function is for exporting data from kneading command database to list.
        /// </summary>
        /// <returns></returns>
        Task<PrintExternalPreProductNameItem> SearchRecordsForPrintingNormal();

        Task<PrintExternalPreProductNameItem> SearchRecordsForPrintingExternal();

        //Task<PrintExternalPreProductNameItem> SearchRecordsForPrintingAll();

        string GetById(string preProductCode);
    }
}
