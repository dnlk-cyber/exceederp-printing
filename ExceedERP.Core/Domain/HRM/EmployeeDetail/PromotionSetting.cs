using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class PromotionYearSetting: TrackUserOperation
    {
        [Key]
        public int PromotionYearSettingId { get; set; }
        [Display(Name = "No_ Promotion Years")]
        public int NumberOfPromotionYears { get; set; }
        [Display(Name = "Step amounts to promote")]
        public int StepAmountsToPromote { get; set; }

    }

   
}
