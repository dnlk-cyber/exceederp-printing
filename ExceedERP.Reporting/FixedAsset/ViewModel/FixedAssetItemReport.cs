namespace ExceedERP.Reporting.FixedAsset.ViewModel
{
    public class FixedAssetItemReport
    {
        public string BarCode { get; set; }
        public string Tag { get; set; }
        public string Category { get; set;  }
        public string SubCategory { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string Location { get; set; }
        public string SerialNo { get; set; }
        public string ChasisNo { get; set; }
        public string PlateNo { get; set; }
        public string EngineNo { get; set;  }
        public string SivDate { get; set; }
        public string SivNo { get; set; }
        public string GrnDate { get; set; }
        public string GrnNo { get; set; }
        public decimal BookValue { get; set; }
        public decimal PurchasePrice { get; set; }
        public string CurrentUser { get; set; }
        public string Condition { get; set; }
        public string Remark { get; set; }
        //added
        public string Model { get; set; }
        public string Supplier { get; set; }
        public string Manufacturer { get; set; }
        public string ManufacturingYear { get; set; }
        public string Branch { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }

    }

    
}
