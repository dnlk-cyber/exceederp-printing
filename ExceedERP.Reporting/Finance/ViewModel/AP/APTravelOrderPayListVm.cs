
using System;

namespace ExceedERP.Reporting.Finance.ViewModel
{
    public class APTravelOrderPayListVm
    {
        public string TravelId { get; set; }
        public string DirectPayId { get; set; }     
        public string EmployeeCode { get; set; }
        public string EmployeeFullName { get; set; }
        public string SupplierName { get; set; }
        public string WorkUnit { get; set; }
        public string BankAccount { get; set; }
        public DateTime RequestDate { get; set; }
        public decimal Amount { get; set; }
        public string Paid { get; set; }
    }
}
