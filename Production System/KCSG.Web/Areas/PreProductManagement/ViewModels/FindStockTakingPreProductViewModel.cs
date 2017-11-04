using KCSG.jsGrid.MVC;

namespace KCSG.Web.Areas.PreProductManagement.ViewModels
{
    public class FindStockTakingPreProductViewModel
    {
        public int FromRow { get; set; }

        public int FromBay { get; set; }

        public int FromLevel { get; set; }

        public int ToRow { get; set; }

        public int ToBay { get; set; }

        public int ToLevel { get; set; }
    }
}