using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.PrintingEstimation
{
   public class EstimationForm : TrackUserSettingOperation
    {
        
        public int EstimationFormId {get; set;}
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string JobNumber { get; set; }
        [Required]
        public decimal Quantity { get; set; }
        [Required]
        public string Size { get; set; }
        [Required]
        public decimal TextPaper { get; set; }
        [Required]
        public decimal TextPrint { get; set; }
        [Required]
        public string CoverPrint { get; set; }
        [Required]
        public string BindingStyle { get; set; }
        public string Others { get; set; }
        public virtual ICollection<MaterialCost> MaterialCosts { get; set; } = new List<MaterialCost>();
        public virtual ICollection<LaborCost> LaborCosts { get; set; } = new List<LaborCost>();





    }
}
