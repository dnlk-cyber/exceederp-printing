using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CRM.TenderManagement

{

    public class SalesLotItem: TrackUserSettingOperation
    {
        public int SalesLotItemId { get; set; }
        public int ProductCategoryId { get; set; }
        public string ProductCategoryDesc { get; set; }
       // public ProductCategory ProductCategory { get; set; }
        public int? ProductID { get; set; }
        public string ProductName { get; set; }
       // public Product Product { get; set; }
        public decimal Quantity { get; set; }        
        public string UOM { get; set; }        
        public string ProductArea { get; set; }        
        public int? ProductAreaNumber { get; set; }             
        public int SalesLotId { get; set; }
        public virtual SalesLot Lot { get; set; }

    }
}
