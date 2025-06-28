using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CRM.TenderManagement

{
    public class SalesLot : Operations
    {
        public int SalesLotId { get; set; }
        [Display(Name = "Lot Name")]
        public string LotName { get; set; }
        public string Description { get; set; }
        public virtual SaleTender Tender { get; set; }
        public bool IsSendForInvoice { get; set; }
        public int SaleTenderId { get; set; }
        public string BidParticpatePlace { get; set; }
        public int BranchId { get; set; }
        public int BusinessUnitiId { get; set; }
        public string StoreCode { get; set; }
        public virtual ICollection<SalesLotItem> SalesLotItems { get; set; }
        public virtual ICollection<Winner> Winners { get; set; }
        public virtual ICollection<SalesLotPurchase> SalesLotPurchases { get; set; }
    }

}