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
    using System.Collections.Generic;
    
    public partial class TM05_Conveyor
    {
        public string F05_ConveyorCode { get; set; }
        public string F05_TerminalNo { get; set; }
        public int F05_MaxBuffer { get; set; }
        public string F05_StrRtrSts { get; set; }
        public int F05_BufferUsing { get; set; }
        public string F05_UsingTerm { get; set; }
        public string F05_LineNo { get; set; }
        public string F05_ColorClass { get; set; }
        public System.DateTime F05_AddDate { get; set; }
        public System.DateTime F05_UpdateDate { get; set; }
        public Nullable<int> F05_UpdateCount { get; set; }
    }
}