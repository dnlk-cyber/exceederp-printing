using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;

namespace ExceedERP.Core.Domain.FleetManagement.EquipmentInspection
{
    public class CheckList : TrackUserSettingOperation
    {
        public int CheckListId { get; set; }

        [Required]
        public Guid EquipmentIdentityID { get; set; }
        public Guid EquipmentTypeID { get; set; }
        [Required]
        public CheckListType Type { get; set; }
        [NotMapped]
        public string Name { get; set; }
        public bool IsMaintenace { get; set; }
     
        public string Code { get; set; }
    
        public string Description { get; set; }
        //public bool IsNormal { get; set; }

        public string Remark { get; set; }

        [NotMapped]
        public bool Selected { get; set; }

    }
}
