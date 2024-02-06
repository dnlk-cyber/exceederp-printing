using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp
{
    public class FurnitureJobType
    {
        public int FurnitureJobTypeId { get; set; }
        public string Type { get; set; }
        [Display(Name ="Tax Type")]
        public int GLTaxRateID { get; set; }
        public decimal VAT { get; set; }
        public decimal Margin { get; set; }

    }
    public class FurnitureJobTypeItem
    {
        public int FurnitureJobTypeItemId { get; set; }
        public int FurnitureJobTypeId { get; set; }
        public string Name { get; set; }
    }
}
