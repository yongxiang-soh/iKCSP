using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.ManagementReport
{
    public class FindPrintMaterialNameItem
    {
        public string MaterialCode { get; set; }

        public string MaterialName { get; set; }        
        public string PreProductName { get; set; }        
        public string PreProductCode { get; set; }
        public string CF { get; set; }
        public DateTime? SDate1 { get; set; }
        public string SDate { get; set; }
        public string Stime { get; set; }
        public string LotNo { get; set; }

        public string PalletNo { get; set; }

        public string RBL { get; set; }

        public double Quantity { get; set; }
        public string Quantityst { get; set; }
        
        
    }
    public class FindPrintMaterialNameGroupItem
    {

        public string Total { get; set; }
        public IList<FindPrintMaterialNameItem> FindPrintMaterialNameItem { get; set; }
        public IList<FindPrintMaterialSheifItem> FindPrintMaterialSheifItem { get; set; }
        public IList<SubFindPrintMaterialNameItem> SubFindPrintMaterialNameItem { get; set; }
        public IList<SubReceivedConsumedMaterialItem> SubReceivedConsumedMaterialItem { get; set; }
        public IList<SubReceivedConsumedPreproductItem> SubReceivedConsumedPreproductItem { get; set; }
        public IList<SubReceivedConsumedProductItem> SubReceivedConsumedProductItem { get; set; }


    }

    public class SubFindPrintMaterialNameItem
    {
        public string SupplementaryMaterialCode { get; set; }

        public string SupplementaryMaterialName { get; set; }

        public double PackingUnit { get; set; }
        public string PackingUnitst { get; set; }
        

        public string Unit { get; set; }

        public double Quantity { get; set; }
        public string Quantityst { get; set; }


    }
    public class SubReceivedConsumedMaterialItem
    {
        public string MaterialCode { get; set; }

        public string MaterialName { get; set; }

        public double Remain { get; set; }
        

        public double Used { get; set; }

        public double Recieved { get; set; }
        public double Remaincurr { get; set; }
        public string Remainst { get; set; }


        public string Usedst { get; set; }

        public string Receivedst { get; set; }
        public string Remaincurrst { get; set; }
        public DateTime? Updatedate { get; set; }
        public string Updatedatest { get; set; }
        public DateTime? YearMonth { get; set; }
        public string YearMonthst { get; set; }


    }
    public class SubReceivedConsumedPreproductItem
    {
        public string PreProductCode { get; set; }

        public string PreProductName { get; set; }

        public double Remain { get; set; }
        

        public double Used { get; set; }

        public double Recieved { get; set; }
        public double Remaincurr { get; set; }
        public string Remainst { get; set; }


        public string Usedst { get; set; }

        public string Recievedst { get; set; }
        public string Remaincurrst { get; set; }
        public DateTime? Updatedate { get; set; }
        public string Updatedatest { get; set; }
        public DateTime? YearMonth { get; set; }
        public string YearMonthst { get; set; }


    }

    public class SubReceivedConsumedProductItem
    {
        public string ProductCode { get; set; }

        public string ProductName { get; set; }

        public double Remain { get; set; }


        public double Used { get; set; }

        public double Recieved { get; set; }
        public double Remaincurr { get; set; }
        public string Remainst { get; set; }


        public string Usedst { get; set; }

        public string Recievedst { get; set; }
        public string Remaincurrst { get; set; }
        public DateTime? Updatedate { get; set; }
        public string Updatedatest { get; set; }
        public DateTime? YearMonth { get; set; }
        public string YearMonthst { get; set; }


    }
    public partial class TX91MaterialTotal
    {
        public System.DateTime F91_YearMonth { get; set; }
        public string F91_MaterialCode { get; set; }
        public double F91_PrevRemainder { get; set; }
        public double F91_Received { get; set; }
        public double F91_Used { get; set; }
        public Nullable<System.DateTime> F91_AddDate { get; set; }
        public Nullable<System.DateTime> F91_UpdateDate { get; set; }
    }
    public partial class TX92PreProductTotal
    {
        public System.DateTime F92_YearMonth { get; set; }
        public string F92_PreProductCode { get; set; }
        public double F92_PrevRemainder { get; set; }
        public double F92_Received { get; set; }
        public double F92_Used { get; set; }
        public Nullable<System.DateTime> F92_AddDate { get; set; }
        public Nullable<System.DateTime> F92_UpdateDate { get; set; }
    }
    public partial class TX93ProductTotal
    {
        public System.DateTime F93_YearMonth { get; set; }
        public string F93_ProductCode { get; set; }
        public double F93_PrevRemainder { get; set; }
        public double F93_Received { get; set; }
        public double F93_Used { get; set; }
        public Nullable<System.DateTime> F93_AddDate { get; set; }
        public Nullable<System.DateTime> F93_UpdateDate { get; set; }
    }
    public class FindPrintMaterialSheifItem:FindPrintMaterialNameItem
    {
        public string PONo { get; set; }

        public string PD { get; set; }

        public DateTime? SDate1 { get; set; }
        public string SDate { get; set; }


        public string Stime { get; set; }

        public string BF { get; set; }
        public string AF { get; set; }


    }
}
