namespace ExceedERP.Web.Areas.Procurement.Models
{
    public class ServiceReceiveItemVm
    {
        public string CatCode { get; set; }
        public string CategoryName { get; set; }
        public string ItmCode { get; set; }
        public string Name { get; set; }
        public string ItemName { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public int GLTaxRateID { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }
        public bool IsExtra { get; set; }
        public decimal? ExtraQuantity { get; set; }
        public decimal ExtraAmount { get; set; }
        public string ItemSpecification { get; set; }
        public string UOM { get; set; }

    }
}