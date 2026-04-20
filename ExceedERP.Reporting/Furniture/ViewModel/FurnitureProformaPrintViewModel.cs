using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Furniture.ViewModel
{
    public class FurnitureProformaItemViewModel
    {        
        public string JobType { get; set; }
        public string UOM { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPriceBeforeVAT { get; set; }
        public decimal VAT { get; set; }
        public decimal TransportCost { get; set; }
        public decimal SellingPrice { get; set; }
    }
    public class FurnitureProformaViewModel : FurnitureCommonViewModel
    {
        public string CustomerName { get; set; }
        public int ProformaNo { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int PriceValidityPeriod { get; set; }
        public string ModeOfPayment { get; set; }
        public decimal TransportCost { get; set; }
        public string VAT { get; set; }
        public decimal VATAmount { get; set; }

        public decimal TransportCostWithVAT { get; set; }
    }
    public class FurnitureTotalViewModel
    {
        public decimal Total { get; set; }
    }
}
