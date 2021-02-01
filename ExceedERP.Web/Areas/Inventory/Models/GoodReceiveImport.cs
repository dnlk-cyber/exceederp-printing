using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExceedERP.Web.Areas.Inventory.Models
{
    public class GoodReciveImport
    {
        public string ReciveNo { get; set; }
        public int ReceiveType { get; set;  }
        public string ReferenceId { get; set;  }
        public string Store { get; set;  }
        public string SupplierCode { get; set;  }
        public string SupplierInvoice { get; set;  }
        public DateTime ReciveDate { get; set;  }
        public string ItemCode { get; set;  }
        public decimal Quantity { get; set;  }
        public string ReferenceDocument { get; set;  }
    }
}