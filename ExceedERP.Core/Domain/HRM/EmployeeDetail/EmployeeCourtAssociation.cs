using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class EmployeeCourtAssociation : TrackUserOperation
    {
        [Key]
        public int AssociationId { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_EmployeeAssociation_EmployeeId", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Display(Name = "HR_EmployeeDetail_EmployeeAssociation_Name", ResourceType = typeof(Localization.Resources))]
        public string Name { get; set; }
        [Display(Name = "HR_EmployeeDetail_EmployeeAssociation_AssociationType", ResourceType = typeof(Localization.Resources))]
        public AssociationType AssociationType { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Common_PersonEmployee_DateOfBirth", ResourceType = typeof(Localization.Resources))]

        public DateTime DateOfBirth { get; set; }
        [Required]
        [Display(Name = "Common_PersonEmployee_DateOfBirth", ResourceType = typeof(Localization.Resources))]

        public string DateOfBirthAmharic { get; set; }

        [Required]
        [Display(Name = "Common_PersonEmployee_Gender", ResourceType = typeof(Localization.Resources))]
        public Gender Gender { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}