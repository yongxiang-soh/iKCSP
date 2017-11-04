using System.Collections.Generic;
using System.Net;
using KCSG.Core.Constants;
using KCSG.Domain.Models;

namespace KCSG.Domain.Interfaces
{
    public interface ILabelPrintService
    {
        #region Properties

        /// <summary>
        /// List of external printers in the system.
        /// </summary>
        IList<PrinterSetting> ExternalPrinters { get; set; }

        /// <summary>
        /// 
        /// </summary>
        IList<PrinterSetting> InternalPrinters { get; set; }

        /// <summary>
        /// List of pre-product printer.
        /// </summary>
        IList<PrinterSetting> PreProductPrinters { get; set; }

        /// <summary>
        /// List of internal labels with their content.
        /// </summary>
        Dictionary<string, string> InternalLabelOriginalContent { get; set; }
        
        /// <summary>
        /// List of external labels with their content.
        /// </summary>
        Dictionary<string, string> ExternalLabelOriginalContent { get; set; }

        /// <summary>
        /// Pre-product original text.
        /// </summary>
        string PreProductLabelOrignalText { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Send print command to external
        /// </summary>
        /// <param name="labelPrintEndPoint"></param>
        /// <param name="command"></param>
        void Print(IPEndPoint labelPrintEndPoint, string command);

        /// <summary>
        /// Find content which can be printed by using label name.
        /// </summary>
        /// <param name="labelName"></param>
        /// <returns></returns>
        string FindInternalPrintContent(string labelName);

        /// <summary>
        /// Find content which can be printed by using label name.
        /// </summary>
        /// <param name="labelName"></param>
        /// <returns></returns>
        string FindExternalPrintContent(string labelName);
        
        #endregion
    }
}