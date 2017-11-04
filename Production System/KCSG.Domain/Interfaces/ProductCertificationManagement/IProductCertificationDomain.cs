using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.ProductCertificationManagement
{
    public interface IProductCertificationDomain
    {
        ProductCertificationItem GetById(string prodCode, string prePdtLotNo, string productFlg);
        void Create(ProductCertificationItem productCertification);
        void Update(ProductCertificationItem productCertification);
        bool Delete(string code, string prePdtLotNo, string productFlg);
        bool CheckUnique(string productCode, string productLotNo);
        bool CheckUnique67(string productCode, string prePdtLotNo);
        ResponseResult CreateOrUpdate(ProductCertificationItem model);
        ResponseResult<GridResponse<ProductCertificationItem>> SearchCriteria(string date, GridSettings gridSettings);
        IEnumerable<TH67_CrfHst> GetProductCertification(string productCode);
        ResponseResult<GridResponse<ProductCertificationItem>> SearchSample(string date, GridSettings gridSettings);

        ResponseResult<GridResponse<StorageOfProductItem>> SearchNormal(string date, GridSettings gridSettings);

        ResponseResult<GridResponse<ProductCertificationOutOfPlanItem>> SearchOutOfPlan(string date, GridSettings gridSettings);


        /// <summary>
        /// Sign product certification as Ok status.
        /// </summary>
        /// <param name="certificationFlag"></param>
        /// <param name="productCode"></param>
        /// <param name="prePdtLotNo"></param>
        /// <param name="productLotNo"></param>
        /// <param name="quantity"></param>
        /// <param name="certificationDate"></param>
        /// <returns></returns>
        Task MakeProductCertificationOkAsync(string status, string productCode,
            string prePdtLotNo, string productLotNo, double quantity, string certificationDate);

        /// <summary>
        /// Sign product certification as NG status.
        /// </summary>
        /// <param name="certificationFlag"></param>
        /// <param name="productCode"></param>
        /// <param name="prePdtLotNo"></param>
        /// <param name="productLotNo"></param>
        /// <param name="quantity"></param>
        /// <param name="certificationDate"></param>
        /// <returns></returns>
        Task MakeProductCertificationNgAsync(string status, string productCode,
            string prePdtLotNo, string productLotNo, double quantity, string certificationDate);

        /// <summary>
        /// Find product certification for printing.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        Task<object> FindProductCertificationsForPrinting(Constants.PrintProductCertificationStatus status,
            DateTime? beginDate, DateTime? endDate);
    }
}
