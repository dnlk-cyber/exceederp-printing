using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.EquipmentInspection
{
    public class FleetCheckList : TrackUserSettingOperation
    {
        public int FleetCheckListId { get; set; }

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
    public class FleetLineCheckList : Operations
    {
        public int FleetLineCheckListId { get; set; }
        public int FleetCheckListId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        //public bool IsNormal { get; set; }
        public virtual FleetCheckList FleetCheckList { get; set; }
    }
}
