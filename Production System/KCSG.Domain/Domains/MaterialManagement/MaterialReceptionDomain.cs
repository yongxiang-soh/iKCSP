using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.jsGrid.MVC;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using KCSG.Core.Helper;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.Services;

namespace KCSG.Domain.Domains.MaterialManagement
{
    public class MaterialReceptionDomain : BaseDomain, IMaterialReceptionDomain
    {
      

        #region Constructor

        public MaterialReceptionDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
           
        }

        #endregion

        #region Methods

        /// <summary>
        /// Find material reception by using primary keys.
        /// </summary>
        /// <param name="prcOrdNo"></param>
        /// <param name="prtDvrNo"></param>
        /// <returns></returns>
        public async Task<MaterialReceptionItem> SearchByPrimaryKeys(string prcOrdNo, string prtDvrNo)
        {
            var result = await _unitOfWork.ReceptionRepository.GetAll()
                .FirstOrDefaultAsync(r => (r.F30_PrcOrdNo.Trim().Equals(prcOrdNo) && r.F30_PrtDvrNo.Trim().Equals(prtDvrNo)));
            var entity = Mapper.Map<MaterialReceptionItem>(result);
            return entity;
        }

        public void Create(MaterialReceptionItem materialReception)
        {
            var entity = Mapper.Map<TX30_Reception>(materialReception);
            _unitOfWork.ReceptionRepository.Add(entity);
            _unitOfWork.Commit();
        }

        public void Update(MaterialReceptionItem materialReception)
        {
            var entity = Mapper.Map<TX30_Reception>(materialReception);
            _unitOfWork.ReceptionRepository.Update(entity);
            _unitOfWork.Commit();
        }

        public ResponseResult<GridResponse<MaterialReceptionItem>> SearchCriteria(string prcOrdNo, string parDelivery, jsGrid.MVC.GridSettings gridSettings)
        {
            var result = _unitOfWork.ReceptionRepository.GetAll();
            if (!string.IsNullOrEmpty(prcOrdNo) && !string.IsNullOrEmpty(parDelivery))
            {
                result = result.Where(i => i.F30_PrcOrdNo.ToUpper().StartsWith(prcOrdNo.ToUpper()) && i.F30_PrtDvrNo.ToUpper().StartsWith(parDelivery.ToUpper()));
            }

            var itemCount = result.Count();
            OrderByAndPaging(ref result, gridSettings);

            var lstMaterialCode = result.Select(i => i.F30_MaterialCode);
            var listMaterial = _unitOfWork.MaterialRepository.GetAll().Where(i => lstMaterialCode.Contains(i.F01_MaterialCode));
            var lstResult = new List<MaterialReceptionItem>();

            foreach (var reception in result)
            {
                var materialReceptionItem = new MaterialReceptionItem();
                var lstMaterialItem = listMaterial.FirstOrDefault(i => i.F01_MaterialCode == reception.F30_MaterialCode);
                if (lstMaterialItem != null)
                {
                    materialReceptionItem.Name = lstMaterialItem.F01_MaterialDsp;
                }
                materialReceptionItem.F30_PrcOrdNo = reception.F30_PrcOrdNo;
                materialReceptionItem.F30_PrtDvrNo = reception.F30_PrtDvrNo;
                materialReceptionItem.F30_MaterialCode = reception.F30_MaterialCode;
                materialReceptionItem.F30_ExpectAmount = reception.F30_ExpectAmount;
                materialReceptionItem.F30_StoragedAmount = reception.F30_StoragedAmount;
                materialReceptionItem.F30_AcceptClass = reception.F30_AcceptClass;
                materialReceptionItem.F30_ExpectDate = reception.F30_ExpectDate;
                lstResult.Add(materialReceptionItem);
            }
            var resultModel = new GridResponse<MaterialReceptionItem>(lstResult, itemCount);
            return new ResponseResult<GridResponse<MaterialReceptionItem>>(resultModel, true);
        }

        public ResponseResult CreateOrUpdate(MaterialReceptionItem model)
        {
            
            if (model.IsCreate)
            {
                if (!string.IsNullOrEmpty(model.F30_PrcOrdNo) && !string.IsNullOrEmpty(model.F30_PrtDvrNo))
                {
                    if (CheckUnique(model.F30_PrcOrdNo,model.F30_PrtDvrNo))
                    {
                        return new ResponseResult(false, Constants.Messages.Material_MSG03);
                    }
                }
                var entity = Mapper.Map<TX30_Reception>(model);
                entity.F30_AddDate = DateTime.Now;
                entity.F30_UpdateDate = DateTime.Now;
                entity.F30_UpdateCount = 1;
                entity.F30_AcceptClass = Constants.TX30_Reception.NonAccept.ToString("D");
                _unitOfWork.ReceptionRepository.Add(entity);
            }
            else
            {
                var entity = _unitOfWork.ReceptionRepository.GetMany(i => i.F30_PrcOrdNo.Trim().Equals(model.F30_PrcOrdNo.Trim()) && i.F30_PrtDvrNo.Trim().Equals(model.F30_PrtDvrNo.Trim())).FirstOrDefault(); ;
                if (entity == null)
                {
                    return new ResponseResult(false, Constants.Messages.Material_MSG001);
                }
                Mapper.Map(model, entity);
                entity.F30_UpdateDate = DateTime.Now;
                entity.F30_UpdateCount++;
                _unitOfWork.ReceptionRepository.Update(entity);
            }
            _unitOfWork.Commit();
            return new ResponseResult(true);
        }

        public MaterialReceptionItem GetByMaterialReception(string prcOrdNo, string prtDvrNo)
        {
            var entity = _unitOfWork.ReceptionRepository.GetMany(i => i.F30_PrcOrdNo.Trim().Equals(prcOrdNo.Trim()) && i.F30_PrtDvrNo.Trim().Equals(prtDvrNo.Trim()));
            var lstMaterialCode = entity.Select(i => i.F30_MaterialCode);
            var materialItem = _unitOfWork.MaterialRepository.GetMany(i => lstMaterialCode.Contains(i.F01_MaterialCode)).FirstOrDefault();

            var result= Mapper.Map<MaterialReceptionItem>(entity.FirstOrDefault());
            if (materialItem != null)
            {
                result.Name = materialItem.F01_MaterialDsp;
            }
            return result;

        }

        public bool Delete(string prcOrdNo, string prtDvrNo, string materialCode)
        {
            var mtrShfItem =
                _unitOfWork.MaterialShelfRepository.GetMany(
                    i => i.F32_PrcOrdNo.Trim().Equals(prcOrdNo.Trim()) && i.F32_PrtDvrNo.Trim().Equals(prtDvrNo.Trim()));

            if (mtrShfItem.Any())
            {
                return false;
            }
            _unitOfWork.ReceptionRepository.Delete(s => s.F30_PrcOrdNo.Trim().Equals(prcOrdNo.Trim()) && s.F30_PrtDvrNo.Trim().Equals(prtDvrNo.Trim()) && s.F30_MaterialCode.Trim().Equals(materialCode.Trim()));
            _unitOfWork.Commit();
            return true;
        }

        public bool CheckUnique(string prcOrdNo, string prtDvrNo)
        {
            return _unitOfWork.ReceptionRepository.GetAll().Any(m => m.F30_PrcOrdNo.Trim().Equals(prcOrdNo.Trim()) && m.F30_PrtDvrNo.Trim().Equals(prtDvrNo.Trim()));
        }

        public bool CheckPrtDvrNo(string prtDvrNo)
        {
            return _unitOfWork.ReceptionRepository.GetAll().Any(m => m.F30_PrtDvrNo.Trim().Equals(prtDvrNo.Trim()));
        }

        public MaterialReceptionItem GetById(string id)
        {
            var entity = _unitOfWork.ReceptionRepository.GetById(id);
            return Mapper.Map<MaterialReceptionItem>(entity);
        }

       
        #endregion
    }
}
