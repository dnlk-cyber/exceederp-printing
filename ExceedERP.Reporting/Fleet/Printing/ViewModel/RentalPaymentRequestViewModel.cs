using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
    public class RentalPaymentRequestViewModel : Organization
    {
        public int Id { get; set; }

        public string EmployeeName { get; set; }
        public string Departnment { get; set; }
        public string Branch { get; set; }
        public string SupplierName { get; set; }
        public string ReasonForPayment { get; set; }
        [DisplayName("Request Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RequestDate { get; set; }
        public string Refference { get; set; }
        public string EquipmentType { get; set; }
        public string EquipmentName { get; set; }
        public string Period { get; set; }
        [DisplayName("Request Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DurationFrom { get; set; }
        [DisplayName("Request Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DurationTo { get; set; }
        public decimal Amount { get; set; }
        public decimal Tax { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal FuelAmount { get; set; }
        public decimal Commusion { get; set; }
        public decimal AdvancedAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string RequestBy { get; set; }
        public string PreparedBy { get; set; }
        public DateTime PreparedOn { get; set; }
        public string DepartmentApprovedBy { get; set; }
        public DateTime DepartmentApprovedOn { get; set; }
        public string FinalApproval { get; set; }
        public DateTime FinalApprovalOn { get; set; }
        public string BudgetApprovalBy { get; set; }
        public DateTime BudgetApprovalOn { get; set; }

        public DateTime ApprovedDate { get; set; }
        public string ApprovalBy { get; set; }
    }
}
