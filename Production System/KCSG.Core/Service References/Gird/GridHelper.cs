using System.Web.Mvc;

namespace KCSG.jsGrid.MVC
{
    public static class GridHelper
    {
        public static Grid Grid(this HtmlHelper helper, Grid grid)
        {
            return grid;
        }
    }
}
