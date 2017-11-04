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
    public class MaterialDomain : BaseDomain, IMaterialDomain
    {
      
        #region Constructor
        public MaterialDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            
        }
        #endregion

        #region Methods

        public MaterialItem GetById(string id)
        {
            var entity = _unitOfWork.MaterialRepository.GetAll().FirstOrDefault(i=>i.F01_MaterialCode.Trim().Equals(id.Trim()));
            return Mapper.Map<MaterialItem>(entity);
        }

        public void Create(MaterialItem material)
        {
           
            var entity = Mapper.Map<TM01_Material>(material);
            _unitOfWork.MaterialRepository.Add(entity);
            _unitOfWork.Commit();
        }

        public void Update(MaterialItem material)
        {
            var entity = Mapper.Map<TM01_Material>(material);
            _unitOfWork.MaterialRepository.Update(entity);
            _unitOfWork.Commit();
        }

        //public void Delete(string code)
        //{
        //    throw new NotImplementedException();
        //}

        public void Delete(string id)
        {
            _unitOfWork.MaterialRepository.Delete(s => s.F01_MaterialCode.Equals(id.Trim()));
            _unitOfWork.Commit();
        }

        public ResponseResult<GridResponse<MaterialItem>> SearchCriteria(string code, GridSettings gridSettings)
        {
            
            var result = _unitOfWork.MaterialRepository.GetAll();
            if (!string.IsNullOrEmpty(code))
            {
                result = result.Where(i => i.F01_MaterialCode.ToUpper().Contains(code.ToUpper()));
            }
            var itemCount = result.Count();

            if (gridSettings != null)
                OrderByAndPaging(ref result, gridSettings);

            var lstResult = Mapper.Map<IEnumerable<TM01_Material>, IEnumerable<MaterialItem>>(result.ToList());
            var resultModel = new GridResponse<MaterialItem>(lstResult, itemCount);
            return new ResponseResult<GridResponse<MaterialItem>>(resultModel, true);
        }

        public bool CheckUnique(string materialCode)
        {
            return _unitOfWork.MaterialRepository.GetAll().Any(m => m.F01_MaterialCode.Trim().Equals(materialCode.Trim()));
        }

        public ResponseResult CreateOrUpdate(MaterialItem model)
        {
            if (model.IsCreate)
            {
                if (!string.IsNullOrEmpty(model.F01_MaterialCode))
                {
                    if (CheckUnique(model.F01_MaterialCode))
                    {
                        return new ResponseResult(false, Constants.Messages.Material_MSG004);
                    }
                }
                var entity = Mapper.Map<TM01_Material>(model);
                entity.F01_AddDate = DateTime.Now;
                entity.F01_UpdateDate = DateTime.Now;
                _unitOfWork.MaterialRepository.Add(entity);
            }
            else
            {
                var entity = _unitOfWork.MaterialRepository.GetById(model.F01_MaterialCode);
                if (entity == null)
                {
                    return new ResponseResult(false, Constants.Messages.Material_MSG001);
                }
                Mapper.Map(model, entity);
                entity.F01_UpdateDate = DateTime.Now;
                _unitOfWork.MaterialRepository.Update(entity);
            }
            _unitOfWork.Commit();
            return new ResponseResult(true);
        }

        public IEnumerable<TM01_Material> GetMaterials(string materialCode)
        {
            return _unitOfWork.MaterialRepository.GetAll().Where(i => i.F01_MaterialCode.ToUpper().Contains(materialCode.ToUpper()));
        }

       
        #endregion
    }
}
