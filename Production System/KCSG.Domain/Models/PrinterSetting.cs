namespace KCSG.Domain.Models
{
    public class PrinterSetting
    {
        /// <summary>
        /// Network address of printer.
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Port of printer.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Whether printer is enabled or not.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Whether the printer is usb connected or not.
        /// </summary>
        public bool IsUsbPrinter { get; set; }

        /// <summary>
        /// Terminal which printer belongs to.
        /// </summary>
        public string Terminal { get; set; }
    }
}