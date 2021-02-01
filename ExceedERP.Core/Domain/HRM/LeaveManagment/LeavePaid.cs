using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.HRM.LeaveManagment
{
    public class LeavePaid:TrackUserOperation 

    {
        [Key ]
        public int LeavePaidId { get; set; }
        [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
        [Display(Name = "HR_LeaveManagment_LeavePaid_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        [Required ]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required ]
        [Display(Name = "HR_LeaveManagment_LeavePaid_LeavePeriod", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        [Required ]
        [Display(Name = "HR_LeaveManagment_LeavePaid_Amount", ResourceType = typeof(Localization.Resources))]
        public double Amount { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }


    }
}