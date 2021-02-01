using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Inventory
{
    public class ItemInspection : TrackUserOperation
    {
        public int ItemInspectionId { get; set; }
        [Display(Name = "Inspection Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime InspectionDate { get; set; }

        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        [Display(Name = "Accepted?")]
        public bool Accepted { get; set; }
        public string Remark { get; set; }
        public int GoodReceiveId { get; set; }
        public virtual GoodReceive GoodReceive { get; set;  }
        
    }

    //public class ItemInspectionItem :TrackUserOperation
    //{
    //    public int ItemInspectionItemID { get; set; }

    //    public int ItemInspectionId { get; set; }
    //    public virtual ItemInspection ItemInspection { get; set; }
        
    //    [Required]
    //    [Display(Name = "Common Item")]
    //    public string ItemCode { get; set; }
    //    [Display(Name = "Description")]
    //    public string ItemDescription { get; set; }
    //    [Display(Name = "Model")]
    //    public string Model { get; set; }
    //    [Display(Name = "Serial No.")]
    //    public string SerialNo { get; set; }
        
       
    //    [Display(Name = "Requested Quantity")]
    //    public decimal RequestedQuantity { get; set; }
    //    [Display(Name = "Supplied Quantity")]
    //    public decimal SuppliedQuantity { get; set; }
    //    [Display(Name = "Extra Quantity")]
    //    public decimal ExtraQuantity { get; set; }
    //    [Display(Name = "Under Quantity")]
    //    public decimal UnderQuantity { get; set; }
    //    [Display(Name = "Quantity Delivered Out of Contract")]
    //    public decimal OutOfContractQuantity { get; set; }
    //    public string Remark { get; set; }
       
    //}

    //public class ItemInspectionValidation  :TrackUserSettingOperation
    //{
    //    public int ItemInspectionValidationID { get; set; }
    //    public int ItemInspectionId { get; set; }
    //    public virtual ItemInspection ItemInspection { get; set; }
    //    public string Status { get; set; }
    //    public string Remark { get; set; }
    //}
}
