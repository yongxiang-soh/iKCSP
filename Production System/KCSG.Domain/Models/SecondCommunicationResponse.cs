using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Models
{
   public class SecondCommunicationResponse:TX50_PrePdtWhsCmd
    {
        /// <summary>
        /// Product which has been affected.
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Old status.
        /// </summary>
        public string OldStatus { get; set; }

     
        /// <summary>
        /// Warehouse item.
        /// </summary>
        public object ProductWarehouseItem { get; set; }

       /// <summary>
       /// Pre Product Code
       /// </summary>
        public string PreProductCode { get; set; }
    }
}
