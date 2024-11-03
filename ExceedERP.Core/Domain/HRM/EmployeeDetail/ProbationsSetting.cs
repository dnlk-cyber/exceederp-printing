using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class ProbationsSetting: TrackUserOperation
    {
        public int ProbationsSettingId { get; set; }
        [Display(Name = "Promotion Days")]
        public int PromotionDays { get; set; }
        [Display(Name = "Minimum fit value")]
        public decimal MinimumFitValue { get; set; }
        public string  Remark { get; set; }
    }
}
