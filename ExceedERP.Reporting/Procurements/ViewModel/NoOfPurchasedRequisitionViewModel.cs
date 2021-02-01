using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;

namespace ExceedERP.Reporting.Procurements.ViewModel
{
    public class NoOfPurchasedRequisitionViewModel
    {
        public PurchaseItemType ItemType { get; set; }
        public string Branch { get; set; }
        public int RequisitionNo { get; set; }
        public string ReferenceNo { get; set; }
        public int? OpenTender { get; set; }
        public int? PendingTenders { get; set; }
        public int? SelectedTender { get; set; }
        public string Workunit { get; set; }
        public int? Proforma { get; set; }
        public int? DirectPurchase { get; set; }
        public DateTime GenerateDate { get; set; }
        public string ApprovedDate { get; set; }
    }

    public class PrViewModel
    {
        
        public string PrNo { get; set; }
        public string Remark { get; set; }
    }
}
