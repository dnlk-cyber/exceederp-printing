using System;
using System.Drawing;
using ExceedERP.Reporting.Inventory.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Inventory.DataSource.Print.ViewModel
{
    public class PurchaseRequistionHeader : InventoryVoucherHeaderVm
    {
        public string PurchaseRequistionNo { get; set; }
        public string StoreRequistionNo { get; set; }
        public string Requestor { get; set; }
        public string ApproverName { get; set; }//sr
        public DateTime ApprovalDate { get; set; }//sr
        public Image ApprovalBySignature { get; set; }

        public string PrApproverName { get; set; }//pr
        public DateTime PrApprovalDate { get; set; }//pr
        public string AuthorizerName { get; set; }
        public DateTime AuthorizationDate { get; set; }
        public Image AuthorizerBySignature { get; set; }
        public string BudgetAuthorizerName { get; set; }
        public DateTime BudgetAuthorizationDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Image PreparedBySignature { get; set; }
        public string RequestingDate { get; set; }
        public string RequestingDepartment { get; set; }
        public string RequestingStore { get; set; }
        public string RequestingProject { get; set; }
        public string ForProject { get; set; }
        public string PRDate { get; set; }
        public string Model { get; set; }
        public string PlateNo { get; set; }
        public string Remark { get; set; }
    }
}