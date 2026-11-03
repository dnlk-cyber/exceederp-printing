using ExceedERP.Core.Domain.Common;
using System.ComponentModel;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation.EducationOpportunity
{
   public class EducationFiledMapping: TrackUserOperation
   {
        public int EducationFieldMappingId { get; set; }
        public int? EducationLevelId { get; set; }
        public int EducationFieldId { get; set; }
        public int? RelevantEducationLevelId { get; set; }
        public EducationCategoryType CategoryType { get; set; }
        [DisplayName("Education Type")]
        public EducationType EducationType { get; set; }
        public int? RelevantFieldId { get; set; }
        public decimal RelevantPoint { get; set; }
        public int? RelatedEducationLevelId { get; set; }
        public int? RelatedFieldId { get; set; }
        public decimal RelatedPoint { get; set; }


    }
    public class EducationFiledMappingDetail: TrackUserOperation
   {
        public int EducationFiledMappingDetailId { get; set; }
        public int EducationLevelId { get; set; }
        public decimal Point { get; set; }
        public int EducationFieldMappingId { get; set; }
        public virtual EducationFiledMapping FieldMapping { get; set; }
    }

    public enum EducationType
    {
        [Description("Natural Science")]
        Natural_Science = 1,

        [Description("Social Science")]
        Social_Science = 2
    }
}
