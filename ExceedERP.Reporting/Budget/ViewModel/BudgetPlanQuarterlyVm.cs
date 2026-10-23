using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Budget.ViewModel
{
    public class BudgetPlanByQuarterVM
    {
        public string BudgetType { get; set; }
        public decimal Quarter1 { get; set; }
        public decimal Quarter2 { get; set; }
        public decimal Quarter3 { get; set; }
        public decimal Quarter4 { get; set; }
        public decimal Total { get; set; }
        public List<BudgetPlanByQuarterCategoryVM> BudgetPlanByQuarterCategoryVMs { get; set; }
    }

    public class BudgetPlanByQuarterCategoryVM
    {
        public string BudgetType { get; set; }
        public string BudgetCategory { get; set; }
        public string BudgetItem { get; set; }
        public string CostCenter { get; set; }
        public decimal Quarter1 { get; set; }
        public decimal Quarter2 { get; set; }
        public decimal Quarter3 { get; set; }
        public decimal Quarter4 { get; set; }
        public decimal Total { get; set; }
        
    }

   

}
