using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Maintenance
{
    public class MaintenancePlan : Operations
    {
        [Key]
        public int MaintenancePlanId { get; set; }
        [Display(Name = "Equipment Category")]
        public Guid EquipmentCategoryId { get; set; }
        [Display(Name = "Equipment Type")]
        public Guid EquipmentTypeId { get; set; }
        [Display(Name = "Equipment Name")]
        
        public int GlFiscalYearId { get; set; } // GlFiscalYear
        public int TotalQuantity { get; set; }
        public decimal AverageAvailableQuantity { get; set; }
        public decimal AverageMaintenanceCostPM { get; set; }
        public decimal AverageMaintenanceCostCM { get; set; }

    }

    public class MaintenancePlanDetail : TrackUserSettingOperation
    {
        [Key]
        public int MaintenancePlanDetailId { get; set; }

        [Display(Name = "Period")]
        public int GlPeriodId { get; set; }

        public int CMQuantity { get; set; } 
        public int PMQuantity { get; set; }
        public decimal CMCost { get; set; }
        public decimal PMCost { get; set; }
        public int MaintenancePlanId { get; set; }
        public MaintenancePlan MaintenancePlan { get; set; } 
    }
}
