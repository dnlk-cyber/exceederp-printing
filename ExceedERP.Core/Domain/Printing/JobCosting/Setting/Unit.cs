using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.printing.JobCosting.Setting

{
    public class Unit : TrackUserSettingOperation
    {
        public int UnitId { get; set; }
        public string UOM { get; set; }
        public string Description { get; set; }
    }
}
