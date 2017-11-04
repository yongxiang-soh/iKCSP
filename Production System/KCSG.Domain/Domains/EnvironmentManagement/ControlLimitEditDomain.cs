using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.EnvironmentManagement;
using KCSG.Domain.Models.EnvironmentManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.EnvironmentManagement
{
    public class ControlLimitEditDomain:BaseDomain,IControlLimitEditDomain
    {
        #region Constructor

        public ControlLimitEditDomain(IUnitOfWork unitOfWork):base(unitOfWork)
        {
            
        }
        #endregion

        public ResponseResult<GridResponse<CalculationOfControlLimitItem>> SearchCriteria(string location,GridSettings gridSettings)
        {
            var f80Type = Constants.EnvType.TYPE_RM.ToString("D");
            var result = _unitOfWork.EnvMespRepository.GetMany(i => i.F80_Type.Equals(f80Type) && i.F80_Name.Equals(location));

            var itemCount = result.Count();
            OrderByAndPaging(ref result, gridSettings);

            var lstResult = Mapper.Map<IEnumerable<Te80_Env_Mesp>, IEnumerable<CalculationOfControlLimitItem>>(result.ToList());
            var resultModel = new GridResponse<CalculationOfControlLimitItem>(lstResult, itemCount);
            return new ResponseResult<GridResponse<CalculationOfControlLimitItem>>(resultModel, true);
        }


        public void Update(ControlLimitEditItem item)
        {
            var id = int.Parse(item.Location);
            var f80Type = Constants.EnvType.TYPE_RM.ToString("D");
            var te80EnvMesps =
                _unitOfWork.EnvMespRepository.GetMany(
                    i => i.F80_Type.Equals(f80Type) && i.F80_Id.Equals(id));

            foreach (var te80EnvMesp in te80EnvMesps)
            {
                te80EnvMesp.F80_T_Ucl = item.TempUCL;
                te80EnvMesp.F80_T_Lcl = item.TempLCL;
                te80EnvMesp.F80_T_Mean = item.TempMean;
                te80EnvMesp.F80_T_Sigma = item.TempSigma;
                te80EnvMesp.F80_T_Cp = item.TempCp;
                te80EnvMesp.F80_T_Cpk = item.TempCpk;
                te80EnvMesp.F80_T_Range = item.TempRange;

                te80EnvMesp.F80_H_Ucl = item.HumUCL;
                te80EnvMesp.F80_H_Lcl = item.HumLCL;
                te80EnvMesp.F80_H_Mean = item.HumMean;
                te80EnvMesp.F80_H_Sigma = item.HumSigma;
                te80EnvMesp.F80_H_Cp = item.HumCp;
                te80EnvMesp.F80_H_Cpk = item.HumCpk;
                te80EnvMesp.F80_H_Range = item.HumRange;

                te80EnvMesp.F80_D_From = ConvertHelper.ConvertToDateTimeFull(item.From);
                te80EnvMesp.F80_D_To = ConvertHelper.ConvertToDateTimeFull(item.To);

                _unitOfWork.EnvMespRepository.Update(te80EnvMesp);
            }

            _unitOfWork.Commit();
        }
    }
}
