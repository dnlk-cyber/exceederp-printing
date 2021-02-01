using System;

namespace ExceedERP.Core.Domain.FixedAsset.Issues
{
    
    public class FixedAssetIssueImport
    {
        public string RequistionNo { get; set; }
        public string IssueNo { get; set; }
        public int BranchId { get; set; }
        public int OrgStructureId { get; set; }
        public DateTime IssueDate { get; set; }
        public string DocumentRef { get; set; }

        public string CategoryCode { get; set; }
        public string SubCategoryCode { get; set; }
        public string TagNo { get; set; }
        public decimal RequestedQuantity { get; set; }
        public decimal ApprovedQuantity { get; set; }
        public decimal IssuedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
    }
}
