using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Budget
{

    public class BudgetRequestTransferDistribution : VoucherDistribution
    {

        public int BudgetRequestTransferDistributionId { get; set; }
        public int BudgetRequestId { get; set; }
        public virtual BudgetRequest BudgetRequest { get; set; }
    }
 
}
