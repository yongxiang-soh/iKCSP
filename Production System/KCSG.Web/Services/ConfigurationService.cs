using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Ports;
using System.Web;
using KCSG.Core.Enumerations;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using Newtonsoft.Json;

namespace KCSG.Web.Services
{
    public class ConfigurationService : IConfigurationService
    {
        #region Properties

        /// <summary>
        ///     Find material device in configuration file.
        /// </summary>
        public string MaterialDeviceCode
        {
            get { return ConfigurationManager.AppSettings["MatDeviceCode"]; }
        }

        /// <summary>
        ///     Find Product device code in configuration file.
        /// </summary>
        public string ProductDeviceCode
        {
            get { return ConfigurationManager.AppSettings["PdtDeviceCode"]; }
        }

        /// <summary>
        ///     Find pre-product device code in configuration file.
        /// </summary>
        public string PreProductDeviceCode
        {
            get { return ConfigurationManager.AppSettings["PrePdtDeviceCode"]; }
        }

        /// <summary>
        ///     Find company name in configuration setting.
        /// </summary>
        public string CompanyName
        {
            get { return ConfigurationManager.AppSettings["CompanyName"]; }
        }

        /// <summary>
        ///     Find communication port name configured in system.
        /// </summary>
        public string CommunicationPortName
        {
            get
            {
                var communicationPortName = ConfigurationManager.AppSettings["CommunicationPortName"];
                if (string.IsNullOrEmpty(communicationPortName))
                    return "COM1";

                return communicationPortName;
            }
        }

        /// <summary>
        ///     Application data path.
        /// </summary>
        public string ApplicationDataPath
        {
            get { return HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ApplicationDataPath"]); }
        }

        /// <summary>
        ///     Find baudrate of communication port configured in system.
        /// </summary>
        public int CommunicationPortBaudRate
        {
            get
            {
                var configuredCommunicationPortBaudRate = ConfigurationManager.AppSettings["CommunicationPortBaudRate"];
                try
                {
                    var baudRate = int.Parse(configuredCommunicationPortBaudRate);
                    return baudRate;
                }
                catch
                {
                    return 0;
                }
            }
        }
        
        /// <summary>
        ///     Find communication port parity configured in system.
        /// </summary>
        public Parity CommunicationPortParity
        {
            get
            {
                var txtCommunicationPortParity = ConfigurationManager.AppSettings["CommunicationPortParity"];
                try
                {
                    // Cast to enum.
                    return (Parity) Enum.Parse(typeof(Parity), txtCommunicationPortParity);
                }
                catch
                {
                    return Parity.None;
                }
            }
        }

        /// <summary>
        ///     Find communication port databit configured in configuration file.
        /// </summary>
        public int CommunicationPortDataBits
        {
            get
            {
                var txtCommunicationPortDataBits = ConfigurationManager.AppSettings["CommunicationPortDataBits"];
                try
                {
                    // Cast to enum.
                    var dataBit = int.Parse(txtCommunicationPortDataBits);
                    return dataBit;
                }
                catch
                {
                    return 5;
                }
            }
        }

        /// <summary>
        ///     Find communication port stopbits in configuration file.
        /// </summary>
        public StopBits CommunicationPortStopBits
        {
            get
            {
                var txtCommunicationPortStopBits = ConfigurationManager.AppSettings["CommunicationPortStopBits"];
                try
                {
                    // Cast to enum.
                    return (StopBits) Enum.Parse(typeof(StopBits), txtCommunicationPortStopBits);
                }
                catch
                {
                    return StopBits.None;
                }
            }
        }
        
        /// <summary>
        /// List of screens in system.
        /// </summary>
        public Dictionary<string, AreaInformation> Areas { get; set; }

        /// <summary>
        /// Load configuration from specific file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        public T LoadConfigurationFromFile<T>(string relativeUrl)
        {
            // Find specific file url.
            var file = HttpContext.Current.Server.MapPath(relativeUrl);

            // File doesn't exist.
            if (!File.Exists(file))
                return default(T);

            // Read whole content of file.
            var content = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<T>(content);
        }
        
        #endregion
    }
}