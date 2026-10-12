using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class ProbationsSetting: TrackUserOperation
    {
        public int ProbationsSettingId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_ResponsibilityLevel", ResourceType = typeof(Localization.Resources))]
        public ResponsibilityLevel? ResponsibilityLevel { get; set; }
        [Display(Name = "Probation Days")]
        public int ProbationDays { get; set; }
        [Display(Name = "Minimum fit value")]
        public decimal MinimumFitValue { get; set; }
        public string  Remark { get; set; }
    }
}
