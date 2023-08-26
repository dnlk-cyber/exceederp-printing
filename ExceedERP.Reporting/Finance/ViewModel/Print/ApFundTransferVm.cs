using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Finance.ViewModel.Print
{

    public class ApFundTransferVm : Organization

    {
        public string VoucherId { get; set; }
        public string ReasonOfTransfer { get; set; }
        public DateTime RequestDate { get; set; }
        public string Branch { get; set; }
        public string FromBranch { get; set; }
        public string FromBusinessUnit { get; set; }
        public string BankAccount { get; set; }
        public string WorkUnit { get; set; }
        public string Employee { get; set; }
        public string PreparedBy { get; set; }
        public DateTime? PreparedDate { get; set; }
        public Image PreparedBySignature { get; set; }
        public string DepartmentApprovedBy { get; set; }
        public DateTime? DepartmentApprovedDate { get; set; }
        public Image DepartmentApproverSignature { get; set; }
        public string DepartmentSecondApprovedBy { get; set; }
        public DateTime? DepartmentSecondApprovedDate { get; set; }
        public Image DepartmentSecondApproverSignature { get; set; }
        public string AccountantCheckedBy { get; set; }
        public DateTime? AccountantCheckedDate { get; set; }
        public Image AccountantCheckerSignature { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public Image ApproverSignature { get; set; }
        public string BudgetApprovedBy { get; set; }
        public DateTime? BudgetApprovedDate { get; set; }
        public Image BudgetSignature { get; set; }
        public decimal Total { get; set; }
    }


    public class FundTransferDetailVm
    {
        public string Description { get; set; }
        public decimal Requested { get; set; }
        public decimal Approved { get; set; }
        public string Remark { get; set; }
    }

}
