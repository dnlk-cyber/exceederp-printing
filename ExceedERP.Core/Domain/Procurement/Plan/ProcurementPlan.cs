using ExceedERP.Core.Domain.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Procurement.Plan
{


    public class ProcurementPlan : TrackTransaction
    {
        public int ProcurementPlanId { get; set; }
        [Display(Name = "Year")]
        public int GlFiscalYearId { get; set; }
        [Required]
        [DisplayName("Branch or Project")]
        public int BranchId { get; set; }
        [DisplayName("Business Unit")]
        public int BusinessUnitId { get; set; }
        [Display(Name = "Description")]
        public string Name { get; set; }
        public string Remark { get; set; }
        public virtual ICollection<ProcurementPlanItem> PlanItems { get; set; }
        public virtual ICollection<ProcurementPlanValidation> PlanValidation { get; set; }
    }
}
