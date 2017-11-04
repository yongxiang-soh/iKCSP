using System;
using System.Configuration;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using KCSG.AutomatedWarehouse.Enumeration;
using KCSG.Data.DatabaseContext;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using KCSG.AutomatedWarehouse.Model;
using log4net;
using Newtonsoft.Json;
using KCSG.Core.Models;

namespace KCSG.AutomatedWarehouse.ViewModel.AutoWarehouse
{
    public class AutoControllerViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Product auto warehouse device code.
        /// </summary>
        private string _productAutoWarehouseDeviceCode;

        /// <summary>
        /// PreProduct auto warehouse device code.
        /// </summary>
        private string _preProductAutoWarehouseDeviceCode;

        /// <summary>
        /// Material auto warehouse device code.
        /// </summary>
        private string _materialAutoWarehouseDeviceCode;

        /// <summary>
        /// Logging instance.
        /// </summary>
        private ILog _log;

        /// <summary>
        /// Logging instance.
        /// </summary>
        public ILog Log
        {
            get
            {
                return _log ?? (_log = LogManager.GetLogger(typeof(AutoControllerViewModel)));
            }
            set
            {
                _log = value;
            }
        }
        /// <summary>
        ///     Instance handles message business.
        /// </summary>
        private MessageViewModel _message;

        /// <summary>
        /// Instance handles material auto warehouse controller business.
        /// </summary>
        private MaterialAutoWarehouseController _materialAutoWarehouseController;

        /// <summary>
        ///     Instance which handles message business.
        /// </summary>
        public MessageViewModel Message
        {
            get
            {
                if (_message == null)
                    _message = SimpleIoc.Default.GetInstance<MessageViewModel>();

                return _message;
            }
        }

        /// <summary>
        /// Instance handles material auto warehouse controller business.
        /// </summary>
        public MaterialAutoWarehouseController MaterialAutoWarehouseController
        {
            get
            {
                if (_materialAutoWarehouseController == null)
                    _materialAutoWarehouseController = SimpleIoc.Default.GetInstance<MaterialAutoWarehouseController>();
                return _materialAutoWarehouseController;
            }
            set { _materialAutoWarehouseController = value; }
        }

        /// <summary>
        /// Product auto warehouse device code.
        /// </summary>
        public string ProductAutoWarehouseDeviceCode
        {
            get
            {
                if (string.IsNullOrEmpty(_productAutoWarehouseDeviceCode))
                    _productAutoWarehouseDeviceCode =
                        ConfigurationManager.AppSettings["ProductAutoControllerDeviceCode"];

                return _productAutoWarehouseDeviceCode;
            }
            set { _productAutoWarehouseDeviceCode = value; }
        }

        /// <summary>
        /// Pre-product auto warehouse device code.
        /// </summary>
        public string PreProductAutoWarehouseDeviceCode
        {
            get
            {
                if (string.IsNullOrEmpty(_preProductAutoWarehouseDeviceCode))
                    _preProductAutoWarehouseDeviceCode =
                        ConfigurationManager.AppSettings["PreProductAutoWarehouseDeviceCode"];
                return _preProductAutoWarehouseDeviceCode;
            }
            set { _preProductAutoWarehouseDeviceCode = value; }
        }

        /// <summary>
        /// Material auto warehouse device code.
        /// </summary>
        public string MaterialAutoWarehouseDeviceCode
        {
            get
            {
                if (string.IsNullOrEmpty(_materialAutoWarehouseDeviceCode))
                    _materialAutoWarehouseDeviceCode = ConfigurationManager.AppSettings["MaterialDeviceCode"];
                return _materialAutoWarehouseDeviceCode;
            }
            set { _materialAutoWarehouseDeviceCode = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Find the automated material warehouse command with highest priority.
        /// </summary>
        /// <returns></returns>
        public IList<TX34_MtrWhsCmd> FindPendingMaterialCommands()
        {
            // Initiate unit of work instance.
            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                var pendingCommands = unitOfWork.MaterialWarehouseCommandRepository.GetAll();
                pendingCommands = pendingCommands.Where(i => i.F34_Status.Trim().Equals("0") ||
                                                             i.F34_Status.Trim().Equals("1") ||
                                                             i.F34_Status.Trim().Equals("2") ||
                                                             i.F34_Status.Trim().Equals("4") ||
                                                             i.F34_Status.Trim().Equals("5"))
                    .OrderByDescending(i => i.F34_Priority)
                    .ThenByDescending(i => i.F34_Status);

                return pendingCommands.ToList();
            }
        }

        /// <summary>
        ///     Find the automated product warehouse command with highest priority.
        /// </summary>
        /// <returns></returns>
        public List<TX47_PdtWhsCmd> FindPendingProductCommands()
        {
            // Initiate unit of work instance.
            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                var autoWarehouseProduct = ProductAutoWarehouseDeviceCode;
                if (autoWarehouseProduct == null)
                    return null;

                var autoWarehouseProductDevice = unitOfWork.DeviceRepository.GetById(autoWarehouseProduct);
                if (!"0".Equals(autoWarehouseProductDevice.F14_DeviceStatus))
                    throw new Exception("Automated warehouse product is offline.");

                var pendingCommands = unitOfWork.ProductWarehouseCommandRepository.GetAll();
                pendingCommands = pendingCommands.Where(i => i.F47_Status.Trim().Equals("0") ||
                                                             i.F47_Status.Trim().Equals("1") ||
                                                             i.F47_Status.Trim().Equals("2") ||
                                                             i.F47_Status.Trim().Equals("4") ||
                                                             i.F47_Status.Trim().Equals("5"))
                    .OrderByDescending(i => i.F47_Priority)
                    .ThenByDescending(i => i.F47_Status);

                return pendingCommands.ToList();
            }
        }

        /// <summary>
        ///     Find the automated pre-product warehouse command with the highest priority.
        /// </summary>
        /// <returns></returns>
        public IList<TX50_PrePdtWhsCmd> FindPendingPreProductCommands()
        {
            // Initiate unit of work instance.
            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                var autoWarehousePreProduct = PreProductAutoWarehouseDeviceCode;
                if (autoWarehousePreProduct == null)
                    return null;

                var autoWarehousePreProductDevice = unitOfWork.DeviceRepository.GetById(autoWarehousePreProduct);
                if (!"0".Equals(autoWarehousePreProductDevice.F14_DeviceStatus))
                    throw new Exception("Automated warehouse pre-product is offline.");

                var pendingCommands = unitOfWork.PreProductWarehouseCommandRepository.GetAll();
                pendingCommands = pendingCommands.Where(i => i.F50_Status.Trim().Equals("0") ||
                                                             i.F50_Status.Trim().Equals("1") ||
                                                             i.F50_Status.Trim().Equals("2") ||
                                                             i.F50_Status.Trim().Equals("4") ||
                                                             i.F50_Status.Trim().Equals("5"))
                    .OrderByDescending(i => i.F50_Priority)
                    .ThenByDescending(i => i.F50_Status);
                return pendingCommands.ToList();
            }
        }
        
        /// <summary>
        /// Send http message to web server.
        /// </summary>
        /// <param name="signalrMessageViewModel"></param>
        /// <returns></returns>
        public async Task SendHttpMessage(object signalrMessageViewModel)
        {
            var url = ConfigurationManager.AppSettings["AutowarehouseMessageLink"];
            if (string.IsNullOrWhiteSpace(url))
                return;

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var uri = new Uri(url);
                    var content = new StringContent(JsonConvert.SerializeObject(signalrMessageViewModel), Encoding.UTF8, "application/json");
                    await httpClient.PostAsync(uri, content);
                }
            }
            catch (Exception exception)
            {
                // Suppress exception message, then log error to file for further diagnostics.
                Log.Error(exception);
            }

        }

        #endregion
    }
}