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
  
    public class ApTravelOrderVm : Organization

    {
        public string VoucherId { get; set; }
        public string EmployeeName { get; set; }
        public decimal Salary { get; set; }
        public string Department { get; set; }
        public string WorkPlace { get; set; }
        public string PurposeOfTravel { get; set; }
        public DateTime RequestDate { get; set; }
        public decimal AdvanceAmount { get; set; }
        public string Reference { get; set; }
        public string PreparedBy { get; set; }
        public string DepartmentApprovedBy { get; set; }
        public string ApprovedBy { get; set; }
    }


    public class ApTravelAdvanceLineVm
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
        [Required]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }
    }

    public class ApTravelDepartureLineVm
    {
        [Display(Name = "Type")]
        public string Type { get; set; }
        [Display(Name = "From")]
        public string From { get; set; }
        [Display(Name = "To")]
        public string To { get; set; }
        [Display(Name = "Date")]
        public DateTime Date { get; set; }
        [Display(Name = "Time")]
        public DateTime Time { get; set; }

        [Display(Name = "Mode of Transport")]
        public string TransportMode { get; set; }
        public string Remark { get; set; }
    }
}
