using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Finance.ViewModel.Print
{
  
    public class ApPaymentRequestVm : Organization

    {
        public string VoucherId { get; set; }
        public string Supplier { get; set; }
        public string Purchaser { get; set; }
        public string ReasonOfPayment { get; set; }
        public DateTime RequestDate { get; set; }
        public string InvoiceNo { get; set; }
        public string PurchaseRequestNo { get; set; }
        public string PONo { get; set; }
        public string GRN { get; set; }
        public string Branch { get; set; }
        public string WorkUnit { get; set; }
        public string Employee { get; set; }
        public string PreparedBy { get; set; }
        public DateTime? PreparedDate { get; set; }
        public string DepartmentApprovedBy { get; set; }
        public DateTime? DepartmentApprovedDate { get; set; }
        public string DepartmentSecondApprovedBy { get; set; }
        public DateTime? DepartmentSecondApprovedDate { get; set; }
        public string AccountantCheckedBy { get; set; }
        public DateTime? AccountantCheckedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string BudgetApprovedBy { get; set; }
        public DateTime? BudgetApprovedDate { get; set; }
        public decimal Total { get; set; }
    }


    public class ApPaymentRequestLineVm
    {
        public string ItemName { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string TaxName { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Remark { get; set; }
    }

    public class FundTransferLineVm
    {        
        public string BranchName { get; set; }
        public decimal Material { get; set; }
        public decimal FuelAndLubricant { get; set; }
        public decimal ManPower { get; set; }
        public decimal Machinery { get; set; }
        public decimal CompensationAndSeverance { get; set; }
        public decimal FoodAndPerDiem { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalApproved { get; set; }
        public decimal TotalPermited { get; set; }
        public string Remark { get; set; }
    }
}
