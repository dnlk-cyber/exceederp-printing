using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp
{
   public class FurnitureFinishedGoodReceiving
    {
        public int FurnitureFinishedGoodReceivingId { get; set; }
        public int FurnitureCustomerId { get; set; }
        public string VoucherNo { get; set; }
        public string Store { get; set; }
        public  int FurnitureJobId { get; set; }
        public string DeliveredBy { get; set; }
        public string ReceivedBy { get; set; }
        public string Supervisor { get; set; }
    }
}
