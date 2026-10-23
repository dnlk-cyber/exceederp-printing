using ExceedERP.Core.Domain.Budget;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Budget.ViewModel
{
    public class BudgetPlanViewModel 
    {   
        public string AccountCode { get; set; } 
        public string BudgetCategory { get; set; }
        public string AccountDesc { get; set; }
        public string BusinessUnit { get; set; }
        public string Branch { get; set; }
        public string CostCenter { get; set; }
        public string BudgetItem { get; set; }
        public decimal ThisYearBudget { get; set; }
        public decimal CurrentBudget { get; set; }        
    }
}
