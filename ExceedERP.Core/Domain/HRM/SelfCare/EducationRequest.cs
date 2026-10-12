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
        [Display(Name = "EducationType", ResourceType = typeof(Localization.Resources))]

        public string EducationType { get; set; }

        [Display(Name = "HR_EmployeeDetail_Placement_PersonEmployeeID", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public string Reason { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Desired Training Date(s)")]
        public DateTime? DesiredTrainingDate { get; set; }
        [Display(Name = "Desired Training Date(s)")]
        public string DesiredTrainingDateAm { get; set; }
        [Display(Name = "Duration of Training")]
        public string DurationofTraining { get; set; }

        [Display(Name = "Training Provider")]
        public string TrainingProvider { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_JobTitleName", ResourceType = typeof(Localization.Resources))]
        public int JobTitleId { get; set; }

    }
    public class EducationRequestAttachement
    {
        public int EducationRequestAttachementId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
        public int EducationRequestId { get; set; }
        public EducationRequest Request { get; set; }
        //attachment
    }

}
