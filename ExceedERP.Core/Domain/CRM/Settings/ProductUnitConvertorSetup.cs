using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CRM
{
    public class ProductUnitConvertorSetup : TrackUserOperation
    {
        public int ProductUnitConvertorSetupId { get; set; }

        [Display(Name ="Product")]
        public int ProductId { get; set; }

        [Display(Name ="From")]
        public string FromUoM { get; set; }

        [Display(Name ="To")]
        public string ToUoM { get; set; }

        [Display(Name ="Multiply by")]
        public decimal MultiplicationFactor { get; set; }
    }
}
