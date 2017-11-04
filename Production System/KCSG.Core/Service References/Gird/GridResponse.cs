using System.Collections.Generic;
using System.Linq;

namespace KCSG.jsGrid.MVC
{
    public class GridResponse<T>
    {
        public IEnumerable<T> data { get; set; }
        public int itemsCount { get; set; }


        public GridResponse(IEnumerable<T> data, int itemsCount)
        {
            this.data = data;
            this.itemsCount = itemsCount;
        }
        public GridResponse(IQueryable<T> data, int itemsCount)
        {
            this.data = data;
            this.itemsCount = itemsCount;
        }

        
    }
}
