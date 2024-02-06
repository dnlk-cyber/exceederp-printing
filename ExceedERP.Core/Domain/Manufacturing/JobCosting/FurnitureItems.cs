using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace ExceedERP.Core.Domain.Manufacturing.JobCosting
{       
        public class FurnitureItems
    {
            public int FurnitureMasterItemsID { get; set; }
            public string RecordedBy { get; set; }
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public DateTime RecordTime { get; set; }
            [Required]
            [Display(Name = "Item Category Code")]
            public string MasterItemCode { get; set; }
            [Required]
            [Display(Name = "Master Item Name")]
            public string MasterItemName { get; set; }
            [Required]
            [Display(Name = "Description")]
            public string Description { get; set; }
            [Display(Name = "Check if non stock items")]
            public bool NonStock { get; set; }
            public virtual ICollection<Items> Items { get; set; }
            [Required]
            [Display(Name = "Account Code")]
            public string AccountCode { get; set; }
            [Required]
            [Display(Name = "Issue Account Code")]
            public string IssueAccountCode { get; set; }
        }
        public class Items
        {
            public int ItemsID { get; set; }
            [Display(Name = "Item Category")]
            public int MasterItemsID { get; set; }
            public virtual FurnitureItems MasterItem { get; set; }
            public string RecordedBy { get; set; }
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public DateTime RecordTime { get; set; }
            [Required]
            [Display(Name = "Item Code")]
            public string ItemCode { get; set; }
            [Required]
            [Display(Name = "Item Name")]
            public string Itemname { get; set; }
            [Display(Name = "Description")]
            public string Description { get; set; }
            [Display(Name = "Part No")]
            public int PartNo { get; set; }
            [Required]
            [Display(Name = "Unit")]
            public string Unit { get; set; }
            //public decimal AccountNumber { get; set; }
            //[Display(Name = "Minimum Qty")]
            public float Minimum { get; set; }
            [Display(Name = "Maximum Qty")]
            public float Maximum { get; set; }
            [Display(Name = "Reorder Qty")]
            public float Reorder { get; set; }
            [Display(Name = "Balance")]
            public float Balance { get; set; }
            public float CurrentTotal { get; set; }
            public float AvgPrice { get; set; }
            public float BeginningQty { get; set; }
            public float BeginningAmount { get; set; }
            public string Remark { get; set; }
            public float Total { get; set; }
        }
        public class StoreItems
        {
            public int StoreItemsID { get; set; }
            [Display(Name = "Store")]
            public int StoreID { get; set; }
            
            [Display(Name = "Item Category")]
            public string MasterItems { get; set; }
            [Required]
            [Display(Name = "Item")]
            public string ItemsID { get; set; }
            public virtual Items Item { get; set; }
            public string Balance { get; set; }
            public string Price { get; set; }
            public DateTime RecordTime { get; set; }
            public bool checkBeginningBalance { get; set; }
        }
    }