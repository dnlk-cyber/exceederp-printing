
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Procurement.Settings
{
    public enum PurchasePaymentType
    {
        Cash,
        Credit
    }
    public enum TemplateType
    {
        TechnicalEn,
        TechnicalAm,
        AwardEn,
        AwardAm,
        TechnicalOpen,
        FinancialOpen,
        Cancellation,
        TechnicalPassFailEng,
        TechnicalPassFailAmh,
        ResultNotifEng,
        ResultNotifAmh,
        GuaranteeExtension,
        GuaranteeRelease,
        GuaranteForfeited,
        InvitationLetter
    }
    public enum GuaranteeTypes
    {
        [Description("Advance Payment")]
        AdvancePayment,
        [Description("Bid Security")]
        BidSecurity,
        Performance
    }
    //not used
    public enum GuaranteeModes
    {
        Cash,
        CPO,
        UnconditionalBankGuarantee,
        Insurance
    }

    public enum GuaranteeValidityStatus
    {
        Active,
        Released,
        Forfeited,
        Expired
    }
    public enum GuaranteeRenewalStatus
    {
        NA,
        Requested,
        Renewed
    }

    public enum ProcurementBudgetSource
    {
        Operating,
        Capital
    }
    public enum ProcurementCategory
    {
        Goods,
        Service,
        SupplyAndFix
    }
    public enum POType
    {
        PettyCash,
        CashBase,
        CreditPurchase
    }
    public enum POSource
    {
        Direct,
        Proforma,
        Tender
    }

    public enum PurchaseCategory
    {
        LocalPurchase,
        ForeignPurchase
    }
    public enum ModeOfPayment
    {
        LC,
        TT,
        CAD
    }
    public enum PaymentType
    {
        Advance,
        Payment,
        Credit
    }

    public enum AmountType
    {
        Percent,
        Amount
    }

    public enum PurchaseStatus
    {
        New,
        Floated,
        Pending,
        Cancelled,
        Rebid,
        Extended,
        Closed,
        Awarded
    }

    public enum TenderType
    {
        Open,
        Selected,
        TwoStage,
        NA
    }
    public enum ProcurementMethod
    {
        Tender,
        Proforma,
        DirectPurchase
    }
    public enum PaymentTerm
    {
        [Display(Name = "One Time")]
        OneTime,
        [Display(Name = "Two Times")]
        TwoTimes,
        [Display(Name = "Three Times")]
        ThreeTimes,
        [Display(Name = "Four Times")]
        FourTimes
    }

    public enum DeliveryTerm
    {
        EXW,
        FOB,
        CIF,
        DDU,
        DDP
    }
}
