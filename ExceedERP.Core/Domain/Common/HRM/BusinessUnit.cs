using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common.HRM
{
    public class BusinessUnit : TrackUserOperation 
    {
        public int BusinessUnitId { get; set; }
        [Required]
        [Display(Name = "Name", ResourceType = typeof(Localization.Resources))]
        public string Name { get; set; }
        [Display(Name = "AccountCode", ResourceType = typeof(Localization.Resources))]
        public string Accountcode { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }

    public class BusinessUnitBranch
    {
        [Key]
        public int BusinessUnitBranchId { get; set; } // Add PK for better tracking

        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }

        public int BusinessUnitId { get; set; }
        public virtual BusinessUnit BusinessUnit { get; set; }

        public bool IsSubBranch { get; set; }

        public bool IsActive { get; set; } = true;
    }

}