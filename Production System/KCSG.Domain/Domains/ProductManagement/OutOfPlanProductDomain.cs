using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using KCSG.Core.Models;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.ProductManagement;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Data.DataModel;
using KCSG.Domain.Interfaces.Services;

namespace KCSG.Domain.Domains.ProductManagement
{
    public class OutOfPlanProductDomain : BaseDomain, IOutOfPlanProductDomain
    {
        /// <summary>
        ///     Initialize product shipping command domain with dependency injection.
        /// </summary>
        /// <param name="iUnitOfWork"></param>
        /// <param name="configurationService"></param>
        public OutOfPlanProductDomain(IUnitOfWork iUnitOfWork, IConfigurationService configurationService) : base(iUnitOfWork, configurationService)
        {
        }

        /// <summary>
        ///     Find list of out of plan of product.
        /// </summary>
        /// <param name="productCode"></param>
        /// <param name="gridSettings"></param>
        /// <returns></returns>
        public ResponseResult<GridResponse<OutOfPlanProductItem>> FindOutOfPlanProduct(string productCode,
            GridSettings gridSettings)
        {
            var outPlanProducts = _unitOfWork.OutOfPlanProductRepository.GetAll();
            var products = _unitOfWork.ProductRepository.GetAll();
            var result = from outPlanProduct in outPlanProducts
                         join product in products on outPlanProduct.F58_ProductCode.Trim() equals  product.F09_ProductCode.Trim()
                         where outPlanProduct.F58_StorageAmt.Equals(0)
                         orderby new
                         {
                             outPlanProduct.F58_PrePdtLotNo,
                             outPlanProduct.F58_ProductCode
                         }
                         select new
                {
                    outPlanProduct.F58_ProductCode,
                    product.F09_ProductDesp,
                    outPlanProduct.F58_PrePdtLotNo,
                    outPlanProduct.F58_ProductLotNo,
                    outPlanProduct.F58_TbtCmdEndPackAmt, //Pack Qty
                    outPlanProduct.F58_TbtCmdEndFrtAmt, //Fraction
                    outPlanProduct.F58_TbtEndDate //Tabletising 
                };
            
            if (!string.IsNullOrEmpty(productCode))
                result = result.Where(x => x.F58_ProductCode.Trim().StartsWith(productCode));
            var lstResult = new List<OutOfPlanProductItem>();
            foreach (var item in result)
            {
                var storageOfProductItem = new OutOfPlanProductItem();
                storageOfProductItem.F58_ProductCode = item.F58_ProductCode;
                storageOfProductItem.ProductName = item.F09_ProductDesp;
                storageOfProductItem.F58_PrePdtLotNo = item.F58_PrePdtLotNo;
                storageOfProductItem.F58_ProductLotNo = item.F58_ProductLotNo;
                storageOfProductItem.F58_TbtCmdEndPackAmt = item.F58_TbtCmdEndPackAmt;
                storageOfProductItem.F58_TbtCmdEndFrtAmt = item.F58_TbtCmdEndFrtAmt;
                storageOfProductItem.F58_TbtEndDate = item.F58_TbtEndDate;
                lstResult.Add(storageOfProductItem);
            }

            var itemCount = lstResult.Count();

            var lsResult = lstResult.AsQueryable();
            if (gridSettings != null)
                OrderByAndPaging(ref lsResult, gridSettings);

            var resultModel = new GridResponse<OutOfPlanProductItem>(lsResult, itemCount);
            return new ResponseResult<GridResponse<OutOfPlanProductItem>>(resultModel, true);
        }

        public OutOfPlanProductItem GetByProductsCode(string productCode,string prePdtLotNo)
        {
            var outPlanProduct = _unitOfWork.OutOfPlanProductRepository.Get(i => i.F58_ProductCode.Trim().Equals(productCode.Trim()) && i.F58_PrePdtLotNo.Trim().Equals(prePdtLotNo.Trim()) && i.F58_StorageAmt.Equals(0));
            var products = _unitOfWork.ProductRepository.Get(i => i.F09_ProductCode.Trim().Equals(productCode.Trim()));
            if (outPlanProduct != null && products != null)
            {
                return new OutOfPlanProductItem
                {
                    F58_ProductCode = outPlanProduct.F58_ProductCode,
                    ProductName = products.F09_ProductDesp,
                    F58_PrePdtLotNo = outPlanProduct.F58_PrePdtLotNo,
                    F58_ProductLotNo = outPlanProduct.F58_ProductLotNo,
                    F58_TbtCmdEndPackAmt = outPlanProduct.F58_TbtCmdEndPackAmt,
                    F58_TbtCmdEndFrtAmt = outPlanProduct.F58_TbtCmdEndFrtAmt,
                    F58_TbtEndDate = outPlanProduct.F58_TbtEndDate,
                    F58_TbtEndDateString = outPlanProduct.F58_TbtEndDate.Value.ToString("dd/MM/yyyy")
            };
            }
            return null;
        }

        public ResponseResult CreateOrUpdate(OutOfPlanProductItem model,bool isCreate)
        {
            var products = _unitOfWork.ProductRepository.Get(i => i.F09_ProductCode.Trim().Equals(model.F58_ProductCode.Trim()));
           
            if (isCreate)
            {
                var serialNo = "";
                if (!string.IsNullOrEmpty(model.F58_ProductCode))
                {
                    if (CheckUnique(model.F58_ProductCode,model.F58_PrePdtLotNo))
                    {
                        return new ResponseResult(false, Constants.Messages.OutOfPlanProduct_MSG41);
                    }
                }
                var noManage = _unitOfWork.NoManageRepository.Get(m => m.F48_SystemId == "00000");
                if (noManage != null)
                {
                    //	Increase [f48_mtrwhscmdno] by 1.
                    noManage.F48_OutPlanPdtCmdNo += 1;
                    //	If [f48_mtrwhscmdno] is greater than 9999, the system will:
                    if (noManage.F48_OutPlanPdtCmdNo > 9999)
                    {
                        //	Update [f48_mtrwhscmdno] as 1.
                        noManage.F48_OutPlanPdtCmdNo = 1;
                    }
                    //	Update [f48_updatedate] as current date and time.
                    noManage.F48_UpdateDate = DateTime.Now;
                    _unitOfWork.NoManageRepository.Update(noManage);

                    var sequenceNo = noManage.F48_OutPlanPdtCmdNo;
                    serialNo = "000000" + sequenceNo.ToString();
                    serialNo = serialNo.Substring(serialNo.Length - 6);

                }
                else
                {
                    //insert tx48
                    noManage = new TX48_NoManage();
                    noManage.F48_SystemId = "00000";
                    noManage.F48_MegaKndCmdNo = 0;
                    noManage.F48_GnrKndCmdNo = 0;
                    noManage.F48_MtrWhsCmdNo = 0;
                    noManage.F48_PrePdtWhsCmdNo = 0;
                    noManage.F48_PdtWhsCmdNo = 0;
                    noManage.F48_KndCmdBookNo = 0;
                    noManage.F48_CnrKndCmdNo = 0;
                    noManage.F48_KneadSheefNo = 0;
                    noManage.F48_AddDate = DateTime.Now;
                    noManage.F48_UpdateDate = DateTime.Now;
                    
                    noManage.F48_OutKndCmdNo = 1;
                    
                    _unitOfWork.NoManageRepository.Add(noManage);

                    serialNo = "000000";
                }
                var entity = new TX58_OutPlanPdt
                {
                    F58_PrePdtLotNo = model.F58_PrePdtLotNo,
                    F58_ProductCode = model.F58_ProductCode,
                    F58_PdtSeqNo = serialNo,
                    F58_ProductLotNo = model.F58_ProductLotNo,
                    F58_Status = "3",
                    F58_TbtCmdEndPackAmt = model.F58_TbtCmdEndPackAmt,
                    F58_TbtCmdEndFrtAmt = model.F58_TbtCmdEndFrtAmt,
                    //[f58_tbtcmdendamt] = Pack Quantity textbox value * Pack Unit + Fraction
                    F58_TbtCmdEndAmt = products.F09_PackingUnit*model.F58_TbtCmdEndPackAmt + model.F58_TbtCmdEndFrtAmt,
                    F58_StorageAmt = 0,
                    F58_TbtEndDate = ConvertHelper.ConvertToDateTimeFull(model.F58_TbtEndDateString),
                    F58_CertificationFlag = "0",
                    F58_AddDate = DateTime.Now,
                    F58_UpdateDate = DateTime.Now,
                    F58_UpdateCount = 0
                };
                _unitOfWork.OutOfPlanProductRepository.Add(entity);
            }
            else
            {
                var entity = _unitOfWork.OutOfPlanProductRepository.GetMany(i => i.F58_PrePdtLotNo.Trim().Equals(model.F58_PrePdtLotNo.Trim())
                                   && i.F58_ProductCode.Trim().Equals(model.F58_ProductCode.Trim())
                                   && i.F58_StorageAmt.Equals(0)).FirstOrDefault();
                if (entity == null)
                {
                    return new ResponseResult(false, Constants.Messages.Material_MSG001);
                }
                entity.F58_ProductLotNo = model.F58_ProductLotNo;
                entity.F58_TbtCmdEndPackAmt = model.F58_TbtCmdEndPackAmt;
                entity.F58_TbtCmdEndFrtAmt = model.F58_TbtCmdEndFrtAmt;
                entity.F58_TbtCmdEndAmt = products.F09_PackingUnit * model.F58_TbtCmdEndPackAmt + model.F58_TbtCmdEndFrtAmt;
                entity.F58_TbtEndDate = ConvertHelper.ConvertToDateTimeFull(model.F58_TbtEndDateString);
                entity.F58_UpdateDate = DateTime.Now;
                _unitOfWork.OutOfPlanProductRepository.Update(entity);
            }
            _unitOfWork.Commit();
            return new ResponseResult(true);
        }

        public bool Delete(string productCode,string prepdtlotno)
        {
            _unitOfWork.OutOfPlanProductRepository.Delete(s => s.F58_ProductCode.Trim().Equals(productCode.Trim()) && s.F58_PrePdtLotNo.Trim().Equals(prepdtlotno));
            _unitOfWork.Commit();
            return true;
        }

        public bool CheckUnique(string productCode,string prepdtlotno)
        {
            return _unitOfWork.OutOfPlanProductRepository.GetAll().Any(m => m.F58_ProductCode.Trim().Equals(productCode) && m.F58_PrePdtLotNo.Trim().Equals(prepdtlotno)) 
                || _unitOfWork.TabletProductRepository.GetAll().Any(m => m.F56_ProductCode.Trim().Equals(productCode) && m.F56_PrePdtLotNo.Trim().Equals(prepdtlotno));
        }

        
    }
}