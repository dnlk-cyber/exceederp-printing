using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.printing.PrintingEstimation
{
   public class EstimationForm : TrackUserSettingOperation
    {
        
        public int EstimationFormId {get; set;}
        public int CustomerId { get; set; }

        [Required]
        public DateTime Date { get; set; }
        [Required]
        [Display(Name ="Job Type")]
        public int  JobTypeId { get; set; }
        public string  JT { get; set; }
        [Display(Name = "Job Number")]
        public string JobNumber { get; set; }
        [Required]
        public decimal Quantity { get; set; }
        [Required]
        public string Size { get; set; }
        [Required]
        public string TextPaper { get; set; }
        [Required]
        public string TextPrint { get; set; }
        [Required]
        public string CoverPrint { get; set; }
        [Required]
        public string BindingStyle { get; set; }
        public string Others { get; set; }
        public bool IsChecked { get; set; }
        public string CheckedBy { get; set; }
        public string ApprovedBy { get; set; }
        public bool IsApproved { get; set; }
        public virtual ICollection<MaterialCost> MaterialCosts { get; set; } = new List<MaterialCost>();
        public virtual ICollection<LaborCost> LaborCosts { get; set; } = new List<LaborCost>();





    }
}
