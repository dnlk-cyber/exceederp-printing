using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Finance.ViewModel.Print
{
  
    public class ApTravelExpenseClaimVm : Organization

    {
        public string VoucherId { get; set; }
        public string ApTravelOrderId { get; set; }
        public string EmployeeName { get; set; }
        public decimal Salary { get; set; }
        public string Department { get; set; }
        public string WorkPlace { get; set; }
        public string PurposeOfTravel { get; set; }
        public DateTime ClaimDate { get; set; }
        public string Reference { get; set; }
        public string PreparedBy { get; set; }
        public string DepartmentApprovedBy { get; set; }
        public string ApprovedBy { get; set; }

    }
    public class ApTravelExpenseExpenseLineVm
    {
        public TravelPaymentType Type { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "UoM")]
        public string UoM { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }

        [Display(Name = "Total")]
        public decimal Total { get; set; }

    }

    public class ApTravelPaymentLineVm
    {
        
        [Display(Name = "Description")]
        public string Description { get; set; }
        [DisplayName("Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }
        public string Reference { get; set; }
    }

    public class ApTravelExpenseRefundLineVm
    {
        [Display(Name = "Description")]
        public string Description { get; set; }
        [DisplayName("Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }
        public string Reference { get; set; }
    }
}
