using System;
using System.Collections.Generic;

namespace KCSG.Domain.Models.KneadingCommand
{
    public class PrintKneadingCommandItem
    {
        public string CommandNo { get; set; }

        public string CommandDate { get; set; }

        public string PreProductCode { get; set; }

        public string PreProductName { get; set; }

        public string LotNo { get; set; }

        public string Issued { get; set; }

        public string IssuedNo { get; set; }

        public string MixedMode { get; set; }

        public string Batch { get; set; }

        public IList<FindPrintKneadingCommandItem> FindPrintKneadingCommandItems { get; set; }
    }
}