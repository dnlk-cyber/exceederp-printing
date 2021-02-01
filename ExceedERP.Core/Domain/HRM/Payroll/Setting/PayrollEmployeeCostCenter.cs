using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.Payroll.Setting
{
    public class PayrollEmployeeCostCenter
    {
        public int PayrollEmployeeCostCenterId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        [Required]
        [Display(Name = "Employee ID")]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required]
        [Display(Name = "Full Name")]
        public string EmpFullName { get; set; }
        [Required]
        [Display(Name = "Cost Center Code")]
        public string CostCenterAccountCode { get; set; }
        [Required]
        [Display(Name = "Cost Center Description")]
        public string CostCenterAccountDesc { get; set; }
        [Display(Name = "Branch Name")]
        public string OrgBranchName { get; set; }
        public string Status { get; set; }
        public bool isSendForApproval { get; set; }
        public bool isOnlineApproved { get; set; }
        public string OnlineApprovedBy { get; set; }
        public DateTime? OnlineApprovedTime { get; set; }
        public bool isVoid { get; set; }
        public string VoidBy { get; set; }
        public DateTime? VoidTime { get; set; }
    }
}