using KCSG.Core.Models;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.SystemManagement;

namespace KCSG.Domain.Domains.SystemManagement
{
    public class DailyProcessDomain : IDailyProcessDomain
    {
        #region Properties

        /// <summary>
        ///     Unit of work.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Contructor

        public DailyProcessDomain(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Methods

        public ResponseResult DailyProcess(string terminalNo)
        {
            var terminalStatusItem = _unitOfWork.TermStatusRepository.GetId(terminalNo);
            if (terminalStatusItem != null)
            {
                terminalStatusItem.F17_InUsePictureNo = null;
                _unitOfWork.TermStatusRepository.Update(terminalStatusItem);
                _unitOfWork.Commit();
            }

            return new ResponseResult(true);
        }

        #endregion
    }
}