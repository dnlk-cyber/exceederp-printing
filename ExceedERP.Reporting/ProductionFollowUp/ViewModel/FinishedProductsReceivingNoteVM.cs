using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.ProductionFollowUp.ViewModel
{
    public class FinishedProductsReceivingNoteVM : Organization
    {
        public string From { get; set; }
        public string To { get; set; }
        public string DeliveredBy { get; set; }
        public string ReceivedBy { get; set; }
        public List<FinishedProductsReceivingNoteVMItem> FinishedProductsReceivingNoteVMItems { get; set; }

    }

    public class FinishedProductsReceivingNoteVMItem
    {
        public string JobNo { get; set; }
        public string Client { get; set; }
        public string JobType { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalPriceBirr { get; set; }
        public decimal TotalPriceCent { get; set; }
        public string Remark { get; set; }
    }
}
