using System;
using System.Data.Entity.Validation;
using System.Windows;
using log4net;
using log4net.Config;

namespace KCSG.AutomatedWarehouse
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        ///     Callback which is fired when application starts.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();
            base.OnStartup(e);

            // Initiate exception handler.
            //AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        ///// <summary>
        ///// Handle unhandled exception.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="unhandledExceptionEventArgs"></param>
        //private void OnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        //{
        //    var exception = unhandledExceptionEventArgs.ExceptionObject;
        //    if (exception == null || !(exception is Exception))
        //        return;
            
        //    if (!(exception is DbEntityValidationException))
        //        return;
            
        //    var dbEntityValidationException = (DbEntityValidationException) exception;
        //    var log = LogManager.GetLogger("log-validation-error");
        //    foreach (var eve in dbEntityValidationException.EntityValidationErrors)
        //    {
        //        foreach (var ve in eve.ValidationErrors)
        //            log.Error(string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
        //    }
        //}
    }
}