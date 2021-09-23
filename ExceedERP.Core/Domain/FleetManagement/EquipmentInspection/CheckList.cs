using System;
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
        public Guid EquipmentTypeId { get; set; }
        public Guid EquipmentCategory { get; set; }
        [Required]
        public CheckListType Type { get; set; }
        [NotMapped]
        public string Name { get; set; }
        public bool IsMaintenace { get; set; }
     
        public string Code { get; set; }
    
        public string Description { get; set; }

        public string Remark { get; set; }

        [NotMapped]
        public bool Selected { get; set; }
        [ForeignKey("EquipmentTypeId")]
        public virtual EquipmentIdentity EquipmentIdentity { get; set; }
    }
}
