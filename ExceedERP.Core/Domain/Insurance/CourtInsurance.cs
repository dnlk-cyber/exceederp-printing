using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Insurance
{
    public class CourtInsurance : TrackUserOperation
    {
        [Key]
        public int CourtInsuranceId { get; set; }
        [Required]
        [Display(Name = "HR_HealthAndInsurance_CourtInsurance_EmployeeId", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        //public virtual Person_Employee Person_Employee { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_HealthAndInsurance_CourtInsurance_Date", ResourceType = typeof(Localization.Resources))]
        public DateTime Date { get; set; }
        [Display(Name = "HR_HealthAndInsurance_CourtInsurance_Date", ResourceType = typeof(Localization.Resources))]
        public string  DateAssocAmharic { get; set; }
        [Display(Name = "HR_HealthAndInsurance_CourtInsurance_Date", ResourceType = typeof(Localization.Resources))]
        public string DateInsuAmhairc { get; set; }

        [Display(Name = "HR_HealthAndInsurance_CourtInsurance_For", ResourceType = typeof(Localization.Resources))]
        [Required]
        public int AssociationId { get; set; }
        // public virtual EmployeeAssociation EmployeeAssociation { get; set; }
        [Required]
        [Display(Name = "HR_HealthAndInsurance_CourtInsurance_Reason", ResourceType = typeof(Localization.Resources))]
        public string Reason { get; set; }
        [Display(Name = "HR_HealthAndInsurance_CourtInsurance_InsuranceAmount", ResourceType = typeof(Localization.Resources))]

        public decimal InsuranceAmount { get; set; }
        public string Type { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }





    }
}