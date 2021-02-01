using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Requisitions;

namespace ExceedERP.Core.Domain.FixedAsset.Issues
{


  
    public class FixedAssetIssue : Operations
    {

        public int FixedAssetIssueID { get; set; }
        [Display(Name = "Store Requisition Number")]
        public int FixedAssetRequisitionId { get; set; }
        public virtual FixedAssetRequisition FixedAssetRequisition { get; set; }
        [DisplayName("Branch or Project")]
        public int BranchId { get; set; }
        [DisplayName("Work Unit")]
        public int OrgStructureId { get; set; }
        [Required]
        [Display(Name = "Store")]
        public string StoreCode { get; set; }
        [Display(Name = " Gate Pass")]
        public string GatePass { get; set; }
        [Display(Name = " Reference")]
        public string Reference { get; set; }
        [Display(Name = "Issue Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime IssueDate { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        [Display(Name = "Requester Name")]
        public int EmployeeId { get; set; }
        [Display(Name = "Recipient Name")]
        public int RecipientId { get; set; }
        public virtual ICollection<FixedAssetIssueItem> FixedAssetIssueItems { get; set; }
        public virtual ICollection<FixedAssetIssueValidation> FixedAssetIssueValidation { get; set; }
        //public virtual ICollection<FixedAssetIssueDistribution> FixedAssetIssueDistribution { get; set; }
    }


    //public class FixedAssetIssueDistribution
    //{
    //    public int FixedAssetIssueDistributionID { get; set; }
    //    public int FixedAssetIssueID { get; set; }
    //    public virtual FixedAssetIssue FixedAssetIssue { get; set; }
    //    public int FixedAssetIssueItemIDTrackBack { get; set; }
    //    public string AccountCode { get; set; }
    //    [Display(Name = "Account Description")]
    //    public string AccountDesc { get; set; }
    //    public decimal Debit { get; set; }
    //    public decimal Credit { get; set; }
    //}
}