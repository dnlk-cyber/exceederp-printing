using System;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class UserCardItemsList 
    {

        public string ItemDesc { get; set; } 
        public string UOM { get; set; }
        public decimal Quanity { get; set; }
      
        public decimal UnitPrice { get; set; }
       
        public string TagNumber { get; set; }
       
        public string  IssueDate { get; set; }
       
        public string SIVNo { get; set; }

        public string ReturnDate { get; set; }
       
        public string SRNNo { get; set; }
        public string Remark { get; set; }

        public string AccNo { get; set; }
        public string AssetNo { get; set; }
        public string Description { get; set; }
        public string Size { get; set; }
        public string Model { get; set; }
        public string SerialPartNo { get; set; }
        public string Condition { get; set; }
        public string Unit { get; set; }
        public string Qty { get; set; }
        public string MTITNo { get; set; }
        public DateTime MTITDate { get; set; }
        public string GRNNo { get; set; }
        public DateTime GRNDate { get; set; }
    }
}