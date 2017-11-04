using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;

namespace KCSG.Domain.Models.EnvironmentManagement
{
    public class TemporaryTable
    {
        public DateTime S_FLD_DTM { get; set; }
        public Constants.TypeOfTable S_FLD_ID { get; set; }
        public double S_FLD_DATA { get; set; }
        public double S_FLD_MEAN { get; set; }
        public double S_FLD_UPPER { get; set; }
        public double S_FLD_LOWER { get; set; }
        public string S_FLD_LOT { get; set; }
    }
}
