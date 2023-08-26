using System;
using System.Drawing;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Finance.ViewModel.Print
{
    public class ApMiscPaymentViewModel : Organization

    {
        public string VoucherId { get; set; }
        public string DocTitile { get; set; }
        public string SupplierName { get; set; }
        public decimal Amount { get; set; }
        public decimal NetPayable { get; set; }
        public string PaymentReference { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentRequest { get; set; }
        public string Remark { get; set; }
        public string PreparedBy { get; set; }
        public DateTime? PreparedDate { get; set; }
        public Image PreparedBySignature { get; set; }
        public string AccountantCheckedBy { get; set; }
        public DateTime? AccountantCheckedDate { get; set; }
        public Image AccountantCheckerSignature { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public Image ApproverSignature { get; set; }
    }
}
