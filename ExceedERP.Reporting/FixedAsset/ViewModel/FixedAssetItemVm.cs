using System;

namespace ExceedERP.Reporting.FixedAsset.ViewModel
{
    public class FixedAssetMachineryItemVm
    {
        public string TagNo { get; set; }
        public string BarCode { get; set;  }
        public string Description { get; set;  }
        public string Manufacturer { get; set; }
        public string Model { get; set;  }
        public string SerialNo { get; set;  }
        public string ChasisNo { get; set; }
        public string MotorNo { get; set; }
        public string PlateNo { get; set; }
        public string Condition { get; set; }
        public string Location { get; set; }
        public string SIV { get; set; }
        public string RrtNo { get; set; }
        public string Remark { get; set; }
    }

    public class FixedAssetItemVm
    {
        public string SubCategory { get; set; }
        public string Tag { get; set; }
        public string Location { get; set; }
        public string BarCode { get; set; }
        public string Description { get; set; }
        public string IssueOrTransfer { get; set; }
        public string Size { get; set; }
        public string Model { get; set; }
        public string SerialNo { get; set; }
        public string Condition { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public string RrtNo { get; set; }
        public DateTime? GrnDate { get; set; }
        public string BranchOrProject { get; set; }
        public string SIV { get; set; }
        public string PlateNo { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }
}
