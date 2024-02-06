using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.JobCosting
{
    public class FurnitureTax
    {
        public int FurnitureTaxID { get; set; }
        public string RecordedBy { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RecordTime { get; set; }
        [Required]
        [Display(Name = "Tax Description")]
        public string TaxDescription { get; set; }
        [Required]
        [Display(Name = "Tax Rate(%)")]
        public float TaxRate { get; set; }
    }
 
    }
