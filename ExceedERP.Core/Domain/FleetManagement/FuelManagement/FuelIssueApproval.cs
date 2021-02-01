using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FuelManagement
{
    public class FuelIssueApproval : TrackUserOperation
    {
        public int FuelIssueApprovalID { get; set; }
        public Guid FuelIssueId { get; set; }
        public virtual Fuel Fuel { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }

}