using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using AutoMapper;
using DataTables.Mvc;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.ProductionPlanning;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.ProductionPlanning
{
    public class PreProductPlanDomain :BaseDomain, IPreProductPlanDomain
    {
         

        #region Constructor
        public PreProductPlanDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            
        }
        #endregion
        #region Methods
        public PreProductPlanItem GetById(string date,string code)
        {
            var Ddate = ConvertHelper.ConvertToDateTimeFull(date);
            var entity = _unitOfWork.PreProductPlanRepository.Get(p => p.F94_PrepdtCode.Equals(code) && p.F94_YearMonth.Equals(Ddate));
            return Mapper.Map<PreProductPlanItem>(entity);
        }
        public void Delete(DateTime date,string code)
        {
            //var list = lstCode.Split(',');
            //foreach (var code in list)
            //{
            //    _unitOfWork.PreProductPlanRepository.Delete(s => s.F94_PrepdtCode.Equals(code.Trim()));
            //}
            _unitOfWork.PreProductPlanRepository.Delete(s => s.F94_PrepdtCode.Equals(code.Trim()) && s.F94_YearMonth.Equals(date));
            _unitOfWork.Commit();
        }
        public ResponseResult<GridResponse<PreProductPlanItem>> SearchCriteria(string date, GridSettings gridSettings)
        {

            var result = _unitOfWork.PreProductPlanRepository.GetAll();
            if (!string.IsNullOrEmpty(date))
            {
                date = "01/" + date;
                DateTime myDate = ConvertHelper.ConvertToDateTimeFull(date);
                result = result.Where(i => i.F94_YearMonth.Equals(myDate));
            }
            var itemCount = result.Count();
            if(gridSettings!=null)
                OrderByAndPaging(ref result, gridSettings);
            var lstPreproduct = _unitOfWork.PreProductRepository.GetAll();
            var preproductPlan =
                result.Select(
                    i =>
                        new PreProductPlanItem()
                        {
                            F94_PrepdtCode = i.F94_PrepdtCode,
                            F94_YearMonth = i.F94_YearMonth,
                            F94_amount = i.F94_amount,
                            PreProductName =  lstPreproduct.FirstOrDefault(o=>o.F03_PreProductCode==i.F94_PrepdtCode).F03_PreProductName
                        });
            var resultModel = new GridResponse<PreProductPlanItem>(preproductPlan, itemCount);
            return new ResponseResult<GridResponse<PreProductPlanItem>>(resultModel, true);
            
            
        }
        public bool CheckUnique(string preproductCode,DateTime yearMonth)
        {
            return _unitOfWork.PreProductPlanRepository.GetAll().Any(m => m.F94_PrepdtCode.Equals(preproductCode.Trim())&&m.F94_YearMonth.Equals(yearMonth));
        }

        public ResponseResult CreateOrUpdate(PreProductPlanItem model)
        {
            if (model.IsCreate)
            {
                if (!string.IsNullOrEmpty(model.F94_PrepdtCode))
                {
                    if (CheckUnique(model.F94_PrepdtCode,model.F94_YearMonth))
                    {
                        return new ResponseResult(false, Constants.Messages.Material_MSG004);
                    }
                }
                var entity = Mapper.Map<TX94_Prepdtplan>(model);
                entity.F94_AddDate = DateTime.Now;
               // entity.F94_UpdateDate = DateTime.Now;
                _unitOfWork.PreProductPlanRepository.Add(entity);
            }
            else
            {
                var entity = _unitOfWork.PreProductPlanRepository.Get(m=>m.F94_PrepdtCode.Equals(model.F94_PrepdtCode) && m.F94_YearMonth.Equals(model.F94_YearMonth));
                if (entity == null)
                {
                    return new ResponseResult(false, Constants.Messages.Material_MSG001);
                }
                entity.F94_amount = model.F94_amount;
                 entity.F94_UpdateDate = DateTime.Now;
                _unitOfWork.PreProductPlanRepository.Update(entity);
            }
            _unitOfWork.Commit();
            return new ResponseResult(true);
        }

        #endregion
    }
}
