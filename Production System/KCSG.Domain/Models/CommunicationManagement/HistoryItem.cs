using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models
{
    public class HistoryItem
    {
        public string CommandNo { get; set; }
        public string CommandSeqNo { get; set; }
        public string CommandType { get; set; }
        public int Priority { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime? CommandDate { get; set; }
        public string PalletNo { get; set; }
        public string AbnormalCode { get; set; }
        public DateTime AddDate { get; set; }

    }
}
