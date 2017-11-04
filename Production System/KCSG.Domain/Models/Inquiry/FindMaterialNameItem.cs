using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.Inquiry
{
    public class FindMaterialNameItem
    {
        public string F42_KndCmdNo { get; set; }

        public string F42_PreProductCode { get; set; }

        public string F03_PreProductName { get; set; }

        /// <summary>
        /// Product status
        /// </summary>
        public string ProductStatus { get; set; }

        /// <summary>
        /// Product lot number.
        /// </summary>
        public string LotNo { get; set; }

        public string F39_ColorClass { get; set; }

        /// <summary>
        /// Kneading status
        /// </summary>
        public string KneadingStatus { get; set; }


        /// <summary>
        /// Updated Date 1 (hidden column)
        /// </summary>
        public DateTime UpdatedDate1 { get; set; }

        /// <summary>
        /// Updated date 2 (hidden column)
        /// </summary>
        public DateTime UpdateDate2 { get; set; }

        /// <summary>
        /// Production date
        /// </summary>
        public DateTime ProductionDate { get; set; }

        public int F42_CommandSeqNo { get; set; }

        public int F42_LotSeqNo { get; set; }
    }
}
