using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.SelfCare
{
    public class EducationRequest : DepartmentOperations
    {
        [Key]
        public int EducationRequestId { get; set; }
        [Display(Name = "HR_SelfCare_TrainingApplicant_OrgStructureId", ResourceType = typeof(Localization.Resources))]
        public int OrgStructureId { get; set; }
        public virtual MainCompanyStructure MainCompanyStructure { get; set; }

        [Required]
        [Display(Name = "HR_SelfCare_TrainingApplicant_CourseName", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_SelfCare_TrainingApplicant_Date", ResourceType = typeof(Localization.Resources))]
        public DateTime Date { get; set; }
        [Display(Name = "HR_SelfCare_TrainingApplicant_Date", ResourceType = typeof(Localization.Resources))]
        public string DateAmharic { get; set; }
        //[Display(Name = "HR_SelfCare_TrainingApplicant_NoofTrainee", ResourceType = typeof(Localization.Resources))]
        //public int NoofTrainee { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        public string EducationType { get; set; }
        public int EmployeeId { get; set; }
        

    }
}
