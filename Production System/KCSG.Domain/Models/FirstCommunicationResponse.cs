using KCSG.Data.DataModel;

namespace KCSG.Domain.Models
{
    public class FirstCommunicationResponse : TX34_MtrWhsCmd
    {
        public string OldStatus { get; set; }

        public string MaterialCode { get; set; }
        public string F34_PalletNo { get;  set; }
        public string F34_CommandNo  { get; set; }
        public FirstCommunicationResponse()
        {
            OldStatus = "";
            MaterialCode = "";
        }
    }
}