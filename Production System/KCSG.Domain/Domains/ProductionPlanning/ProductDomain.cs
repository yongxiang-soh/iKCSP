using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.ProductionPlanning;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.ProductionPlanning
{
    public class ProductDomain :BaseDomain, IProductDomain
    {
      
        #region Constructor

        public ProductDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
           
        }

        #endregion

        #region Methods

        public ProductItem GetById(string id)
        {
            var entity = _unitOfWork.ProductRepository.GetById(id);
            var tm11Entity =
                _unitOfWork.PckMtrRepository.GetAll().Where(p => p.F11_ProductCode == entity.F09_ProductCode).ToList();
            var tm11LstItem = new List<PckMtrItem>();
            if (tm11Entity.Any())
            {
                var tm11Item = new PckMtrItem();
                foreach (var item in tm11Entity)
                {
                    tm11Item = Mapper.Map<PckMtrItem>(item);
                    tm11LstItem.Add(tm11Item);
                }
            }
            var result =  Mapper.Map<ProductItem>(entity);
            result.ListPckMtr = tm11LstItem;
            return result;
        }

        public void Create(ProductItem product)
        {
            var entity = Mapper.Map<TM09_Product>(product);
            _unitOfWork.ProductRepository.Add(entity);
            _unitOfWork.Commit();
        }

        public void Update(ProductItem product)
        {
            var entity = Mapper.Map<TM09_Product>(product);
            _unitOfWork.ProductRepository.Update(entity);
            _unitOfWork.Commit();
        }

        public void Delete(string lstCode)
        {
            _unitOfWork.PckMtrRepository.Delete(s => s.F11_ProductCode.Trim().Equals(lstCode.Trim()));
            _unitOfWork.ProductRepository.Delete(s => s.F09_ProductCode.Equals(lstCode.Trim()));
           
            _unitOfWork.Commit();
        }

        /// <summary>
        /// Search product for printing.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="gridSettings"></param>
        /// <returns></returns>
        public ResponseResult<GridResponse<ProductItem>> SearchPrint(string code, GridSettings gridSettings)
        {
            var result = _unitOfWork.ProductRepository.GetAll();

            if (!string.IsNullOrEmpty(code))
                result = result.Where(i => i.F09_ProductCode.ToUpper().Contains(code.ToUpper()));
            
            var itemCount = result.Count();

            if (gridSettings != null)
                OrderByAndPaging(ref result, gridSettings);

            var resultMap = Mapper.Map<IEnumerable<TM09_Product>, IEnumerable<ProductItem>>(result.ToList());
            var resultModel = new GridResponse<ProductItem>(resultMap, itemCount);
            return new ResponseResult<GridResponse<ProductItem>>(resultModel, true);
        }

        public ResponseResult<GridResponse<TM09_Product>> SearchCriteria(string code, GridSettings gridSettings)
        {

            var result = _unitOfWork.ProductRepository.GetAll();
            if (!string.IsNullOrEmpty(code))
            {
                result = result.Where(i => i.F09_ProductCode.ToUpper().Contains(code.ToUpper()));
            }
            var itemCount = result.Count();
            OrderByAndPaging(ref result, gridSettings);
            
            //result = result.Skip((gridSettings.PageIndex - 1) * gridSettings.PageSize).Take(gridSettings.PageSize);
            var resultModel = new GridResponse<TM09_Product>(result, itemCount);
            return new ResponseResult<GridResponse<TM09_Product>>(resultModel, true);
        }
        public bool CheckUnique(string productCode)
        {
            return _unitOfWork.ProductRepository.GetAll().Any(m => m.F09_ProductCode.Equals(productCode.Trim()));
        }

        public ResponseResult CreateOrUpdate(ProductItem model)
        {
            if (model.IsCreate)
            {
                if (!string.IsNullOrEmpty(model.F09_ProductCode))
                {
                    if (CheckUnique(model.F09_ProductCode))
                    {
                        return new ResponseResult(false, Constants.Messages.Material_MSG004);
                    }
                }
                var entity = Mapper.Map<TM09_Product>(model);
                entity.F09_AddDate = DateTime.Now;
                entity.F09_UpdateDate = DateTime.Now;
                entity.F09_UpdateCount = 0;
                _unitOfWork.ProductRepository.Add(entity);
                _unitOfWork.PckMtrRepository.UpdateWithProduct(entity, true);
            }
            else
            {
                var entity = _unitOfWork.ProductRepository.GetById(model.F09_ProductCode);
                if (entity == null)
                {
                    return new ResponseResult(false, Constants.Messages.Material_MSG001);
                }
                var addDate = entity.F09_AddDate;
                var updateCount = entity.F09_UpdateCount ?? 0;
                Mapper.Map(model, entity);
                entity.F09_AddDate = addDate;
                entity.F09_UpdateDate = DateTime.Now;
                entity.F09_UpdateCount = updateCount + 1;
                _unitOfWork.ProductRepository.Update(entity);
                //update tm11
                _unitOfWork.PckMtrRepository.UpdateWithProduct(entity, false);
            }
            _unitOfWork.Commit();
            return new ResponseResult(true);
        }

        public ProductItem GetByCode(string code)
        {
            var entity = _unitOfWork.ProductRepository.Get(x => x.F09_ProductCode.Trim().Equals(code.Trim()));
            return Mapper.Map<TM09_Product, ProductItem>(entity);
        }

        //public ResponseResult BindingDatatableToForm(string lstCode)
        //{
        //    var list = lstCode.Split(',');
        //    foreach (var code in list)
        //    {
        //        _unitOfWork.ProductRepository.Delete(s => s.F09_ProductCode.Equals(code.Trim()));
        //    }
        //    return new ResponseResult(true);
        //}

        #endregion
    }
}
