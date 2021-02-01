 using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Budget
{
    public class RequestedMonth : Operations
    {

        public int RequestedMonthID { get; set; }
        public int BudgetRequestID { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }
}
