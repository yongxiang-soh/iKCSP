using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.Inquiry
{
    public class InquiryKneadingLotNoDomain : BaseDomain, IInquiryKneadingLotNoDomain
    {
        private readonly INotificationService _notificationService;
        #region Constructors

        /// <summary>
        ///     Initialize domain with dependency injections.
        /// </summary>
        /// <param name="unitOfWork"></param>
        public InquiryKneadingLotNoDomain(IUnitOfWork unitOfWork, INotificationService notificationService, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        #endregion

        #region Methods


        public ResponseResult<GridResponse<KneadingLotNoItem>> SearchCriteria(string preProCode, string preProLot,
   GridSettings gridSettings)
        {
            //67
            var procerts = _unitOfWork.ProductCertificationRepository.GetAll();
            //09
            var productlst = _unitOfWork.ProductRepository.GetAll();
            //56
            var results = _unitOfWork.TabletProductRepository.GetAll();
            //41
            var tablecmds = _unitOfWork.TabletCommandRepository.GetAll();


            if (!string.IsNullOrEmpty(preProCode) || !string.IsNullOrEmpty(preProLot))
            {
                var result1 = from result in results
                              from product in productlst
                              from procert in procerts
                              from tablecmd in tablecmds
                              where
                                                     procert.F67_PrePdtLotNo.Trim().Equals(result.F56_PrePdtLotNo.Trim()) &&
                                                     procert.F67_ProductLotNo.Trim().Equals(result.F56_ProductLotNo.Trim())
                                                     && procert.F67_ProductCode.Trim().Equals(result.F56_ProductCode.Trim()) &&
                                                     product.F09_ProductCode.Trim().Equals(result.F56_ProductCode.Trim()) &&
                                                     tablecmd.F41_KndCmdNo.Trim().Equals(result.F56_KndCmdNo.Trim()) &&
                                                     tablecmd.F41_PrePdtLotNo.Trim().Equals(result.F56_PrePdtLotNo.Trim())&&
                                                     tablecmd.F41_PreproductCode.Trim().Equals(preProCode.Trim())&&
                                                     tablecmd.F41_PrePdtLotNo.Trim().Equals(preProLot.Trim())
                              select new KneadingLotNoItem
                                                 {
                                                    F09_ProductDesp = product.F09_ProductDesp,
                                                    F56_ProductLotNo = result.F56_ProductLotNo,
                                                    F56_TbtCmdEndAmt = result.F56_TbtCmdEndAmt,
                                                    F56_ProductCode = result.F56_ProductCode,
                                                    F56_PrePdtLotNo= result.F56_PrePdtLotNo,
                                                    F56_CertificationDate = procert.F67_CertificationDate

                                                 };

                var itemCount = result1.Count();
                if (gridSettings != null)
                    OrderByAndPaging(ref result1, gridSettings);
                //total = result.Sum(i => i.F43_LayinginAmount);
                var resultModel = new GridResponse<KneadingLotNoItem>(result1, itemCount);
                return new ResponseResult<GridResponse<KneadingLotNoItem>>(resultModel, true);
            }
            else
            {
                var result1 = from result in results
                              from product in productlst
                              from procert in procerts
                              from tablecmd in tablecmds
                              where
                                                     procert.F67_PrePdtLotNo.Trim().Equals(result.F56_PrePdtLotNo.Trim()) &&
                                                     procert.F67_ProductLotNo.Trim().Equals(result.F56_ProductLotNo.Trim())
                                                     && procert.F67_ProductCode.Trim().Equals(result.F56_ProductCode.Trim()) &&
                                                     product.F09_ProductCode.Trim().Equals(result.F56_ProductCode.Trim()) &&
                                                     tablecmd.F41_KndCmdNo.Trim().Equals(result.F56_KndCmdNo.Trim()) &&
                                                     tablecmd.F41_PrePdtLotNo.Trim().Equals(result.F56_PrePdtLotNo.Trim())
                              select new KneadingLotNoItem
                              {
                                  F09_ProductDesp = product.F09_ProductDesp,
                                  F56_ProductLotNo = result.F56_ProductLotNo,
                                  F56_TbtCmdEndAmt = result.F56_TbtCmdEndAmt,
                                  F56_ProductCode = result.F56_ProductCode,
                                  F56_PrePdtLotNo = result.F56_PrePdtLotNo,
                                  F56_CertificationDate = procert.F67_CertificationDate

                              };
                var itemCount = result1.Count();
                if (gridSettings != null)
                    OrderByAndPaging(ref result1, gridSettings);
                //total = result.Sum(i => i.F43_LayinginAmount);
                var resultModel = new GridResponse<KneadingLotNoItem>(result1, itemCount);
                return new ResponseResult<GridResponse<KneadingLotNoItem>>(resultModel, true);
            }
            //var itemCount = lstResult.Count();
            //var pdtPlnItems = lstResult.AsQueryable();
            //OrderByAndPaging(ref pdtPlnItems, gridSettings);
            //var resultModel = new GridResponse<PdtPlnItem>(pdtPlnItems, itemCount);
            //return new ResponseResult<GridResponse<PdtPlnItem>>(resultModel, true);
        }

        #endregion
    }
}