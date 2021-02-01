using ExceedERP.Core.Domain.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.APTS
{
   public class DrugLists
    {
        public int DrugListsId { get; set; }
        [Display(Name = "Drug Code")]
        public string DrugCode { get; set; }
        [Display(Name = "Internal Drug Code")]
        public string InternalDrugCode { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }
        [Display(Name = "Is Our?")]
        public bool IsOur { get; set; }
        public DateTime? DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public string ItemCategoryCode { get; set; }
        public ItemCategory ItemCategory { get; set; }
        public string ItemCode { get; set; }
        public InventoryItem InventoryItem { get; set; }
    }
}
