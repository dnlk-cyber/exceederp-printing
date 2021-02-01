using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Reporting.Inventory.ViewModel
{
    public class IssueVm
    {
        public List<IssueItemViewModel> IssueItemList { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public decimal CategoryTotal { get; set; }
    }
    public class IssueItemViewModel
    {
        public string Id { get; set; }
        public string Reference { get; set; }
        public DateTime IssueDate { get; set; }
        public string InventoryItem { get; set; }
        public decimal Quantity { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Total")]
        public decimal Total { get; set; }
        [Display(Name = "Store")]
        public string StoreName { get; set; }
        public string ToDepartment { get; set; }
        public string PlateNo { get; set; }
        public string WorkItemCode { get; set; }
        public string Branch { get; set; }
        public string ForBranch { get; set; }
    }
}