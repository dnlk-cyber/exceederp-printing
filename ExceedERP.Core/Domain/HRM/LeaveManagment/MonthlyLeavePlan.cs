using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.LeaveManagment
{
    public class MonthlyLeavePlan : TrackUserOperation
    {
        public int MonthlyLeavePlanId { get; set; }
        [Display(Name = "Employee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        [Display(Name = "HR_EmployeeDetail_ApplicantionForm_Period", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        
    }
}
