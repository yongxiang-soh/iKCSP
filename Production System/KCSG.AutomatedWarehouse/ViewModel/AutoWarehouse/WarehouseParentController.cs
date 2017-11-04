using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using GalaSoft.MvvmLight.Ioc;
using KCSG.AutomatedWarehouse.Enumeration;
using KCSG.AutomatedWarehouse.Model;
using KCSG.Data.DatabaseContext;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using log4net;

namespace KCSG.AutomatedWarehouse.ViewModel.AutoWarehouse
{
    public class WarehouseParentController
    {
        #region Properties

        /// <summary>
        ///     Commands queue which contains commands which have already been sent.
        /// </summary>
        protected IList<TerminalMessage> CommandsQueue { get; }

        /// <summary>
        /// Instance stores application settings.
        /// </summary>
        private SettingViewModel _setting;

        /// <summary>
        ///     Instance handles application message exchange & display.
        /// </summary>
        private MessageViewModel _message;
        
        /// <summary>
        ///     Instance for handling logging progress.
        /// </summary>
        private ILog _log;

        /// <summary>
        /// Find instance of setting in application.
        /// </summary>
        public SettingViewModel Setting
        {
            get
            {
                return _setting ?? (_setting = SimpleIoc.Default.GetInstance<SettingViewModel>());
            }
            set { _setting = value; }
        }

        /// <summary>
        ///     Instance for handling logging progress.
        /// </summary>
        public ILog Log
        {
            get { return _log ?? (_log = LogManager.GetLogger(typeof(MaterialAutoWarehouseController))); }
            set { _log = value; }
        }

        /// <summary>
        ///     Instance handles application message exchange & display.
        /// </summary>
        public MessageViewModel Message
        {
            get
            {
                if (_message == null)
                    _message = SimpleIoc.Default.GetInstance<MessageViewModel>();
                return _message;
            }
            set { _message = value; }
        }



        #endregion

        #region Constructor

        /// <summary>
        /// Initiate parent controller with default settings.
        /// </summary>
        public WarehouseParentController()
        {
            CommandsQueue = new List<TerminalMessage>();
        }

        #endregion

        #region Methods
        
        /// <summary>
        ///     Check whether terminal message is sendable or not.
        /// </summary>
        /// <param name="terminalMessage"></param>
        /// <returns></returns>
        public bool IsSendable(TerminalMessage terminalMessage)
        {
            // Time limit is not exceeded.
            if (DateTime.Now - terminalMessage.Sent < TimeSpan.FromMilliseconds(Setting.CommandTimeOut))
                return false;

            // Already acknowledged or the command has been sent out more than 3 times.
            if (terminalMessage.IsAck || terminalMessage.SentCount >= 3)
                return false;

            return true;
        }

        /// <summary>
        ///     Find command in queue.
        /// </summary>
        /// <param name="commandSeq"></param>
        /// <param name="commandIndex"></param>
        /// <returns></returns>
        public TerminalMessage FindCommandInQueue(string commandSeq, string commandIndex)
        {
            return CommandsQueue.FirstOrDefault(x => x.CommandSequence == commandSeq && x.CommandIndex == commandIndex);
        }

        /// <summary>
        /// Insert terminal message into queue if it doesn't exist in queue and so on.
        /// </summary>
        /// <param name="terminalMessage"></param>
        public void InsertOrUpdate(TerminalMessage terminalMessage)
        {
            // Find command in existing queue.
            var instance = FindCommandInQueue(terminalMessage.CommandSequence, terminalMessage.CommandIndex);

            // Command doesn't exist in queue.
            if (instance == null)
            {
                // Insert command in queue.
                CommandsQueue.Add(terminalMessage);
                return;
            }

            // Update the command.
            instance.CommandSequence = terminalMessage.CommandSequence;
            instance.CommandIndex = terminalMessage.CommandIndex;
            instance.Command = terminalMessage.Command;
            instance.Status = terminalMessage.Status;
            instance.From = terminalMessage.From;
            instance.To = terminalMessage.To;
            instance.PalletNo = terminalMessage.PalletNo;
            instance.Filter = terminalMessage.Filter;
            instance.Date = terminalMessage.Date;
            instance.Sent = terminalMessage.Sent;
            instance.IsAck = terminalMessage.IsAck;
            instance.SentCount = terminalMessage.SentCount;
        }

        /// <summary>
        /// Insert terminal message into queue.
        /// </summary>
        /// <param name="terminalMessage"></param>
        public void Insert(TerminalMessage terminalMessage)
        {
            CommandsQueue.Add(terminalMessage);
        }

        /// <summary>
        /// Delete terminal message from queue.
        /// </summary>
        /// <param name="terminalMessage"></param>
        public void Delete(TerminalMessage terminalMessage)
        {
            CommandsQueue.Remove(terminalMessage);
        }

        /// <summary>
        ///     Set automated warehouse to offline mode.
        /// </summary>
        /// <param name="deviceCode"></param>
        public void SetAutoWarehouseOffline(string deviceCode)
        {
            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                var device = unitOfWork.DeviceRepository.GetByDeviceCode(deviceCode);
                if (device != null)
                {
                    device.F14_DeviceStatus = "1";
                    device.F14_UpdateDate = DateTime.Now;
                    unitOfWork.DeviceRepository.Update(device);
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, "Autoware House is at error status..");
                }
                else
                {
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, "Set Device Status failed");
                }
                unitOfWork.Commit();
            }
        }
        
        /// <summary>
        ///     Check whether auto warehouse controller is enabled or not.
        /// </summary>
        /// <param name="autoWarehouseDevice"></param>
        /// <returns></returns>
        public bool IsAutoWarehouseOnline(string autoWarehouseDevice)
        {
            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                var autoWarehouseMaterialDevice = unitOfWork.DeviceRepository.GetById(autoWarehouseDevice);
                return AutoControllerStatus.Online.Equals(autoWarehouseMaterialDevice.F14_DeviceStatus);
            }
        }

        /// <summary>
        /// Clear the queue.
        /// </summary>
        public void ClearQueue()
        {
            CommandsQueue.Clear();
        }

        /// <summary>
        /// Remove command from cache.
        /// </summary>
        /// <param name="commandSequence"></param>
        /// <param name="commandIndex"></param>
        public void RemoveCommand(string commandSequence, string commandIndex)
        {
            var command =
                CommandsQueue.FirstOrDefault(
                    x =>
                        x.CommandSequence.Equals(commandSequence, StringComparison.InvariantCultureIgnoreCase) &&
                        x.CommandIndex.Equals(commandIndex, StringComparison.InvariantCultureIgnoreCase));
            if (command != null)
                CommandsQueue.Remove(command);
        }

        /// <summary>
        /// Remove command by using specific condition.
        /// </summary>
        /// <param name="condition"></param>
        public void RemoveCommand(Func<TerminalMessage, bool> condition)
        {
            var command = CommandsQueue.FirstOrDefault(condition);
            if (command != null)
                CommandsQueue.Remove(command);
        }
        
        #endregion
    }
}