using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Budget.Task
{
    public class TaskPlanApproval : TrackUserOperation
    {

       public int TaskPlanApprovalId { get; set; }
        public int BudgetPlanId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }

      
    }
}
