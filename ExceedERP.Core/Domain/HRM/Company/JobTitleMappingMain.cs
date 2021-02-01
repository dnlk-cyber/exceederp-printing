using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.Company
{
    public class JobTitleMappingMain : TrackUserOperation
    {
        [Key]
        public int JobTitleMappingMianId { get; set; }
        [Display(Name = "HR_Company_JobTitleMappingDetail_MainFunction", ResourceType = typeof(Localization.Resources))]
        [DataType(DataType.MultilineText)]
        public string MainFunction { get; set; }
        public int JobTitleMappingId { get; set; }
        public virtual JobTitleMapping JobTitleMapping { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}