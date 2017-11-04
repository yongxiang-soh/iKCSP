using System;
using KCSG.Data.DataModel;

namespace KCSG.AutomatedWarehouse.Model
{
    public class TerminalMessage
    {
        #region Properties

        /// <summary>
        /// Command sequence.
        /// </summary>
        public string CommandSequence { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        public string CommandIndex { get; set; }

        /// <summary>
        /// Command type.
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// From
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// To
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Pallet which should be stored, retrieved.
        /// </summary>
        public string PalletNo { get; set; }

        /// <summary>
        /// Filter of command.
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// When command was created.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// When command was sent to external terminal.
        /// </summary>
        public DateTime Sent { get; set; }

        /// <summary>
        /// Whether message has been acknowledeged or not.
        /// </summary>
        public bool IsAck { get; set; }

        /// <summary>
        /// How many time this command has been sent to external terminal.
        /// </summary>
        public int SentCount { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initiate message with default settings.
        /// </summary>
        public TerminalMessage()
        {
            CommandSequence = new string(' ', 4);
            CommandIndex = new string(' ', 4);
            Command = new string(' ', 4);
            Status = new string(' ', 4);
            From = new string(' ', 6);
            To = new string(' ', 6);
            PalletNo = new string(' ', 4);
            Filter = new string(' ', 18);
            Date = DateTime.Now;
        }

        /// <summary>
        /// Initiate message with material warehouse command.
        /// </summary>
        /// <param name="materialWarehouseCommand"></param>
        public TerminalMessage(TX34_MtrWhsCmd materialWarehouseCommand) : this()
        {
            CommandSequence = materialWarehouseCommand.F34_CmdSeqNo.PadRight(4);
            CommandIndex = materialWarehouseCommand.F34_CommandNo.PadRight(4);
            Command = materialWarehouseCommand.F34_CommandType.PadRight(4);
            From = materialWarehouseCommand.F34_From.PadRight(6);
            To = materialWarehouseCommand.F34_To.PadRight(6);

            if (string.IsNullOrEmpty(materialWarehouseCommand.F34_PalletNo))
                PalletNo = "";
            else
                PalletNo = materialWarehouseCommand.F34_PalletNo;

            PalletNo = PalletNo.PadRight(4);
        }

        /// <summary>
        /// Initiate message with material warehouse command.
        /// </summary>
        /// <param name="productWarehouseCommand"></param>
        public TerminalMessage(TX47_PdtWhsCmd productWarehouseCommand) : this()
        {
            CommandSequence = productWarehouseCommand.F47_CmdSeqNo.PadRight(4);
            CommandIndex = productWarehouseCommand.F47_CommandNo.PadRight(4);
            Command = productWarehouseCommand.F47_CommandType.PadRight(4);
            From = productWarehouseCommand.F47_From.PadRight(6);
            To = productWarehouseCommand.F47_To.PadRight(6);

            if (string.IsNullOrEmpty(productWarehouseCommand.F47_PalletNo))
                PalletNo = "";
            else
                PalletNo = productWarehouseCommand.F47_PalletNo;

            PalletNo = PalletNo.PadRight(4);
        }

        public TerminalMessage(TX50_PrePdtWhsCmd preProductWarehouseCommand) : this()
        {
            if (preProductWarehouseCommand == null)
                return;

            if (string.IsNullOrEmpty(preProductWarehouseCommand.F50_CmdSeqNo))
                CommandSequence = " ".PadRight(4);
            else
                CommandSequence = preProductWarehouseCommand.F50_CmdSeqNo.PadRight(4);
            CommandIndex = preProductWarehouseCommand.F50_CommandNo.PadRight(4);
            Command = preProductWarehouseCommand.F50_CommandType.PadRight(4);
            From = preProductWarehouseCommand.F50_From.PadRight(6);
            To = preProductWarehouseCommand.F50_To.PadRight(6);
        }

        #endregion

        #region Methods

        /// <summary>
        /// From parameters to message which will be sent to external terminal.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return
                $"{CommandSequence}{CommandIndex}{Command}{Status}{From}{To}{PalletNo}{Filter}{Date.ToString("yyyyMMddHHmmss")}";
        }

        /// <summary>
        /// Parse message into parameters.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="throwException">Whether null should be returned or exception should be thrown.</param>
        public static TerminalMessage Parse(string message, bool throwException)
        {
            try
            {
                var terminalMessage = new TerminalMessage();
                terminalMessage.CommandSequence = message.Substring(0, 4);
                terminalMessage.CommandIndex = message.Substring(4, 4);
                terminalMessage.Command = message.Substring(8, 4);
                terminalMessage.Status = message.Substring(12, 4);
                terminalMessage.From = message.Substring(16, 6);
                terminalMessage.To = message.Substring(22, 6);
                terminalMessage.PalletNo = message.Substring(28, 4);
                terminalMessage.Filter = message.Substring(32, 18);
                return terminalMessage;
            }
            catch
            {
                if (throwException)
                    throw new Exception("Terminal message is not correct.");
                return null;
            }
        }


        #endregion
    }
}