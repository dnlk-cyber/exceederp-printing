using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.ProductionFollowUp.ViewModel
{
   public class ProductDeliveryVM : Organization
    {
        public string Tel1 { get; set; }
        public string CustomerRefNo { get; set; }
        public string CustomerName { get; set; }
        public string ContactName { get; set; }
        public string BrandName { get; set; }
        public string CreditSaleNo { get; set; }
        public string CashSaleNo { get; set; }
        public string Tel2 { get; set; }
        public string PrepareBy { get; set; }
        public string ApprovedBy { get; set; }
        public string DeliveryDate { get; set; }
        public string IdentificationNo { get; set; }
        public List<ProductDeliveryItemsVM> DeliveryItems { get; set; }
    }
}
