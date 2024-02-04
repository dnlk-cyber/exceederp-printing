using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing.ProductionFollowUp
{
    public class JobType
    {
        public int JobTypeId { get; set; }
        public string Type { get; set; }
        [Display(Name ="Tax Type")]
        public int GLTaxRateID { get; set; }
        public decimal VAT { get; set; }
        public decimal Margin { get; set; }

    }
    public class JobTypeItem
    {
        public int JobTypeItemId { get; set; }
        public int JobTypeId { get; set; }
        public string Name { get; set; }
    }
}
