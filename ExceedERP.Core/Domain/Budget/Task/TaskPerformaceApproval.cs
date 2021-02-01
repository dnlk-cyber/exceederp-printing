using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Budget.Task
{
    public class TaskPerformaceApproval : TrackUserOperation
    {

        public int TaskPerformaceApprovalId { get; set; }
        public int TaskPerformaceId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }

      
    }
}
