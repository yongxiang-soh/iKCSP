using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models;
using KCSG.jsGrid.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Domain.Models.Inquiry.ByPreProductName;
using KCSG.Domain.Models.ManagementReport;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.Domain.Models.ProductionPlanning;

namespace KCSG.Domain.Interfaces.ManagementReport
{
    public interface IMaterialStockDomain
    {

        /// <returns></returns>
        Task<PrintMaterialNameItem> SearchRecordsForPrinting(string status);

        Task<PrintMaterialNameItem> SearchSupplementaryRecordsForPrintingAll();
        Task<PrintMaterialNameItem> SearchRecordsForPrintingAll();

        Task<PrintPreProductNameItem> SearchPreProductForPrint();
        Task<PrintPreProductNameItem> SearchExtPreProductForPrint();
        Task<PrintMaterialNameItem> SearchProductForPrint();

        Task<PrintPreProductNameItem> SearchMaterialPalletForPrint();
        Task<PrintPreProductNameItem> SearchProductPalletForPrint();
        Task<PrintMaterialNameItem> SearchMetarialShelftForPrintingAll();

        Task<PrintPreProductNameItem> SearchPreProductContainerForPrint();
        MaterialItem GetById(string materialCode);
    }
}
