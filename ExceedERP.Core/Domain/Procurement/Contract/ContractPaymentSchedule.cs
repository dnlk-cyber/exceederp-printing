using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Procurement.Settings;

namespace ExceedERP.Core.Domain.Procurement.Contract
{
    public class ContractPaymentSchedule: TrackUserOperation
    {
        public int ContractPaymentScheduleId { get; set; }
        public int ContractManagementID { get; set; }
        public virtual ContractManagement ContractManagement { get; set; }
        [Display(Name = "Payment Type")]
        public PaymentType PaymentType { get; set; }
        [Display(Name = "Percentage")]
        public decimal Percentage { get; set; }
        public string Remark { get; set; }
    }
}