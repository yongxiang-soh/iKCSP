using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Core.Models
{
    public class GetPagingRequest
    {
        public string Draw { get; set; }
        public string Start { get; set; }
        public string Length { get; set; }
        public string SortColumn { get; set; }
        public string SortColumnDir { get; set; }
        public string Keyword { get; set; }

        public List<KeyValuePair<string, string>> ListKeywords { get; set; }
    }
}
