using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public class TechnicalJobApproval : TrackUserOperation
    {
        [Key]
        public int TechnicalJobApprovalID { get; set; }
        public Guid TechnicalJobId { get; set; }
      //  public virtual JobOrder TechnicalJob { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }


}
