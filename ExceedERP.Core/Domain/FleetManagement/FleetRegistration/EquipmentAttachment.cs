 using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public class EquipmentAttachment : Operations 
    {
        public int EquipmentAttachmentID { get; set; }
        public Guid EquipmentID { get; set; }
        public string Type { get; set; }
        public string FileName { get; set; }
      
    }
}
