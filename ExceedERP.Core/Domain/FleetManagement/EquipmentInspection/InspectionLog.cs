using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.EquipmentInspection
{
    public class InspectionLog : Operations
    {
        public int InspectionLogId { get; set; }

        public string InspectionId { get; set; }
        public string CategoryName { get; set; }
        public string LineName { get; set; }
        public bool IsActive { get; set; }
    }
}
