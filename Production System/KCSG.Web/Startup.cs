using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using KCSG.Core.Controls.CustomerValidate;
using KCSG.Core.Helper;
using KCSG.Data.DatabaseContext;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Domains.CommunicationManagement;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.Products.AppStart;
using KCSG.Web;
using KCSG.Web.Attributes;
using KCSG.Web.Services;
using log4net;
using Microsoft.Owin;
using Newtonsoft.Json;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace KCSG.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AreaRegistration.RegisterAllAreas();
            AutoMapperConfig.RegisterProfiles();
            AutoFacConfig.RegisterContainers();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            app.MapSignalR();
            
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(ExtRange), typeof(RangeAttributeAdapter));
            ModelBinders.Binders.Add(typeof(double), new DoubleModelBinder());
            ModelBinders.Binders.Add(typeof(double?), new DoubleModelBinder());
            ModelBinders.Binders.Add(typeof(int), new IntModelBinder());
            ModelBinders.Binders.Add(typeof(int?), new IntModelBinder());

            // Delete all broadcasted realtime connection.
            var initialUnitOfWork = new UnitOfWork(new KCSGDbContext());
            initialUnitOfWork.RealtimeConnectionRepository.Delete(x => x.Id > 0);
            initialUnitOfWork.Commit();

            var configurationService =
                (IConfigurationService) DependencyResolver.Current.GetService(typeof(IConfigurationService));
            var commonDomain = (ICommonDomain) DependencyResolver.Current.GetService(typeof(ICommonDomain));

            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var unitOfWork = new UnitOfWork(new KCSGDbContext());
                        var log = LogManager.GetLogger(typeof(Startup));
                        var notificationService = new NotificationService(unitOfWork, log, configurationService);
                        var deviceCode = new ConfigurationService().ProductDeviceCode;
                        var thirdCommunicationDomain = new Communication3Domain(unitOfWork, notificationService,
                            configurationService, log);
                        thirdCommunicationDomain.ProcessData(false, deviceCode);
                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine(exception.Message);
                    }

                    Thread.Sleep(ConvertHelper.ToInteger(ConfigurationManager.AppSettings["TimerProcess"]));
                }
            });

            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var unitOfWork = new UnitOfWork(new KCSGDbContext());
                        var notificationService = new NotificationService(unitOfWork,
                            LogManager.GetLogger(typeof(NotificationService)), configurationService);
                        var log = LogManager.GetLogger(typeof(Startup));
                        var deviceCode = new ConfigurationService().MaterialDeviceCode;
                        var firstCommunicationDomain = new Communication1Domain(unitOfWork, notificationService,
                            commonDomain, LogManager.GetLogger(typeof(Communication1Domain)));
                        firstCommunicationDomain.ProcessData(false, deviceCode);
                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine(exception.Message);
                    }

                    Thread.Sleep(ConvertHelper.ToInteger(ConfigurationManager.AppSettings["TimerProcess"]));
                }
            });

            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var unitOfWork = new UnitOfWork(new KCSGDbContext());
                        var deviceCode = new ConfigurationService().PreProductDeviceCode;
                        var notificationService = new NotificationService(unitOfWork,
                            LogManager.GetLogger(typeof(NotificationService)), configurationService);
                        var secondCommunicationDomain = new Communication2Domain(unitOfWork, notificationService,
                            configurationService);
                        secondCommunicationDomain.ProcessData(deviceCode);
                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine(exception.Message);
                    }

                    Thread.Sleep(ConvertHelper.ToInteger(ConfigurationManager.AppSettings["TimerProcess"]));
                }
            });

            #region Load configuration from file

            // Find label print service in dependency injection.
            var labelPrintService = DependencyResolver.Current.GetService<ILabelPrintService>();
            labelPrintService.PreProductLabelOrignalText =
                ReadRelativeFileContent(ConfigurationManager.AppSettings["PreProductLabelFile"]);

            labelPrintService.InternalLabelOriginalContent =
                FindInternalLabelFromFile(
                    ConfigurationManager.AppSettings["InternalLabelConfigurationFile"]);
            labelPrintService.ExternalLabelOriginalContent =
                FindExternalLabelFromFile(
                    ConfigurationManager.AppSettings["ExternalLabelConfigurationFile"]);

            // Find print configuration.
            var printerConfiguration =
                configurationService.LoadConfigurationFromFile<PrinterConfiguration>(
                    ConfigurationManager.AppSettings["LabelPrinterConfigurationFile"]);
            labelPrintService.InternalPrinters = printerConfiguration.InternalPrinters;
            labelPrintService.ExternalPrinters = printerConfiguration.ExternalPrinters;
            labelPrintService.PreProductPrinters = printerConfiguration.PreProductPrinters;
            #endregion

            #region Screens list

            // Load screen list from file.
            configurationService.Areas =
                configurationService.LoadConfigurationFromFile<Dictionary<string, AreaInformation>>(
                    ConfigurationManager.AppSettings["ApplicationScreensFile"]);

            #endregion
        }

        /// <summary>
        ///     Find internal label list from configuration file.
        /// </summary>
        /// <param name="internalLabelListFile"></param>
        private Dictionary<string, string> FindInternalLabelFromFile(string internalLabelListFile)
        {
            // Find server path.
            var path = HttpContext.Current.Server.MapPath(internalLabelListFile);

            // File doesn't exist.
            if (!File.Exists(path))
                return null;

            // Read key-value pairs in the configuration file.
            var fileContent = File.ReadAllText(path);
            var parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileContent);
            var labelsList = new Dictionary<string, string>();
            // Read all related content.
            foreach (var key in parameters.Keys)
            {
                var fileContentPath = HttpContext.Current.Server.MapPath(parameters[key]);
                if (!File.Exists(fileContentPath))
                    continue;

                // Read file content.
                var content = File.ReadAllText(fileContentPath);

                if (labelsList.ContainsKey(key))
                {
                    labelsList[key] = content;
                    continue;
                }


                labelsList.Add(key, content);
            }

            return labelsList;
        }

        /// <summary>
        ///     Find internal label list from configuration file.
        /// </summary>
        /// <param name="externalabelListFile"></param>
        private Dictionary<string, string> FindExternalLabelFromFile(string externalabelListFile)
        {
            // Find server path.
            var path = HttpContext.Current.Server.MapPath(externalabelListFile);

            // File doesn't exist.
            if (!File.Exists(path))
                return null;

            // Read key-value pairs in the configuration file.
            var fileContent = File.ReadAllText(path);
            var parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileContent);
            var labelsList = new Dictionary<string, string>();
            // Read all related content.
            foreach (var key in parameters.Keys)
            {
                var fileContentPath = HttpContext.Current.Server.MapPath(parameters[key]);
                if (!File.Exists(fileContentPath))
                    continue;

                // Read file content.
                var content = File.ReadAllText(fileContentPath);

                if (labelsList.ContainsKey(key))
                {
                    labelsList[key] = content;
                    continue;
                }


                labelsList.Add(key, content);
            }

            return labelsList;
        }

        /// <summary>
        ///     Read relative file content.
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        private string ReadRelativeFileContent(string relativeUrl)
        {
            // Return null.
            if (string.IsNullOrEmpty(relativeUrl))
                return null;

            var path = HttpContext.Current.Server.MapPath(relativeUrl);
            try
            {
                return File.ReadAllText(path);
            }
            catch
            {
                return null;
            }
        }
    }
}