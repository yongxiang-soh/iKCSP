using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models;
using KCSG.jsGrid.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Domain.Models.Inquiry.BySupplierName;
using KCSG.Domain.Models.MaterialManagement;

namespace KCSG.Domain.Interfaces.Inquiry
{
    public interface IInquiryBySupplierNameDomain
    {
        ResponseResult<GridResponse<MaterialShelfStatusItem>> SearchCriteria(string supplierCode,
            GridSettings gridSettings, out double total);

        /// <summary>
        /// This function is for exporting data from kneading command database to list.
        /// </summary>
        /// <returns></returns>
        Task<PrintSupplierNameItem> SearchRecordsForPrintingNormal();

        Task<PrintSupplierNameItem> SearchRecordsForPrintingBailment();

        //Task<PrintExternalPreProductNameItem> SearchRecordsForPrintingAll();

        string GetById(string preProductCode);
    }
}
