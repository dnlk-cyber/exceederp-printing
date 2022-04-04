using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.PrintingEstimation
{
  public class MaterialCost: TrackUserSettingOperation
    {
       
        public int MaterialCostId { get; set; }

        public int EstimationFormId { get; set; }
        public EstimationForm EstimationForm { get; set; }
        [Display(Name = "Inventory Category")]

        public string InventoryCategoryId { get; set; }
        [Display(Name = "Inventory")]
        public string InventoryId { get; set; }
       
        public int? JobId { get; set; }

        [Required]
        public decimal Quantity { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }
     [ReadOnly(true)]
        public decimal TotalPrice { get; set; }

    }
}
