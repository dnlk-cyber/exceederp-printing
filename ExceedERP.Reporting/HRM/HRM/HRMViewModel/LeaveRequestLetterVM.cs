using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
   public class LeaveRequestLetterVM
    {
        public string FullName { get; set; }
        public string Year { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string LeaveDays { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string AmharicName { get; set; }
        public string Abbreviation { get; set; }
        public string Address { get; set; }
        public Image LogoFileName { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public string City { get; set; }
        public string Fax { get; set; }
        public string Pobox { get; set; }
        public string Tel { get; set; }
        public string Date { get; set; }
        public string LeaveType { get; set; }
        public string Approver { get; set; }
        public Image ApproverSign { get; set; }
        public string DepApprover { get; set; }
        public Image DepApproverSign { get; set; }
        public decimal TotalLeaveBalance { get; set; }
        public decimal TotalLeaveTaken { get; set; }
        public decimal RemainingBalance { get; set; }
        public string Remark { get; set; }
        public List<LeaveRequestDetails> RequestDetails { get; set; }


    }

    public class LeaveRequestDetails
    {
        public string FiscalYear { get; set; }
        public decimal Balance { get; set; }

    }
}
