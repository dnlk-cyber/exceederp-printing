using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common.HRM
{
    public class MainCompany :TrackUserOperation 
    {

        [Key]
        public int MainCompanyId { get; set; }
        [Required]
        [Display(Name = "Common_Company_MainCompany_TradeName", ResourceType = typeof(Localization.Resources))]
        public string TradeName { get; set; }
        [Display(Name = "Common_Company_MainCompany_BrandName", ResourceType = typeof(Localization.Resources))]
        public string BrandName { get; set; }
        [Required]
        [Display(Name = "Common_Company_MainCompany_BusinessType", ResourceType = typeof(Localization.Resources))]
        public BusinessType BusinessType { get; set; }
        [Display(Name = "Common_Company_MainCompany_Category", ResourceType = typeof(Localization.Resources))]
        public string Category { get; set; }

        [Display(Name = "Common_Company_MainCompany_AmharicName", ResourceType = typeof(Localization.Resources))]
        public string AmharicName { get; set; }
        [Display(Name = "Common_Company_MainCompany_Abbrevation", ResourceType = typeof(Localization.Resources))]
        public string Abbreviation { get; set; }
        [Display(Name = "Common_Company_MainCompany_IsActive", ResourceType = typeof(Localization.Resources))]
        public bool IsActive { get; set; }


    }

    public class CompanyEmail
    {
        [Key]
        public int CompanyEmailId { get; set; }
          [Display(Name = "Enum_AddressType_OFFICE_OFFICE_EMAIL", ResourceType = typeof(Localization.Resources))]
        [Required ]
        public string EmailAddress { get; set; }
         [Display(Name = "CompanyEmail_Password", ResourceType = typeof(Localization.Resources))]
         [Required]
        public string Password { get; set; }
        public string ServerType { get; set; }
        public int MainCompanyId { get; set; }
        public virtual MainCompany MainCompany { get; set; }
    }
}