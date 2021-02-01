using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.FuelManagement
{
   public  class FuelInventory  : Operations
    {

       public Guid FuelInventoryId { get; set; }
        [Required]
        [DisplayName("Site Name ")]
       public string SiteName { get; set; }
          [Required]
          [DisplayName("Fuel Type ")]
       public string  FuelType { get; set; }
          [Required]
       public decimal Quantity { get; set; }
          [Required]
          [DisplayName("Unit Price ")]
       public decimal UnitPrice { get; set; }
            [DisplayName("Total Price")]
       public decimal TotalPrice { get; set; }
    }
}
