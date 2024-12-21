using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Inventory
{
    public class StoreReturn : Operations
    {
        public int StoreReturnID { get; set; }

        public string Reference { get; set; }
        [Required]
        [Display(Name = "Store")]
        public string StoreCode { get; set; }
        [Display(Name = "Return Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReturnDate { get; set; }
        [Required]
        [Display(Name = "Reason")]
        public string Reason { get; set; }
        [Required]
        [DisplayName("Work Unit")]
        public int OrgStructureId { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        [DisplayName("For Other Branch/Project?")]
        public bool IsForOther { get; set; }

        [DisplayName("For Branch/Project")]
        public int? ForBranchId { get; set; }
        [DisplayName("Issue Number")]
        public string IssueNumber { get; set; }
        [DisplayName("Job Order")]
        public string JobOrderId { get; set; }
        [Display(Name = "Confirmation Letter")]
        public string ConfirmationLetter { get; set; }//if bad items are returned need to a
        [DisplayName("Branch or Project")]
        public int BranchId { get; set; }
        [DisplayName("Business Unit")]
        public int BusinessUnitId { get; set; }
        public virtual ICollection<StoreReturnItems> StoreReturnItems { get; set; }
        public virtual ICollection<StoreReturnValidation> StoreReturnValidation { get; set; }
        public virtual ICollection<StoreReturnDistribution> StoreReturnDistribution { get; set; }
        public virtual ICollection<StoreReturnAttachment> StoreReturnAttachments { get; set; }
    }
    public class StoreReturnItems : TrackUserOperation, IValidatableObject
    {
        public int StoreReturnItemsID { get; set; }

        public int StoreReturnID { get; set; }
        public virtual StoreReturn StoreReturn { get; set; }
        [Required]
        [DisplayName("Item Category")]
        public string ItemCategoryCode { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        [Required]
        [DisplayName("Item Code")]
        public string ItemCode { get; set; }
        //public string ItemName { get; set; }
        public virtual InventoryItem InventoryItem { get; set; }
        [Display(Name = "Quantity")]
        [Range(0.0, Double.MaxValue, ErrorMessage = "The field {0} must be greater than {0}.")]
        public decimal Quantity { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string ProductionJobNo { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if  (UnitPrice <0)
            {
                yield return new ValidationResult(
                    $"Price should be positive number.",
                    new[] { nameof(UnitPrice) });
            }
        }
    }
    public class StoreReturnValidation : TrackUserSettingOperation
    {
        public int StoreReturnValidationID { get; set; }

        public int StoreReturnID { get; set; }
        public virtual StoreReturn StoreReturn { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Approver { get; set; }
        public string Remark { get; set; }
    }
    public class StoreReturnDistribution:VoucherDistribution
    {
        public int StoreReturnDistributionID { get; set; }
        public int StoreReturnID { get; set; }
        public virtual StoreReturn StoreReturn { get; set; }
    }

    public class StoreReturnAttachment : TrackUserOperation
    {
        public int StoreReturnAttachmentId { get; set; }
        public int StoreReturnID { get; set; }
        public virtual StoreReturn StoreReturn { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}