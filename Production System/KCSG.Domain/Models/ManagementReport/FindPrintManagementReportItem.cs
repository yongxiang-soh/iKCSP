using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.ManagementReport
{
    public class FindPrintManagementReportItem
    {
        public string PreProductCode { get; set; }

        public string OutsidePreProductCode { get; set; }
        public string RBL { get; set; }
        public string Conveyor { get; set; }
        public double Lose { get; set; }
        public string Losest { get; set; }
        public double test1 { get; set; }
        public double Fraction { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string SRF { get; set; }
        public string SID { get; set; }
        public string CmdID { get; set; }
        public string CmdType { get; set; }
        public string CmdNo { get; set; }
        public string CmdEndDate { get; set; }
        public string TerminalNo { get; set; }
        public string PictureNo { get; set; }
        public string AbnormalCode { get; set; }
        public string SRT { get; set; }

        public string PreProductName { get; set; }
        public string ShippingNo { get; set; }
        public string Delivery { get; set; }
        public string EndUserCode { get; set; }
        public string EndUserName { get; set; }
        public string RetrievalDate { get; set; }
        public int BatchSeqNo { get; set; }
        public int BatchLot { get; set; }

        public string PalletNo { get; set; }
        public string PalletClass { get; set; }
        public string SupplierCode { get; set; }
        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public string MaterialLotNo { get; set; }
        public double ReceiveQuantity { get; set; }
        public string ReceiveQuantityst { get; set; }
        public double ValidQuantity { get; set; }
        public string ValidQuantity1 { get; set; }
        public string LotNo { get; set; }
        public string LotNochild { get; set; }
        public string PONo { get; set; }
        public string PD { get; set; }
        public double PU { get; set; }
        public string PUstring { get; set; }
        public string Unit { get; set; }
        public string CF { get; set; }

        public string ContainerCode { get; set; }
        public string ContainerType { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductionLine { get; set; }
        public string ContainerNo { get; set; }
        public string PreProductLotNo { get; set; }
        public DateTime? SDate1 { get; set; }
        public string SDate1st { get; set; }
        public string SDate2st { get; set; }
        public DateTime? SDate2 { get; set; }

        public string SDate { get; set; }
        public string Stime { get; set; }
        public string OutsidePreProductLotNo { get; set; }
        public string Status { get; set; }

        public string ShelfNo { get; set; }
        public double Quantity { get; set; }
        public string Quantityst { get; set; }
        public int? Quantityint { get; set; }
        public string LoadQuantity { get; set; }
        public double Amount { get; set; }
        public double Total { get; set; }
        public string Totalst { get; set; }
        public double Used { get; set; }
        public double Remain { get; set; }
        public double Received { get; set; }
        public string Usedst { get; set; }
        public string Remainst { get; set; }
        public string Remaincurrst { get; set; }
        public string Receivedst { get; set; }
        public double? TotalGroup { get; set; }
        public string TotalGroupst { get; set; }
        public double Schagre { get; set; }
        public string TotalSchagre { get; set; }
        public double? LayingAmuont { get; set; }
        //region var for 5x6item uc 13
        public string r11 { get; set; }
        public string r12 { get; set; }
        public string r13 { get; set; }

        public string r14 { get; set; }
        public string r15 { get; set; }
        public string r16 { get; set; }
        public string r21 { get; set; }
        public string r22 { get; set; }
        public string r23 { get; set; }

        public string r24 { get; set; }
        public string r25 { get; set; }
        public string r26 { get; set; }

        public string r31 { get; set; }
        public string r32 { get; set; }
        public string r33 { get; set; }

        public string r34 { get; set; }
        public string r35 { get; set; }
        public string r36 { get; set; }

        public string r41 { get; set; }
        public string r42 { get; set; }
        public string r43 { get; set; }

        public string r44 { get; set; }
        public string r45 { get; set; }
        public string r46 { get; set; }

        public string r51 { get; set; }
        public string r52 { get; set; }
        public string r53 { get; set; }

        public string r54 { get; set; }
        public string r55 { get; set; }
        public string r56 { get; set; }

        public IList<FindPrintRetrievalItem> FindPrintRetrievalItem { get; set; }
    }

    public class GroupItems: FindPrintManagementReportItem
    {
        public string Totalgroup { get; set; }
        public IList<FindPrintManagementReportItem> FindPrintManagementReportItem { get; set; }
    }

    public class FindPrintRetrievalItem
    {
        public int No { get; set; }

        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public string MaterialLotNo { get; set; }

        public string Scharged { get; set; }
        public string Charge1 { get; set; }
        public string Charge2 { get; set; }
        public string Charge3 { get; set; }
        public string Charge4 { get; set; }
        public string Charge5 { get; set; }
        public string Chargev1 { get; set; }
        public string Chargev2 { get; set; }
        public string Chargev3 { get; set; }
        public string Chargev4 { get; set; }
        public string Chargev5 { get; set; }
        public string Total { get; set; }
    }
}
