namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CRMModuleUpgrade : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("CRM.AdvancePaymentDistibution", "AdvancePaymentLineId", "CRM.AdvancePaymentLine");
            DropForeignKey("CRM.ServiceLineItem", "CustomerServiceId", "CRM.CustomerService");
            DropForeignKey("CRM.AdditionalCostLog", "LineItemId", "CRM.LineItem");
            DropIndex("CRM.AdditionalCostLog", new[] { "LineItemId" });
            DropIndex("CRM.AdvancePaymentDistibution", new[] { "AdvancePaymentLineId" });
            DropIndex("CRM.ServiceLineItem", new[] { "CustomerServiceId" });
            RenameColumn(table: "CRM.AdditionalCostLog", name: "LineItemId", newName: "LineItem_LineItemID");
            CreateTable(
                "CRM.AnnualSalesPlan",
                c => new
                    {
                        AnnualSalesPlanId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        CategoryId = c.String(),
                        SubCategoryId = c.String(),
                        AveragePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Profit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAnnualPlanQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAnnualPlanValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalMonthlyPlanQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalMonthlyPlanValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalWeeklyPlanQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalWeeklyPlanValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDailyPlanQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDailyPlanValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsPostedToSecondLedger = c.Boolean(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        IsOnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AnnualSalesPlanId);
            
            CreateTable(
                "CRM.AnnualSalesPlanValidation",
                c => new
                    {
                        AnnualSalesPlanValidationId = c.Int(nullable: false, identity: true),
                        AnnualSalesPlanId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AnnualSalesPlanValidationId)
                .ForeignKey("CRM.AnnualSalesPlan", t => t.AnnualSalesPlanId, cascadeDelete: true)
                .Index(t => t.AnnualSalesPlanId);
            
            CreateTable(
                "CRM.BranchAnnualSalesPlan",
                c => new
                    {
                        BranchAnnualSalesPlanId = c.Int(nullable: false, identity: true),
                        AnnualSalesPlanId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        Ratio = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsPostedToSecondLedger = c.Boolean(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        IsOnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BranchAnnualSalesPlanId)
                .ForeignKey("CRM.AnnualSalesPlan", t => t.AnnualSalesPlanId, cascadeDelete: true)
                .Index(t => t.AnnualSalesPlanId);
            
            CreateTable(
                "CRM.CashSalesInvoiceValidation",
                c => new
                    {
                        CashSalesInvoiceValidationId = c.Int(nullable: false, identity: true),
                        CashSalesInvoiceId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CashSalesInvoiceValidationId)
                .ForeignKey("CRM.CashSalesInvoice", t => t.CashSalesInvoiceId, cascadeDelete: true)
                .Index(t => t.CashSalesInvoiceId);
            
            CreateTable(
                "CRM.CommissionLineItem",
                c => new
                    {
                        CommissionLineItemId = c.Int(nullable: false, identity: true),
                        Reason = c.String(),
                        CostAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GLTaxRateID = c.Int(nullable: false),
                        VoucherId = c.Int(nullable: false),
                        VoucherType = c.Int(nullable: false),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CommissionLineItemId);
            
            CreateTable(
                "CRM.CreditSettlementDistribution",
                c => new
                    {
                        CreditSettlementDistributionId = c.Int(nullable: false, identity: true),
                        CreditSettlementId = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        AdditionalDescription = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CreditSettlementDistributionId)
                .ForeignKey("CRM.CreditSettlement", t => t.CreditSettlementId, cascadeDelete: true)
                .Index(t => t.CreditSettlementId);
            
            CreateTable(
                "CRM.CreditSettlementValidation",
                c => new
                    {
                        CreditSettlementValidationId = c.Int(nullable: false, identity: true),
                        CreditSettlementId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CreditSettlementValidationId)
                .ForeignKey("CRM.CreditSettlement", t => t.CreditSettlementId, cascadeDelete: true)
                .Index(t => t.CreditSettlementId);
            
            CreateTable(
                "CRM.CRMDashboardTodateSalesBranchData",
                c => new
                    {
                        CRMDashboardTodateSalesBranchDataId = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        SalesType = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 20, scale: 4),
                    })
                .PrimaryKey(t => t.CRMDashboardTodateSalesBranchDataId)
                .ForeignKey("HRM.Branch", t => t.BranchId, cascadeDelete: true)
                .Index(t => t.BranchId);
            
            CreateTable(
                "CRM.CRMDashboardTodateSalesData",
                c => new
                    {
                        CRMDashboardTodateSalesDataId = c.Int(nullable: false, identity: true),
                        SalesType = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 20, scale: 4),
                    })
                .PrimaryKey(t => t.CRMDashboardTodateSalesDataId);
            
            CreateTable(
                "CRM.CRMDeposit",
                c => new
                    {
                        CRMDepositId = c.Int(nullable: false, identity: true),
                        SalesPersonUserName = c.String(),
                        SalesPersonFullName = c.String(),
                        BranchId = c.Int(nullable: false),
                        StoreID = c.Int(nullable: false),
                        CRMPeriodId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        IsPostedToSecondLedger = c.Boolean(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        IsOnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CRMDepositId);
            
            CreateTable(
                "CRM.CRMDepositDistribution",
                c => new
                    {
                        CRMDepositDistributionId = c.Int(nullable: false, identity: true),
                        CRMDepositId = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        AdditionalDescription = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CRMDepositDistributionId)
                .ForeignKey("CRM.CRMDeposit", t => t.CRMDepositId, cascadeDelete: true)
                .Index(t => t.CRMDepositId);
            
            CreateTable(
                "CRM.CRMDepositLine",
                c => new
                    {
                        CRMDepositLineId = c.Int(nullable: false, identity: true),
                        CRMDepositId = c.Int(nullable: false),
                        ReceiveMethodId = c.Int(nullable: false),
                        DepositAmount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Reference = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CRMDepositLineId)
                .ForeignKey("CRM.CRMDeposit", t => t.CRMDepositId, cascadeDelete: true)
                .Index(t => t.CRMDepositId);
            
            CreateTable(
                "CRM.CRMDepositValidation",
                c => new
                    {
                        CRMDepositValidationId = c.Int(nullable: false, identity: true),
                        CRMDepositId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CRMDepositValidationId)
                .ForeignKey("CRM.CRMDeposit", t => t.CRMDepositId, cascadeDelete: true)
                .Index(t => t.CRMDepositId);
            
            CreateTable(
                "CRM.CRMPeriod",
                c => new
                    {
                        CRMPeriodId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DateFrom = c.DateTime(nullable: false),
                        DateFromAmharic = c.String(),
                        DateTo = c.DateTime(nullable: false),
                        DateToAmharic = c.String(),
                        Status = c.Int(nullable: false),
                        IsPermanentlyClosed = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CRMPeriodId);
            
            CreateTable(
                "CRM.InvestmentType",
                c => new
                    {
                        InvestmentTypeId = c.Int(nullable: false, identity: true),
                        InvestmentName = c.String(),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.InvestmentTypeId);
            
            CreateTable(
                "CRM.LocationMap",
                c => new
                    {
                        LocationMapId = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        LocationName = c.String(),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LocationMapId)
                .ForeignKey("HRM.Branch", t => t.BranchId, cascadeDelete: true)
                .Index(t => t.BranchId);
            
            CreateTable(
                "CRM.LocationUserAssignment",
                c => new
                    {
                        LocationUserAssignmentId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        LocationMapId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LocationUserAssignmentId)
                .ForeignKey("CRM.LocationMap", t => t.LocationMapId, cascadeDelete: true)
                .Index(t => t.LocationMapId);
            
            CreateTable(
                "CRM.PaymentSchedule",
                c => new
                    {
                        PaymentScheduleId = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Description = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        VoucherId = c.Int(nullable: false),
                        VoucherType = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PaymentScheduleId);
            
            CreateTable(
                "CRM.PosSetting",
                c => new
                    {
                        PosSettingId = c.Int(nullable: false, identity: true),
                        SettingType = c.Int(nullable: false),
                        Value = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PosSettingId);
            
            CreateTable(
                "CRM.PriceEstimationQuoteVoucherFirstValidation",
                c => new
                    {
                        PriceEstimationQuoteVoucherFirstValidationId = c.Int(nullable: false, identity: true),
                        PriceEstimationQuoteVoucherId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PriceEstimationQuoteVoucherFirstValidationId)
                .ForeignKey("CRM.PriceEstimationQuoteVouchers", t => t.PriceEstimationQuoteVoucherId, cascadeDelete: true)
                .Index(t => t.PriceEstimationQuoteVoucherId);
            
            CreateTable(
                "CRM.PriceEstimationQuoteVouchers",
                c => new
                    {
                        PriceEstimationQuoteVoucherId = c.Int(nullable: false, identity: true),
                        OrganizationCustomerId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        GrandTotal = c.Decimal(nullable: false, precision: 20, scale: 4),
                        PlateNo = c.String(),
                        BankAccountId = c.Int(),
                        IsMiddleOnlineApproved = c.Boolean(nullable: false),
                        MiddleOnlineApprovedBy = c.String(),
                        MiddleOnlineApprovedTime = c.DateTime(),
                        IsMiddleSendForApproval = c.Boolean(nullable: false),
                        MiddleSendForApprovalBy = c.String(),
                        MiddleSendForApprovalTime = c.DateTime(),
                        IsSecondOnlineApproved = c.Boolean(nullable: false),
                        SecondOnlineApprovedBy = c.String(),
                        SecondOnlineApprovedTime = c.DateTime(),
                        IsSecondSendForApproval = c.Boolean(nullable: false),
                        SecondSendForApprovalBy = c.String(),
                        SecondSendForApprovalTime = c.DateTime(),
                        IsSecondVoid = c.Boolean(nullable: false),
                        SecondVoidBy = c.String(),
                        SecondVoidTime = c.DateTime(),
                        IsPostedToSecondLedger = c.Boolean(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        IsOnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PriceEstimationQuoteVoucherId)
                .ForeignKey("CRM.OrganizationCustomer", t => t.OrganizationCustomerId, cascadeDelete: true)
                .Index(t => t.OrganizationCustomerId);
            
            CreateTable(
                "CRM.PriceEstimationQuoteVoucherLines",
                c => new
                    {
                        PriceEstimationQuoteVoucherLineId = c.Int(nullable: false, identity: true),
                        CategoryId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        PriceEstimationQuoteVoucherId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        UnitAmount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TaxTypeId = c.Int(),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        WithholdingTypeId = c.Int(),
                        WithholdingAmount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        WithholdTax = c.Boolean(nullable: false),
                        WithholdTaxAmount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 4),
                    })
                .PrimaryKey(t => t.PriceEstimationQuoteVoucherLineId)
                .ForeignKey("CRM.PriceEstimationQuoteVouchers", t => t.PriceEstimationQuoteVoucherId, cascadeDelete: true)
                .Index(t => t.PriceEstimationQuoteVoucherId);
            
            CreateTable(
                "CRM.PriceEstimationQuoteVoucherSecondValidation",
                c => new
                    {
                        PriceEstimationQuoteVoucherSecondValidationId = c.Int(nullable: false, identity: true),
                        PriceEstimationQuoteVoucherId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PriceEstimationQuoteVoucherSecondValidationId)
                .ForeignKey("CRM.PriceEstimationQuoteVouchers", t => t.PriceEstimationQuoteVoucherId, cascadeDelete: true)
                .Index(t => t.PriceEstimationQuoteVoucherId);
            
            CreateTable(
                "CRM.PriceEstimationQuoteVoucherServices",
                c => new
                    {
                        PriceEstimationQuoteVoucherServiceId = c.Int(nullable: false, identity: true),
                        CategoryCode = c.String(),
                        SubCategoryCode = c.String(),
                        ServiceTypeId = c.String(),
                        PriceEstimationQuoteVoucherId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        UnitAmount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TaxTypeId = c.Int(),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        WithholdingTypeId = c.Int(),
                        WithholdingAmount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        WithholdTax = c.Boolean(nullable: false),
                        WithholdTaxAmount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 4),
                    })
                .PrimaryKey(t => t.PriceEstimationQuoteVoucherServiceId)
                .ForeignKey("CRM.PriceEstimationQuoteVouchers", t => t.PriceEstimationQuoteVoucherId, cascadeDelete: true)
                .Index(t => t.PriceEstimationQuoteVoucherId);
            
            CreateTable(
                "CRM.PriceEstimationQuoteVoucherValidation",
                c => new
                    {
                        PriceEstimationQuoteVoucherValidationId = c.Int(nullable: false, identity: true),
                        PriceEstimationQuoteVoucherId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PriceEstimationQuoteVoucherValidationId)
                .ForeignKey("CRM.PriceEstimationQuoteVouchers", t => t.PriceEstimationQuoteVoucherId, cascadeDelete: true)
                .Index(t => t.PriceEstimationQuoteVoucherId);
            
            CreateTable(
                "CRM.ProductPriceLog",
                c => new
                    {
                        ProductPriceLogId = c.Int(nullable: false, identity: true),
                        StoreItemAssignmentID = c.Int(nullable: false),
                        InitialDate = c.DateTime(nullable: false),
                        InitialExchangeRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InitialPrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        NewDate = c.DateTime(nullable: false),
                        NewExchangeRate = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PriceType = c.String(),
                        ProposedNewPrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ApprovedNewPrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProductPriceLogId)
                .ForeignKey("EXCEED.StoreItemAssignment", t => t.StoreItemAssignmentID, cascadeDelete: true)
                .Index(t => t.StoreItemAssignmentID);
            
            CreateTable(
                "CRM.ProductUnitConvertorSetup",
                c => new
                    {
                        ProductUnitConvertorSetupId = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        FromUoM = c.String(),
                        ToUoM = c.String(),
                        MultiplicationFactor = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProductUnitConvertorSetupId);
            
            CreateTable(
                "CRM.Queue",
                c => new
                    {
                        QueueId = c.Int(nullable: false, identity: true),
                        QueueNumber = c.Int(nullable: false),
                        QueueCustomerId = c.Int(nullable: false),
                        QueueTypeId = c.Int(nullable: false),
                        TotalRequestedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WeeklyRequest = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ApprovedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BranchId = c.Int(nullable: false),
                        LocationMapId = c.Int(nullable: false),
                        Store = c.String(),
                        QueueDate = c.DateTime(nullable: false),
                        QueueStatus = c.Int(nullable: false),
                        AdvancePayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        IsPostedToSecondLedger = c.Boolean(nullable: false),
                        Status = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        IsOnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.QueueId)
                .ForeignKey("CRM.QueueCustomer", t => t.QueueCustomerId, cascadeDelete: true)
                .ForeignKey("CRM.QueueType", t => t.QueueTypeId, cascadeDelete: true)
                .Index(t => t.QueueCustomerId)
                .Index(t => t.QueueTypeId);
            
            CreateTable(
                "CRM.QueueCustomer",
                c => new
                    {
                        QueueCustomerId = c.Int(nullable: false, identity: true),
                        CustomerName = c.String(nullable: false),
                        TinNumber = c.String(nullable: false, maxLength: 10),
                        Phone = c.String(nullable: false, maxLength: 10),
                        InvestmentTypeId = c.Int(nullable: false),
                        InvestmentAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Grade = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.QueueCustomerId)
                .ForeignKey("CRM.InvestmentType", t => t.InvestmentTypeId, cascadeDelete: true)
                .Index(t => t.InvestmentTypeId);
            
            CreateTable(
                "CRM.QueueType",
                c => new
                    {
                        QueueTypeId = c.Int(nullable: false, identity: true),
                        QueueName = c.String(),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.QueueTypeId);
            
            CreateTable(
                "CRM.QueueStatus",
                c => new
                    {
                        QueueStatusId = c.Int(nullable: false, identity: true),
                        QueueId = c.Int(nullable: false),
                        ShippedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RemainingQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QueueStatusStatus = c.Int(nullable: false),
                        ShippedDate = c.DateTime(nullable: false),
                        Remark = c.String(),
                        IsPostedToSecondLedger = c.Boolean(nullable: false),
                        Status = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        IsOnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.QueueStatusId)
                .ForeignKey("CRM.Queue", t => t.QueueId, cascadeDelete: true)
                .Index(t => t.QueueId);
            
            CreateTable(
                "CRM.QueueStatusValidation",
                c => new
                    {
                        QueueStatusValidationId = c.Int(nullable: false, identity: true),
                        QueueStatusId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.QueueStatusValidationId)
                .ForeignKey("CRM.QueueStatus", t => t.QueueStatusId, cascadeDelete: true)
                .Index(t => t.QueueStatusId);
            
            CreateTable(
                "CRM.QueueValidation",
                c => new
                    {
                        QueueValidationId = c.Int(nullable: false, identity: true),
                        QueueId = c.Int(nullable: false),
                        ApprovedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.QueueValidationId)
                .ForeignKey("CRM.Queue", t => t.QueueId, cascadeDelete: true)
                .Index(t => t.QueueId);
            
            CreateTable(
                "CRM.QuotationTerm",
                c => new
                    {
                        QuotationTermId = c.Int(nullable: false, identity: true),
                        Desription = c.String(),
                        Remark = c.String(),
                        QuotationInDays = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.QuotationTermId);
            
            CreateTable(
                "CRM.SalesOrderVoucherValidation",
                c => new
                    {
                        SalesOrderVoucherValidationId = c.Int(nullable: false, identity: true),
                        SalesOrderVoucherId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SalesOrderVoucherValidationId)
                .ForeignKey("CRM.SalesOrderVoucher", t => t.SalesOrderVoucherId, cascadeDelete: true)
                .Index(t => t.SalesOrderVoucherId);
            
            CreateTable(
                "CRM.SalesPersonAssignment",
                c => new
                    {
                        SalesPersonAssignmentId = c.Int(nullable: false, identity: true),
                        SalesStoreId = c.Int(nullable: false),
                        UserName = c.String(),
                        FullName = c.String(),
                        PaymentMethodId = c.Int(nullable: false),
                        CashOnHandPaymentMethodId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SalesPersonAssignmentId)
                .ForeignKey("CRM.SalesStore", t => t.SalesStoreId, cascadeDelete: true)
                .Index(t => t.SalesStoreId);
            
            CreateTable(
                "CRM.SalesQuoteVoucherFirstValidation",
                c => new
                    {
                        SalesQuoteVoucherFirstValidationId = c.Int(nullable: false, identity: true),
                        SalesQuoteVoucherId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SalesQuoteVoucherFirstValidationId)
                .ForeignKey("CRM.SalesQuoteVoucher", t => t.SalesQuoteVoucherId, cascadeDelete: true)
                .Index(t => t.SalesQuoteVoucherId);
            
            CreateTable(
                "CRM.SalesQuoteVoucherSecondValidation",
                c => new
                    {
                        SalesQuoteVoucherSecondValidationId = c.Int(nullable: false, identity: true),
                        SalesQuoteVoucherId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SalesQuoteVoucherSecondValidationId)
                .ForeignKey("CRM.SalesQuoteVoucher", t => t.SalesQuoteVoucherId, cascadeDelete: true)
                .Index(t => t.SalesQuoteVoucherId);
            
            CreateTable(
                "CRM.SalesQuoteVoucherValidation",
                c => new
                    {
                        SalesQuoteVoucherValidationId = c.Int(nullable: false, identity: true),
                        SalesQuoteVoucherId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SalesQuoteVoucherValidationId)
                .ForeignKey("CRM.SalesQuoteVoucher", t => t.SalesQuoteVoucherId, cascadeDelete: true)
                .Index(t => t.SalesQuoteVoucherId);
            
            CreateTable(
                "CRM.SalesShipmentDistribution",
                c => new
                    {
                        SalesShipmentDistributionId = c.Int(nullable: false, identity: true),
                        SalesShipmentId = c.Int(nullable: false),
                        DistributionType = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        AdditionalDescription = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SalesShipmentDistributionId)
                .ForeignKey("CRM.SalesShipments", t => t.SalesShipmentId, cascadeDelete: true)
                .Index(t => t.SalesShipmentId);
            
            CreateTable(
                "CRM.SalesShipments",
                c => new
                    {
                        SalesShipmentId = c.Int(nullable: false, identity: true),
                        CashSalesInvoiceID = c.Int(),
                        FsInvoiceNumber = c.String(),
                        SalesOrderVoucherID = c.Int(),
                        ShipmentDate = c.DateTime(nullable: false),
                        Source = c.Int(nullable: false),
                        OrganizationCustomerId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        StoreID = c.Int(nullable: false),
                        CRMPeriodId = c.Int(nullable: false),
                        MethodOfShipment = c.String(),
                        PlateNo = c.String(),
                        TrailerPlateNo = c.String(),
                        JobOrderNumber = c.String(),
                        StoreRequisitionNumber = c.String(),
                        IsPostedToSecondLedger = c.Boolean(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        IsOnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SalesShipmentId);
            
            CreateTable(
                "CRM.SalesShipmentLineItem",
                c => new
                    {
                        SalesShipmentLineItemId = c.Int(nullable: false, identity: true),
                        LineItemId = c.Int(nullable: false),
                        SalesShipmentId = c.Int(nullable: false),
                        SalesStoreId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Variety = c.String(),
                        Class = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 4),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalQuantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ShipedQuantityTilPreShipment = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ShipedQuantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        RemainingQuantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SalesShipmentLineItemId)
                .ForeignKey("CRM.SalesShipments", t => t.SalesShipmentId, cascadeDelete: true)
                .Index(t => t.SalesShipmentId);
            
            CreateTable(
                "CRM.SalesShipmentValidation",
                c => new
                    {
                        SalesShipmentValidationId = c.Int(nullable: false, identity: true),
                        SalesShipmentId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SalesShipmentValidationId)
                .ForeignKey("CRM.SalesShipments", t => t.SalesShipmentId, cascadeDelete: true)
                .Index(t => t.SalesShipmentId);
            
            CreateTable(
                "CRM.StoreAnnualSalesPlan",
                c => new
                    {
                        StoreAnnualSalesPlanId = c.Int(nullable: false, identity: true),
                        BranchAnnualSalesPlanId = c.Int(nullable: false),
                        SalesStoreId = c.Int(nullable: false),
                        Ratio = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AnnualQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AnnualValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StoreAnnualSalesPlanId)
                .ForeignKey("CRM.BranchAnnualSalesPlan", t => t.BranchAnnualSalesPlanId, cascadeDelete: true)
                .Index(t => t.BranchAnnualSalesPlanId);
            
            CreateTable(
                "CRM.StoreMonthlySalesPlan",
                c => new
                    {
                        StoreMonthlySalesPlanId = c.Int(nullable: false, identity: true),
                        StoreAnnualSalesPlanId = c.Int(nullable: false),
                        CRMPeriodId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StoreMonthlySalesPlanId)
                .ForeignKey("CRM.StoreAnnualSalesPlan", t => t.StoreAnnualSalesPlanId, cascadeDelete: true)
                .Index(t => t.StoreAnnualSalesPlanId);
            
            CreateTable(
                "CRM.Tax",
                c => new
                    {
                        TaxID = c.Int(nullable: false, identity: true),
                        Category = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TaxID);
            
            CreateTable(
                "CRM.TenderFollowup",
                c => new
                    {
                        TenderFollowupId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        BidBondAmount = c.Decimal(nullable: false, precision: 20, scale: 4),
                        DateAnnounceBidValidatiy = c.DateTime(),
                        ServiceCharge = c.Decimal(precision: 20, scale: 4),
                        ClosingDate = c.DateTime(nullable: false),
                        OpeningDate = c.DateTime(nullable: false),
                        Location = c.String(),
                        DocumentPreparationStartDate = c.DateTime(),
                        Status = c.String(),
                        TenderResult = c.String(),
                        Remark = c.String(),
                        LatestLetter = c.String(),
                        ContactPerson = c.String(),
                        BidAmount = c.Decimal(nullable: false, precision: 20, scale: 4),
                        BidSecurityCharge = c.Decimal(nullable: false, precision: 20, scale: 4),
                        BidValidty = c.Decimal(nullable: false, precision: 20, scale: 4),
                        ReasonOfNotParticipating = c.String(),
                        ReasonOfLosing = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TenderFollowupId);
            
            CreateTable(
                "CRM.TenderFollowupLineItem",
                c => new
                    {
                        TenderFollowupLineItemId = c.Int(nullable: false, identity: true),
                        TenderFollowupId = c.Int(nullable: false),
                        ItemCategoryCode = c.String(),
                        ItemCode = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsExternalProduct = c.Boolean(nullable: false),
                        Category = c.String(),
                        Product = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TenderFollowupLineItemId)
                .ForeignKey("CRM.TenderFollowup", t => t.TenderFollowupId, cascadeDelete: true)
                .Index(t => t.TenderFollowupId);
            
            CreateTable(
                "CRM.VoucherContractDocument",
                c => new
                    {
                        VoucherContractDocumentId = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                        Content = c.Binary(),
                        ContentType = c.String(),
                        Remark = c.String(),
                        VoucherId = c.Int(nullable: false),
                        VoucherType = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.VoucherContractDocumentId);
            
            AddColumn("CRM.AdditionalCostLog", "CostAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("CRM.AdditionalCostLog", "GLTaxRateID", c => c.Int(nullable: false));
            AddColumn("CRM.AdditionalCostLog", "VoucherId", c => c.Int(nullable: false));
            AddColumn("CRM.AdditionalCostLog", "VoucherType", c => c.Int(nullable: false));
            AddColumn("CRM.AdditionalCostLog", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("CRM.AdditionalCostLog", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("CRM.AdditionalCostLog", "OrgBranchName", c => c.String());
            AddColumn("CRM.AdditionalCostLog", "DateCreated", c => c.DateTime());
            AddColumn("CRM.AdditionalCostLog", "LastModified", c => c.DateTime());
            AddColumn("CRM.AdditionalCostLog", "CreatedBy", c => c.String());
            AddColumn("CRM.AdditionalCostLog", "ModifiedBy", c => c.String());
            AddColumn("CRM.LineItem", "Discount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("CRM.LineItem", "StoreRequisitionNumber", c => c.String());
            AddColumn("CRM.LineItem", "ShipmentNumber", c => c.String());
            AddColumn("CRM.Product", "SubCategoryId", c => c.String());
            AddColumn("CRM.Product", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("CRM.SalesStore", "Type", c => c.Int(nullable: false));
            AddColumn("CRM.SalesStore", "PosId", c => c.String());
            AddColumn("CRM.SalesStore", "PosServiceBaseUrl", c => c.String());
            AddColumn("CRM.AdvancePaymentDistibution", "AdvancePaymentId", c => c.Int(nullable: false));
            AddColumn("CRM.AdvancePayment", "BranchId", c => c.Int(nullable: false));
            AddColumn("CRM.CashSalesInvoice", "DueDate", c => c.DateTime());
            AddColumn("CRM.CashSalesInvoice", "AmountDue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("CRM.CashSalesInvoice", "IsPaidWithCash", c => c.Boolean(nullable: false));
            AddColumn("CRM.CashSalesInvoice", "IsManual", c => c.Boolean(nullable: false));
            AddColumn("CRM.CashSalesInvoice", "ReceiptReference", c => c.String());
            AddColumn("CRM.CashSalesInvoice", "PaymentTermsID", c => c.Int(nullable: false));
            AddColumn("CRM.CashSalesInvoice", "BranchId", c => c.Int(nullable: false));
            AddColumn("CRM.CashSalesInvoice", "StoreID", c => c.Int(nullable: false));
            AddColumn("CRM.CashSalesInvoice", "CRMPeriodId", c => c.Int(nullable: false));
            AddColumn("CRM.CashSalesInvoice", "Source", c => c.Int(nullable: false));
            AddColumn("CRM.CashSalesInvoice", "ShipmentGenerationMethod", c => c.Int(nullable: false));
            AddColumn("CRM.CashSalesInvoice", "InvoiceNumber", c => c.String());
            AddColumn("CRM.CashSalesInvoice", "PosPrintType", c => c.Int(nullable: false));
            AddColumn("CRM.CashSalesInvoice", "FsInvoiceNo", c => c.String());
            AddColumn("CRM.CashSalesInvoice", "EJNo", c => c.String());
            AddColumn("CRM.CashSalesInvoice", "MachineID", c => c.String());
            AddColumn("CRM.CashSalesInvoice", "IsShipped", c => c.Boolean(nullable: false));
            AddColumn("CRM.CashSalesInvoice", "IsPosSyncLocked", c => c.Boolean(nullable: false));
            AddColumn("CRM.CashSalesInvoice", "JobOrderNumber", c => c.String());
            AddColumn("CRM.CashSalesInvoice", "PlateNo", c => c.String());
            AddColumn("CRM.CashSalesInvoice", "IsSendForHold", c => c.Boolean(nullable: false));
            AddColumn("CRM.CashSalesInvoice", "SendForHoldTime", c => c.DateTime());
            AddColumn("CRM.CashSalesInvoice", "SendForHoldBy", c => c.String());
            AddColumn("CRM.CashSalesInvoice", "InvoicePrintTime", c => c.DateTime());
            AddColumn("CRM.CashSalesInvoice", "InvoicePrintedBy", c => c.String());
            AddColumn("CRM.CreditSalesInvoice", "IsManual", c => c.Boolean(nullable: false));
            AddColumn("CRM.CreditSalesInvoice", "ReceiptReference", c => c.String());
            AddColumn("CRM.CreditSalesInvoice", "PaymentTermsID", c => c.Int(nullable: false));
            AddColumn("CRM.CreditSalesInvoice", "BranchId", c => c.Int(nullable: false));
            AddColumn("CRM.CreditSalesInvoice", "CRMPeriodId", c => c.Int(nullable: false));
            AddColumn("CRM.CreditSettlement", "BranchId", c => c.Int(nullable: false));
            AddColumn("CRM.CreditSettlement", "ReceiptNumber", c => c.String());
            AddColumn("CRM.CreditSettlement", "CRMPeriodId", c => c.Int(nullable: false));
            AddColumn("CRM.LineCreditSettlement", "NetBalance", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("CRM.LineCreditSettlement", "WithholdingType", c => c.Int(nullable: false));
            AddColumn("CRM.LineCreditSettlement", "WithholdingAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("EXCEED.ProductCategory", "InventoryCategoryCode", c => c.String());
            AddColumn("EXCEED.ProductCategory", "UnEarnedAccountCode", c => c.String());
            AddColumn("CRM.SalesOrderVoucher", "BranchId", c => c.Int(nullable: false));
            AddColumn("CRM.SalesOrderVoucher", "StoreID", c => c.Int(nullable: false));
            AddColumn("CRM.SalesOrderVoucher", "CRMPeriodId", c => c.Int(nullable: false));
            AddColumn("CRM.SalesOrderVoucher", "Source", c => c.Int(nullable: false));
            AddColumn("CRM.SalesOrderVoucher", "IsShipped", c => c.Boolean(nullable: false));
            AddColumn("CRM.SalesOrderVoucher", "JobOrderNumber", c => c.String());
            AddColumn("CRM.SalesOrderVoucher", "PlateNo", c => c.String());
            AddColumn("CRM.SalesQuoteVoucher", "Source", c => c.Int(nullable: false));
            AddColumn("CRM.SalesQuoteVoucher", "BranchId", c => c.Int(nullable: false));
            AddColumn("CRM.SalesQuoteVoucher", "StoreID", c => c.Int(nullable: false));
            AddColumn("CRM.SalesQuoteVoucher", "CRMPeriodId", c => c.Int(nullable: false));
            AddColumn("CRM.SalesQuoteVoucher", "QuotationTermId", c => c.Int(nullable: false));
            AddColumn("CRM.SalesQuoteVoucher", "DueDate", c => c.DateTime());
            AddColumn("CRM.SalesQuoteVoucher", "JobOrderNumber", c => c.String());
            AddColumn("CRM.SalesQuoteVoucher", "PlateNo", c => c.String());
            AddColumn("CRM.SalesQuoteVoucher", "BankAccountId", c => c.Int());
            AddColumn("CRM.SalesQuoteVoucher", "IsMiddleOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("CRM.SalesQuoteVoucher", "MiddleOnlineApprovedBy", c => c.String());
            AddColumn("CRM.SalesQuoteVoucher", "MiddleOnlineApprovedTime", c => c.DateTime());
            AddColumn("CRM.SalesQuoteVoucher", "IsMiddleSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("CRM.SalesQuoteVoucher", "MiddleSendForApprovalBy", c => c.String());
            AddColumn("CRM.SalesQuoteVoucher", "MiddleSendForApprovalTime", c => c.DateTime());
            AddColumn("CRM.SalesQuoteVoucher", "IsSecondOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("CRM.SalesQuoteVoucher", "SecondOnlineApprovedBy", c => c.String());
            AddColumn("CRM.SalesQuoteVoucher", "SecondOnlineApprovedTime", c => c.DateTime());
            AddColumn("CRM.SalesQuoteVoucher", "IsSecondSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("CRM.SalesQuoteVoucher", "SecondSendForApprovalBy", c => c.String());
            AddColumn("CRM.SalesQuoteVoucher", "SecondSendForApprovalTime", c => c.DateTime());
            AddColumn("CRM.SalesQuoteVoucher", "IsSecondVoid", c => c.Boolean(nullable: false));
            AddColumn("CRM.SalesQuoteVoucher", "SecondVoidBy", c => c.String());
            AddColumn("CRM.SalesQuoteVoucher", "SecondVoidTime", c => c.DateTime());
            AddColumn("CRM.ServiceLineItem", "VoucherType", c => c.Int(nullable: false));
            AddColumn("CRM.ServiceLineItem", "VoucherId", c => c.Int(nullable: false));
            AddColumn("CRM.ServiceLineItem", "ServiceTypeId", c => c.String());
            AddColumn("CRM.ServiceLineItem", "ServiceTypeCode", c => c.String());
            AddColumn("CRM.ServiceLineItem", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("CRM.ServiceLineItem", "UnitAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("CRM.ServiceLineItem", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("CRM.ServiceLineItem", "TaxTypeId", c => c.Int());
            AddColumn("CRM.ServiceLineItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("CRM.ServiceLineItem", "WithholdingTypeId", c => c.Int());
            AddColumn("CRM.ServiceLineItem", "WithholdingAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("CRM.ServiceLineItem", "WithholdTax", c => c.Boolean(nullable: false));
            AddColumn("CRM.ServiceLineItem", "WithholdTaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("CRM.AdditionalCostLog", "LineItem_LineItemID", c => c.Int());
            AlterColumn("CRM.LineItem", "StockBalanceReserved", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("CRM.LineItem", "StockBalance", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("CRM.LineItem", "UnitAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("CRM.LineItem", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("CRM.LineItem", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("CRM.LineItem", "Tax", c => c.String());
            AlterColumn("CRM.LineItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("CRM.LineItem", "CalculatedCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("CRM.LineItem", "WithholdingAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("CRM.LineItem", "WithholdTaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("CRM.LineItem", "Vat", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("CRM.AdvancePaymentDistibution", "Debit", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("CRM.AdvancePaymentDistibution", "Credit", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("CRM.SalesInvoicePaymentDistibution", "Debit", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("CRM.SalesInvoicePaymentDistibution", "Credit", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("CRM.LineCreditSettlement", "SettelmentAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("CRM.LineCreditSettlement", "RemainingAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("CRM.Price", "Value", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("CRM.ServiceLineItem", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            CreateIndex("CRM.AdditionalCostLog", "LineItem_LineItemID");
            CreateIndex("CRM.AdvancePaymentDistibution", "AdvancePaymentId");
            AddForeignKey("CRM.AdvancePaymentDistibution", "AdvancePaymentId", "CRM.AdvancePayment", "AdvancePaymentId", cascadeDelete: true);
            AddForeignKey("CRM.AdditionalCostLog", "LineItem_LineItemID", "CRM.LineItem", "LineItemID");
            DropColumn("CRM.AdditionalCostLog", "Value");
            DropColumn("CRM.LineItem", "PriceTagDetail");
            DropColumn("CRM.AdvancePaymentDistibution", "AdvancePaymentLineId");
            DropColumn("CRM.ServiceLineItem", "ServiceUsageId");
            DropColumn("CRM.ServiceLineItem", "CustomerServiceId");
            DropColumn("CRM.ServiceLineItem", "UnitPrice");
            DropColumn("CRM.ServiceLineItem", "Tax");
            DropColumn("CRM.ServiceLineItem", "IsPostedToSecondLedger");
            DropColumn("CRM.ServiceLineItem", "Status");
            DropColumn("CRM.ServiceLineItem", "Remark");
            DropColumn("CRM.ServiceLineItem", "IsVoid");
            DropColumn("CRM.ServiceLineItem", "VoidBy");
            DropColumn("CRM.ServiceLineItem", "VoidTime");
            DropColumn("CRM.ServiceLineItem", "IsOnlineApproved");
            DropColumn("CRM.ServiceLineItem", "OnlineApprovedBy");
            DropColumn("CRM.ServiceLineItem", "OnlineApprovedTime");
            DropColumn("CRM.ServiceLineItem", "IsOnlineTransferred");
            DropColumn("CRM.ServiceLineItem", "IsOnlineTransferredBy");
            DropColumn("CRM.ServiceLineItem", "OnlineTransferredTime");
            DropColumn("CRM.ServiceLineItem", "IsSendForApproval");
            DropColumn("CRM.ServiceLineItem", "SendForApprovalBy");
            DropColumn("CRM.ServiceLineItem", "SendForApprovalTime");
        }
        
        public override void Down()
        {
            AddColumn("CRM.ServiceLineItem", "SendForApprovalTime", c => c.DateTime());
            AddColumn("CRM.ServiceLineItem", "SendForApprovalBy", c => c.String());
            AddColumn("CRM.ServiceLineItem", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("CRM.ServiceLineItem", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("CRM.ServiceLineItem", "IsOnlineTransferredBy", c => c.String());
            AddColumn("CRM.ServiceLineItem", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("CRM.ServiceLineItem", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("CRM.ServiceLineItem", "OnlineApprovedBy", c => c.String());
            AddColumn("CRM.ServiceLineItem", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("CRM.ServiceLineItem", "VoidTime", c => c.DateTime());
            AddColumn("CRM.ServiceLineItem", "VoidBy", c => c.String());
            AddColumn("CRM.ServiceLineItem", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("CRM.ServiceLineItem", "Remark", c => c.String());
            AddColumn("CRM.ServiceLineItem", "Status", c => c.String());
            AddColumn("CRM.ServiceLineItem", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.ServiceLineItem", "Tax", c => c.String(nullable: false));
            AddColumn("CRM.ServiceLineItem", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("CRM.ServiceLineItem", "CustomerServiceId", c => c.Int(nullable: false));
            AddColumn("CRM.ServiceLineItem", "ServiceUsageId", c => c.Int(nullable: false));
            AddColumn("CRM.AdvancePaymentDistibution", "AdvancePaymentLineId", c => c.Int(nullable: false));
            AddColumn("CRM.LineItem", "PriceTagDetail", c => c.String());
            AddColumn("CRM.AdditionalCostLog", "Value", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropForeignKey("CRM.AdditionalCostLog", "LineItem_LineItemID", "CRM.LineItem");
            DropForeignKey("CRM.TenderFollowupLineItem", "TenderFollowupId", "CRM.TenderFollowup");
            DropForeignKey("CRM.StoreMonthlySalesPlan", "StoreAnnualSalesPlanId", "CRM.StoreAnnualSalesPlan");
            DropForeignKey("CRM.StoreAnnualSalesPlan", "BranchAnnualSalesPlanId", "CRM.BranchAnnualSalesPlan");
            DropForeignKey("CRM.SalesShipmentValidation", "SalesShipmentId", "CRM.SalesShipments");
            DropForeignKey("CRM.SalesShipmentLineItem", "SalesShipmentId", "CRM.SalesShipments");
            DropForeignKey("CRM.SalesShipmentDistribution", "SalesShipmentId", "CRM.SalesShipments");
            DropForeignKey("CRM.SalesQuoteVoucherValidation", "SalesQuoteVoucherId", "CRM.SalesQuoteVoucher");
            DropForeignKey("CRM.SalesQuoteVoucherSecondValidation", "SalesQuoteVoucherId", "CRM.SalesQuoteVoucher");
            DropForeignKey("CRM.SalesQuoteVoucherFirstValidation", "SalesQuoteVoucherId", "CRM.SalesQuoteVoucher");
            DropForeignKey("CRM.SalesPersonAssignment", "SalesStoreId", "CRM.SalesStore");
            DropForeignKey("CRM.SalesOrderVoucherValidation", "SalesOrderVoucherId", "CRM.SalesOrderVoucher");
            DropForeignKey("CRM.QueueValidation", "QueueId", "CRM.Queue");
            DropForeignKey("CRM.QueueStatusValidation", "QueueStatusId", "CRM.QueueStatus");
            DropForeignKey("CRM.QueueStatus", "QueueId", "CRM.Queue");
            DropForeignKey("CRM.Queue", "QueueTypeId", "CRM.QueueType");
            DropForeignKey("CRM.Queue", "QueueCustomerId", "CRM.QueueCustomer");
            DropForeignKey("CRM.QueueCustomer", "InvestmentTypeId", "CRM.InvestmentType");
            DropForeignKey("CRM.ProductPriceLog", "StoreItemAssignmentID", "EXCEED.StoreItemAssignment");
            DropForeignKey("CRM.PriceEstimationQuoteVoucherValidation", "PriceEstimationQuoteVoucherId", "CRM.PriceEstimationQuoteVouchers");
            DropForeignKey("CRM.PriceEstimationQuoteVoucherServices", "PriceEstimationQuoteVoucherId", "CRM.PriceEstimationQuoteVouchers");
            DropForeignKey("CRM.PriceEstimationQuoteVoucherSecondValidation", "PriceEstimationQuoteVoucherId", "CRM.PriceEstimationQuoteVouchers");
            DropForeignKey("CRM.PriceEstimationQuoteVoucherLines", "PriceEstimationQuoteVoucherId", "CRM.PriceEstimationQuoteVouchers");
            DropForeignKey("CRM.PriceEstimationQuoteVoucherFirstValidation", "PriceEstimationQuoteVoucherId", "CRM.PriceEstimationQuoteVouchers");
            DropForeignKey("CRM.PriceEstimationQuoteVouchers", "OrganizationCustomerId", "CRM.OrganizationCustomer");
            DropForeignKey("CRM.LocationUserAssignment", "LocationMapId", "CRM.LocationMap");
            DropForeignKey("CRM.LocationMap", "BranchId", "HRM.Branch");
            DropForeignKey("CRM.CRMDepositValidation", "CRMDepositId", "CRM.CRMDeposit");
            DropForeignKey("CRM.CRMDepositLine", "CRMDepositId", "CRM.CRMDeposit");
            DropForeignKey("CRM.CRMDepositDistribution", "CRMDepositId", "CRM.CRMDeposit");
            DropForeignKey("CRM.CRMDashboardTodateSalesBranchData", "BranchId", "HRM.Branch");
            DropForeignKey("CRM.CreditSettlementValidation", "CreditSettlementId", "CRM.CreditSettlement");
            DropForeignKey("CRM.CreditSettlementDistribution", "CreditSettlementId", "CRM.CreditSettlement");
            DropForeignKey("CRM.CashSalesInvoiceValidation", "CashSalesInvoiceId", "CRM.CashSalesInvoice");
            DropForeignKey("CRM.BranchAnnualSalesPlan", "AnnualSalesPlanId", "CRM.AnnualSalesPlan");
            DropForeignKey("CRM.AnnualSalesPlanValidation", "AnnualSalesPlanId", "CRM.AnnualSalesPlan");
            DropForeignKey("CRM.AdvancePaymentDistibution", "AdvancePaymentId", "CRM.AdvancePayment");
            DropIndex("CRM.TenderFollowupLineItem", new[] { "TenderFollowupId" });
            DropIndex("CRM.StoreMonthlySalesPlan", new[] { "StoreAnnualSalesPlanId" });
            DropIndex("CRM.StoreAnnualSalesPlan", new[] { "BranchAnnualSalesPlanId" });
            DropIndex("CRM.SalesShipmentValidation", new[] { "SalesShipmentId" });
            DropIndex("CRM.SalesShipmentLineItem", new[] { "SalesShipmentId" });
            DropIndex("CRM.SalesShipmentDistribution", new[] { "SalesShipmentId" });
            DropIndex("CRM.SalesQuoteVoucherValidation", new[] { "SalesQuoteVoucherId" });
            DropIndex("CRM.SalesQuoteVoucherSecondValidation", new[] { "SalesQuoteVoucherId" });
            DropIndex("CRM.SalesQuoteVoucherFirstValidation", new[] { "SalesQuoteVoucherId" });
            DropIndex("CRM.SalesPersonAssignment", new[] { "SalesStoreId" });
            DropIndex("CRM.SalesOrderVoucherValidation", new[] { "SalesOrderVoucherId" });
            DropIndex("CRM.QueueValidation", new[] { "QueueId" });
            DropIndex("CRM.QueueStatusValidation", new[] { "QueueStatusId" });
            DropIndex("CRM.QueueStatus", new[] { "QueueId" });
            DropIndex("CRM.QueueCustomer", new[] { "InvestmentTypeId" });
            DropIndex("CRM.Queue", new[] { "QueueTypeId" });
            DropIndex("CRM.Queue", new[] { "QueueCustomerId" });
            DropIndex("CRM.ProductPriceLog", new[] { "StoreItemAssignmentID" });
            DropIndex("CRM.PriceEstimationQuoteVoucherValidation", new[] { "PriceEstimationQuoteVoucherId" });
            DropIndex("CRM.PriceEstimationQuoteVoucherServices", new[] { "PriceEstimationQuoteVoucherId" });
            DropIndex("CRM.PriceEstimationQuoteVoucherSecondValidation", new[] { "PriceEstimationQuoteVoucherId" });
            DropIndex("CRM.PriceEstimationQuoteVoucherLines", new[] { "PriceEstimationQuoteVoucherId" });
            DropIndex("CRM.PriceEstimationQuoteVouchers", new[] { "OrganizationCustomerId" });
            DropIndex("CRM.PriceEstimationQuoteVoucherFirstValidation", new[] { "PriceEstimationQuoteVoucherId" });
            DropIndex("CRM.LocationUserAssignment", new[] { "LocationMapId" });
            DropIndex("CRM.LocationMap", new[] { "BranchId" });
            DropIndex("CRM.CRMDepositValidation", new[] { "CRMDepositId" });
            DropIndex("CRM.CRMDepositLine", new[] { "CRMDepositId" });
            DropIndex("CRM.CRMDepositDistribution", new[] { "CRMDepositId" });
            DropIndex("CRM.CRMDashboardTodateSalesBranchData", new[] { "BranchId" });
            DropIndex("CRM.CreditSettlementValidation", new[] { "CreditSettlementId" });
            DropIndex("CRM.CreditSettlementDistribution", new[] { "CreditSettlementId" });
            DropIndex("CRM.CashSalesInvoiceValidation", new[] { "CashSalesInvoiceId" });
            DropIndex("CRM.BranchAnnualSalesPlan", new[] { "AnnualSalesPlanId" });
            DropIndex("CRM.AnnualSalesPlanValidation", new[] { "AnnualSalesPlanId" });
            DropIndex("CRM.AdvancePaymentDistibution", new[] { "AdvancePaymentId" });
            DropIndex("CRM.AdditionalCostLog", new[] { "LineItem_LineItemID" });
            AlterColumn("CRM.ServiceLineItem", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("CRM.Price", "Value", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("CRM.LineCreditSettlement", "RemainingAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("CRM.LineCreditSettlement", "SettelmentAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("CRM.SalesInvoicePaymentDistibution", "Credit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("CRM.SalesInvoicePaymentDistibution", "Debit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("CRM.AdvancePaymentDistibution", "Credit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("CRM.AdvancePaymentDistibution", "Debit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("CRM.LineItem", "Vat", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("CRM.LineItem", "WithholdTaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("CRM.LineItem", "WithholdingAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("CRM.LineItem", "CalculatedCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("CRM.LineItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("CRM.LineItem", "Tax", c => c.String(nullable: false));
            AlterColumn("CRM.LineItem", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("CRM.LineItem", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("CRM.LineItem", "UnitAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("CRM.LineItem", "StockBalance", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("CRM.LineItem", "StockBalanceReserved", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("CRM.AdditionalCostLog", "LineItem_LineItemID", c => c.Int(nullable: false));
            DropColumn("CRM.ServiceLineItem", "WithholdTaxAmount");
            DropColumn("CRM.ServiceLineItem", "WithholdTax");
            DropColumn("CRM.ServiceLineItem", "WithholdingAmount");
            DropColumn("CRM.ServiceLineItem", "WithholdingTypeId");
            DropColumn("CRM.ServiceLineItem", "TaxAmount");
            DropColumn("CRM.ServiceLineItem", "TaxTypeId");
            DropColumn("CRM.ServiceLineItem", "SubTotal");
            DropColumn("CRM.ServiceLineItem", "UnitAmount");
            DropColumn("CRM.ServiceLineItem", "Quantity");
            DropColumn("CRM.ServiceLineItem", "ServiceTypeCode");
            DropColumn("CRM.ServiceLineItem", "ServiceTypeId");
            DropColumn("CRM.ServiceLineItem", "VoucherId");
            DropColumn("CRM.ServiceLineItem", "VoucherType");
            DropColumn("CRM.SalesQuoteVoucher", "SecondVoidTime");
            DropColumn("CRM.SalesQuoteVoucher", "SecondVoidBy");
            DropColumn("CRM.SalesQuoteVoucher", "IsSecondVoid");
            DropColumn("CRM.SalesQuoteVoucher", "SecondSendForApprovalTime");
            DropColumn("CRM.SalesQuoteVoucher", "SecondSendForApprovalBy");
            DropColumn("CRM.SalesQuoteVoucher", "IsSecondSendForApproval");
            DropColumn("CRM.SalesQuoteVoucher", "SecondOnlineApprovedTime");
            DropColumn("CRM.SalesQuoteVoucher", "SecondOnlineApprovedBy");
            DropColumn("CRM.SalesQuoteVoucher", "IsSecondOnlineApproved");
            DropColumn("CRM.SalesQuoteVoucher", "MiddleSendForApprovalTime");
            DropColumn("CRM.SalesQuoteVoucher", "MiddleSendForApprovalBy");
            DropColumn("CRM.SalesQuoteVoucher", "IsMiddleSendForApproval");
            DropColumn("CRM.SalesQuoteVoucher", "MiddleOnlineApprovedTime");
            DropColumn("CRM.SalesQuoteVoucher", "MiddleOnlineApprovedBy");
            DropColumn("CRM.SalesQuoteVoucher", "IsMiddleOnlineApproved");
            DropColumn("CRM.SalesQuoteVoucher", "BankAccountId");
            DropColumn("CRM.SalesQuoteVoucher", "PlateNo");
            DropColumn("CRM.SalesQuoteVoucher", "JobOrderNumber");
            DropColumn("CRM.SalesQuoteVoucher", "DueDate");
            DropColumn("CRM.SalesQuoteVoucher", "QuotationTermId");
            DropColumn("CRM.SalesQuoteVoucher", "CRMPeriodId");
            DropColumn("CRM.SalesQuoteVoucher", "StoreID");
            DropColumn("CRM.SalesQuoteVoucher", "BranchId");
            DropColumn("CRM.SalesQuoteVoucher", "Source");
            DropColumn("CRM.SalesOrderVoucher", "PlateNo");
            DropColumn("CRM.SalesOrderVoucher", "JobOrderNumber");
            DropColumn("CRM.SalesOrderVoucher", "IsShipped");
            DropColumn("CRM.SalesOrderVoucher", "Source");
            DropColumn("CRM.SalesOrderVoucher", "CRMPeriodId");
            DropColumn("CRM.SalesOrderVoucher", "StoreID");
            DropColumn("CRM.SalesOrderVoucher", "BranchId");
            DropColumn("EXCEED.ProductCategory", "UnEarnedAccountCode");
            DropColumn("EXCEED.ProductCategory", "InventoryCategoryCode");
            DropColumn("CRM.LineCreditSettlement", "WithholdingAmount");
            DropColumn("CRM.LineCreditSettlement", "WithholdingType");
            DropColumn("CRM.LineCreditSettlement", "NetBalance");
            DropColumn("CRM.CreditSettlement", "CRMPeriodId");
            DropColumn("CRM.CreditSettlement", "ReceiptNumber");
            DropColumn("CRM.CreditSettlement", "BranchId");
            DropColumn("CRM.CreditSalesInvoice", "CRMPeriodId");
            DropColumn("CRM.CreditSalesInvoice", "BranchId");
            DropColumn("CRM.CreditSalesInvoice", "PaymentTermsID");
            DropColumn("CRM.CreditSalesInvoice", "ReceiptReference");
            DropColumn("CRM.CreditSalesInvoice", "IsManual");
            DropColumn("CRM.CashSalesInvoice", "InvoicePrintedBy");
            DropColumn("CRM.CashSalesInvoice", "InvoicePrintTime");
            DropColumn("CRM.CashSalesInvoice", "SendForHoldBy");
            DropColumn("CRM.CashSalesInvoice", "SendForHoldTime");
            DropColumn("CRM.CashSalesInvoice", "IsSendForHold");
            DropColumn("CRM.CashSalesInvoice", "PlateNo");
            DropColumn("CRM.CashSalesInvoice", "JobOrderNumber");
            DropColumn("CRM.CashSalesInvoice", "IsPosSyncLocked");
            DropColumn("CRM.CashSalesInvoice", "IsShipped");
            DropColumn("CRM.CashSalesInvoice", "MachineID");
            DropColumn("CRM.CashSalesInvoice", "EJNo");
            DropColumn("CRM.CashSalesInvoice", "FsInvoiceNo");
            DropColumn("CRM.CashSalesInvoice", "PosPrintType");
            DropColumn("CRM.CashSalesInvoice", "InvoiceNumber");
            DropColumn("CRM.CashSalesInvoice", "ShipmentGenerationMethod");
            DropColumn("CRM.CashSalesInvoice", "Source");
            DropColumn("CRM.CashSalesInvoice", "CRMPeriodId");
            DropColumn("CRM.CashSalesInvoice", "StoreID");
            DropColumn("CRM.CashSalesInvoice", "BranchId");
            DropColumn("CRM.CashSalesInvoice", "PaymentTermsID");
            DropColumn("CRM.CashSalesInvoice", "ReceiptReference");
            DropColumn("CRM.CashSalesInvoice", "IsManual");
            DropColumn("CRM.CashSalesInvoice", "IsPaidWithCash");
            DropColumn("CRM.CashSalesInvoice", "AmountDue");
            DropColumn("CRM.CashSalesInvoice", "DueDate");
            DropColumn("CRM.AdvancePayment", "BranchId");
            DropColumn("CRM.AdvancePaymentDistibution", "AdvancePaymentId");
            DropColumn("CRM.SalesStore", "PosServiceBaseUrl");
            DropColumn("CRM.SalesStore", "PosId");
            DropColumn("CRM.SalesStore", "Type");
            DropColumn("CRM.Product", "Price");
            DropColumn("CRM.Product", "SubCategoryId");
            DropColumn("CRM.LineItem", "ShipmentNumber");
            DropColumn("CRM.LineItem", "StoreRequisitionNumber");
            DropColumn("CRM.LineItem", "Discount");
            DropColumn("CRM.AdditionalCostLog", "ModifiedBy");
            DropColumn("CRM.AdditionalCostLog", "CreatedBy");
            DropColumn("CRM.AdditionalCostLog", "LastModified");
            DropColumn("CRM.AdditionalCostLog", "DateCreated");
            DropColumn("CRM.AdditionalCostLog", "OrgBranchName");
            DropColumn("CRM.AdditionalCostLog", "TotalAmount");
            DropColumn("CRM.AdditionalCostLog", "TaxAmount");
            DropColumn("CRM.AdditionalCostLog", "VoucherType");
            DropColumn("CRM.AdditionalCostLog", "VoucherId");
            DropColumn("CRM.AdditionalCostLog", "GLTaxRateID");
            DropColumn("CRM.AdditionalCostLog", "CostAmount");
            DropTable("CRM.VoucherContractDocument");
            DropTable("CRM.TenderFollowupLineItem");
            DropTable("CRM.TenderFollowup");
            DropTable("CRM.Tax");
            DropTable("CRM.StoreMonthlySalesPlan");
            DropTable("CRM.StoreAnnualSalesPlan");
            DropTable("CRM.SalesShipmentValidation");
            DropTable("CRM.SalesShipmentLineItem");
            DropTable("CRM.SalesShipments");
            DropTable("CRM.SalesShipmentDistribution");
            DropTable("CRM.SalesQuoteVoucherValidation");
            DropTable("CRM.SalesQuoteVoucherSecondValidation");
            DropTable("CRM.SalesQuoteVoucherFirstValidation");
            DropTable("CRM.SalesPersonAssignment");
            DropTable("CRM.SalesOrderVoucherValidation");
            DropTable("CRM.QuotationTerm");
            DropTable("CRM.QueueValidation");
            DropTable("CRM.QueueStatusValidation");
            DropTable("CRM.QueueStatus");
            DropTable("CRM.QueueType");
            DropTable("CRM.QueueCustomer");
            DropTable("CRM.Queue");
            DropTable("CRM.ProductUnitConvertorSetup");
            DropTable("CRM.ProductPriceLog");
            DropTable("CRM.PriceEstimationQuoteVoucherValidation");
            DropTable("CRM.PriceEstimationQuoteVoucherServices");
            DropTable("CRM.PriceEstimationQuoteVoucherSecondValidation");
            DropTable("CRM.PriceEstimationQuoteVoucherLines");
            DropTable("CRM.PriceEstimationQuoteVouchers");
            DropTable("CRM.PriceEstimationQuoteVoucherFirstValidation");
            DropTable("CRM.PosSetting");
            DropTable("CRM.PaymentSchedule");
            DropTable("CRM.LocationUserAssignment");
            DropTable("CRM.LocationMap");
            DropTable("CRM.InvestmentType");
            DropTable("CRM.CRMPeriod");
            DropTable("CRM.CRMDepositValidation");
            DropTable("CRM.CRMDepositLine");
            DropTable("CRM.CRMDepositDistribution");
            DropTable("CRM.CRMDeposit");
            DropTable("CRM.CRMDashboardTodateSalesData");
            DropTable("CRM.CRMDashboardTodateSalesBranchData");
            DropTable("CRM.CreditSettlementValidation");
            DropTable("CRM.CreditSettlementDistribution");
            DropTable("CRM.CommissionLineItem");
            DropTable("CRM.CashSalesInvoiceValidation");
            DropTable("CRM.BranchAnnualSalesPlan");
            DropTable("CRM.AnnualSalesPlanValidation");
            DropTable("CRM.AnnualSalesPlan");
            RenameColumn(table: "CRM.AdditionalCostLog", name: "LineItem_LineItemID", newName: "LineItemId");
            CreateIndex("CRM.ServiceLineItem", "CustomerServiceId");
            CreateIndex("CRM.AdvancePaymentDistibution", "AdvancePaymentLineId");
            CreateIndex("CRM.AdditionalCostLog", "LineItemId");
            AddForeignKey("CRM.AdditionalCostLog", "LineItemId", "CRM.LineItem", "LineItemID", cascadeDelete: true);
            AddForeignKey("CRM.ServiceLineItem", "CustomerServiceId", "CRM.CustomerService", "CustomerServiceId", cascadeDelete: true);
            AddForeignKey("CRM.AdvancePaymentDistibution", "AdvancePaymentLineId", "CRM.AdvancePaymentLine", "AdvancePaymentLineId", cascadeDelete: true);
        }
    }
}
