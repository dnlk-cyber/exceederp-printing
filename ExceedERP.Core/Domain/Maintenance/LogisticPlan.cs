using System;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.Maintenance
{
    public class LogisticPlan : Operations
    {
        public int LogisticPlanId { get; set; }
          [Required]
        [DisplayName("Equipment Type")]
        public Guid EquipmentTypeId { get; set; }
          [Required]
          [DisplayName("Year")]
        public int  GlFiscalYearId { get; set; }
          [Required]
          [DisplayName("Month")]
        public int  GLPeriodId { get; set; }
        [Required]
        public string Model { get; set; }
          [Required]
        public string Location { get; set; }
        public string Description { get; set; }

        public string Type { get; set; }
        public ApprovalStatuses ApprovalStatus { get; set; }

         [NotMapped]
         public string FiscalYear { get; set; }
         [NotMapped]
         public string Period { get; set; }
        public virtual GLPeriod GLPeriod { get; set; }
         public virtual EquipmentType EquipmentType { get; set; }
    }


}
