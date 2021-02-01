using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common.HRM
{
    public class Country
    {
        public int CountryId { get; set; }
        [Display(Name = "Common_Country_Name", ResourceType = typeof(Localization.Resources))]
        public string Name { get; set; }
        [Display(Name = "Common_Country_PolticalName", ResourceType = typeof(Localization.Resources))]
        public string PolticalName { get; set; }
        [Display(Name = "Common_Country_Continent", ResourceType = typeof(Localization.Resources))]

        public string Continent { get; set; }
        [Display(Name = "Common_Country_TelephoneCode", ResourceType = typeof(Localization.Resources))]
        public string TelephoneCode { get; set; }
        [Display(Name = "Common_Country_TimeZone", ResourceType = typeof(Localization.Resources))]

        public string TimeZone { get; set; }
        [Display(Name = "Common_Country_Nationality", ResourceType = typeof(Localization.Resources))]
        public string Nationality { get; set; }
        [Display(Name = "Common_Country_CountryCode", ResourceType = typeof(Localization.Resources))]

        public string CountryCode { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]

        public string Remark { get; set; }

    }
}