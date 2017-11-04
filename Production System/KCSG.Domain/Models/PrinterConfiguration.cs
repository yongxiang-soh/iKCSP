using System.Collections.Generic;

namespace KCSG.Domain.Models
{
    public class PrinterConfiguration
    {
        #region Properties

        /// <summary>
        /// Internal printers list.
        /// </summary>
        public IList<PrinterSetting> InternalPrinters { get; set; }

        /// <summary>
        /// External printers list.
        /// </summary>
        public IList<PrinterSetting> ExternalPrinters { get; set; }

        /// <summary>
        /// Pre-product printers list.
        /// </summary>
        public IList<PrinterSetting> PreProductPrinters { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initiate instance with default settings.
        /// </summary>
        public PrinterConfiguration()
        {
            InternalPrinters = new List<PrinterSetting>();
            ExternalPrinters = new List<PrinterSetting>();
            PreProductPrinters = new List<PrinterSetting>();
        }

        #endregion
    }
}