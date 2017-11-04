using System;
using System.Reflection;
using System.Web.Mvc;
using log4net;

namespace KCSG.Web.Attributes
{
    public class MvcExceptionFilter : IExceptionFilter
    {
        #region Properties

        /// <summary>
        /// Instance which is for logging.
        /// </summary>
        private ILog _log;

        #endregion

        #region Constructors

        /// <summary>
        /// Initiate without IoC (for future detection)
        /// </summary>
        public MvcExceptionFilter()
        {
            _log = LogManager.GetLogger(Assembly.GetExecutingAssembly().GetType());
        }

        /// <summary>
        /// Initiate filter with IoC.
        /// </summary>
        /// <param name="log"></param>
        public MvcExceptionFilter(ILog log)
        {
            _log = log;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Callback is called when exception is thrown by service.
        /// </summary>
        /// <param name="exceptionContext"></param>
        public void OnException(ExceptionContext exceptionContext)
        {
            if (_log == null)
                return;
            _log.Error(exceptionContext.Exception.Message);
        }

        #endregion
    }
}