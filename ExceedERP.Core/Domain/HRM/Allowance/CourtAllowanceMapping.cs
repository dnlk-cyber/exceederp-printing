using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.Allowance
{
    public class CourtAllowanceMapping : TrackUserOperation
    {
        [Key]
        public int AllowanceMappingId { get; set; }
        public int AllowanceDefinationId { get; set; }
        public virtual AllowanceDefination AllowanceDefination { get; set; }

        [Display(Name = "HR_Allowance_AllowanceDefination_LookupId", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }
        [Display(Name = "HR_Allowance_AllowanceDefination_BranchId", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }

    }
}