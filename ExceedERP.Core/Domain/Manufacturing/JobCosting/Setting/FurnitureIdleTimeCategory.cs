using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.JobCosting.Setting
{
    public class FurnitureIdleTimeCategory :  TrackUserSettingOperation
    {
        public int FurnitureIdleTimeCategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool IsControllable { get; set; }
        public string CategoryDescription { get; set; }

    }
}
