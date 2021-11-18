using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.ProductionFollowUp.ViewModel
{
    public class FinishedGoodReceivalVM
    {
        public string VoucherNo { get; set; }
        public string Store { get; set; }
        public string JobOrderNo { get; set; }
        public string CustomerName { get; set; }
        public string DeliveredBy { get; set; }
        public string ReceivedBy { get; set; }
        public string Supervisor { get; set; }
        public List<FinishedGoodsVM> FinishedGoods { get; set; }
    }
}
