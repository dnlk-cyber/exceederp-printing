using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.LeaveManagment
{
    public class MonthlyLeavePlanDetail: TrackUserOperation
    {
        public int MonthlyLeavePlanDetailId { get; set; }
        public int MonthlyLeavePlanId { get; set; }
        [Display(Name = "HR_EmployeeDetail_ApplicantionForm_Period", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_BackPayment_GLPeriodId", ResourceType = typeof(Localization.Resources))]
        public int GLPeriodId { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_FixedContribution_Amount", ResourceType = typeof(Localization.Resources))]

        public decimal Amount { get; set; }
        [Display(Name = "HR_SelfCare_TrainingApplicant_Remark", ResourceType = typeof(Localization.Resources))]

        public string Remark { get; set; }
    }
}
