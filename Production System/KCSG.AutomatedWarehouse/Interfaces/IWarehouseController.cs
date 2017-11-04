using KCSG.AutomatedWarehouse.Model;

namespace KCSG.AutomatedWarehouse.Interfaces
{
    public interface IWarehouseController
    {
        #region Methods

        /// <summary>
        /// Procceed command sent from external terminal.
        /// </summary>
        /// <param name="terminalMessage"></param>
        /// <param name="deviceCode"></param>
        void ProceedIncommingCommand(TerminalMessage terminalMessage, string deviceCode);

        /// <summary>
        /// Whether terminal message can be sent out to external terminal or not.
        /// </summary>
        /// <param name="terminalMessage"></param>
        /// <returns></returns>
        bool IsSendable(TerminalMessage terminalMessage);

        /// <summary>
        /// Find the terminal message in commands queue.
        /// </summary>
        /// <param name="commandSeq"></param>
        /// <param name="commandIndex"></param>
        /// <returns></returns>
        TerminalMessage FindCommandInQueue(string commandSeq, string commandIndex);

        /// <summary>
        /// Insert terminal message into queue if it doesn't exist in queue and so on.
        /// </summary>
        /// <param name="terminalMessage"></param>
        void InsertOrUpdate(TerminalMessage terminalMessage);

        /// <summary>
        /// Set conveyor status by using specific information.
        /// </summary>
        /// <param name="conveyorCode"></param>
        /// <param name="nStatus"></param>
        /// <returns></returns>
        int SetConveyorStatus(string conveyorCode, int nStatus);

        /// <summary>
        /// Insert terminal message into queue.
        /// </summary>
        /// <param name="terminalMessage"></param>
        void Insert(TerminalMessage terminalMessage);

        /// <summary>
        /// Delete terminal message from queue.
        /// </summary>
        /// <param name="terminalMessage"></param>
        void Delete(TerminalMessage terminalMessage);

        /// <summary>
        /// Turn off aw.
        /// </summary>
        /// <param name="deviceCode"></param>
        void SetAutoWarehouseOffline(string deviceCode);

        /// <summary>
        ///     Check whether auto warehouse controller is enabled or not.
        /// </summary>
        /// <param name="autoWarehouseDevice"></param>
        /// <returns></returns>
        bool IsAutoWarehouseOnline(string autoWarehouseDevice);

        /// <summary>
        /// Update stock flag.
        /// </summary>
        /// <param name="lpPallet"></param>
        /// <param name="nFlag"></param>
        /// <returns></returns>
        int SetStockFlag(string lpPallet, int nFlag);

        #endregion

    }
}