using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Reporting.Inventory.ViewModel
{
    public class StoreRequisitionVm
    {
        public List<StoreRequisitionItemViewModel> StoreRequisitionItemList { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public decimal CategoryTotal { get; set; }
    }
    public class StoreRequisitionItemViewModel
    {
        public string Id { get; set; }
        public string Reference { get; set; }
        public DateTime RequisitionDate { get; set; }
        [Display(Name = "Item")]
        public string InventoryItem { get; set; }
        public string PlateNo { get; set; }
        public string PartNo { get; set; }
        public string ModelNo { get; set; }
        public decimal RequestedQuantity { get; set; }
        public decimal ApprovedQuantity { get; set; }
        public string RequestingDepartment { get; set; }
        public string ForProject { get; set; }
    }
}