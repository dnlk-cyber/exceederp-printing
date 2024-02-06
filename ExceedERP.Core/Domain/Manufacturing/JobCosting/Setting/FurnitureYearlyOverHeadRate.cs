using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.JobCosting.Setting
{
    public  class FurnitureYearlyOverHeadRate : TrackUserSettingOperation
    {
        public int FurnitureYearlyOverHeadRateId { get; set; }
        public int GlFiscalYearId { get; set; }
        public string Values { get; set; }
        public decimal Rate { get; set; }
        public string Description { get; set; }
    }
}
