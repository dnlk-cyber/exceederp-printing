using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.FixedAsset.ViewModel
{
  
    public class DepreciationExpenseVm
    {
        public string Category { get; set; }
        public decimal OriginalValue { get; set; }
        public decimal TotalDepForYear { get; set; }
        public decimal TotalDepForQuarter { get; set; }
        public decimal BookValueAtEnd { get; set; }
        public ICollection<DepreciationExpenseLineVm> DepreciationExpenseLineVms { get; set; }
    }

    public class DepreciationExpenseLineVm
    {
        public DateTime? PurchaseDate { get; set; }
        public string Reference { get; set; }
        [DisplayName("Original Cost")]
        public decimal OriginalValue { get; set; }
        [DisplayName("No. of Months")]
        public int NoOfMonths { get; set; }
        [DisplayName("No. of Days")]
        public int NoOfDays { get; set; }
        public decimal Rate { get; set; }
       
        [DisplayName("Total Depreciation for the Year")]
        public decimal TotalDepForYear { get; set; }
        [DisplayName("Expense for the Quarter ")]
        public decimal TotalDepForQuarter { get; set; }
        [DisplayName("Book Value")]
        public decimal BookValueAtEnd { get; set; }
    }
}
