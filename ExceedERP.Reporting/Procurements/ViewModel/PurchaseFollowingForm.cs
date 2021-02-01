using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;

namespace ExceedERP.Reporting.Procurements.ViewModel
{
    public class PurchaseFollowingForm
    {
        public string Branch { get; set; }
        public string RequisitionNo { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public PurchaseItemType ItemType { get; set; }
        public decimal? ItemAmount { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public DateTime TransportPaymentSendingDate { get; set; }
        public DateTime TransportPanymentDate { get; set; }
        public DateTime ItemPaymentSendingDate { get; set; }
        public int? ItemRecieptNo { get; set; }
        public int NoOfItemRecieved { get; set; }
        public int ItemLeft { get; set; }
        public decimal Vat { get; set; }
        public decimal SubTotal { get; set; }
        public string Remark { get; set; }
    }
}
