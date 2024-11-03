using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class Cosign:TrackUserOperation 
    {
        [Key]
        public int CosignId { get; set; }
        [Display(Name = "Common_PersonEmployee_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
        [Required]
        [ Display(Name = "HR_EmployeeDetail_Cosign_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_SelfCare_OverTimeRequest_FromDate", ResourceType = typeof(Localization.Resources))]
        public DateTime AgreementDate { get; set; }
        [Display(Name = "HR_SelfCare_OverTimeRequest_FromDate", ResourceType = typeof(Localization.Resources))]
        [Required]
        public string AgreementDateAm { get; set; }
        [Required ]
        [Display(Name = "HR_EmployeeDetail_Cosign_FullName", ResourceType = typeof(Localization.Resources))]
        public string FullName { get; set; }
        //[Required]
        //[Display(Name = "HR_EmployeeDetail_Cosign_BirthPlace", ResourceType = typeof(Localization.Resources))]
        //public string BirthPlace { get; set; }
        [Display(Name = "HR_EmployeeDetail_Cosign_Occupation", ResourceType = typeof(Localization.Resources))]
        public string Occupation { get; set; }
        [Display(Name = "HR_EmployeeDetail_Cosign_Workorganization", ResourceType = typeof(Localization.Resources))]
        public string Workorganization { get; set; }
        [Display(Name = "HR_EmployeeDetail_Cosign_ReferenceType", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        public virtual Lookup CosignType { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Salary", ResourceType = typeof(Localization.Resources))]
        public decimal? Salary { get; set; }

    }
}