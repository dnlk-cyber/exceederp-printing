using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.RentalPayment
{
    public class RentalAdvancePaymentRequest: DepartmentNewOperations
    {
        public int RentalAdvancePaymentRequestId { get; set; }

        [DisplayName("Employee Name")]
        public int EmployeeId { get; set; }
        [DisplayName("Supplier Name")]
        public int? ProcurementSupplierID { get; set; }
        [DisplayName("Period")]
        public int GLPeriodId { get; set; }
        [DisplayName("Fiscal Year")]
        public int GLFiscalYearId { get; set; }
        [DisplayName("Duration From")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DurationFrom { get; set; }
        [DisplayName(" Duration To")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DurationTo { get; set; }
        [DisplayName("Department Name")]
        public int OrgStructureId { get; set; }
        [DisplayName("Branch")]
        public int BranchId { get; set; }
        public int BusinessUnitId { get; set; }
        [DisplayName("Reason of Payment")]
        public string ReasonofPayment { get; set; }
        [DisplayName("Request Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RequestDate { get; set; }
        [DisplayName("Refference")]
        public string Refference { get; set; }
        public bool Paid { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountDue { get; set; }

        public bool IsBudgetApproved { get; set; }
        public string BudgetApprovedBy { get; set; }
        public DateTime? BudgetApprovedTime { get; set; }
        // Finace 
        public bool IsFinanceSendForApproval { get; set; }
        public string FinanceSendForApprovalBy { get; set; }
        public DateTime? FinanceSendForApprovalTime { get; set; }
        public bool IsFinanceApproved { get; set; }
        public string FinanceApprovedBy { get; set; }
        public DateTime? FinanceApprovedTime { get; set; }
        public int AdvancePaymentId { get; set; }
        public string FleetId { get; set; }
        public string PlateNumber { get; set; }
        public DateTime AgreementStartDate { get; set; }
    }
}
