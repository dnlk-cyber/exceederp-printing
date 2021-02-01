using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
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
        public string DepartmentApprovedBy { get; set; }        
        public string ApprovedBy { get; set; }
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
}
