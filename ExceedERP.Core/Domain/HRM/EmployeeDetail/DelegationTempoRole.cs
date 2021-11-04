using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
   public class DelegationTempoRole
    {
        public int DelegationTempoRoleId { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string RoleName { get; set; }
    }
}
