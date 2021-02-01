using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Budget
{
     public class BudgetUsageApproval : TrackUserOperation
    {
         public int BudgetUsageApprovalId { get; set; }
         public int BudgetUsageId { get; set; }
         public ApprovalStatuses Status { get; set; }
         public string Remark { get; set; }
         public virtual BudgetUsage BudgetUsage { get; set; }

    }
}
