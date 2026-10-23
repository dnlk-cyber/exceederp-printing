using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Budget.ViewModel
{
    public class BudgetItemVM
    {
        public string BudgetCode { get; set; }
        public string BudgetItem { get; set; }
        public decimal Budget { get; set; }
        public decimal Actual { get; set; }
    }

    public class BudgetCategoryVM
    {
        public string Name { get; set; }
        public decimal BudgetTotal { get; set; }
        public decimal ActualTotal { get; set; }
        public List<BudgetItemVM> Items { get; set; }
    }

    public class BudgetTypeVM
    {
        public string Name { get; set; }
        public decimal BudgetTotal { get; set; }
        public decimal ActualTotal { get; set; }
        public List<BudgetCategoryVM> Categories { get; set; }
    }

}
