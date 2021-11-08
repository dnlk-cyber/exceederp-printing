using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.PrintingEstimation
{
    public class Customer
    {
        public int CustomerId { get; set; }
        [Required]
       
        public string Name { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }
        [Display(Name = "P.O. Box")]
        public string City { get; set; }
        public string SubCity { get; set; }
        public string Woreda { get; set; }
        public string Pobox { get; set; }
        public decimal? VAT { get; set; }
        public decimal? Margin { get; set; }
        public decimal? OverHead { get; set; }
        public string Description { get; set; }
        public ICollection<EstimationForm> EstimationForms { get; set; }

    }
}
