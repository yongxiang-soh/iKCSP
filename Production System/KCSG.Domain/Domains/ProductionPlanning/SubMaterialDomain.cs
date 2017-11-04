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
    public class SubMaterialDomain :BaseDomain,ISubMaterialDomain
    {
      

        #region Constructor
        public SubMaterialDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
           
        }
        #endregion
        #region Methods

        public SubMaterialItem GetById(string id)
        {
            var entity = _unitOfWork.SubMaterialRepository.GetById(id);
            return Mapper.Map<SubMaterialItem>(entity);
        }
        public void Delete(string code)
        {
            _unitOfWork.SupMaterialStockRepository.Delete(s=>s.F46_SubMaterialCode.Trim().Equals(code.Trim()));
           _unitOfWork.SubMaterialRepository.Delete(s => s.F15_SubMaterialCode.Trim().Equals(code.Trim()));
           _unitOfWork.Commit();
        }

        public ResponseResult<GridResponse<SubMaterialItem>> SearchCriteria(string code, GridSettings gridSettings)
        {

            var result = _unitOfWork.SubMaterialRepository.GetAll();
            if (!string.IsNullOrEmpty(code))
            {
                result = result.Where(i => i.F15_SubMaterialCode.ToUpper().Contains(code.ToUpper()));
            }

            // Sort and paging
            var itemCount = result.Count();
            OrderByAndPaging(ref result, gridSettings);

            var lstResult = Mapper.Map<IEnumerable<TM15_SubMaterial>, IEnumerable<SubMaterialItem>>(result.ToList());

            var resultModel = new GridResponse<SubMaterialItem>(lstResult, itemCount);
            return new ResponseResult<GridResponse<SubMaterialItem>>(resultModel, true);
        }

    

        public ResponseResult CreateOrUpdate(SubMaterialItem model)
        {
            if (model.IsCreate)
            {
                if (!string.IsNullOrEmpty(model.F15_SubMaterialCode))
                {
                    if (CheckUnique(model.F15_SubMaterialCode))
                    {
                        return new ResponseResult(false, Constants.Messages.Material_MSG004);
                    }
                }
                var entity = Mapper.Map<TM15_SubMaterial>(model);
                entity.F15_AddDate = DateTime.Now;
                entity.F15_UpdateCount = 0;
                entity.F15_EntrustedClass = "Norm".Equals(model.Bail, StringComparison.InvariantCultureIgnoreCase)
                    ? Constants.Bailment.Normal.ToString("D")
                    : Constants.Bailment.Bailment.ToString("D");
                entity.F15_UpdateDate = DateTime.Now;
                _unitOfWork.SubMaterialRepository.Add(entity);

                //Add f46_SubMaterialCode
                var subMtrStk = new TX46_SupMtrStk();
                subMtrStk.F46_SubMaterialCode = model.F15_SubMaterialCode;
                subMtrStk.F46_Comment = "";
                subMtrStk.F46_StorageDate = DateTime.Now;
                subMtrStk.F46_Amount = 0.0;
                subMtrStk.F46_AddDate = DateTime.Now;
                subMtrStk.F46_UpdateDate = DateTime.Now;
                subMtrStk.F46_UpdateCount = 0;
                _unitOfWork.SupMaterialStockRepository.Add(subMtrStk);
            }
            else
            {
                var entity = _unitOfWork.SubMaterialRepository.GetAll().FirstOrDefault(i=>i.F15_SubMaterialCode.Trim().Equals(model.F15_SubMaterialCode.Trim()));
                if (entity == null)
                {
                    return new ResponseResult(false, Constants.Messages.Material_MSG001);
                }
                var addDate = entity.F15_AddDate;
                var count = entity.F15_UpdateCount;
                Mapper.Map(model, entity);
                entity.F15_EntrustedClass = "Norm".Equals(model.Bail, StringComparison.InvariantCultureIgnoreCase)
                  ? Constants.Bailment.Normal.ToString("D")
                  : Constants.Bailment.Bailment.ToString("D");
                entity.F15_AddDate = addDate;
                entity.F15_UpdateDate = DateTime.Now;
                entity.F15_UpdateCount = count + 1;
                _unitOfWork.SubMaterialRepository.Update(entity);
            }
            _unitOfWork.Commit();
            return new ResponseResult(true);
        }

        public bool CheckUnique(string materialCode)
        {
            return _unitOfWork.SubMaterialRepository.GetAll().Any(m => m.F15_SubMaterialCode.Trim().Equals(materialCode.Trim()));
        }


        #endregion
    }
}
