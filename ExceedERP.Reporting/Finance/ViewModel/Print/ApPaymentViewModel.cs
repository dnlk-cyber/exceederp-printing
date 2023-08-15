using ExceedERP.Reporting.SharedDataSources;
using System;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Finance.ViewModel.Print
{
    public class ApPaymentViewModel : Organization
    {
        public string VoucherId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierId { get; set; }
        public bool IsPrepayment { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal RemainingBalance { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentReference { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal NetAmount { get; set; }
        public bool HasPrepayment { get; set; }
        public decimal PrepaymentAmount { get; set; }
        public string Remark { get; set; }

        //
        public string PreparedBy { get; set; }
        public DateTime? PreparedDate { get; set; }
        public string AccountantCheckedBy { get; set; }
        public DateTime? AccountantCheckedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
    }


}
