using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.Inventory
{
    public class InventoryStore : TrackUserSettingOperation
    {
        [Required]
        [Display(Name = "Store Code")]
        [Key]
        public string StoreCode { get; set; }
        [Required]
        [DisplayName("Store Name")]
        public string Name { get; set; }
        [Required]
        [DisplayName("Branch or Project")]
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        public string Location { get; set; }
        public string Remark { get; set; }
        public bool Enable { get; set; }
        [DisplayName("Temporary Store")]
        public bool IsTemp { get; set; }
        public int BusinessUnitId { get; set; }
        public virtual ICollection<StoreKeeperAssignment> StoreKeeperAssignments { get; set;  }
        public virtual ICollection<StoreItemAssignment> StoreItemAssignments { get; set;  }
        public virtual ICollection<InventoryStorePeriodStatus> InventoryStorePeriodStatuses { get; set; }        
    }

    public class InventoryStorePeriodStatus : TrackUserSettingOperation
    {
        public int InventoryStorePeriodStatusId { get; set; }
        [Required]
        [Display(Name = "Store Code")]
        public string StoreCode { get; set; }
        public virtual InventoryStore InventoryStore { get; set; }
        [Required]
        public int InventoryPeriodId { get; set; }
        public virtual InventoryPeriod InventoryPeriod { get; set; }
        public FiscalYearStatus Status { get; set; }
        [NotMapped]
        public string PeriodName { get; set; }
    }
}