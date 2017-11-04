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
using KCSG.Domain.Interfaces.KneadingCommand;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.Inquiry
{
    public class InquiryKneadingCommandNoDomain : BaseDomain, IInquiryKneadingCommandNoDomain
    {
        private readonly INotificationService _notificationService;
        #region Constructors

        /// <summary>
        ///     Initialize domain with dependency injections.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="notificationService"></param>
        /// <param name="configurationService"></param>
        public InquiryKneadingCommandNoDomain(IUnitOfWork unitOfWork, INotificationService notificationService, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        #endregion

        #region Methods


        public ResponseResult<GridResponse<KneadingCommandItem>> SearchCriteria(string productCode,
   GridSettings gridSettings)
        {
            var result = _unitOfWork.KneadingCommandRepository.GetAll();

            var productsheft = _unitOfWork.ProductShelfRepository.GetAll();
            var productsheftstock = _unitOfWork.ProductShelfStockRepository.GetAll();
            if (!string.IsNullOrEmpty(productCode))
            {

                result = result.Where(x => x.F42_KndCmdNo.Trim() == productCode.Trim()).OrderBy(x => x.F42_LotSeqNo);

                var result1 = result.Select(x => new KneadingCommandItem()
                {
                    F42_PrePdtLotNo = x.F42_PrePdtLotNo,
                    F42_KndBgnDate = x.F42_KndBgnDate,
                    F42_TrwEndDate = x.F42_TrwEndDate,
                    F42_KndEndDate = x.F42_KndEndDate,
                    F42_StgCtnAmt = x.F42_StgCtnAmt,
                    F42_Status = x.F42_Status.Trim().Equals("0") ? "Not Kneaded" : x.F42_Status.Trim().Equals("1") ? "Kneading" : x.F42_Status.Trim().Equals("2") ? "Completed" : x.F42_Status.Trim().Equals("3") ? "Forced Completed" : x.F42_Status.Trim().Equals("4") ? "Stored" : x.F42_Status.Trim().Equals("5") ? "Tabletised" : x.F42_Status.Trim().Equals("6") ? "Knead Command" : x.F42_Status.Trim().Equals("7") ? "Tablet Command" : x.F42_Status.Trim().Equals("8") ? "Forced Retrieving" : "Forced Retrieved",
                    F42_LotSeqNo = x.F42_LotSeqNo,
                    F42_ThrowAmount = x.F42_ThrowAmount


                });
                var itemCount = result1.Count();
                if (gridSettings != null)
                    OrderByAndPaging(ref result1, gridSettings);
                //total = result.Sum(i => i.F43_LayinginAmount);
                var resultModel = new GridResponse<KneadingCommandItem>(result1, itemCount);
                return new ResponseResult<GridResponse<KneadingCommandItem>>(resultModel, true);
            }
            else
            {
                var result1 = result.Select(x => new KneadingCommandItem()
                {
                    F42_PrePdtLotNo = x.F42_PrePdtLotNo,
                    F42_KndBgnDate = x.F42_KndBgnDate,
                    F42_TrwEndDate = x.F42_TrwEndDate,
                    F42_KndEndDate = x.F42_KndEndDate,
                    F42_StgCtnAmt = x.F42_StgCtnAmt,
                    F42_Status = x.F42_Status.Trim().Equals("0") ? "Not Kneaded" : x.F42_Status.Trim().Equals("1") ? "Kneading" : x.F42_Status.Trim().Equals("2") ? "Completed" : x.F42_Status.Trim().Equals("3") ? "Forced Completed" : x.F42_Status.Trim().Equals("4") ? "Stored" : x.F42_Status.Trim().Equals("5") ? "Tabletised" : x.F42_Status.Trim().Equals("6") ? "Knead Command" : x.F42_Status.Trim().Equals("7") ? "Tablet Command" : x.F42_Status.Trim().Equals("8") ? "Forced Retrieving" : "Forced Retrieved",
                    F42_LotSeqNo = x.F42_LotSeqNo,
                    F42_ThrowAmount = x.F42_ThrowAmount

                });
                var itemCount = result.Count();
                if (gridSettings != null)
                    OrderByAndPaging(ref result1, gridSettings);
                //total = result.Sum(i => i.F43_LayinginAmount);
                var resultModel = new GridResponse<KneadingCommandItem>(result1, itemCount);
                return new ResponseResult<GridResponse<KneadingCommandItem>>(resultModel, true);
            }
            //var itemCount = lstResult.Count();
            //var pdtPlnItems = lstResult.AsQueryable();
            //OrderByAndPaging(ref pdtPlnItems, gridSettings);
            //var resultModel = new GridResponse<PdtPlnItem>(pdtPlnItems, itemCount);
            //return new ResponseResult<GridResponse<PdtPlnItem>>(resultModel, true);
        }

        public InquiryKneadingCommandNoRestlt GetCodeNamebycmNo(string commandNo)
        {
            var f42_PreProductCode = string.Empty;
            var f03_PreProductName = string.Empty;

            var tx42_KndCmd = _unitOfWork.KneadingCommandRepository.GetAll().FirstOrDefault(c => c.F42_KndCmdNo.Trim() == commandNo.Trim());

            if (tx42_KndCmd != null)
            {
                var tm03_PreProduct = _unitOfWork.PreProductRepository.GetAll().FirstOrDefault(c => c.F03_PreProductCode.Trim() == tx42_KndCmd.F42_PreProductCode.Trim());
                f42_PreProductCode = tx42_KndCmd.F42_PreProductCode.Trim();
                f03_PreProductName = tm03_PreProduct != null ? tm03_PreProduct.F03_PreProductName.Trim() : string.Empty;
            }

            if (!string.IsNullOrEmpty(f42_PreProductCode))
            {
                return new InquiryKneadingCommandNoRestlt()
                {
                    Success = true,
                    F42_PreProductCode = f42_PreProductCode,
                    F03_PreProductName = f03_PreProductName
                };
            }
            else
            {
                return new InquiryKneadingCommandNoRestlt()
                {
                    Success = false,
                    F42_PreProductCode = string.Empty,
                    F03_PreProductName = string.Empty
                };
            }

        }

        #endregion
    }

    public class InquiryKneadingCommandNoRestlt
    {
        public bool Success { get; set; }
        public string F42_PreProductCode { get; set; }
        public string F03_PreProductName { get; set; }
    }
}