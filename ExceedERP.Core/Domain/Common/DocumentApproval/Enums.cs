using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Common.DocumentApproval
{
    public enum ForwardMethod
    {
        Direct,
        Hierarchy
    }
    public enum ApprovalHierarchy
    {
        PlacementHierarchy,
        AdhokHierarchy
    }
    public enum ApprovalDocumentType//sync with below
    {
        ApInvoice,
        ApPayment,
        ApMiscPayment,
        ApTravelOrder,
        ApTravelExpenseClaim,
        ApPettyCash,
        StoreRequisition,
        FixedAssetRequisition,
        PurchaseOrder,
        Tender,
        ContractManagement
    }
    public enum ApprovalWorkflow
    {
        StoreRequisitionApproval,
        FixedAssetRequisition,
        PurchaseOrder,
        Tender
    }
    public enum ApprovalGroupRuleObject
    {
        DocumentTotal,
        AccountRange
    }

    public enum ApprovalRuleType
    {
        Include,
        Exclude
    }
   
}
