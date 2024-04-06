using ExceedERP.Core.Localization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class InternalApplicantScreening
    {
        public int InternalApplicantScreeningId { get; set; }
        public int InternalApplicantScreeningResultId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Employee))]
        public int EmployeeId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Department))]
        [NotMapped]
        public string Department { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Rpt_Total))]
        [NotMapped]
        public decimal TotalResult { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.PassOrFail))]
        [NotMapped]
        public string PassOrFail { get; set; }


    }
}
