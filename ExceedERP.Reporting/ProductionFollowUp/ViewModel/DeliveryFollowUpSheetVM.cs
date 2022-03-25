using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.ProductionFollowUp.ViewModel
{
   public class DeliveryFollowUpSheetVM : Organization
    {
        
        public string From { get; set; }
        public string To { get; set; }
      
        public List<DeliveryFollowUpSheetItemsVM> DeliveryFollowUpSheetItemsVMs { get; set; }
    }
    public class DeliveryFollowUpSheetItemsVM
    {
        public string Code { get; set; }
        public string DoNo { get; set; }
        public string Date { get; set; }
        public string Customer { get; set; }
        public string Credit { get; set; }
        public string FS { get; set; }
        public decimal Amount { get; set; }
        public decimal VAT { get; set; }
        public decimal Total { get; set; }
        public decimal GrandTotal { get; set; }
      
    }
}
