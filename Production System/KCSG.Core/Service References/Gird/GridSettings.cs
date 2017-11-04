using System.Web.Mvc;
using KCSG.jsGrid.MVC.Enums;

namespace KCSG.jsGrid.MVC
{
    [ModelBinder(typeof(GridModelBinder))]
    public class GridSettings
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortField { get; set; }
        public SortOrder SortOrder { get; set; }
    }
}
