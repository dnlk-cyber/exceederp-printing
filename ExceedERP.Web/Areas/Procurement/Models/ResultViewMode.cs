using ExceedERP.Core.Domain.Procurement.Proforma;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.Core.Domain.Procurement.Tender.Lots;

namespace ExceedERP.Web.Areas.Procurement.Models
{
    public class ResultViewMode
    {
        public int ResultViewModeId { get; set;  }
        public float Financial { get; set;  }
        public float Technical { get; set;  }
        public PassFailStatus Pass { get; set; }
        public float Total { get; set;  }
        public string Supplier { get; set;  }
        public string ItmCode { get; set;  }
        public int SupplierId { get; set;  }
        public int LotId { get; set;  }
        public Lot Lot { get; set; }
    }
}