using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models;
using KCSG.jsGrid.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Domain.Models.Inquiry;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.Domain.Models.ProductionPlanning;

namespace KCSG.Domain.Interfaces.Inquiry
{
    public interface IInquiryByMaterialNameDomain
    {
        ResponseResult CreateOrUpdate(string selectedValue, int within);
        bool DeleteMaterialName(string deleteDate, string deleteCode);

        ResponseResult<GridResponse<MaterialShelfStatusItem>> SearchCriteria(string materialCode,
            GridSettings gridSettings, out double total);

        /// <summary>
        /// This function is for exporting data from kneading command database to list.
        /// </summary>
        /// <returns></returns>
        Task<PrintMaterialNameItem> SearchRecordsForPrinting();

        Task<PrintMaterialNameItem> SearchRecordsForPrintingBailment();
        MaterialItem GetById(string materialCode);
    }
}
