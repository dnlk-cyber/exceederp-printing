using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.RentalPayment
{
    public class PaymentCirtificate : TrackUserSettingOperation
    {
        public int PaymentCirtificateId { get; set; }
        public string Lessor { get; set; }
        public string Lessee { get; set; }
        public string Project { get; set; }
        public string Contract { get; set; }
        public string PlateNumber { get; set; }
        public string Model { get; set; }
        public string Duration { get; set; }
        public decimal MaintContract { get; set; }
        public decimal SupplimentaryContract { get; set; }
        public decimal VariationOrders { get; set; }
        public decimal TotalSum { get; set; }
        public decimal Deduction { get; set; }
        public decimal PreviousPayment { get; set; }
        public decimal Rebate { get; set; }
        public decimal RetentionExcludingVat { get; set; }
        public decimal Penality { get; set; }
        public decimal AdvancePayment { get; set; }
        public decimal ThisMonthDeduction { get; set; }
        public decimal Subtotal { get; set; }
        public decimal NetSumDueToCustomer { get; set; }

        public string PreparedBy { get; set; }
        public DateTime PreparedDate { get; set; }
        public string CheckedBy { get; set; }
        public DateTime CheckedDate { get; set; }
        public bool IsChecked { get; set; }
        public bool IsRejected { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApprovalDate { get; set; }
        public bool IsApproved { get; set; }
        public string AuthorizedBy { get; set; }
        public DateTime AuthorizedDate { get; set; }
        public bool IsAuthorized { get; set; }
        public string DGMApprovedBy { get; set; }
        public DateTime DGMApprovalDate { get; set; }
        public bool IsDGMApproved { get; set; }
    }
}
