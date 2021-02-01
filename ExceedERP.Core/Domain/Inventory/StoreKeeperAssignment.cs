using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Inventory
{
    public class StoreKeeperAssignment:TrackUserSettingOperation
    {
        public int StoreKeeperAssignmentID { get; set; }
        [Required]
        [Display(Name = "Store Code")]
        public string StoreCode { get; set; }
        public virtual InventoryStore InventoryStore { get; set; }
        [Required]
        [Display(Name = "Store Keeper")]
        public string StoreKeeper { get; set; }
       
    }
  


}