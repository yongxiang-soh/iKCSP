//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KCSG.Data.DataModel
{
    using System;
    
    public partial class GetPdtShfSts_Result
    {
        public string F51_ShelfRow { get; set; }
        public string F51_ShelfBay { get; set; }
        public string F51_ShelfLevel { get; set; }
        public string F51_ShelfStatus { get; set; }
        public string F51_ShelfType { get; set; }
        public Nullable<int> F51_CmdShfAgnOrd { get; set; }
        public Nullable<int> F51_LowTmpShfAgnOrd { get; set; }
        public int F51_LoadAmount { get; set; }
        public string F51_StockTakingFlag { get; set; }
        public string F51_PalletNo { get; set; }
        public string F51_TerminalNo { get; set; }
        public Nullable<System.DateTime> F51_StorageDate { get; set; }
        public Nullable<System.DateTime> F51_RetrievalDate { get; set; }
        public System.DateTime F51_AddDate { get; set; }
        public System.DateTime F51_UpdateDate { get; set; }
        public Nullable<int> F51_UpdateCount { get; set; }
    }
}
