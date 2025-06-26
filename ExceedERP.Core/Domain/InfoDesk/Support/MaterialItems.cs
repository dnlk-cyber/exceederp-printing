using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.InfoDesk.Support
{
   public class MaterialItems
    {
        public int MaterialItemsId { get; set; }
        public string ItemName { get; set; }
        [Display(Name ="Category")]
        public string  CategoryId { get; set; }
        [Display(Name = "Inventory-Item")]
        public string  InventoryItemId { get; set; }
        public string Remark { get; set; }
    }
}
