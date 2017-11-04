using System.Collections.Generic;
using System.IO.Ports;
using KCSG.Core.Enumerations;
using KCSG.Domain.Models;

namespace KCSG.Domain.Interfaces.Services
{
    public interface IConfigurationService
    {
        #region Configurations

        /// <summary>
        /// Find material device code.
        /// </summary>
        /// <returns></returns>
        string MaterialDeviceCode { get; }

        /// <summary>
        /// Find Pre-product device code.
        /// </summary>
        /// <returns></returns>
        string PreProductDeviceCode { get; }

        /// <summary>
        /// Find product device code.
        /// </summary>
        /// <returns></returns>
        string ProductDeviceCode { get; }

        /// <summary>
        /// Name of company.
        /// </summary>
        string CompanyName { get; }

        string ApplicationDataPath { get; }
        
        /// <summary>
        /// List of screens in system.
        /// </summary>
        Dictionary<string, AreaInformation> Areas { get; set; }

        #endregion

        #region Communication port

        /// <summary>
        /// Name of communication should be used for connecting with weighing terminal.
        /// </summary>
        string CommunicationPortName { get; }

        /// <summary>
        /// Baurate of communication port.
        /// </summary>
        int CommunicationPortBaudRate { get; }

        /// <summary>
        /// Parity of communication port
        /// </summary>
        Parity CommunicationPortParity { get; }

        /// <summary>
        /// Communication port databits.
        /// </summary>
        int CommunicationPortDataBits { get; }

        /// <summary>
        /// Communication port stop bits.
        /// </summary>
        StopBits CommunicationPortStopBits { get;}

        #endregion


        #region Methods

        /// <summary>
        /// Load configuration from specific file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        T LoadConfigurationFromFile<T>(string relativeUrl);

        #endregion
    }
}