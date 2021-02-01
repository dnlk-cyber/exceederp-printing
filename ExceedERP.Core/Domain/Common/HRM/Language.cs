using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common.HRM
{
    public class Language
    {
        public int LanguageId { get; set; }
        [Display(Name = "Common_Language_Description", ResourceType = typeof(Localization.Resources))]
        public string Description { get; set; }
        [Display(Name = "Common_Language_LanguageType", ResourceType = typeof(Localization.Resources))]
        public LanguageType LanguageType { get; set; }
        [Display(Name = "Common_Language_CountryId", ResourceType = typeof(Localization.Resources))]
        public int CountryId { get; set; }
        public virtual Country Country { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }

    }
}