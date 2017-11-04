using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;

namespace KCSG.Domain.Models.CommunicationManagement
{
   public class WeighingEquipmentViewSearchModel
    {
        public Constants.ViewSelectedC4 ViewSelect { get; set; }
        public string Status { get; set; }
        public string Model { get; set; }
        public Constants.SelectData SelectData { get; set; }
        public string CodeNo { get; set; }


        public string Date { get; set; }
        public Constants.Line Line { get; set; }

        public List<DateTime> SelectedRec { get; set; }
       public string TerminalNo { get; set; }
       public string KndCmdNo { get; set; }
       public string PrePdtLotNo { get; set; }
       public int LN { get; set; }
    }
}
