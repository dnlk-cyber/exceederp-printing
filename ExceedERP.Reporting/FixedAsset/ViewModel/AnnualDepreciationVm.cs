using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.FixedAsset.ViewModel
{
  
    public class AnnualDepreciationVm 
    {
        [Display(Name = "Tag")]
        public string Tag { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [DisplayName("Original Value")]
        public decimal OriginalValue { get; set; }
        [DisplayName("Depreciation Amount")]
        public decimal DepreciationAmount { get; set; }
        [Display(Name = "Book Value")]
        public decimal BookValue { get; set; }
        [DisplayName("Accum. Depre. Amount")]
        public decimal AccumulatedDepreciationAmount { get; set; }
    }
}
