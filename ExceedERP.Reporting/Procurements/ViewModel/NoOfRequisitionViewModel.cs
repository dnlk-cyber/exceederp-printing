using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;

namespace ExceedERP.Reporting.Procurements.ViewModel
{
   public class NoOfRequisitionViewModel
    {
        public PurchaseItemType ItemType { get; set; }
        public string Branch { get; set; }
        public string ReferenceNo { get; set; }
        public int RequisitionNo { get; set; }
        public string Workunit { get; set; }
    }
}
