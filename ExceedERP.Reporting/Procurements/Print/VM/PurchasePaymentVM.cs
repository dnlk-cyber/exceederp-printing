using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace ExceedERP.Reporting.Procurements.Print.VM
{
    public class PurchasePaymentVM: Organization
    {
        public int PurchasePaymentId { get; set; }
        public DateTime Date { get; set; }
        public string SupplierInvoice { get; set; }
        public decimal PaymentAmount { get; set; }
        public bool ApplyPenality { get; set; }
        public decimal PenalityAmount { get; set; }
        public decimal NetPayable { get; set; }
        public decimal AmountDue { get; set; }
        public string Paid { get; set; }
        public bool WithOutGRN { get; set; }
        public string SupplierName { get; set; }
        public string Type { get; set; }
        public string PurchasePaymentType { get; set; }
        public string PoOrContractNumber { get; set; }
        public int? ProcurementOfficer { get; set; }
        public int? ProcurementAnalyzer { get; set; }
        public string PurposeOfPayment { get; set; }
        public string PaymentType { get; set; }
        public string Branch { get; set; }
        public string CreatedBy { get; set; }
        public string CreateDate { get; set; }
        public Image CreatedBySignature { get; set; }
        public string Checker { get; set; }
        public string CheckedDate { get; set; }
        public Image CheckerSignature { get; set; }
        public string Approver { get; set; }
        public string ApprovedDate { get; set; }
        public Image ApproverSignature { get; set; }
        public string BudgetApprover { get; set; }
        public DateTime? BudgetApprovedDate { get; set; }
        public Image BudgetSignature { get; set; }
        public string AccountantCheckedBy { get; set; }
        public DateTime? AccountantCheckedDate { get; set; }
        public Image AccountantCheckerSignature { get; set; }
        public string FinanceApprovedBy { get; set; }
        public DateTime? FinanceApprovedDate { get; set; }
        public Image FinanceApproverSignature { get; set; }
    }
}