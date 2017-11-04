namespace KCSG.Web.Areas.ProductionPlanning.ViewModels.MasterList
{
    public class MasterListPrintViewModel
    {
        /// <summary>
        /// Product/material/Pre-product code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Which should be printed.
        /// </summary>
        public uint SearchBy { get; set; }
    }
}