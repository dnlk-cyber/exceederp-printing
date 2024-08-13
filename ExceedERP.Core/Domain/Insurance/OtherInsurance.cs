using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Insurance
{
    public class OtherInsurance : TrackUserOperation
    {
        [Key]
        public int OtherInsuranceId { get; set; }
        [Required]
        [Display(Name = "HR_HealthAndInsurance_InsuranceCategory_Name", ResourceType = typeof(Localization.Resources))]
        public int InsuranceCategoryId { get; set; }
        public virtual InsuranceCategory InsuranceCategory { get; set; }
        [Display(Name = "HR_HealthAndInsurance_InsuranceCategory_InsuranceTypeId", ResourceType = typeof(Localization.Resources))]
        public int? InsuranceTypeId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_HealthAndInsurance_Insurance_From", ResourceType = typeof(Localization.Resources))]
        public DateTime From { get; set; }
        [Display(Name = "HR_HealthAndInsurance_Insurance_From", ResourceType = typeof(Localization.Resources))]
        public string FromInsuranceAmhric { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_HealthAndInsurance_Insurance_To", ResourceType = typeof(Localization.Resources))]
        public DateTime To { get; set; }
        [Display(Name = "HR_HealthAndInsurance_Insurance_To", ResourceType = typeof(Localization.Resources))]
        public string  ToInsuranceAmharic { get; set; }
        [Display(Name = "HR_HealthAndInsurance_Insurance_SumofInsured", ResourceType = typeof(Localization.Resources))]
        public decimal SumofInsured { get; set; }
        [Display(Name = "HR_HealthAndInsurance_Insurance_Rate", ResourceType = typeof(Localization.Resources))]
        public decimal Rate { get; set; }
        [Display(Name = "HR_HealthAndInsurance_Insurance_Premium", ResourceType = typeof(Localization.Resources))]
        public decimal Premium { get; set; }
        [Display(Name = "HR_HealthAndInsurance_Insurance_Stamp", ResourceType = typeof(Localization.Resources))]
        public decimal Stamp { get; set; }
        [Display(Name = "HR_HealthAndInsurance_Insurance_ThirdParty", ResourceType = typeof(Localization.Resources))]
        public decimal ThirdParty { get; set; }
        [Display(Name = "HR_HealthAndInsurance_Insurance_Total", ResourceType = typeof(Localization.Resources))]
        public decimal Total { get; set; }
        public string Name { get; set; }

        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}