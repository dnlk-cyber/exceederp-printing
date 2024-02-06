using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp
{
   public class FurnitureProductDelivery : Operations
    {
        public int FurnitureProductDeliveryId { get; set; }
        public int FurnitureCustomerId { get; set; }
        public string CustomerRefNo { get; set; }
        public string CreditSaleNo { get; set; }
        public string CashSaleNo { get; set; }
        public decimal Cash { get; set; }
        public decimal Credit { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string PreparedBy { get; set; }
        public string ApprovedBy { get; set; }
        public FurnitureDeliveryStatus DeliveryStatus { get; set; }

        public string IdentificationNo { get; set; }
    }
    public enum FurnitureDeliveryStatus
    {
        Delivered,
        Voided

    }
}
