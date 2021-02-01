using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;

namespace ExceedERP.Reporting.Procurements.ViewModel
{
   public class StockAndNonStockPurchaseVM
    {      
        public int PurchaseRequisitionID { get; set; }
        public PurchaseItemType ItemType { get; set; }      
        public int BranchId { get; set; }        
        public int OrgStructureId { get; set; }         
        public string ReferenceNo { get; set; }
        public DateTime GenerateDate { get; set; }
        public DateTime? ApprovedDate { get; set; }

    }
}
