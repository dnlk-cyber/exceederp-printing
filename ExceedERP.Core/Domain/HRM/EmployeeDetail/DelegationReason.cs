using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class DelegationReason
    {
        public int DelegationReasonId { get; set; }
        public int DelegationId { get; set; }
        [DataType(DataType.MultilineText)]
        public string Reason { get; set; }
    }
}
