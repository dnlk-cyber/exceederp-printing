using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.SelfCare
{
    /// <summary>   A training applicant. </summary>
    ///
    /// <remarks>   Mekanehiwot, 11/11/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class TrainingApplicant: DepartmentOperations
    {
        [Key]
        public int TrainingApplicantId { get; set; }
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
        [Display(Name = "HR_SelfCare_TrainingApplicant_NoofTrainee", ResourceType = typeof(Localization.Resources))]
        public int NoofTrainee { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }      
        public TrainingRequestType TrainingRequestType { get; set; }

        public int GlFisicalYearId { get; set; }
        public bool IsMiddleOnlineApproved { get; set; }
        public string MiddleOnlineApprovedBy { get; set; }
        public DateTime? MiddleOnlineApprovedTime { get; set; }


    }
    public class TrainingApplicantAttachment : TrackUserSettingOperation
    {
        [Key]
        public int TrainingApplicantAttachmentId { get; set; }

        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
        public int TrainingApplicantId { get; set; }
        public virtual TrainingApplicant Request { get; set; }
    }

    public class TrainingApplicantSecondValidation : VoucherValidation
    {
        [Key]
        public int TrainingApplicantSecondValidationId { get; set; }

        public int TrainingApplicantId { get; set; }
        public virtual TrainingApplicant TrainingApplicant { get; set; }
    }
}