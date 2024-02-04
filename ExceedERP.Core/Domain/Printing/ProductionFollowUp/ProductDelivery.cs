using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing.ProductionFollowUp
{
   public class ProductDelivery : Operations
    {
        public int ProductDeliveryId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerRefNo { get; set; }
        public string CreditSaleNo { get; set; }
        public string CashSaleNo { get; set; }
        public decimal Cash { get; set; }
        public decimal Credit { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string PreparedBy { get; set; }
        public string ApprovedBy { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; }

        public string IdentificationNo { get; set; }
    }
    public enum DeliveryStatus
    {
        Delivered,
        Voided

    }
}
