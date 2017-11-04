using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.CommunicationManagement
{
    public class MessageFormAW
    {
        public string Sequence { get; set; }
        public string Id { get; set; }
        public string Command { get; set; }
        public string Status { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string PalletNo { get; set; }
        public string Filter { get; set; }
        public MessageFormAW(string message)
        {
            Sequence = message.Substring(0, 4);
            Id = message.Substring(4, 4);
            Command = message.Substring(8, 4);
            Status = message.Substring(12, 4);
            From = message.Substring(16, 6);
            To = message.Substring(22, 6);
            PalletNo = message.Substring(28, 4);
            Filter = message.Substring(32, 18);
        }

       
    }
}
