using System;
using KCSG.Core.Constants;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.SystemManagement;

namespace KCSG.Domain.Domains.SystemManagement
{
   public class MonthlyProcessDomain : IMonthlyProcessDomain
    {
        private IUnitOfWork _iUnitOfWork;

        public MonthlyProcessDomain(IUnitOfWork iUnitOfWork)
        {
            _iUnitOfWork = iUnitOfWork;
        }

        public bool MonthlyProcess()
        {
            try
            {
                var startDate = DateTime.Now.AddDays(-90);
                var notSotoked = Constants.TX33_StkFlg.NotStoked.ToString("D");
                _iUnitOfWork.Context.DeleteMonthlyProcess(startDate, notSotoked);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
