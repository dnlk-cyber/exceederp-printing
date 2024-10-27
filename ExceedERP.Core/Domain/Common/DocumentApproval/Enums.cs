using System.ComponentModel;

namespace ExceedERP.Core.Domain.Common.DocumentApproval
{
    public enum ForwardMethod
    {
        Direct,
        Hierarchy
    }
    public enum ApprovalHierarchy
    {
        [Description("HR Placement Hierarchy")]
        Placement_Hierarchy,
        [Description("Adhoc Hierarchy")]
        Adhoc_Hierarchy
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
        ContractManagement,
        TBPayment,
        ApPaymentRequestSelfCare,
        PurchasePaymentRequest,
        ApTravelOrderSelfCare,
        ApTravelExpenseClaimSelfCare,
        RentalPaymentRequestSelfCare,
        ApFundTransferSelfCare,
        POChangeOrder,
        ProformaPurchase
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
