using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;

namespace ExceedERP.Core.Domain.FixedAsset.Requisitions
{


    public class FixedAssetRequisition : OperationWithThirdValidation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int FixedAssetRequisitionId { get; set; }
        [Display(Name = "Request Reference")]
        public string Reference { get; set; }
        [Required]
        [DisplayName("Branch or Project")]
        public int BranchId { get; set; }
        [Required]
        [DisplayName("Work Unit")]
        public int OrgStructureId { get; set; }
        [Required]
        [Display(Name = "Requester Name")]
        public int EmployeeId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Request Date")]
        public DateTime RequestDate { get; set; }
        [Required]
        [Display(Name = "Store Code")]
        public string StoreCode { get; set; }
        [DisplayName("To Store")]
        public string ToStoreCode { get; set; }
        [DisplayName("Is Fuel Request")]
        public bool IsFuel { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        [DisplayName("Is Transfer")]
        public bool IsTransfer { get; set; }
        public string NextApprover { get; set; }
        [DisplayName("Is For Other")]
        public bool IsForOther { get; set; }

        [DisplayName("For Branch or Project")]
        public int? ForBranchId { get; set; }
        [Display(Name = "Plate No")]
        public string PlateNo { get; set; }
        public int CreatedAtOrgStructureId { get; set; }
        public virtual ICollection<FixedAssetRequisitionItem> FixedAssetRequisitionItem { get; set; }
        public virtual ICollection<FixedAssetRequisitionValidation> FixedAssetRequisitionValidation { get; set; }
        public virtual ICollection<FixedAssetRequisitionSecondValidation> FixedAssetRequisitionSecondValidation { get; set; }
        public virtual ICollection<FixedAssetRequisitionPropertyValidation> FixedAssetRequisitionPropertyValidation { get; set; }
        public virtual ICollection<FixedAssetRequisitionAttachment> FixedAssetRequisitionAttachments { get; set; }
    }

    public class FixedAssetRequisitionAttachment
    {
        public int FixedAssetRequisitionAttachmentId { get; set; }
        public int FixedAssetRequisitionId { get; set; }
        public virtual FixedAssetRequisition FixedAssetRequisition { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
