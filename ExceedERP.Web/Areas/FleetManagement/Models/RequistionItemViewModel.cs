using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;

namespace ExceedERP.Web.Areas.FleetManagement.Models
{
    public class RequistionItemViewModel : TrackUserSettingOperation
        {
            public int StoreRequisitionItemID { get; set; }
            public int StoreRequisitionID { get; set; }
            public virtual StoreRequisition StoreRequisition { get; set; }
            [Required]
            [DisplayName("Item Category")]
            public string ItemCategoryCode { get; set; }
            public virtual ItemCategory ItemCategory { get; set; }
            [Required]
            [DisplayName("Item Code")]
            public string ItemCode { get; set; }
            public virtual InventoryItem InventoryItem { get; set; }
            [Display(Name = "Part No.")]
            public string PartNo { get; set; }
            [Display(Name = "Requested QTY")]
            public decimal RequestedQuantity { get; set; }
            [Display(Name = "Approved QTY")]
            public decimal ApprovedQuantity { get; set; }
            [Display(Name = "Issued QTY")]
            public decimal IssuedQuantity { get; set; }
            [Display(Name = "Purchase QTY")]
            public decimal PurchaseQuantity { get; set; }
            public bool IsApproved { get; set; }
            [Display(Name = "Specification")]
            public string ItemSpecification { get; set; }
            [Display(Name = "Generate PR")]
            public bool GeneratePurchaseReq { get; set; }
            [NotMapped]
            public decimal CurrentBalance { get; set; }
             [Display(Name = "Used Amount")]
            public decimal UsedAmount { get; set; }
             [Display(Name = "Remaining Amount")]
            public decimal RemainingAmount { get; set; }
             public decimal UnitPrice { get; set; }

             public decimal TotalPrice { get; set; }

             public string Remark { get; set; }
        }

    
}