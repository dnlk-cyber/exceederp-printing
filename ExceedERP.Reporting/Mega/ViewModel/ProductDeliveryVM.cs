using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Mega.ViewModel
{
   public class ProductDeliveryVM
    {
        public string Tel1 { get; set; }
        public string POBox { get; set; }
        public string CustomerRefNo { get; set; }
        public string CustomerName { get; set; }
        public string ContactName { get; set; }
        public DateTime Date { get; set; }
        public string CreditSaleNo { get; set; }
        public string CashSaleNo { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Tel2 { get; set; }
        public string PrepareBy { get; set; }
        public string ApprovedBy { get; set; }
        public string IdentificationNo { get; set; }
        public List<ProductDeliveryItemsVM> DeliveryItems { get; set; }
    }
}
