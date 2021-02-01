using System.Collections.Generic;

namespace ExceedERP.Reporting.Procurements.ViewModel
{
    public class ItemCatagories
    {
        public string ItemCatagory { get; set; }
        public List<AwardItems> AwardItems { get; set; }
    }
    public class AwardItems
    {
        public string ItemName { get; set; }
        public string Spec { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public float Result { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalUnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal VAT { get; set; }
        public decimal SubTotal { get; set; }
        public decimal FinalSubTotal { get; set; }
        public decimal GTotal { get; set; }

    }
}
