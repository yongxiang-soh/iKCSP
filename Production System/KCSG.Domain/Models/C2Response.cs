using System.Collections.Generic;

namespace KCSG.Domain.Models
{
    public class C2Response
    {
        public string Status { get; set; }

        public IList<string> Messages { get; set; }

        public C2Response()
        {
            Messages = new List<string>();
        }
    }
}