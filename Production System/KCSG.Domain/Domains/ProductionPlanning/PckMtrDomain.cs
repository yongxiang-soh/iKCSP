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
    public class PckMtrDomain :BaseDomain,IPckMtrDomain
    {
       

        #region Constructor
        public PckMtrDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
           
        }
        #endregion
        #region Methods

        public PckMtrItem GetById(string id)
        {
            var entity = _unitOfWork.PckMtrRepository.GetById(id);
            return Mapper.Map<PckMtrItem>(entity);
        }

        public IEnumerable<PckMtrItem> GetAll()
        {
            var entity = _unitOfWork.PckMtrRepository.GetAll().ToList();
            return Mapper.Map<IEnumerable<PckMtrItem>>(entity);
        }

        public void Create(PckMtrItem pckMtr)
        {
            var entity = Mapper.Map<TM11_PckMtr>(pckMtr);
            _unitOfWork.PckMtrRepository.Add(entity);
            _unitOfWork.Commit();
        }

        public void Update(PckMtrItem pckMtr)
        {
            var entity = Mapper.Map<TM11_PckMtr>(pckMtr);
            _unitOfWork.PckMtrRepository.Update(entity);
            _unitOfWork.Commit();
        }

        public bool Delete(string productCode, string subMaterialCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(subMaterialCode))
                {
                    _unitOfWork.PckMtrRepository.Delete(i => i.F11_ProductCode.Trim().Equals(productCode.Trim()) && i.F11_SubMaterialCode.Trim().Equals(subMaterialCode.Trim()));
                }
                else
                {
                    _unitOfWork.PckMtrRepository.Delete(i => i.F11_ProductCode.Trim().Equals(productCode.Trim()));
                }
               
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
         
        }

        public TM11_PckMtr GetPckMtr(string productCode, string subMaterialCode)
        {
            return
                _unitOfWork.PckMtrRepository.GetMany(
                    i => i.F11_ProductCode == productCode && i.F11_SubMaterialCode == subMaterialCode).FirstOrDefault();
        }

      
        public ResponseResult<GridResponse<TM11_PckMtr>> SearchCriteria(string productCode, GridSettings gridSettings)
        {

            var result = _unitOfWork.PckMtrRepository.GetAll();
            result = result.Where(i => i.F11_ProductCode.ToUpper().Contains(productCode.ToUpper()));
            var itemCount = result.Count();
            OrderByAndPaging(ref result, gridSettings);
            var resultModel = new GridResponse<TM11_PckMtr>(result, itemCount);
            return new ResponseResult<GridResponse<TM11_PckMtr>>(resultModel, true);
        }

        public ResponseResult<PckMtrItem> CreateOrUpdate(PckMtrItem model)
        {
            if (model.IsCreate)
            {
                var entity = Mapper.Map<TM11_PckMtr>(model);
                entity.F11_AddDate = DateTime.Now;
                entity.F11_UpdateDate = DateTime.Now;
                _unitOfWork.PckMtrRepository.Add(entity);
            }
            else
            {
                var entity = GetPckMtr(model.F11_ProductCode, model.F11_SubMaterialCode);
                if (entity == null)
                {
                    return new ResponseResult<PckMtrItem>(null,false, Constants.Messages.Material_MSG001);
                }
                var addDate = entity.F11_AddDate;
                Mapper.Map(model, entity);
                entity.F11_UpdateDate = DateTime.Now;
                entity.F11_AddDate = addDate;
                _unitOfWork.PckMtrRepository.Update(entity);
            }
            _unitOfWork.Commit();
            return new ResponseResult<PckMtrItem>(model,true);
        }

        public bool CheckUnique(string f11_SubMaterialCode, string f11_ProductCode)
        {
            return
                _unitOfWork.PckMtrRepository.GetAll()
                    .Any(
                        i =>
                            i.F11_SubMaterialCode.Trim().Equals(f11_SubMaterialCode.Trim()) &&
                            i.F11_ProductCode.Trim().Equals(f11_ProductCode.Trim()));
        }

        #endregion
    }
}
