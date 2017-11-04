using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.EnvironmentManagement;
using KCSG.Domain.Models.EnvironmentManagement;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.EnvironmentManagement
{
    public class CalculationOfControlLimitDomain : BaseDomain,ICalculationOfControlLimitDomain
    {

        #region Constructor

        public CalculationOfControlLimitDomain(IUnitOfWork unitOfWork):base(unitOfWork)
        {
            
        }
        #endregion

        public ResponseResult<GridResponse<CalculationOfControlLimitItem>> SearchCriteria(GridSettings gridSettings)
        {
            var f80Type = Constants.EnvType.TYPE_RM.ToString("D");
            var result = _unitOfWork.EnvMespRepository.GetMany(i => i.F80_Type.Equals(f80Type));

            var itemCount = result.Count();
            OrderByAndPaging(ref result, gridSettings);

            var lstResult = Mapper.Map<IEnumerable<Te80_Env_Mesp>, IEnumerable<CalculationOfControlLimitItem>>(result.ToList());
            var resultModel = new GridResponse<CalculationOfControlLimitItem>(lstResult, itemCount);
            return new ResponseResult<GridResponse<CalculationOfControlLimitItem>>(resultModel, true);
        }

    }
}
