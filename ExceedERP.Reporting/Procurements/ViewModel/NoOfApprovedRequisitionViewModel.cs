using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;

namespace ExceedERP.Reporting.Procurements.ViewModel
{
    public class NoOfApprovedRequisitionViewModel
    {
        public PurchaseItemType ItemType { get; set; }
        public string Branch { get; set; }
        public int? OpenTender { get; set; }
        public int? SelectedTender { get; set; }
        public int? Proforma { get; set; }
        public int? DirectPurchase { get; set; }
    }
}
