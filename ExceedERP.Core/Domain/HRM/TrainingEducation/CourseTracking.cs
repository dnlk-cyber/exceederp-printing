using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.HRMPlan;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation
{
    public class CourseTracking:TrackUserOperation 
    {
        [Key]
        public int CourseTrackingId { get; set; }
       // [Required]
        [Display(Name = "HR_TrainingEducation_TrainingPlan", ResourceType = typeof(Localization.Resources))]
        public int? TrainingPlanId { get; set; }
       // public virtual TrainingPlan TrainingPlan { get; set; }           
        //[Required ]
        [Display(Name = "HR_TrainingEducation_CourseTracking_Period", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
       // public virtual GlFiscalYear GlFiscalYear { get; set; }
       // [Required ]
        [Display(Name = "HR_TrainingEducation_CourseTracking_Course", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
      //  public virtual Lookup Lookup { get; set; }
       // [Required ]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_TrainingEducation_CourseTracking_From", ResourceType = typeof(Localization.Resources))]
        public DateTime From { get; set; }
        [Display(Name = "HR_TrainingEducation_CourseTracking_From", ResourceType = typeof(Localization.Resources))]
        public string FromAmharic { get; set; }
        //[Required ]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_TrainingEducation_CourseTracking_To", ResourceType = typeof(Localization.Resources))]
        public DateTime To { get; set; }
        [Display(Name = "HR_TrainingEducation_CourseTracking_To", ResourceType = typeof(Localization.Resources))]
        public string ToAmharic { get; set; }
        [Display(Name = "HR_TrainingEducation_CourseTracking_Cost", ResourceType = typeof(Localization.Resources))]
        public decimal Cost { get; set; }
        [Display(Name = "HR_TrainingEducation_CourseTracking_TrainingType", ResourceType = typeof(Localization.Resources))]
        public TrainingType  TrainingType { get; set; }
        [Display(Name = "HR_TrainingEducation_CourseTracking_TrainingTerm", ResourceType = typeof(Localization.Resources))]
        public TrainingTerm  TrainingTerm { get; set; }
        [Display(Name = "HR_TrainingEducation_CourseTracking_TrainingLocation", ResourceType = typeof(Localization.Resources))]
        public TrainingLocation TrainingLocation { get; set; }
        [Display(Name = "HR_TrainingEducation_CourseTracking_Organization", ResourceType = typeof(Localization.Resources))]
        public int MainCompanyId { get; set; }
       // public virtual MainCompany MainCompany { get; set; }
        [Display(Name = "HR_TrainingEducation_CourseTracking_IsOnJob", ResourceType = typeof(Localization.Resources))]
        public bool IsOnJob { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "HR_HRMPlan_TrainingPlan_NoofTrainee", ResourceType = typeof(Localization.Resources))]
        public int NoOfTrainee { get; set; }
        public TrainingRequestType TrainingRequestType { get; set; }

    }
}                                                