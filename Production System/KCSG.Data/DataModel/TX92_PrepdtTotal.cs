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
    
    public partial class TX92_PrepdtTotal
    {
        public System.DateTime F92_YearMonth { get; set; }
        public string F92_PrepdtCode { get; set; }
        public double F92_PrevRemainder { get; set; }
        public double F92_Received { get; set; }
        public double F92_Used { get; set; }
        public Nullable<System.DateTime> F92_AddDate { get; set; }
        public Nullable<System.DateTime> F92_UpdateDate { get; set; }
    }
}