using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExceedERP.Web.Areas.Inventory.Models
{
    public class IssueImport
    {
        public string RequistionNo { get; set; }
        public string IssueNo { get; set; }
        public int BranchId { get; set; }
        public int OrgStructureId { get; set; }
        public DateTime IssueDate { get; set;  }
        public string DocumentRef { get; set; }

        public string ItemCategoryCode { get; set; }
        public string ItemCode { get; set; }
        public decimal RequestedQuantity { get; set; }
        public decimal ApprovedQuantity { get; set; }
        public decimal IssuedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
    }


}