using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Helper;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.EnvironmentManagement;

namespace KCSG.Domain.Domains.EnvironmentManagement
{
    public class LotDataCleanupDomain : ILotDataCleanupDomain
    {
        private readonly IUnitOfWork _unitOfWork;

        #region Constructor

        public LotDataCleanupDomain(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        public int GetLots()
        {
            var result = _unitOfWork.EnvLotRepository.GetAll().Select(i => i.F84_ProductLotNo).Distinct().Count();
            return result;
        }

        public IList<Te84_Env_Lot> GetListTe84EnvLots()
        {
            var te84EnvLot = _unitOfWork.EnvLotRepository.GetAll().OrderBy(i => i.F84_S_Time);
            return te84EnvLot.ToList();
        }


        public int Testing(string stringNewCutOffDate, string stringNewCutOffTime)
        {
            DateTime newCutOffTime = DateTime.Parse(stringNewCutOffTime, System.Globalization.CultureInfo.CurrentCulture);

            var stringdtmCutOff = stringNewCutOffDate + ' ' + newCutOffTime.ToString("hh:mm tt");
            var dtmCutOffDate = ConvertHelper.ConvertToDateTimeFullHourMinute(stringdtmCutOff);

            var lots =
                _unitOfWork.EnvLotRepository.GetMany(i => i.F84_S_Time >= dtmCutOffDate)
                    .Select(i => i.F84_ProductLotNo)
                    .Distinct()
                    .Count();
            return lots;
        }

        public bool Delete(string stringNewCutOffDate, string stringNewCutOffTime)
        {
            DateTime newCutOffTime = DateTime.Parse(stringNewCutOffTime, System.Globalization.CultureInfo.CurrentCulture);

            var stringdtmCutOff = stringNewCutOffDate + ' ' + newCutOffTime.ToString("hh:mm tt");
            var dtmCutOffDate = ConvertHelper.ConvertToDateTimeFullHourMinute(stringdtmCutOff);

            var lstResult = _unitOfWork.EnvLotRepository.GetMany(i => i.F84_S_Time < dtmCutOffDate);
            if(!lstResult.Any())
                return false;
            foreach (var item in lstResult)
            {
                _unitOfWork.EnvLotRepository.Delete(item);
            }
            _unitOfWork.Commit();
            return true;
        }
    }
}