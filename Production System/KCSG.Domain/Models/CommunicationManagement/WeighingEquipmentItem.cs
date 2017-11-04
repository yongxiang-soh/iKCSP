using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.CommunicationManagement
{
    public class WeighingEquipmentItem
    {
        public DateTime Date { get; set; }
        public string Terminal { get; set; }
        public string Class { get; set; }
        public string Status { get; set; }
        public string MasterCode { get; set; }
        public string ErrorCode { get; set; }
        public string MaterialCode { get; set; }
        public string PalletNo { get; set; }
        public string SendFlag { get; set; }
        public string AbnormalCode { get; set; }
        public string CommandNo { get; set; }
        public string PreproductLotNo { get; set; }
        public string PreproductCode { get; set; }
        public string MaterialLotNo { get; set; }
        public string KneadingLine { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Sequence { get; set; }
        public double ChargedQty { get; set; }
    }
}
