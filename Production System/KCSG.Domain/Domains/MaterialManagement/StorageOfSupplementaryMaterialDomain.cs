using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Models;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.jsGrid.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.ProductionPlanning;

namespace KCSG.Domain.Domains.MaterialManagement
{
    public class StorageOfSupplementaryMaterialDomain : BaseDomain, IStorageOfSupplementaryMaterialDomain
    {
        #region Constructor

        public StorageOfSupplementaryMaterialDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
        }

        #endregion

        #region Methods

        public StorageOfSupplementaryMaterialItem GetById(string id)
        {
            var entity = _unitOfWork.SubMaterialRepository.GetById(id);
            var supMtrStkItem = _unitOfWork.SupMaterialStockRepository.GetById(id);

            var lstSupplementaryMaterial = new StorageOfSupplementaryMaterialItem();
            lstSupplementaryMaterial.F15_SubMaterialCode = entity.F15_SubMaterialCode;
            lstSupplementaryMaterial.F15_MaterialDsp = entity.F15_MaterialDsp;
            lstSupplementaryMaterial.F15_Unit = entity.F15_Unit;
            lstSupplementaryMaterial.F15_PackingUnit = entity.F15_PackingUnit;
            if (supMtrStkItem != null)
            {
                lstSupplementaryMaterial.PackQuantity = supMtrStkItem.F46_Amount;
                //lstSupplementaryMaterial.InventoryQuantity = supMtrStkItem.F46_Amount;
                //lstSupplementaryMaterial.PackQuantity = supMtrStkItem.F46_Amount;
                //lstSupplementaryMaterial.InventoryQuantity = supMtrStkItem.F46_Amount;
                if (supMtrStkItem.F46_Comment != null)
                {
                    lstSupplementaryMaterial.Comment = supMtrStkItem.F46_Comment.Trim();
                }                
            }

            return lstSupplementaryMaterial;
        }

        public void Create(SubMaterialItem supMaterial)
        {
            var entity = Mapper.Map<TM15_SubMaterial>(supMaterial);
            _unitOfWork.SubMaterialRepository.Add(entity);
            _unitOfWork.Commit();
        }

        public void Update(SubMaterialItem supMaterial)
        {
            var entity = Mapper.Map<TM15_SubMaterial>(supMaterial);
            _unitOfWork.SubMaterialRepository.Update(entity);
            _unitOfWork.Commit();
        }

        public void Delete(string id)
        {
            _unitOfWork.SubMaterialRepository.Delete(s => s.F15_SubMaterialCode.Equals(id.Trim()));
            _unitOfWork.Commit();
        }

        public bool CheckUnique(string supMaterialCode)
        {
            return
                _unitOfWork.SupMaterialStockRepository.GetAll()
                    .Any(m => m.F46_SubMaterialCode.Trim().Equals(supMaterialCode.Trim()));
        }

        public ResponseResult CreateOrUpdate(StorageOfSupplementaryMaterialItem model)
        {
            if (!string.IsNullOrEmpty(model.F15_SubMaterialCode))
            {
                var lstSupMtrStk = _unitOfWork.SupMaterialStockRepository.GetById(model.F15_SubMaterialCode);
                if (lstSupMtrStk != null)
                {
                    lstSupMtrStk.F46_Comment = model.Comment;
                    lstSupMtrStk.F46_Amount = model.IsStore
                        ? lstSupMtrStk.F46_Amount + model.AddQuantity
                        : model.InventoryQuantity;
                    lstSupMtrStk.F46_UpdateDate = DateTime.Now;
                    var entity = Mapper.Map<TX46_SupMtrStk>(lstSupMtrStk);
                    _unitOfWork.SupMaterialStockRepository.Update(entity);
                }
                else
                {
                    var entity = new TX46_SupMtrStk();
                    entity.F46_AddDate = DateTime.Now;
                    entity.F46_SubMaterialCode = model.F15_SubMaterialCode;
                    entity.F46_Comment = model.Comment;
                    entity.F46_StorageDate = DateTime.Now;
                    entity.F46_Amount = model.AddQuantity;
                    entity.F46_UpdateDate = DateTime.Now;
                    entity.F46_UpdateCount = 0;
                    _unitOfWork.SupMaterialStockRepository.Add(entity);
                }
            }
            _unitOfWork.Commit();
            return new ResponseResult(true);
        }

        public ResponseResult<jsGrid.MVC.GridResponse<SubMaterialItem>> SearchCriteria(string code,
            jsGrid.MVC.GridSettings gridSettings)
        {
            var result = _unitOfWork.SubMaterialRepository.GetAll();
            if (!string.IsNullOrEmpty(code))
            {
                result = result.Where(i => i.F15_SubMaterialCode.ToUpper().Contains(code.ToUpper()));
            }

            var itemCount = result.Count();
            OrderByAndPaging(ref result, gridSettings);
            result = result.Skip((gridSettings.PageIndex - 1)*gridSettings.PageSize).Take(gridSettings.PageSize);
            var lstSubMaterialCode = result.Select(i => i.F15_SubMaterialCode);
            var listSubMaterial =
                _unitOfWork.SupMaterialStockRepository.GetAll()
                    .Where(i => lstSubMaterialCode.Contains(i.F46_SubMaterialCode));
            var lstResult = new List<SubMaterialItem>();

            foreach (var supMaterial in result)
            {
                var subMaterialItem = new SubMaterialItem();
                subMaterialItem.F15_SubMaterialCode = supMaterial.F15_SubMaterialCode;
                subMaterialItem.F15_MaterialDsp = supMaterial.F15_MaterialDsp;
                subMaterialItem.F15_Unit = supMaterial.F15_Unit;
                subMaterialItem.F15_PackingUnit = supMaterial.F15_PackingUnit;
                var submaterial =
                    listSubMaterial.FirstOrDefault(i => i.F46_SubMaterialCode == supMaterial.F15_SubMaterialCode);
                if (submaterial != null)
                {
                    subMaterialItem.Quantity = submaterial.F46_Amount;
                    subMaterialItem.Comment = submaterial.F46_Comment;
                }
                lstResult.Add(subMaterialItem);
            }


            var resultModel = new GridResponse<SubMaterialItem>(lstResult, itemCount);
            return new ResponseResult<GridResponse<SubMaterialItem>>(resultModel, true);
        }

        public IEnumerable<TM15_SubMaterial> GetSupMaterials(string supMaterialCode)
        {
            return
                _unitOfWork.SubMaterialRepository.GetAll()
                    .Where(i => i.F15_SubMaterialCode.ToUpper().Contains(supMaterialCode.ToUpper()));
        }

        #endregion
    }
}