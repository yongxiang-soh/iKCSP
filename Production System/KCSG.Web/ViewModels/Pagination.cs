using KCSG.jsGrid.MVC.Enums;

namespace KCSG.Web.ViewModels
{
    public class Pagination
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string SortField { get; set; } = "";

        public SortOrder SortOrder { get; set; } = SortOrder.Asc;
    }
}