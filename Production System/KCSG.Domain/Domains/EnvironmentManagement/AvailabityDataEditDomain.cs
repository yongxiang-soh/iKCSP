using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Helper;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.EnvironmentManagement;

namespace KCSG.Domain.Domains.EnvironmentManagement
{
    public class AvailabityDataEditDomain : BaseDomain, IAvailabityDataEditDomain
    {
        #region Constructor

        public AvailabityDataEditDomain(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        #endregion

        public void Edit(string status, string environmentDate, string time, int id)
        {
            DateTime newTime = DateTime.Parse(time, System.Globalization.CultureInfo.CurrentCulture);

            var envTime = environmentDate + ' ' + newTime.ToString("hh:mm tt");

            var f82EnvTime = ConvertHelper.ConvertToDateTimeFullHourMinute(envTime);

            DateTime timeEnd = DateTime.Parse("08:00", System.Globalization.CultureInfo.CurrentCulture);
            DateTime timeStart = DateTime.Parse("00:00",System.Globalization.CultureInfo.CurrentCulture);

            if (newTime >= timeStart && newTime < timeEnd)
            {
                f82EnvTime = f82EnvTime.AddDays(1);
            }
            var te82EnvAval =
                _unitOfWork.EnvAvalRepository.Get(i => i.F82_Env_Time.Equals(f82EnvTime) && i.F82_Id.Equals(id));

            if (te82EnvAval != null)
            {
                te82EnvAval.F82_Status = status;
                _unitOfWork.EnvAvalRepository.Update(te82EnvAval);

                _unitOfWork.Commit();
            }
        }

        public string GetStatusInTe82_Env_Aval(int id, string environmentDate, string time)
        {
            DateTime newTime = DateTime.Parse(time, System.Globalization.CultureInfo.CurrentCulture);

            var envTime = environmentDate + ' ' + newTime.ToString("hh:mm tt");

            var f82EnvTime = ConvertHelper.ConvertToDateTimeFullHourMinute(envTime);

            DateTime timeEnd = DateTime.Parse("08:00", System.Globalization.CultureInfo.CurrentCulture);
            DateTime timeStart = DateTime.Parse("00:00", System.Globalization.CultureInfo.CurrentCulture);

            if (newTime >= timeStart && newTime < timeEnd)
            {
                f82EnvTime = f82EnvTime.AddDays(1);
            }

            var te82EnvAval =
                _unitOfWork.EnvAvalRepository.Get(i => i.F82_Env_Time.Equals(f82EnvTime) && i.F82_Id.Equals(id));

            if (te82EnvAval != null)
                return te82EnvAval.F82_Status;
            return null;
        }


    }
}