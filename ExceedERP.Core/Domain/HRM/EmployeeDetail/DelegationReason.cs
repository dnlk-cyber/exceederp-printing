using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
