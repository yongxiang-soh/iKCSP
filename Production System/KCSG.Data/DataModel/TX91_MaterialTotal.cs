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
    
    public partial class TX91_MaterialTotal
    {
        public System.DateTime F91_YearMonth { get; set; }
        public string F91_MaterialCode { get; set; }
        public double F91_PrevRemainder { get; set; }
        public double F91_Received { get; set; }
        public double F91_Used { get; set; }
        public Nullable<System.DateTime> F91_AddDate { get; set; }
        public Nullable<System.DateTime> F91_UpdateDate { get; set; }
    }
}
