using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Localization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class ApplicantScreening
    {
        public int ApplicantScreeningId { get; set; }
        public int ApplicantDetailId { get; set; }
        public int ApplicantScreenResultId { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Gender))]
        public Gender Sex { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Age))]
        public int Age { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.ApplicantName))]
        public string ApplicantName { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Address))]
        public string Address { get; set; }
        [Display(ResourceType = typeof(Localization.Resources), Name = nameof(Resources.Rpt_TotalResult))]
        [NotMapped]
        public decimal TotalResult { get; set; }
        [NotMapped]
        public string PassOrFail { get; set; } 
     
        public bool IsProfileGenerated { get; set; }
    }
}
