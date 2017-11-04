using System.Windows;

namespace KCSG.AutomatedWarehouse.Model
{
    public class SystemViewMessage
    {
        /// <summary>
        /// Title of dialog.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Content of dialog.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Button which should be displayed in message box.
        /// </summary>
        public MessageBoxButton Button { get; set; } = MessageBoxButton.OK;

        /// <summary>
        /// Image of message box.
        /// </summary>
        public MessageBoxImage Image { get; set; } = MessageBoxImage.Error;

        /// <summary>
        /// Default result of message box.
        /// </summary>
        public MessageBoxResult Result { get; set; } = MessageBoxResult.OK;
    }
}