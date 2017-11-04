using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
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
   public class MaterialRequirementListDomain:  BaseDomain,IMaterialRequirementListDomain
    {
 
    

        #region Constructor
        public MaterialRequirementListDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
           
        }
        #endregion

       public ResponseResult<GridResponse<object>> SearchCriteria(DateTime? YearMonth, GridSettings gridSettings)
       {
           var lstPreProductPlan =
               _unitOfWork.PreProductPlanRepository.GetAll();
           if (YearMonth.HasValue)
           {
               lstPreProductPlan =
                   lstPreProductPlan.Where(
                       i =>
                           i.F94_YearMonth.Month == YearMonth.Value.Month &&
                           i.F94_YearMonth.Year == YearMonth.Value.Year);
           }

           var lstMaterial = _unitOfWork.MaterialRepository.GetAll();
           var lstPreProduct = _unitOfWork.PreProductRepository.GetAll();
           var lstPrePdtMkp = _unitOfWork.PrePdtMkpRepository.GetAll();
           var lstAmount = 0.0;
            var result = from material in lstMaterial
               join prePdtMkp in lstPrePdtMkp on material.F01_MaterialCode.Trim() equals
                   prePdtMkp.F02_MaterialCode.Trim()
               join preProduct in lstPreProduct on prePdtMkp.F02_PreProductCode.Trim() equals
                   preProduct.F03_PreProductCode.Trim()
               join preProductPlan in lstPreProductPlan on preProduct.F03_PreProductCode.Trim() equals
                   preProductPlan.F94_PrepdtCode.Trim()
               select new {material,prePdtMkp, preProduct, preProductPlan};
                var lstMaterialResult = result.ToList().Select(i => new
               {
                   i.material.F01_MaterialCode,
                   i.material.F01_MaterialDsp,
                   i.material.F01_Unit,
                   F33_PalletNo = CalculatorAmount( i.prePdtMkp.F02_3FLayinAmount, i.prePdtMkp.F02_4FLayinAmount, 
                   lstPrePdtMkp.Where(o => o.F02_PreProductCode == i.preProduct.F03_PreProductCode).Sum(o => o.F02_3FLayinAmount + o.F02_4FLayinAmount), i.preProduct.F03_YieldRate, i.preProductPlan.F94_amount),
                   F33_MaterialLotNo ="Kg",
                 
               }).AsQueryable();
           var materialCode = "";
           var firstRow = true;
           var kq = new List<object>();
           var totalAmount = 0.0;
           foreach (var resu in lstMaterialResult.OrderBy(i=>i.F01_MaterialCode))
           {
               if (materialCode != resu.F01_MaterialCode)
               {
                   if (firstRow)
                   {
                       materialCode = resu.F01_MaterialCode;
                       firstRow = false;
                   }
                   else
                   {
                       kq.Add(new { resu.F01_MaterialCode, resu.F01_MaterialDsp,totalAmount = totalAmount.ToString("f"), unit  ="Kg"});
                       totalAmount = 0;
                       materialCode = resu.F01_MaterialCode;
                   }
               }
               totalAmount += resu.F33_PalletNo;
           }
           var lstResult = kq.AsQueryable();
           var itemCount = lstResult.Count();
           if (gridSettings != null)
           {
               lstResult = lstResult.Skip((gridSettings.PageIndex - 1)*gridSettings.PageSize)
                   .Take(gridSettings.PageSize);
           }
           var resultModel = new GridResponse<object>(lstResult, itemCount);
           return new ResponseResult<GridResponse<object>>(resultModel, true);
       }

       private double CalculatorAmount(double f02_3FLayinAmount, double f02_4FLayinAmount, double sum, double f03YieldRate, double f94Amount)
       {
           return  (f94Amount / (f03YieldRate / 100)) * ((f02_3FLayinAmount + f02_4FLayinAmount) / sum);
       }

       
       public ResponseResult<GridResponse<object>> Search(GridSettings gridSettings)
       {
           var lstMaterial = _unitOfWork.MaterialRepository.GetAll();
           var lstMaterialShelfStock = _unitOfWork.MaterialShelfStockRepository.GetAll();
           var lstMterialSheldStatus = _unitOfWork.MaterialShelfStatusRepository.GetAll();
           var result =
               lstMaterialShelfStock.Join(lstMaterial, tx33MtrShfStk => tx33MtrShfStk.F33_MaterialCode,
                   material => material.F01_MaterialCode, (tx33MtrShfStk, material) => new {tx33MtrShfStk, material})
                   .Join(lstMterialSheldStatus, @t => @t.tx33MtrShfStk.F33_PalletNo,
                       tx31MtrShfStse => tx31MtrShfStse.F31_PalletNo, (@t, tx31MtrShfStse) => new {@t, tx31MtrShfStse})
                   .OrderBy(@t => @t.@t.material.F01_MaterialCode)
                   .OrderBy(@t => @t.@t.tx33MtrShfStk.F33_MaterialLotNo)
                   .GroupBy(@t => new {@t.t.material.F01_MaterialCode, @t.@t.material.F01_MaterialDsp})
                   .Select(
                       i =>
                           new
                           {
                               i.Key.F01_MaterialCode,
                               i.Key.F01_MaterialDsp,
                               F31_Amount = i.Sum(t => t.tx31MtrShfStse.F31_Amount),
                               unit = "Kg"
                           });
          
           
              

           var itemCount = result.Count();
           if (gridSettings != null)
               OrderByAndPaging(ref result, gridSettings);
           var resultModel = new GridResponse<object>(result, itemCount);
           return new ResponseResult<GridResponse<object>>(resultModel, true);
       }
    }
}
