using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public class EquipmentCheckList
    {
        public int EquipmentCheckListId{get;set;}
        public Guid FleetsID { get; set; }
        public int EquipmentLineCheckListId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public int Quantity { get; set; }
        public virtual Fleets Fleets { get; set; }
    }
}
