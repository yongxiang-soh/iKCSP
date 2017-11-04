using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.CommunicationManagement
{
    internal class MessageC4Item
    {
        public string stx { get; set; }
        public string mtype { get; set; }
        public string mcno { get; set; }
        public string command { get; set; }
        public string textdata { get; set; }
        public string bcc { get; set; }
        public string etx { get; set; }
    }
}
