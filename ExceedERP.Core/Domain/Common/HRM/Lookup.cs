using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common.HRM
{
    public class Lookup : TrackUserOperation
    {
        [Key]
        public int LookupId { get; set; }

        public string Code { get; set; }
        [Display(Name = "Common_Lookup_Category", ResourceType = typeof(Localization.Resources))]
        public string Category { get; set; }
        public int Index { get; set; }
        [Display(Name = "Common_Lookup_LookupType", ResourceType = typeof(Localization.Resources))]
        public LookUpTypes LookupType { get; set; }//enum
        [Display(Name = "Common_Lookup_Description", ResourceType = typeof(Localization.Resources))]
        public string Description { get; set; }
        [Display(Name = "Common_Lookup_Description_Eng", ResourceType = typeof(Localization.Resources))]
        public string DescriptionEng { get; set; }
        public bool IsDefault { get; set; }

        public bool IsActive { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}