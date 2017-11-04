using System.Collections.Generic;

namespace KCSG.AutomatedWarehouse.Model
{
    public class TerminalConfiguration
    {
        /// <summary>
        /// List of terminal in configuration files.
        /// </summary>
        public IDictionary<string, TerminalSetting> Terminals { get; set; }

        /// <summary>
        /// Default selected terminal.
        /// </summary>
        public string DefaultTerminal { get; set; }



    }
}