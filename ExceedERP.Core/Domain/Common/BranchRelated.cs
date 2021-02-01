using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common
{
    class BranchRelated
    {
    }
    #region Branch

    public class OrgBranchUserMapping
    {
        public int OrgBranchUserMappingID { get; set; }
        [Required]
        public string Branch { get; set; }
        [Required]
        public string User { get; set; }
        [Display(Name = "Is Default")]
        public bool IsDefault { get; set; }

    }

    public class DefaultBranch
    {
        [Key]
        public string UserId { get; set; }
        public string BranchId { get; set; }
    }

    public class OrgBranch
    {
        public int OrgBranchID { get; set; }
        [Required]
        [Display(Name = "Branch Name")]
        public string OrgBranchName { get; set; }
        [Required]
        [Display(Name = "Branch Abbreviation")]
        public string OrgBranchAbbreviation { get; set; }
        [Required]
        [Display(Name = "Account Code")]
        public string BranchAccountCode { get; set; }
        public string Description { get; set; }
    }

    #endregion
}
