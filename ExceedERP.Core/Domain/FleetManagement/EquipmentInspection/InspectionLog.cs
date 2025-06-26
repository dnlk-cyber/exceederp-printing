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
        public Guid AnnualEquipmentInventoryInspectionID { get; set; }
        public string SystemName { get; set; }
        public string CheckList { get; set; }       
        public StatusKey StatusKey { get; set; }
        public int StatusKeyId { get; set; }
    }
}
