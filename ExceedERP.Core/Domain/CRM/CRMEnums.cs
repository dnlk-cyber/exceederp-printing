using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CRM
{
    public class CRMEnums
    {
        public enum AdditionalCostType
        {
            Insurance,
            Freight,
            Other

        }
        public enum ServiceType
        {

            Proposal,
            Sponsorship,
            Bazar_And_Exibition,
            Traninig,
            Promotion

        }
    }
    //public enum FinalizeType
    //{
    //    AWAITING,
    //    ASSIGNED,
    //    RESOLVED,
    //    CLOSED

    //}

    public enum SalesQuoteSource
    {
        [Display(Name = "Manual")]
        Manual = 0,
        [Display(Name = "Maintenance")]
        Maintenance = 1
    }
    public enum SalesOrderSource
    {
        [Display(Name = "Manual")]
        Manual = 0,
        [Display(Name = "Sales Quote")]
        SalesQuote = 1
    }
    public enum SalesInvoiceSource
    {
        [Display(Name = "Manual")]
        Manual = 0,
        [Display(Name = "Sales Order")]
        SalesOrder = 1
    }
    public enum ShipmentGenerationMethod
    {
        Manual = 0,
        AutoGenerate = 1
    }
    public enum ShipmentSource
    {
        [Display(Name = "Sales Invoice")]
        SalesInvoice = 0,
        [Display(Name = "Sales Order")]
        SalesOrder = 1,
        [Display(Name = "Maintenance Store Requisition")]
        MaintenanceStoreRequisition = 2,
    }

    public enum SalesStoreType
    {
        Store = 0,
        Shop = 1
    }
    public enum VoucherType
    {
        SalesQuote = 10,
        SalesOrder = 11,
        // Credit and Cash sales invoices
        SalesInvoice = 12
    }

    public enum CustomerQueueStatus
    {
        Waiting = 0,
        Partial = 1,
        Closed = 2
    }
    public enum PosSettingType
    {
        BaseUrl = 0,
        PosId = 1
    }
    public enum PosPrintType
    {
        CRM,
        Offline
    }
    public enum BankReconciliationType
    {
        Sales = 0,
        Deposit = 1,
        Credit_Settlement = 2
    }
    public enum CRMVoucherType
    {
        SalesQuote = 10,
        SalesOrder = 11,
        SalesInvoice = 12
    }
}
