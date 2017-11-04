using KCSG.Data.DataModel;

namespace KCSG.Domain.Models.ProductionPlanning
{
    public class PrePdtMkpItem : TM02_PrePdtMkp
    {
        public bool IsCreate { get; set; }
       
        public string OldThrawSeqNo { get; set; }
    }
}
