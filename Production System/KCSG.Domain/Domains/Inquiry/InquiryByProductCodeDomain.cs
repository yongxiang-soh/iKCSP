using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.Inquiry;
using KCSG.Domain.Models.Inquiry;
using KCSG.Data.Repositories;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;
using log4net;

namespace KCSG.Domain.Domains.Inquiry
{
    public class InquiryByProductDomain : BaseDomain, IInquiryByProductDomain
    {
        #region Constructor

        public InquiryByProductDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion
        public ResponseResult<GridResponse<StorageOfProductItem>> SearchCriteria(string productCode,
    GridSettings gridSettings)
        {
            var result = _unitOfWork.TabletProductRepository.GetAll();

            var productsheft = _unitOfWork.ProductShelfRepository.GetAll();
            var productsheftstock = _unitOfWork.ProductShelfStockRepository.GetAll();
            if (!string.IsNullOrEmpty(productCode))
            {

                result = result.Where(x => x.F56_ProductCode.Trim() == productCode.Trim()).OrderBy(x => x.F56_KndCmdNo).ThenBy(n => n.F56_ProductLotNo);
                productsheftstock = productsheftstock.Where(y => result.Any(x => y.F40_PrePdtLotNo.Trim() == x.F56_PrePdtLotNo.Trim()
                                                                      && y.F40_ProductCode.Trim() == x.F56_ProductCode.Trim()
                                                                      && y.F40_ProductLotNo.Trim() == x.F56_ProductLotNo.Trim()
                                                                      && y.F40_StockFlag.Trim().Equals("3")));
                var result1 = result.Select(x => new StorageOfProductItem()
                {
                    F56_KndCmdNo =x.F56_KndCmdNo ,
                    F56_TbtCmdAmt = x.F56_TbtCmdAmt,
                    F56_TbtBgnDate = x.F56_TbtBgnDate,
                    F56_TbtEndDate =x.F56_TbtEndDate,
                    F56_Status = x.F56_ShipDate == null ? "Delivered" : x.F56_CertificationFlag.Trim().Equals("1") ? "Cert_OK" : x.F56_CertificationFlag.Trim().Equals("2") ? "Cert_NG" : x.F56_Status.Trim().Equals("0") ? "Yet" : x.F56_Status.Trim().Equals("1") ? "Tabletising" : x.F56_Status.Trim().Equals("2") ? "Change" : x.F56_Status.Trim().Equals("3") ? "Done" : x.F56_Status.Trim().Equals("4") ? "Stored" : x.F56_Status.Trim().Equals("5") ? "Cert_OK" : x.F56_Status.Trim().Equals("6") ? "Cert_NG" : "Delivered",
                    F56_CertificationDate =x.F56_CertificationDate,
                    F56_ShipDate =x.F56_ShipDate,
                    F56_AddDate = productsheft.Where(z => productsheftstock.Any(y => y.F40_PalletNo == z.F57_PalletNo)).Max(z => z.F57_StorageDate).Value != null ? productsheft.Where(z => productsheftstock.Any(y => y.F40_PalletNo == z.F57_PalletNo)).Max(z => z.F57_StorageDate).Value : x.F56_AddDate,
                    F56_ProductLotNo =x.F56_ProductLotNo,
                    F56_TbtCmdEndAmt =x.F56_TbtCmdEndAmt,
                    F56_PrePdtLotNo =x.F56_PrePdtLotNo,
                    F56_ProductCode =x.F56_ProductCode,
                    F56_CertificationFlag =x.F56_CertificationFlag

                });
                var itemCount = result1.Count();
                if (gridSettings != null)
                    OrderByAndPaging(ref result1, gridSettings);
                //total = result.Sum(i => i.F43_LayinginAmount);
                var resultModel = new GridResponse<StorageOfProductItem>(result1, itemCount);
                return new ResponseResult<GridResponse<StorageOfProductItem>>(resultModel, true);
            }
            else
            {
                var result1 = result.Select(x => new StorageOfProductItem()
                {
                    F56_KndCmdNo = x.F56_KndCmdNo,
                    F56_TbtCmdAmt = x.F56_TbtCmdAmt,
                    F56_TbtBgnDate = x.F56_TbtBgnDate,
                    F56_TbtEndDate = x.F56_TbtEndDate,
                    F56_Status = x.F56_ShipDate == null ? "Delivered" : x.F56_CertificationFlag.Trim().Equals("1") ? "Cert_OK" : x.F56_CertificationFlag.Trim().Equals("2") ? "Cert_NG" : x.F56_Status.Trim().Equals("0") ? "Yet" : x.F56_Status.Trim().Equals("1") ? "Tabletising" : x.F56_Status.Trim().Equals("2") ? "Change" : x.F56_Status.Trim().Equals("3") ? "Done" : x.F56_Status.Trim().Equals("4") ? "Stored" : x.F56_Status.Trim().Equals("5") ? "Cert_OK" : x.F56_Status.Trim().Equals("6") ? "Cert_NG" : "Delivered",
                    F56_CertificationDate = x.F56_CertificationDate,
                    F56_ShipDate = x.F56_ShipDate,
                    F56_AddDate = productsheft.Where(z => productsheftstock.Any(y => y.F40_PalletNo == z.F57_PalletNo)).Max(z => z.F57_StorageDate).Value != null ? productsheft.Where(z => productsheftstock.Any(y => y.F40_PalletNo == z.F57_PalletNo)).Max(z => z.F57_StorageDate).Value : x.F56_AddDate,
                    F56_ProductLotNo = x.F56_ProductLotNo,
                    F56_TbtCmdEndAmt = x.F56_TbtCmdEndAmt,
                    F56_PrePdtLotNo = x.F56_PrePdtLotNo,
                    F56_ProductCode = x.F56_ProductCode,
                    F56_CertificationFlag = x.F56_CertificationFlag

                });
                var itemCount = result.Count();
                if (gridSettings != null)
                    OrderByAndPaging(ref result1, gridSettings);
                //total = result.Sum(i => i.F43_LayinginAmount);
                var resultModel = new GridResponse<StorageOfProductItem>(result1, itemCount);
                return new ResponseResult<GridResponse<StorageOfProductItem>>(resultModel, true);
            }
            //var itemCount = lstResult.Count();
            //var pdtPlnItems = lstResult.AsQueryable();
            //OrderByAndPaging(ref pdtPlnItems, gridSettings);
            //var resultModel = new GridResponse<PdtPlnItem>(pdtPlnItems, itemCount);
            //return new ResponseResult<GridResponse<PdtPlnItem>>(resultModel, true);
        }

        public string Searchuser(string proCode, string proName)
        {
            var product = _unitOfWork.ProductRepository.GetAll().Where(m => m.F09_ProductCode.Equals(proCode)).FirstOrDefault();
            var user1=
                _unitOfWork.EndUserRepository.GetAll();
            if (product != null)
            {
                var user = user1.FirstOrDefault(x => x.F10_EndUserCode.Trim().Equals(product.F09_EndUserCode.Trim()));
                if (user != null)
                {
                    //return Resources.ProductManagementResources.MSG1 ;
                    return user.F10_EndUserCode + "|" + user.F10_EndUserName;
                }
            }
            else
            {
                return "" + "|" + "";
            }

            //return Json(new { result }, JsonRequestBehavior.AllowGet);
           
            return string.Empty;
        }

    }
}
