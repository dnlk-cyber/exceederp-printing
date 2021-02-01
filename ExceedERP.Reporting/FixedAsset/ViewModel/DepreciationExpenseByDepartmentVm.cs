using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.FixedAsset.ViewModel
{
  
    public class DepreciationExpenseByDepartmentVm
    {
        public string Category { get; set; }
        public decimal Beginning { get; set; }
        public decimal TotalDepExpense { get; set; }
        public decimal PoHDepExpenseTv { get; set; }
        public decimal PoHDepExpenseRadio { get; set; }
        public decimal MarketingDepExpenseTv { get; set; }
        public decimal MarketingDepExpenseRadio { get; set; }
        public decimal AdminDepExpenseTv { get; set; }
        public decimal AdminDepExpenseRadio { get; set; }
    }

    
}
