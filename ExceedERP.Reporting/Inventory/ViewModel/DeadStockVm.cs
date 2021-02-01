namespace ExceedERP.Reporting.Inventory.ViewModel
{
    public class DeadStockVm
    {
        public string ItemCode { get; set;  }
        public string PartNo { get; set; }
        public string Category { get; set;  }
        public string Description { get; set;  }
        public string Unit { get; set;  }
        public decimal Balance { get; set;  }
        public string Location { get; set; }
        public string Remark { get; set;  }
    }
}
