namespace KCSG.Web.Areas.KneadingCommand.ViewModels.InputOfKneadingCommand
{
    public class InitializeKneadingCommandInputViewModel
    {
        #region Properties

        /// <summary>
        /// Value selected from client-side.
        /// </summary>
        public string SelectedValue { get; set; }

        /// <summary>
        /// Time range within the current.
        /// </summary>
        public int Within { get; set; }

        /// <summary>
        /// Number of similar kneading commands which will be created on database.
        /// </summary>
        public int LotQuantity { get; set; }

        #endregion
    }
}