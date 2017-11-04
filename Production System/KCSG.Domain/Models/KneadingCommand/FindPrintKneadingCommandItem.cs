using System;
using KCSG.Core.Constants;

namespace KCSG.Domain.Models.KneadingCommand
{
    public class FindPrintKneadingCommandItem
    {
        //public string CommandNo { get; set; }
        //public string CommandDate { get; set; }
        //public string PreProductCode { get; set; }

        public string ChargedOrder { get; set; }

        public string PotSeq { get; set; }

        public string MaterialName { get; set; }

        public string LotNo { get; set; }

        public string LoadPosition { get; set; }

        public double ThirdFloorQuantity { get; set; }

        public double FourthFloorQuantity { get; set; }

        public string State { get; set; }

        public string Colour { get; set; }

        public int CrushingOne { get; set; }

        public int CrushingTwo { get; set; }

        public string Weighed { get; set; }

        public string Charged { get; set; }

        public string MeasurementCompleted { get; set; }

        public string PreProductName { get; set; }

        public string MixedMode { get; set; }

        public int BatchLot { get; set; }

        public int LayinPriority { get; set; }

        public double ThirdLayinAmount { get; set; }

        public double FourthLayinAmount { get; set; }
        public DateTime? MixDate1 { get; set; }
        public DateTime? MixDate2 { get; set; }
        public DateTime? MixDate3 { get; set; }

        public string ThirdColumn
        {
            get
            {
                if (ThirdLayinAmount.Equals(0))
                    return null;

                if (LayinPriority == 4)
                    return (ThirdLayinAmount * BatchLot).ToString("#,###.00");

                return ThirdLayinAmount.ToString("#,###.00");
            }
        }

        public string FourthColumn
        {
            get
            {
                if (FourthLayinAmount.Equals(0))
                    return null;

                if (LayinPriority == 4)
                    return (FourthLayinAmount * BatchLot).ToString("#,###.00");

                return FourthLayinAmount.ToString("#,###.00");
            }
        }

        public string ShowLayinPriority
        {
            get
            {
                if (LayinPriority == 1)
                {
                    return Constants.MixTime.MixTime1 +" "+ MixDate1.Value.Minute.ToString() + " mins";
                }
                else if (LayinPriority == 2)
                {
                    return Constants.MixTime.MixTime2 +" "+ MixDate2.Value.Minute.ToString() + " mins";
                }
                else if (LayinPriority == 2)
                {
                    return Constants.MixTime.MixTime3 +" "+ MixDate3.Value.Minute.ToString() + " mins";
                }
                else
                {
                    return "";
                }

            }
        }
    }
}