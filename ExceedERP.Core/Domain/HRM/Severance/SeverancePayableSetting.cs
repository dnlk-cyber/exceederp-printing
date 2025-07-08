using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.Severance
{
    public class SeverancePayableSetting: TrackUserOperation
    {
        [Key]
        public int SeverancePayableSettingId { get; set; }
        [Display(Name = "Responsibility Level")]
        public ResponsibilityLevel ResponsibilityLevel { get; set; }
        [Display(Name = "Salary Value in %")]
        public decimal SalaryValue { get; set; }
        public string Description { get; set; }
    }
   
}
