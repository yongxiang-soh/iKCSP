using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Data.Repositories.ProductCertification;
using KCSG.Domain.Interfaces.ProductCertificationManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.ProductManagement
{
    public class ProductCertificationDomain : BaseDomain, IProductCertificationDomain
    {
        #region Constructor

        public ProductCertificationDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {

        }

        #endregion

        #region Methods

        public ProductCertificationItem GetById(string prodCode, string prePdtLotNo, string productFlg)
        {
            var entity =
                _unitOfWork.ProductCertificationRepository.GetAll()
                    .FirstOrDefault(
                        i =>
                            i.F67_ProductCode.Trim().Equals(prodCode.Trim()) &&
                            i.F67_PrePdtLotNo.Trim().Equals(prePdtLotNo.Trim()) &&
                            i.F67_ProductFlg.Trim().Equals(productFlg.Trim()));
            var tm09Entity = _unitOfWork.ProductRepository.GetById(prodCode.Trim());
            var result = Mapper.Map<ProductCertificationItem>(entity);
            //result. = tm03Entity;
            result.F09_ProductDesp = tm09Entity != null ? tm09Entity.F09_ProductDesp : "";
            //result.CertificationDate = SqlFunctions.DatePart("day", entity.F67_CertificationDate) + "/" +
            //                           SqlFunctions.DatePart("month", entity.F67_CertificationDate) + "/" +
            //                           SqlFunctions.DatePart("year", entity.F67_CertificationDate);
            result.CertificationDate = result.F67_CertificationDate.ToString("dd/MM/yyyy");
            return result;
        }

        public void Create(ProductCertificationItem ProductCertification)
        {

            var entity = Mapper.Map<TH67_CrfHst>(ProductCertification);
            _unitOfWork.ProductCertificationRepository.Add(entity);
            _unitOfWork.Commit();
        }

        public void Update(ProductCertificationItem ProductCertification)
        {
            var entity = Mapper.Map<TH67_CrfHst>(ProductCertification);
            _unitOfWork.ProductCertificationRepository.Update(entity);
            _unitOfWork.Commit();
        }

        //public void Delete(string code)
        //{
        //    throw new NotImplementedException();
        //}

        public bool Delete(string prodCode, string prePdtLotNo, string productFlg)
        {
            _unitOfWork.ProductCertificationRepository.Delete(
                s =>
                    s.F67_ProductCode.Equals(prodCode.Trim()) && s.F67_PrePdtLotNo.Equals(prePdtLotNo) &&
                    s.F67_ProductFlg.Equals(productFlg));
            _unitOfWork.Commit();
            return true;
        }

        public ResponseResult<GridResponse<ProductCertificationItem>> SearchCriteria(string code,
            GridSettings gridSettings)
        {

            var result = _unitOfWork.ProductCertificationRepository.GetAll();
            if (!string.IsNullOrEmpty(code))
            {
                result = result.Where(i => i.F67_ProductCode.ToUpper().Contains(code.ToUpper()));
            }
            var itemCount = result.Count();

            if (gridSettings != null)
                OrderByAndPaging(ref result, gridSettings);

            var lstResult = Mapper.Map<IEnumerable<TH67_CrfHst>, IEnumerable<ProductCertificationItem>>(result.ToList());
            var resultModel = new GridResponse<ProductCertificationItem>(lstResult, itemCount);
            return new ResponseResult<GridResponse<ProductCertificationItem>>(resultModel, true);
        }

        public ResponseResult<GridResponse<StorageOfProductItem>> SearchNormal(string date, GridSettings gridSettings)
        {
            // Find all StorageOfProduct in database.
            var storageOfProducts = _unitOfWork.StorageOfProductRepository.GetAll();

            // Find all product in database.
            var products = _unitOfWork.ProductRepository.GetAll();

            var test = Constants.F56_CertificationFlag.Normal.ToString("D");
            var test1 = Constants.F56_CertificationFlag.OutofSpec.ToString("D");
            var test2 = Constants.F56_Status.StorageOver;
            if (string.IsNullOrEmpty(date))
            {
                var result = from storageOfProduct in storageOfProducts
                    join product in products on storageOfProduct.F56_ProductCode equals product.F09_ProductCode
                    where (storageOfProduct.F56_CertificationFlag.Equals(test
                           ) ||
                           storageOfProduct.F56_CertificationFlag.Equals(
                               test1))
                          && storageOfProduct.F56_Status.Equals(test2)
                    orderby new
                    {
                        storageOfProduct.F56_TbtEndDate,
                        storageOfProduct.F56_ProductCode,
                        storageOfProduct.F56_PrePdtLotNo
                    }
                    select new StorageOfProductItem
                    {
                        F56_TbtEndDate = storageOfProduct.F56_TbtEndDate,
                        F56_ProductCode = storageOfProduct.F56_ProductCode,
                        F56_CertificationFlag = storageOfProduct.F56_CertificationFlag,
                        F56_PrePdtLotNo = storageOfProduct.F56_PrePdtLotNo,
                        F56_ProductLotNo = storageOfProduct.F56_ProductLotNo,
                        F56_TbtCmdEndAmt = storageOfProduct.F56_TbtCmdEndAmt,
                        F09_ProductDesp = product.F09_ProductDesp,
                    };

                var itemCount = result.Count();

                if (gridSettings != null)
                    OrderByAndPaging(ref result, gridSettings);

                //var lstResult = Mapper.Map<IEnumerable<TX56_TbtPdt>, IEnumerable<StorageOfProductItem>>(result.ToList());
                var resultModel = new GridResponse<StorageOfProductItem>(result, itemCount);
                return new ResponseResult<GridResponse<StorageOfProductItem>>(resultModel, true);
            }
            else
            {
                var datetime = ConvertHelper.ConvertToDateTimeFull(date);
                var result = from storageOfProduct in storageOfProducts
                    join product in products on storageOfProduct.F56_ProductCode equals product.F09_ProductCode
                    where (storageOfProduct.F56_CertificationFlag.Equals(test
                           ) ||
                           storageOfProduct.F56_CertificationFlag.Equals(
                               test1))
                          && storageOfProduct.F56_Status.Equals(test2)
                          //&& storageOfProduct.F56_CertificationDate == datetime
                    select new StorageOfProductItem
                    {
                        F56_TbtEndDate = storageOfProduct.F56_TbtEndDate,
                        F56_ProductCode = storageOfProduct.F56_ProductCode,
                        F56_CertificationFlag = storageOfProduct.F56_CertificationFlag,
                        F56_PrePdtLotNo = storageOfProduct.F56_PrePdtLotNo,
                        F56_ProductLotNo = storageOfProduct.F56_ProductLotNo,
                        F56_TbtCmdEndAmt = storageOfProduct.F56_TbtCmdEndAmt,
                        F09_ProductDesp = product.F09_ProductDesp,
                    };

                var itemCount = result.Count();

                if (gridSettings != null)
                    OrderByAndPaging(ref result, gridSettings);

                //var lstResult = Mapper.Map<IEnumerable<TX56_TbtPdt>, IEnumerable<StorageOfProductItem>>(result.ToList());
                var resultModel = new GridResponse<StorageOfProductItem>(result, itemCount);
                return new ResponseResult<GridResponse<StorageOfProductItem>>(resultModel, true);
            }

        }

        public ResponseResult<GridResponse<ProductCertificationItem>> SearchSample(string date,
            GridSettings gridSettings)
        {
            var productCertifications = _unitOfWork.ProductCertificationRepository.GetAll();

            var products = _unitOfWork.ProductRepository.GetAll();

            var test = Constants.F67_SampleFlag.Sample.ToString("D");
            if (string.IsNullOrEmpty(date))
            {
                //if (result != null)
                //{
                //    result = result.Where(i => i.F67_ProductFlg.Equals(test));
                //}
                var result = from productCertification in productCertifications
                    join product in products on productCertification.F67_ProductCode equals product.F09_ProductCode
                    //let convertedDate = productCertification.F67_CertificationDate.ToString("dd/MM/yyyy")
                    where (productCertification.F67_ProductFlg.Equals(test))
                    orderby new
                    {
                        productCertification.F67_ProductCode,
                        productCertification.F67_ProductLotNo
                    }
                    select new ProductCertificationItem
                    {

                        F67_ProductCode = productCertification.F67_ProductCode,
                        F67_ProductLotNo = productCertification.F67_ProductLotNo,
                        F67_Amount = productCertification.F67_Amount,
                        F09_ProductDesp = product.F09_ProductDesp,
                        F67_PrePdtLotNo = productCertification.F67_PrePdtLotNo,
                        F67_ProductFlg = productCertification.F67_ProductFlg,
                        F67_CertificationDate = productCertification.F67_CertificationDate,
                        CertificationDate =
                            SqlFunctions.DatePart("day", productCertification.F67_CertificationDate) + "/" +
                            SqlFunctions.DatePart("month", productCertification.F67_CertificationDate) + "/" +
                            SqlFunctions.DatePart("year", productCertification.F67_CertificationDate)
                        //CertificationDate = productCertification.F67_CertificationDate
                    };



                var itemCount = result.Count();

                if (gridSettings != null)
                    OrderByAndPaging(ref result, gridSettings);

                //var lstResult = Mapper.Map<IEnumerable<TH67_CrfHst>, IEnumerable<ProductCertificationItem>>(result.ToList());
                var resultModel = new GridResponse<ProductCertificationItem>(result, itemCount);
                return new ResponseResult<GridResponse<ProductCertificationItem>>(resultModel, true);
            }
            else
            {
                var datetime = ConvertHelper.ConvertToDateTimeFull(date);
                //if (result != null)
                //{
                //    result = result.Where(i => i.F67_ProductFlg.Equals(test) && i.F67_CertificationDate == datetime);
                //}

                var result = from productCertification in productCertifications
                    join product in products on productCertification.F67_ProductCode equals product.F09_ProductCode
                    where (productCertification.F67_ProductFlg.Equals(test))
                          //productCertification.F67_CertificationDate == datetime
                    select new ProductCertificationItem
                    {

                        F67_ProductCode = productCertification.F67_ProductCode,
                        F67_ProductLotNo = productCertification.F67_ProductLotNo,
                        F67_Amount = productCertification.F67_Amount,
                        F09_ProductDesp = product.F09_ProductDesp,
                        F67_PrePdtLotNo = productCertification.F67_PrePdtLotNo,
                        F67_ProductFlg = productCertification.F67_ProductFlg,
                        F67_CertificationDate = productCertification.F67_CertificationDate
                    };


                var itemCount = result.Count();

                if (gridSettings != null)
                    OrderByAndPaging(ref result, gridSettings);

                //var lstResult = Mapper.Map<IEnumerable<TH67_CrfHst>, IEnumerable<ProductCertificationItem>>(result.ToList());
                var resultModel = new GridResponse<ProductCertificationItem>(result, itemCount);
                return new ResponseResult<GridResponse<ProductCertificationItem>>(resultModel, true);
            }
        }

        public ResponseResult<GridResponse<ProductCertificationOutOfPlanItem>> SearchOutOfPlan(string date,
            GridSettings gridSettings)
        {
            var outOfPlanProducts = _unitOfWork.ProductCertiicationOutOfPlanRepository.GetAll();

            var products = _unitOfWork.ProductRepository.GetAll();

            var test1 = Constants.F56_CertificationFlag.Normal.ToString("D");
            var test2 = Constants.F56_CertificationFlag.OutofSpec.ToString("D");
            var test3 = Constants.F56_Status.StorageOver;
            if (string.IsNullOrEmpty(date))
            {
                var records = from outOfPlanProduct in outOfPlanProducts
                    join product in products on outOfPlanProduct.F58_ProductCode equals product.F09_ProductCode
                    where product.F09_ProductCode.Equals(outOfPlanProduct.F58_ProductCode)
                          &&
                          (outOfPlanProduct.F58_CertificationFlag.Equals(test1) ||
                           outOfPlanProduct.F58_CertificationFlag.Equals(test2))
                          && outOfPlanProduct.F58_Status.Equals(test3)
                    orderby new
                    {
                        outOfPlanProduct.F58_ProductCode,
                        outOfPlanProduct.F58_PrePdtLotNo
                    }
                    select new ProductCertificationOutOfPlanItem
                    {
                        F58_ProductCode = outOfPlanProduct.F58_ProductCode,
                        F58_CertificationFlag = outOfPlanProduct.F58_CertificationFlag,
                        F58_PrePdtLotNo = outOfPlanProduct.F58_PrePdtLotNo,
                        F58_ProductLotNo = outOfPlanProduct.F58_ProductLotNo,
                        F58_TbtCmdEndAmt = outOfPlanProduct.F58_TbtCmdEndAmt,
                        F09_ProductDesp = product.F09_ProductDesp,
                    };

                var itemCount = records.Count();

                if (gridSettings != null)
                    OrderByAndPaging(ref records, gridSettings);

                //var lstResult = Mapper.Map<IEnumerable<TX58_OutPlanPdt>, IEnumerable<ProductCertificationOutOfPlanItem>>(records.ToList());
                var resultModel = new GridResponse<ProductCertificationOutOfPlanItem>(records, itemCount);
                return new ResponseResult<GridResponse<ProductCertificationOutOfPlanItem>>(resultModel, true);
            }
            else
            {
                var datetime = ConvertHelper.ConvertToDateTimeFull(date);
                var records = from outOfPlanProduct in outOfPlanProducts
                    join product in products on outOfPlanProduct.F58_ProductCode equals product.F09_ProductCode
                    where product.F09_ProductCode.Equals(outOfPlanProduct.F58_ProductCode)
                          &&
                          (outOfPlanProduct.F58_CertificationFlag.Equals(test1) ||
                           outOfPlanProduct.F58_CertificationFlag.Equals(test2))
                          && outOfPlanProduct.F58_Status.Equals(test3)
                          //&& outOfPlanProduct.F58_CertificationDate == datetime
                    select new ProductCertificationOutOfPlanItem
                    {
                        F58_ProductCode = outOfPlanProduct.F58_ProductCode,
                        F58_CertificationFlag = outOfPlanProduct.F58_CertificationFlag,
                        F58_PrePdtLotNo = outOfPlanProduct.F58_PrePdtLotNo,
                        F58_ProductLotNo = outOfPlanProduct.F58_ProductLotNo,
                        F58_TbtCmdEndAmt = outOfPlanProduct.F58_TbtCmdEndAmt,
                        F09_ProductDesp = product.F09_ProductDesp,
                    };

                var itemCount = records.Count();

                if (gridSettings != null)
                    OrderByAndPaging(ref records, gridSettings);

                //var lstResult = Mapper.Map<IEnumerable<TX58_OutPlanPdt>, IEnumerable<ProductCertificationOutOfPlanItem>>(records.ToList());
                var resultModel = new GridResponse<ProductCertificationOutOfPlanItem>(records, itemCount);
                return new ResponseResult<GridResponse<ProductCertificationOutOfPlanItem>>(resultModel, true);
            }
        }

        public bool CheckUnique(string productCode,string productLotNo)
        {
            var result = _unitOfWork.ProductCertificationRepository.GetAll()
                    .Any(m => m.F67_ProductCode.Trim().Equals(productCode.Trim()) && m.F67_ProductLotNo.Trim() == productLotNo);
            return result;

        }

        public bool CheckUnique67(string productCode, string prePdtLotNo)
        {
            var result = _unitOfWork.ProductCertificationRepository.GetAll()
                    .Any(m => m.F67_ProductCode.Trim().Equals(productCode.Trim()) && m.F67_PrePdtLotNo.Trim() == prePdtLotNo);
            return result;

        }

        public ResponseResult CreateOrUpdate(ProductCertificationItem model)
        {
            if (model.IsCreate)
            {
                if (!string.IsNullOrEmpty(model.F67_ProductCode))
                {
                    if (CheckUnique(model.F67_ProductCode,model.F67_PrePdtLotNo))
                    {
                        return new ResponseResult(false, Constants.Messages.Material_MSG004);
                    }
                }
                var entity = Mapper.Map<TH67_CrfHst>(model);
                entity.F67_AddDate = DateTime.Now;
                entity.F67_UpdateDate = DateTime.Now;
                entity.F67_PrePdtLotNo = model.F67_ProductLotNo;
                entity.F67_ProductFlg = Constants.F67_SampleFlag.Sample.ToString("D");
                entity.F67_CertificationFlag = Constants.F56_CertificationFlag.OK.ToString("D");
                entity.F67_CertificationDate = ConvertHelper.ConvertToDateTimeFull(model.CertificationDate);
                entity.F67_UpdateCount = 0;

                _unitOfWork.ProductCertificationRepository.Add(entity);
            }
            else
            {
                //var entity = _unitOfWork.ProductCertificationRepository.GetById(model.F67_ProductCode);
                var entity =
                    _unitOfWork.ProductCertificationRepository.Get(
                        m =>
                            m.F67_ProductCode.Trim().Equals(model.F67_ProductCode.Trim()) &&
                            m.F67_PrePdtLotNo.Equals(model.F67_PrePdtLotNo) &&
                            m.F67_ProductFlg.Equals(model.F67_ProductFlg));
                if (entity == null)
                {
                    return new ResponseResult(false, Constants.Messages.Material_MSG001);
                }

                entity.F67_UpdateDate = DateTime.Now;
                entity.F67_CertificationDate = ConvertHelper.ConvertToDateTimeFull(model.CertificationDate);
                entity.F67_Amount = model.F67_Amount;
                entity.F67_UpdateCount = entity.F67_UpdateCount + 1;
                _unitOfWork.ProductCertificationRepository.Update(entity);
            }
            _unitOfWork.Commit();
            return new ResponseResult(true);
        }

        public IEnumerable<TH67_CrfHst> GetProductCertification(string productCode)
        {
            return
                _unitOfWork.ProductCertificationRepository.GetAll()
                    .Where(i => i.F67_ProductCode.ToUpper().Contains(productCode.ToUpper()));
        }

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
        public async Task MakeProductCertificationOkAsync(string status, string productCode, string prePdtLotNo,
            string productLotNo, double quantity, string certificationDate)
        {
            try
            {
                var condition = Constants.F56_Status.StorageOver;
                if (status == Constants.StorageOfProductStatus.Normal.ToString("D"))
                {
                    // Find all tablet products.
                    var tbtPdts = _unitOfWork.TabletProductRepository.GetAll();
                    tbtPdts = tbtPdts.Where(x => x.F56_ProductCode.Equals(productCode)
                                                 && x.F56_PrePdtLotNo.Equals(prePdtLotNo)
                                                 && x.F56_Status.Equals(condition));

                    foreach (var tbtPdt in tbtPdts)
                    {
                        tbtPdt.F56_CertificationFlag = Constants.F40_CertificationFlag.TX40_CrtfctnFlg_Ok.ToString("D");
                        tbtPdt.F56_CertificationDate = ConvertHelper.ConvertToDateTimeFull(certificationDate);
                        //_unitOfWork.TabletProductRepository.Update(tbtPdt);
                    }
                }
                else if (status == Constants.StorageOfProductStatus.OutOfPlan.ToString("D"))
                {
                    /*
                     * •	Update TX58_OUTPLANPDT where [f58_productcode] = Product Code, [f58_prepdtlotno] = Prepdt Lotno and [f58_status] = “Storage Over” (or 4).
                     * 	Set [f58_certificationflag] = “Certification OK” (or 1).
                     * 	Set [f58_certificationdate] = Certification Date
                     */
                    var outPlanPdts = _unitOfWork.OutOfPlanProductRepository.GetAll();
                    outPlanPdts = outPlanPdts.Where(x => x.F58_ProductCode.Equals(productCode)
                                                         && x.F58_PrePdtLotNo.Equals(prePdtLotNo)
                                                         && x.F58_Status.Equals(condition));

                    foreach (var outPlanPdt in outPlanPdts)
                    {
                        outPlanPdt.F58_CertificationFlag = Constants.F40_CertificationFlag.TX40_CrtfctnFlg_Ok.ToString("D");
                        outPlanPdt.F58_CertificationDate = ConvertHelper.ConvertToDateTimeFull(certificationDate);
                        //_unitOfWork.OutOfPlanProductRepository.Update(outPlanPdt);
                    }
                }

                //o	Regardless which radio button is selected, system will update and insert data for some tables by doing as follow:
                /*
                 * •	Update TX40_PDTSHFSTK, where [f40_prepdtlotno] = Prepdt Lotno and [f40_productcode] = Product Code:
                 * 	Set [f40_certificationflg] = “Certification OK” (or 1).
                 * 	Set [f40_certificationdate] = Certification Date.
                 */
                var pdtShfStks = _unitOfWork.ProductShelfStockRepository.GetAll();
                pdtShfStks =
                    pdtShfStks.Where(x => x.F40_PrePdtLotNo.Equals(prePdtLotNo) && x.F40_ProductCode.Equals(productCode));
                foreach (var pdtShfStk in pdtShfStks)
                {
                    pdtShfStk.F40_CertificationFlg = Constants.F40_CertificationFlag.TX40_CrtfctnFlg_Ok.ToString("D");
                    pdtShfStk.F40_CertificationDate = ConvertHelper.ConvertToDateTimeFull(certificationDate);
                    //_unitOfWork.ProductShelfStockRepository.Update(pdtShfStk);
                }

                var a = _unitOfWork.CertificateHistoryRepository.GetAll().Any(i => i.F67_ProductCode.Trim().Equals(productCode)
                    && i.F67_PrePdtLotNo.Trim().Equals(prePdtLotNo));

                if (a)
                {
                   
                }
                //•	Insert data to TH67_CRFHST:
                var crfHst = new TH67_CrfHst();
                crfHst.F67_ProductCode = productCode;
                crfHst.F67_PrePdtLotNo = prePdtLotNo;
                crfHst.F67_ProductFlg = "1";
                crfHst.F67_Amount = quantity;
                crfHst.F67_ProductLotNo = productLotNo;
                crfHst.F67_CertificationFlag = Constants.F40_CertificationFlag.TX40_CrtfctnFlg_Ok.ToString("D");
                crfHst.F67_CertificationDate = ConvertHelper.ConvertToDateTimeFull(certificationDate);
                crfHst.F67_AddDate = crfHst.F67_UpdateDate = DateTime.Now;
                crfHst.F67_UpdateCount = 0;

                _unitOfWork.CertificateHistoryRepository.Add(crfHst);

                // Save all changes.
                await _unitOfWork.CommitAsync();
            }
            catch (Exception)
            {                
                throw;
            }
            
        }

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
        public async Task MakeProductCertificationNgAsync(string status, string productCode, string prePdtLotNo,
            string productLotNo, double quantity, string certificationDate)
        {
            try
            {
                var condition = Constants.F56_Status.StorageOver;
                var certificationFlag = Constants.F40_CertificationFlag.TX40_CrtfctnFlg_NG.ToString("D");
                if (status == Constants.StorageOfProductStatus.Normal.ToString("D"))
                {
                    // Find all tablet products.
                    var tbtPdts = _unitOfWork.TabletProductRepository.GetAll();
                    tbtPdts = tbtPdts.Where(x => x.F56_ProductCode.Equals(productCode)
                                                 && x.F56_PrePdtLotNo.Equals(prePdtLotNo)
                                                 && x.F56_Status.Equals(condition));

                    foreach (var tbtPdt in tbtPdts)
                    {
                        tbtPdt.F56_CertificationFlag = certificationFlag;
                        tbtPdt.F56_CertificationDate = ConvertHelper.ConvertToDateTimeFull(certificationDate);
                    }
                }
                else if (status == Constants.StorageOfProductStatus.OutOfPlan.ToString("D"))
                {
                    /*
                     * •	Update TX58_OUTPLANPDT where [f58_productcode] = Product Code, [f58_prepdtlotno] = Prepdt Lotno and [f58_status] = “Storage Over” (or 4).
                     * 	Set [f58_certificationflag] = “Certification OK” (or 1).
                     * 	Set [f58_certificationdate] = Certification Date
                     */
                    var outPlanPdts = _unitOfWork.OutOfPlanProductRepository.GetAll();
                    outPlanPdts = outPlanPdts.Where(x => x.F58_ProductCode.Equals(productCode)
                                                         && x.F58_PrePdtLotNo.Equals(prePdtLotNo)
                                                         && x.F58_Status.Equals(condition));

                    foreach (var outPlanPdt in outPlanPdts)
                    {
                        outPlanPdt.F58_CertificationFlag = Constants.F40_CertificationFlag.TX40_CrtfctnFlg_NG.ToString("D");
                        outPlanPdt.F58_CertificationDate = ConvertHelper.ConvertToDateTimeFull(certificationDate);
                    }
                }

                //o	Regardless which radio button is selected, system will update and insert data for some tables by doing as follow:
                /*
                 * •	Update TX40_PDTSHFSTK, where [f40_prepdtlotno] = Prepdt Lotno and [f40_productcode] = Product Code:
                 * 	Set [f40_certificationflg] = “Certification OK” (or 1).
                 * 	Set [f40_certificationdate] = Certification Date.
                 */
                var pdtShfStks = _unitOfWork.ProductShelfStockRepository.GetAll();
                pdtShfStks =
                    pdtShfStks.Where(x => x.F40_PrePdtLotNo.Equals(prePdtLotNo) && x.F40_ProductCode.Equals(productCode));
                foreach (var pdtShfStk in pdtShfStks)
                {
                    pdtShfStk.F40_CertificationFlg = Constants.F40_CertificationFlag.TX40_CrtfctnFlg_NG.ToString("D");
                    pdtShfStk.F40_CertificationDate = ConvertHelper.ConvertToDateTimeFull(certificationDate);
                }

                //•	Insert data to TH67_CRFHST:
                var crfHst = new TH67_CrfHst();
                crfHst.F67_ProductCode = productCode;
                crfHst.F67_PrePdtLotNo = prePdtLotNo;
                crfHst.F67_ProductFlg = "1";
                crfHst.F67_Amount = quantity;
                crfHst.F67_ProductLotNo = productLotNo;
                crfHst.F67_CertificationFlag = Constants.F40_CertificationFlag.TX40_CrtfctnFlg_NG.ToString("D");
                crfHst.F67_CertificationDate = ConvertHelper.ConvertToDateTimeFull(certificationDate);
                crfHst.F67_AddDate = crfHst.F67_UpdateDate = DateTime.Now;
                crfHst.F67_UpdateCount = 0;

                _unitOfWork.CertificateHistoryRepository.Add(crfHst);

                // Save all changes.
                await _unitOfWork.CommitAsync();
            }
            catch (Exception exception)
            {
                return;             
            }            
        }

        /// <summary>
        /// Find product certification flag for printing.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<object> FindProductCertificationsForPrinting(Constants.PrintProductCertificationStatus status, DateTime? beginDate, DateTime? endDate)
        {
            // Find products from database.
            var products = _unitOfWork.ProductRepository.GetAll();

            string certificationFlg = null;

            if (status == Constants.PrintProductCertificationStatus.Certificated)
                certificationFlg = Constants.F40_CertificationFlag.TX40_CrtfctnFlg_Ok.ToString("D");
            else if (status == Constants.PrintProductCertificationStatus.OutOfSpec)
                certificationFlg = Constants.F40_CertificationFlag.TX40_CrtfctnFlg_NG.ToString("D");
            else
                certificationFlg = Constants.F40_CertificationFlag.TX40_CrtfctnFlg_Normal.ToString("D");

            var shelfTypeNormal = Constants.F51_ShelfType.Normal.ToString("D");

            var shelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_Pdt;

            // Find product shelf statuses database.
            var productShelfStatuses = _unitOfWork.ProductShelfStatusRepository.GetAll();

            // Find product shelf stocks from database.
            var productShelfStocks = _unitOfWork.ProductShelfStockRepository.GetAll();

            var result = (from product in products
                from productShelfStatus in productShelfStatuses
                from productShelfStock in productShelfStocks
                where product.F09_ProductCode.Equals(productShelfStock.F40_ProductCode)
                      && productShelfStatus.F51_PalletNo.Equals(productShelfStock.F40_PalletNo)
                      && productShelfStock.F40_CertificationFlg.Trim().Equals(certificationFlg)
                      && productShelfStatus.F51_ShelfStatus.Trim().Equals(shelfStatus)
                      && productShelfStatus.F51_ShelfType.Trim().Equals(shelfTypeNormal)
                      && (beginDate == null || (beginDate != null && productShelfStatus.F51_StorageDate >= beginDate))
                      && (endDate == null || (endDate != null && productShelfStatus.F51_StorageDate <= endDate))                
                group new
                {
                    //product.F09_ProductCode,
                    product.F09_ProductDesp,
                    productShelfStock.F40_ProductLotNo,
                    productShelfStock.F40_PalletNo,
                    Rbl =
                        productShelfStatus.F51_ShelfRow + productShelfStatus.F51_ShelfBay +
                        productShelfStatus.F51_ShelfLevel,
                    productShelfStock.F40_Amount
                } by product.F09_ProductCode
                into g
                select new {g}).ToList().Select(i => new
                {
                    F09_ProductCode = i.g.Key,
                    Details = i.g.Select(x => new
                    {                        
                        x.F09_ProductDesp,
                        x.F40_ProductLotNo,
                        x.F40_PalletNo,
                        x.Rbl,
                        //x.F40_Amount = string.Format("{0:yyyy-MM-dd}", x.F40_Amount)
                        //x.F40_Amount
                        F40_Amount = String.Format("{0:#,##0.00}", x.F40_Amount)

                    }),
                    //Total = i.g.Sum(x => x.F40_Amount).ToString("N")
                    Total = String.Format("{0:#,##0.00}", i.g.Sum(x => x.F40_Amount))
                });            
               
            if (!result.Any())
            {
                return null;
            }
            return result;

        }

        #endregion
    }
}
