using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Reporting.Inventory.ViewModel
{
    public class PurchaseRequisitionVm
    {
        public List<PurchaseRequisitionItemVm> PurchaseRequisitionItemVms { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public decimal CategoryTotal { get; set; }
    }
    public class PurchaseRequisitionItemVm
    {
        public string Id { get; set; }
        public string Reference { get; set; }
        public DateTime RequisitionDate { get; set; }
        [Display(Name = "Item")]
        public string ItemName { get; set; }
        public string PartNo { get; set; }
        public decimal Quantity { get; set; }
        public string RequestingDepartment { get; set; }
        public string ForProject { get; set; }
    }
}