using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.HealthAndInsurance
{
    public class Insurance
    {
        [Key]
        public int InsuranceId { get; set; }
        //[Required ]
        //[Display(Name = "HR_HealthAndInsurance_Insurance_InsuranceGroupId", ResourceType = typeof(Localization.Resources))]
        //public int InsuranceGroupId { get; set; }
        //public virtual InsuranceGroup InsuranceGroup { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_HealthAndInsurance_Insurance_From", ResourceType = typeof(Localization.Resources))]
        public DateTime From { get; set; }
        [Display(Name = "HR_HealthAndInsurance_Insurance_From", ResourceType = typeof(Localization.Resources))]
        public string FromAmharic { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_HealthAndInsurance_Insurance_To", ResourceType = typeof(Localization.Resources))]
        public DateTime To { get; set; }
        [Display(Name = "HR_HealthAndInsurance_Insurance_To", ResourceType = typeof(Localization.Resources))]
        public string  ToAmharic { get; set; }
        [Display(Name = "HR_HealthAndInsurance_Insurance_ClassOfBusiness", ResourceType = typeof(Localization.Resources))]
        [Required]
        public int LookupId { get; set; }
        //  public virtual Lookup Lookup { get; set; }
        [Required ]
        [Display(Name = "HR_HealthAndInsurance_Insurance_InsuranceCorporation", ResourceType = typeof(Localization.Resources))]
        public string InsuranceCorporation { get; set; }
        [Display(Name = "HR_HealthAndInsurance_Insurance_Number", ResourceType = typeof(Localization.Resources))]
        public int Number { get; set; }
        [Display(Name = "HR_HealthAndInsurance_Insurance_Salary", ResourceType = typeof(Localization.Resources))]
        public decimal Salary { get; set; }
        [Display(Name = "HR_HealthAndInsurance_Insurance_SumofInsured", ResourceType = typeof(Localization.Resources))]
        public decimal SumofInsured { get; set; }
        [Display(Name = "HR_HealthAndInsurance_Insurance_Premium", ResourceType = typeof(Localization.Resources))]
        public decimal Premium { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "Insurance_Insurance_Type", ResourceType = typeof(Localization.Resources))]
        public int InsuranceTypeId { get; set; }
        [Display(Name = "Insurance_Insurance_Catagory", ResourceType = typeof(Localization.Resources))]
        public int InsuranceCategoryId { get; set; }



    }
}