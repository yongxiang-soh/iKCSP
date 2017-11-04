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
using KCSG.Domain.Models.ManagementReport;

namespace KCSG.Domain.Interfaces.ManagementReport
{
    public interface IManagementReportDomain
    {

        /// <returns></returns>
        Task<PrintManagementReportItem> SearchMaterialMovementHistory(string from,string to);

        Task<PrintManagementReportItem> SearchPreProductMovementHistory(string from, string to);
        Task<PrintManagementReportItem> SearchProductMovementHistory(string from, string to);
        Task<PrintManagementReportItem> SearchMaterialMovementRecord(string from, string to);
        Task<PrintManagementReportItem> SearchPreProductMovementRecord(string from, string to);
        Task<PrintManagementReportItem> SearchProductMovementRecord(string from, string to);
        Task<PrintManagementReportItem> SearchProductShippingRecord(string from, string to);
        Task<PrintManagementReportItem> SearchProductCertificationRecord(string from, string to);
        Task<PrintManagementReportItem> SearchMaterialRetrievalRecord(string from, string to);
        Task<PrintManagementReportItem> SearchPreProductRetrievalRecord(string from, string to);

        bool CheckConsumerMaterials(string yearmonth);
        Task<ResponseResult<GridResponse<object>>> LoadConsumerMaterials(
            string yearmonth, int page);
        bool UpdateConsumer(DateTime yearMonth, double received, double remain, double used, string materialCode);
        Task<ResponseResult<GridResponse<object>>> Recalculate(string yearmonth,string matcode);
        Task<PrintManagementReportItem> PrintConsumerMaterials(string yearmonth);


        bool CheckConsumerCerfiticates(string yearmonth);
        Task<ResponseResult<GridResponse<object>>> LoadConsumerCerfiticates(
    string yearmonth,int page);
        bool UpdateConsumerCerfiticate(DateTime yearMonth, double received, double remain, double used,
            string preproductCode);

        Task<ResponseResult<GridResponse<object>>> RecalculateCerfiticate(string yearmonth, string procode);
        Task<PrintManagementReportItem> PrintConsumerCerfiticates(string yearmonth);

        bool CheckConsumerPreProducts(string yearmonth);
        Task<ResponseResult<GridResponse<object>>> LoadConsumerPreProducts(
    string yearmonth, int page);
        bool UpdatePreProductConsumer(DateTime yearMonth, double received, double remain, double used,
            string preproductCode);

        Task<ResponseResult<GridResponse<object>>> RecalculatePreProduct(string yearmonth,string precode);
        Task<PrintManagementReportItem> PrintConsumerPreProducts(string yearmonth);
    }
}
