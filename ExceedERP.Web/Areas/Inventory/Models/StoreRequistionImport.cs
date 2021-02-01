using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExceedERP.Web.Areas.Inventory.Models
{
    public class StoreRequistionImport
    {
        public string No { get; set;  }
        public string StoreCode { get; set;  }
        public string WorkUnit { get; set;  }
        public string Requestor { get; set;  }
        public DateTime RequestDate { get; set;  }
        public string ItemCategoryCode { get; set; }
        public string ItemCode { get; set;  }
        public string PartNo { get; set; }
        public decimal RequestedQuantity { get; set;  }
        public decimal ApprovedQuantity { get; set; }
        public decimal IssuedQuantity { get; set; }
        public string DocumentReference { get; set;  }

    }

    public class FixedAssetRequistionImport
    {
        public string No { get; set; }
        public string StoreCode { get; set; }
        public string WorkUnit { get; set; }
        public string Requestor { get; set; }
        public DateTime RequestDate { get; set; }
        public string ItemCategoryCode { get; set; }
        public string ItemCode { get; set; }
        public string PartNo { get; set; }
        public int RequestedQuantity { get; set; }
        public int ApprovedQuantity { get; set; }
        public int IssuedQuantity { get; set; }
        public string DocumentReference { get; set; }

    }
}