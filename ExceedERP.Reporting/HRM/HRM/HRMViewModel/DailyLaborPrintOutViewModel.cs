using System;
using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class ApproverInfo
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string JobTitle { get; set; }
        public byte[] Signature { get; set; }
        public DateTime? ApprovedDate { get; set; }
    }

    public class DailyLaborPrintOutViewModel
    {
        // Master Header Details
        public int DailyLaborerId { get; set; }
        public string BranchName { get; set; }
        public string FiscalYearName { get; set; }
        public string PeriodName { get; set; }
        public bool IsOnlineApproved { get; set; }
        public bool IsRejected { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }

        // Resolved Approver Profiles
        public ApproverInfo PreparedByHR { get; set; } = new ApproverInfo();
        public ApproverInfo CheckedBySite { get; set; } = new ApproverInfo();
        public ApproverInfo ApprovedByMaterial { get; set; } = new ApproverInfo();
        public ApproverInfo CheckedByOffice { get; set; } = new ApproverInfo();
        public ApproverInfo ApprovedByHRA { get; set; } = new ApproverInfo();
        public ApproverInfo AuthorizedBy { get; set; } = new ApproverInfo();
        public ApproverInfo RejectedBy { get; set; } = new ApproverInfo();
        public string RejectionReason { get; set; }

        public List<DailyLaborerEmployeeRowVM> LaborerEmployees { get; set; } = new List<DailyLaborerEmployeeRowVM>();
    }

    public class DailyLaborerEmployeeRowVM
    {
        public int DailyLaborerEmployeeId { get; set; }
        public string EmployeeFullName { get; set; }
        public string BankAccountNumber { get; set; }
        public string JobTitle { get; set; }
        public decimal NormalDays { get; set; }
        public decimal DayWage { get; set; }
        public decimal DayOT { get; set; }
        public decimal TotalDays { get; set; }
        public decimal NormalNight { get; set; }
        public decimal NightWage { get; set; }
        public decimal NightOT { get; set; }
        public decimal NightOTWage { get; set; }
        public decimal RestTimeOT { get; set; }
        public decimal RestTimeOTWage { get; set; }
        public decimal HolidayOT { get; set; }
        public decimal HolidayOTWage { get; set; }
        public decimal TotalNights { get; set; }
        public decimal TotalWage { get; set; }
        public decimal IncomeTax { get; set; }
        public decimal NetPay { get; set; }
    }
}