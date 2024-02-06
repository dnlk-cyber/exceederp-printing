using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.JobCosting.Setting
{
    public class FurnitureUnit : TrackUserSettingOperation
    {
        public int FurnitureUnitId { get; set; }
        public string UOM { get; set; }
        public string Description { get; set; }
    }
}
