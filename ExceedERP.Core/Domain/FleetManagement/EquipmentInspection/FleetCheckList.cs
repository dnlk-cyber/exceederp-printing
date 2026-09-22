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
        public Guid? EquipmentTypeID { get; set; }      
        public string Code { get; set; }
        public string DriverVehicleTransferID { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }


        
    }
    public class FleetLineCheckList : Operations
    {
        public int FleetLineCheckListId { get; set; }
        public int FleetCheckListId { get; set; }
        [Required]
        public string SystemName { get; set; }
        public string CheckList { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public virtual FleetCheckList FleetCheckList { get; set; }
    }
    public enum StatusKey
    {
        [Display(Name ="--")]
        NA,
        [Display(Name = "Excellent Condition")]
        ExcellentCondition,
        [Display(Name = "Good Condition")]
        GoodCondition,
        [Display(Name = "Fair Condition")]
        FairCondition,
        [Display(Name = "Workable MinorRepair")]
        Workable_MinorRepair,
        [Display(Name = "Workable MajorRepair")]
        Workable_MajorRepair
    }
}
