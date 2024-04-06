using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common.HRM
{
    public class Branch : TrackUserOperation 
    {

        [Key]
        public int BranchId { get; set; }
        [Required]
        [Display(Name = "Common_Company_Branch_Name", ResourceType = typeof(Localization.Resources))]
        public string Name { get; set; }
        [Display(Name = "Common_Company_Branch_AmharicName", ResourceType = typeof(Localization.Resources))]
        public string AmharicName { get; set; }
        [Required]
        [Display(Name = "Common_Company_Branch_Abbreviation", ResourceType = typeof(Localization.Resources))]
        public string Abbreviation { get; set; }
        [Required]
        [Display(Name = "Common_Company_Branch_Code", ResourceType = typeof(Localization.Resources))]
        public string Code { get; set; }

        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "Common_Company_Branch_BranchTypes", ResourceType = typeof(Localization.Resources))]
        public BranchTypes BranchTypes { get; set; }

          //[Display(Name = "Enum_LookUpTypes_PROJECT_TYPE", ResourceType = typeof(Localization.Resources))]
          //public int? LookupId { get; set; }

          public virtual Lookup Lookup { get; set; }
          public string SubAccount { get; set; }
          public string LocationAccount { get; set; }
        public bool IsActive { get; set; }
        [DisplayName("Parent")]
        public int? ParentBranchId { get; set; }

    }

  
}