namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class financeandprocurementu : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("HRM.BranchCostCenter", "Values", "EXCEED.GLChartOfAccountCostCenter");
            DropForeignKey("EXCEED.GlSecondayLedgerLine", "GlSecondayLedgerId", "EXCEED.GlSecondayLedger");
            DropForeignKey("EXCEED.PaymentRequest", "BranchId", "HRM.Branch");
            DropForeignKey("EXCEED.PaymentRequest", "MainCompanySturucture_OrgStructureId", "HRM.MainCompanyStructure");
            DropForeignKey("EXCEED.ApPaymentSettlementLine", "ApDirectPaymentId", "EXCEED.ApDirectPayment");
            DropIndex("EXCEED.ApPaymentSettlementLine", new[] { "ApDirectPaymentId" });
            DropIndex("HRM.BranchCostCenter", new[] { "Values" });
            DropIndex("EXCEED.GlSecondayLedgerLine", new[] { "GlSecondayLedgerId" });
            DropIndex("EXCEED.PaymentRequest", new[] { "BranchId" });
            DropIndex("EXCEED.PaymentRequest", new[] { "MainCompanySturucture_OrgStructureId" });
            RenameColumn(table: "EXCEED.ApPaymentSettlementLine", name: "ApDirectPaymentId", newName: "ApDirectPayment_ApDirectPaymentId");
            CreateTable(
                "EXCEED.APPaybleInvoicePrepayment",
                c => new
                    {
                        APPaybleInvoicePrepaymentId = c.Int(nullable: false, identity: true),
                        ApPaybleInvoicesId = c.Int(nullable: false),
                        PaymentId = c.Int(nullable: false),
                        PrepaymentAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Apply = c.Boolean(nullable: false),
                        AppliedPrepaymentAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.APPaybleInvoicePrepaymentId)
                .ForeignKey("EXCEED.ApPaybleInvoices", t => t.ApPaybleInvoicesId, cascadeDelete: true)
                .Index(t => t.ApPaybleInvoicesId);
            
            CreateTable(
                "EXCEED.ApPaymentSettlementGrn",
                c => new
                    {
                        ApPaymentSettlementGrnId = c.Int(nullable: false, identity: true),
                        ApPaymentSettlementId = c.Int(nullable: false),
                        GrnNo = c.Int(nullable: false),
                        ItemType = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApPaymentSettlementGrnId)
                .ForeignKey("EXCEED.ApPaymentSettlement", t => t.ApPaymentSettlementId, cascadeDelete: true)
                .Index(t => t.ApPaymentSettlementId);
            
            CreateTable(
                "EXCEED.ApTPaymentRequestResponse",
                c => new
                    {
                        ApTPaymentRequestResponseId = c.Int(nullable: false, identity: true),
                        Request = c.String(),
                        Response = c.String(),
                        Status = c.String(),
                        ApTPaymentId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApTPaymentRequestResponseId)
                .ForeignKey("EXCEED.ApTPayment", t => t.ApTPaymentId, cascadeDelete: true)
                .Index(t => t.ApTPaymentId);
            
            CreateTable(
                "EXCEED.ApTPayment",
                c => new
                    {
                        ApTPaymentId = c.Int(nullable: false, identity: true),
                        PaymentNum = c.Int(nullable: false),
                        SupplierId = c.String(),
                        SupplierName = c.String(),
                        PaymentType = c.Int(nullable: false),
                        PaymentReference = c.String(),
                        PaymentDate = c.DateTime(nullable: false),
                        TCode = c.String(),
                        TAccount = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ForwardedFrom = c.String(),
                        ForwardedTo = c.String(),
                        IsFinalApproval = c.Boolean(nullable: false),
                        CurrentApprovalStep = c.Int(),
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
                .PrimaryKey(t => t.ApTPaymentId);
            
            CreateTable(
                "EXCEED.ApTPaymentValidation",
                c => new
                    {
                        ApTPaymentValidationId = c.Int(nullable: false, identity: true),
                        ApTPaymentId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApTPaymentValidationId)
                .ForeignKey("EXCEED.ApTPayment", t => t.ApTPaymentId, cascadeDelete: true)
                .Index(t => t.ApTPaymentId);
            
            CreateTable(
                "EXCEED.ApTravelExpenseBudgetValidation",
                c => new
                    {
                        ApTravelExpenseBudgetValidationId = c.Int(nullable: false, identity: true),
                        ApTravelExpenseClaimId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApTravelExpenseBudgetValidationId)
                .ForeignKey("EXCEED.ApTravelExpenseClaim", t => t.ApTravelExpenseClaimId, cascadeDelete: true)
                .Index(t => t.ApTravelExpenseClaimId);
            
            CreateTable(
                "EXCEED.ApTravelExpenseDepartmentValidation",
                c => new
                    {
                        ApTravelExpenseDepartmentValidationId = c.Int(nullable: false, identity: true),
                        ApTravelExpenseClaimId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApTravelExpenseDepartmentValidationId)
                .ForeignKey("EXCEED.ApTravelExpenseClaim", t => t.ApTravelExpenseClaimId, cascadeDelete: true)
                .Index(t => t.ApTravelExpenseClaimId);
            
            CreateTable(
                "EXCEED.ApTravelOrderBudgetValidation",
                c => new
                    {
                        ApTravelOrderBudgetValidationId = c.Int(nullable: false, identity: true),
                        ApTravelOrderId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApTravelOrderBudgetValidationId)
                .ForeignKey("EXCEED.ApTravelOrder", t => t.ApTravelOrderId, cascadeDelete: true)
                .Index(t => t.ApTravelOrderId);
            
            CreateTable(
                "EXCEED.ApTravelOrderDepartmentValidation",
                c => new
                    {
                        ApTravelOrderDepartmentValidationId = c.Int(nullable: false, identity: true),
                        ApTravelOrderId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApTravelOrderDepartmentValidationId)
                .ForeignKey("EXCEED.ApTravelOrder", t => t.ApTravelOrderId, cascadeDelete: true)
                .Index(t => t.ApTravelOrderId);
            
            CreateTable(
                "EXCEED.FundTransferLine",
                c => new
                    {
                        FundTransferLineId = c.Int(nullable: false, identity: true),
                        PaymentRequestId = c.Int(nullable: false),
                        BranchId = c.Int(),
                        Material = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelAndLubricant = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ManPower = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Machinery = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CompensationAndSeverance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FoodAndPerDiem = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalApproved = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPermited = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FundTransferLineId)
                .ForeignKey("EXCEED.PaymentRequest", t => t.PaymentRequestId, cascadeDelete: true)
                .Index(t => t.PaymentRequestId);
            
            CreateTable(
                "EXCEED.PaymentRequestAttachment",
                c => new
                    {
                        PaymentRequestAttachmentId = c.Int(nullable: false, identity: true),
                        PaymentRequestId = c.Int(nullable: false),
                        FileName = c.String(),
                        FilePath = c.String(),
                    })
                .PrimaryKey(t => t.PaymentRequestAttachmentId)
                .ForeignKey("EXCEED.PaymentRequest", t => t.PaymentRequestId, cascadeDelete: true)
                .Index(t => t.PaymentRequestId);
            
            CreateTable(
                "EXCEED.GLAccountConfig",
                c => new
                    {
                        GLAccountConfigId = c.Int(nullable: false, identity: true),
                        Category = c.Int(nullable: false),
                        AccountCode = c.String(),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GLAccountConfigId);
            
            CreateTable(
                "EXCEED.GlCostAllocationSetup",
                c => new
                    {
                        GlCostAllocationSetupId = c.Int(nullable: false, identity: true),
                        GLSegmentSetupId = c.Int(nullable: false),
                        SegmentCode = c.String(),
                        CostCenterCode = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GlCostAllocationSetupId);
            
            CreateTable(
                "EXCEED.PaymentRequestBudgetValidation",
                c => new
                    {
                        PaymentRequestBudgetValidationId = c.Int(nullable: false, identity: true),
                        PaymentRequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PaymentRequestBudgetValidationId)
                .ForeignKey("EXCEED.PaymentRequest", t => t.PaymentRequestId, cascadeDelete: true)
                .Index(t => t.PaymentRequestId);
            
            CreateTable(
                "EXCEED.PurchasePaymentAttachment",
                c => new
                    {
                        PurchasePaymentAttachmentId = c.Int(nullable: false, identity: true),
                        PurchasePaymentId = c.Int(nullable: false),
                        FileName = c.String(),
                        FilePath = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchasePaymentAttachmentId)
                .ForeignKey("EXCEED.PurchasePayment", t => t.PurchasePaymentId, cascadeDelete: true)
                .Index(t => t.PurchasePaymentId);
            
            CreateTable(
                "EXCEED.PurchasePayment",
                c => new
                    {
                        PurchasePaymentId = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        SupplierInvoice = c.String(),
                        PaymentAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ApplyPenality = c.Boolean(nullable: false),
                        PenalityAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetPayable = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AmountDue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Paid = c.Boolean(nullable: false),
                        WithOutGRN = c.Boolean(nullable: false),
                        SupplierName = c.String(),
                        SupplierId = c.String(),
                        ForBranchId = c.Int(),
                        BranchId = c.Int(nullable: false),
                        BusinessUnitId = c.Int(nullable: false),
                        NextApprover = c.String(),
                        PaymentType = c.Int(nullable: false),
                        Type = c.String(),
                        PoOrContractNumber = c.String(),
                        ProcurementOfficer = c.Int(),
                        ProcurementAnalyzer = c.Int(),
                        PurposeOfPayment = c.String(),
                        IsBudgetApproved = c.Boolean(nullable: false),
                        BudgetApprovedBy = c.String(),
                        BudgetApprovedTime = c.DateTime(),
                        PurchasePaymentType = c.Int(nullable: false),
                        ForwardedFrom = c.String(),
                        ForwardedTo = c.String(),
                        IsFinalApproval = c.Boolean(nullable: false),
                        CurrentApprovalStep = c.Int(),
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
                .PrimaryKey(t => t.PurchasePaymentId);
            
            CreateTable(
                "EXCEED.PurchasePaymentBudgetValidation",
                c => new
                    {
                        PurchasePaymentBudgetValidationId = c.Int(nullable: false, identity: true),
                        PurchasePaymentId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchasePaymentBudgetValidationId)
                .ForeignKey("EXCEED.PurchasePayment", t => t.PurchasePaymentId, cascadeDelete: true)
                .Index(t => t.PurchasePaymentId);
            
            CreateTable(
                "EXCEED.PurchasePaymentGrn",
                c => new
                    {
                        PurchasePaymentGrnId = c.Int(nullable: false, identity: true),
                        PurchasePaymentId = c.Int(nullable: false),
                        GrnNo = c.Int(nullable: false),
                        ItemType = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchasePaymentGrnId)
                .ForeignKey("EXCEED.PurchasePayment", t => t.PurchasePaymentId, cascadeDelete: true)
                .Index(t => t.PurchasePaymentId);
            
            CreateTable(
                "EXCEED.PurchasePaymentValidation",
                c => new
                    {
                        PurchasePaymentValidationId = c.Int(nullable: false, identity: true),
                        PurchasePaymentId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchasePaymentValidationId)
                .ForeignKey("EXCEED.PurchasePayment", t => t.PurchasePaymentId, cascadeDelete: true)
                .Index(t => t.PurchasePaymentId);
            
            AddColumn("HRM.Branch", "ParentBranchId", c => c.Int());
            AddColumn("EXCEED.ApDirectPayment", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApDirectPayment", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApMiscellaneousPayment", "Reference", c => c.String());
            AddColumn("EXCEED.ApMiscellaneousPayment", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApMiscellaneousPayment", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApMiscellaneousPaymentLine", "DeductAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApPaybleInvoices", "GLTaxRateID", c => c.Int());
            AddColumn("EXCEED.ApPaybleInvoices", "WithholdingType", c => c.String());
            AddColumn("EXCEED.ApPaybleInvoices", "WithholdTax", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApPaybleInvoices", "InvoiceSource", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApPaybleInvoices", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApPaybleInvoices", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApPaybleInvoices", "ForBranchId", c => c.Int());
            AddColumn("EXCEED.ApPayment", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApPayment", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApPayment", "ForBranchId", c => c.Int());
            AddColumn("EXCEED.ApPayment", "IsSettled", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApPaymentSettlement", "CollectedByPurchaser", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApPaymentSettlement", "EmployeeId", c => c.Int());
            AddColumn("EXCEED.ApPaymentSettlementLine", "Source", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApPaymentSettlementLine", "SourceId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApPaymentSettlementLine", "GLTaxRateID", c => c.Int());
            AddColumn("EXCEED.ApPaymentSettlementLine", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApPaymentSettlementLine", "WithholdingType", c => c.String());
            AddColumn("EXCEED.ApPaymentSettlementLine", "WithholdingAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApPaymentSettlementLine", "WithholdTax", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApPaymentSettlementLine", "WithholdTaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApPaymentSettlementLine", "NetAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApPettyCash", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApPettyCash", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApPettyCash", "CurrentApprovalStep", c => c.Int());
            AddColumn("Global.ApprovalGroup", "BusinessUnit", c => c.Int());
            AddColumn("Global.ApprovalGroup", "Step", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApTravelExpenseClaim", "IsBudgetApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApTravelExpenseClaim", "BudgetApprovedBy", c => c.String());
            AddColumn("EXCEED.ApTravelExpenseClaim", "BudgetApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.ApTravelExpenseClaim", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApTravelExpenseClaim", "IsDepartmentOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApTravelExpenseClaim", "DepartmentOnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.ApTravelExpenseClaim", "DepartmentOnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.ApTravelExpenseClaim", "IsDepartmentSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApTravelExpenseClaim", "DepartmentSendForApprovalBy", c => c.String());
            AddColumn("EXCEED.ApTravelExpenseClaim", "DepartmentSendForApprovalTime", c => c.DateTime());
            AddColumn("EXCEED.ApTravelExpenseClaim", "CurrentApprovalStep", c => c.Int());
            AddColumn("EXCEED.ApTravelOrderAdvancePayment", "ItemName", c => c.String());
            AddColumn("EXCEED.ApTravelOrder", "TransportReference", c => c.String());
            AddColumn("EXCEED.ApTravelOrder", "IsBudgetApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApTravelOrder", "BudgetApprovedBy", c => c.String());
            AddColumn("EXCEED.ApTravelOrder", "BudgetApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.ApTravelOrder", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApTravelOrder", "IsDepartmentOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApTravelOrder", "DepartmentOnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.ApTravelOrder", "DepartmentOnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.ApTravelOrder", "IsDepartmentSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApTravelOrder", "DepartmentSendForApprovalBy", c => c.String());
            AddColumn("EXCEED.ApTravelOrder", "DepartmentSendForApprovalTime", c => c.DateTime());
            AddColumn("EXCEED.ApTravelOrder", "CurrentApprovalStep", c => c.Int());
            AddColumn("EXCEED.ApTravelOrderDeparture", "ArrivalDate", c => c.DateTime(nullable: false));
            AddColumn("EXCEED.ApTravelOrderDeparture", "DestinationBranchId", c => c.Int());
            AddColumn("EXCEED.ApTravelOrderDeparture", "Duration", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ArInvoice", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ArInvoice", "IsFullFilled", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ARMiscelaneousCollection", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ArStandardCollection", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ArStandardCollection", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("Global.AssignApprovalGroup", "BusinessUnit", c => c.Int());
            AddColumn("EXCEED.BidAwardedSupplier", "IsTransferredToPo", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.Lot", "Category", c => c.String());
            AddColumn("EXCEED.Tender", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ContractManagement", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ContractManagement", "PurchasePaymentType", c => c.Int(nullable: false));
            AddColumn("EXCEED.ContractManagement", "CurrentApprovalStep", c => c.Int());
            AddColumn("EXCEED.ContractPayment", "CurrentApprovalStep", c => c.Int());
            AddColumn("EXCEED.ContractRenewal", "CurrentApprovalStep", c => c.Int());
            AddColumn("EXCEED.ContractTermination", "CurrentApprovalStep", c => c.Int());
            AddColumn("EXCEED.GLLedgerPostingLines", "ExRate", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("EXCEED.GLLedgerPostingLines", "DebitNonFun", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("EXCEED.GLLedgerPostingLines", "CreditNonFun", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("EXCEED.GLLedgerPostingLines", "BalanceNonFun", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("EXCEED.GLLedgerPostingLines", "JournalId", c => c.Int(nullable: false));
            AddColumn("EXCEED.GLRecordJournalDetail", "ExRate", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("EXCEED.GLRecordJournalDetail", "DebitNonFun", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("EXCEED.GLRecordJournalDetail", "CreditNonFun", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("EXCEED.GlRecordJournal", "IsUnPosted", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.GlRecordJournal", "IsNonFuncCurrency", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.GlRecordJournal", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ProformaPurchase", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ProformaPurchase", "CurrentApprovalStep", c => c.Int());
            AddColumn("EXCEED.PaymentRequest", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.PaymentRequest", "Type", c => c.Int(nullable: false));
            AddColumn("EXCEED.PaymentRequest", "IsBudgetApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PaymentRequest", "BudgetApprovedBy", c => c.String());
            AddColumn("EXCEED.PaymentRequest", "BudgetApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.PaymentRequest", "ForwardedFrom", c => c.String());
            AddColumn("EXCEED.PaymentRequest", "ForwardedTo", c => c.String());
            AddColumn("EXCEED.PaymentRequest", "IsFinalApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PaymentRequest", "CurrentApprovalStep", c => c.Int());
            AddColumn("EXCEED.PurchaseOrder", "TenderID", c => c.Int());
            AddColumn("EXCEED.PurchaseOrder", "BidAwardedSupplierID", c => c.Int());
            AddColumn("EXCEED.PurchaseOrder", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.PurchaseOrder", "PurchasePaymentType", c => c.Int(nullable: false));
            AddColumn("EXCEED.PurchaseOrder", "CurrentApprovalStep", c => c.Int());
            AddColumn("EXCEED.ProcurementSupplier", "TCode", c => c.String());
            AddColumn("EXCEED.ProcurementSupplier", "TAccount", c => c.String());
            AddColumn("EXCEED.PurchaseRequisition", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.PurchaseRequisition", "IsBudgetApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseRequisition", "BudgetApprovedBy", c => c.String());
            AddColumn("EXCEED.PurchaseRequisition", "BudgetApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.PurchaseRequisitionItem", "BudgetCode", c => c.String());
            AddColumn("EXCEED.ReconciliationBankAccount", "ExRate", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("EXCEED.ReconciliationBankAccount", "DebitNonFun", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("EXCEED.ReconciliationBankAccount", "CreditNonFun", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("EXCEED.ReconciliationBookAccount", "PaymentReference", c => c.String());
            AddColumn("EXCEED.ReconciliationBookAccount", "InvoiceReferenceDoc", c => c.String());
            AddColumn("EXCEED.ReconciliationBookAccount", "ExRate", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("EXCEED.ReconciliationBookAccount", "DebitNonFun", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("EXCEED.ReconciliationBookAccount", "CreditNonFun", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("EXCEED.ReconciliationBookAccount", "ReconciledDate", c => c.DateTime());
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "ExRate", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "AmountNonFun", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("EXCEED.StockClearingAccount", "Type", c => c.Int(nullable: false));
            AlterColumn("EXCEED.ApPaymentSettlementLine", "ApDirectPayment_ApDirectPaymentId", c => c.Int());
            AlterColumn("Global.ApprovalGroup", "OpertatingUnit", c => c.Int());
            AlterColumn("EXCEED.ApTravelOrderAdvancePayment", "PaymentMethod", c => c.String());
            AlterColumn("Global.AssignApprovalGroup", "OpertatingUnit", c => c.Int());
            AlterColumn("Global.AssignApprovalGroup", "Job", c => c.Int());
            AlterColumn("Global.AssignApprovalGroup", "EmployeeId", c => c.Int());
            AlterColumn("HRM.BranchCostCenter", "Values", c => c.String());
            AlterColumn("EXCEED.PaymentRequest", "EmployeeId", c => c.Int(nullable: false));
            CreateIndex("EXCEED.ApPaymentSettlementLine", "ApDirectPayment_ApDirectPaymentId");
            AddForeignKey("EXCEED.ApPaymentSettlementLine", "ApDirectPayment_ApDirectPaymentId", "EXCEED.ApDirectPayment", "ApDirectPaymentId");
            DropColumn("EXCEED.AssetDisposal", "IsPostedToSecondLedger");
            DropColumn("EXCEED.AnnualDepreciation", "IsPostedToSecondLedger");
            DropColumn("EXCEED.MachineryAndVehicleTransfer", "IsPostedToSecondLedger");
            DropColumn("EXCEED.MachineryAndVehicleTransferReceive", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.Accident", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.Fleets", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.Accessories", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.AnnualEquipmentInventoryInspection", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.DisposalandReplacementRecommendation", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.EquipmentPlanandEvaluations", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.Fuel", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.FleetInsurance", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.Operator", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.TireIssue", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.TireReturn", "IsPostedToSecondLedger");
            DropColumn("CRM.AnnualSalesPlan", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ApDirectPayment", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ApMiscellaneousPayment", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ApPaybleInvoices", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ApPayment", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ApPaymentSettlement", "IsNewSupplier");
            DropColumn("EXCEED.ApPaymentSettlement", "SupplierName");
            DropColumn("EXCEED.ApPaymentSettlement", "Po");
            DropColumn("EXCEED.ApPaymentSettlement", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ApPettyCash", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ApTravelExpenseClaim", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ApTravelOrder", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ApTravelOrderDeparture", "Time");
            DropColumn("APTS.DrugReceivalModel19", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ArInvoice", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ARMiscelaneousCollection", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ArSalesOrder", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ArSalesOrderPartialPrint", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ArStandardCollection", "IsPostedToSecondLedger");
            DropColumn("EXCEED.AssetTransfer", "IsPostedToSecondLedger");
            DropColumn("EXCEED.AssetTransferReceive", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.AssignedEquipment", "IsPostedToSecondLedger");
            DropColumn("InfoDesk.AssignTicket_HelpDesk", "IsPostedToSecondLedger");
            DropColumn("CRM.AssignTicket", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.BatteryIssue", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.BatteryReturn", "IsPostedToSecondLedger");
            DropColumn("CRM.Beginning", "IsPostedToSecondLedger");
            DropColumn("EXCEED.Tender", "IsPostedToSecondLedger");
            DropColumn("CRM.BranchAnnualSalesPlan", "IsPostedToSecondLedger");
            DropColumn("Budget.BudgetAllocation", "IsPostedToSecondLedger");
            DropColumn("Budget.BudgetPlanMonthly", "IsPostedToSecondLedger");
            DropColumn("Budget.BudgetPlan", "IsPostedToSecondLedger");
            DropColumn("Budget.BudgetRequest", "IsPostedToSecondLedger");
            DropColumn("Budget.BudgetRequestLine", "IsPostedToSecondLedger");
            DropColumn("Budget.BudgetRequestTransferLog", "IsPostedToSecondLedger");
            DropColumn("Budget.BudgetTransfer", "IsPostedToSecondLedger");
            DropColumn("Budget.BudgetUsage", "IsPostedToSecondLedger");
            DropColumn("EXCEED.CampaignProduct", "IsPostedToSecondLedger");
            DropColumn("CRM.Campaign", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.Cannibalization", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.CannibalizationDetail", "IsPostedToSecondLedger");
            DropColumn("CRM.CashSalesInvoice", "IsPostedToSecondLedger");
            DropColumn("CRM.CashSalesPartialItem", "IsPostedToSecondLedger");
            DropColumn("CRM.CashSalesPartialLineItem", "IsPostedToSecondLedger");
            DropColumn("CRM.LineItem", "IsPostedToSecondLedger");
            DropColumn("CRM.Product", "IsPostedToSecondLedger");
            DropColumn("Construction.ContractClaim", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.CheckListLog", "IsPostedToSecondLedger");
            DropColumn("Construction.ConsARStandardCollection", "IsPostedToSecondLedger");
            DropColumn("Construction.ConsReceivableInvoice", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ContractManagement", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ContractPayment", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ContractRenewal", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ContractTermination", "IsPostedToSecondLedger");
            DropColumn("CRM.CreditSalesInvoice", "IsPostedToSecondLedger");
            DropColumn("CRM.CreditSettlement", "IsPostedToSecondLedger");
            DropColumn("CRM.LineCreditSettlement", "IsPostedToSecondLedger");
            DropColumn("CRM.CRMDeposit", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.DailyCareLog", "IsPostedToSecondLedger");
            DropColumn("EXCEED.Disposal", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.DriverAssignment", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.DriverVehicleTransfer", "IsPostedToSecondLedger");
            DropColumn("HRM.EducationRequest", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.EquipmentArrival", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.EquipmentAttachment", "IsPostedToSecondLedger");
            DropColumn("HRM.ExperienceRequest", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.ExternalMaterial", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.ExternalRentedEquipmentCheckList", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.ExternalRentedEquipment", "IsPostedToSecondLedger");
            DropColumn("EXCEED.FixedAssetCount", "IsPostedToSecondLedger");
            DropColumn("EXCEED.FixedAssetReceive", "IsPostedToSecondLedger");
            DropColumn("EXCEED.FixedAssetIssue", "IsPostedToSecondLedger");
            DropColumn("EXCEED.FixedAssetRequisition", "IsPostedToSecondLedger");
            DropColumn("EXCEED.FixedAssetReturn", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.FleetHistory", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.FleetPettyCash", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.FleetReturnItem", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.FleetReturn", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ForeignPurchase", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ForeignPurchaseCost", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.FuelInventory", "IsPostedToSecondLedger");
            DropColumn("EXCEED.GLRecordJournalBeg", "IsPostedToSecondLedger");
            DropColumn("EXCEED.GlRecordJournal", "IsPostedToSecondLedger");
            DropColumn("EXCEED.GoodReceive", "IsPostedToSecondLedger");
            DropColumn("EXCEED.GoodReceiveDonation", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.SalvageStoreGoodReceiving", "IsPostedToSecondLedger");
            DropColumn("EXCEED.GoodSellingAndLending", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.InspectionLog", "IsPostedToSecondLedger");
            DropColumn("Production.ProductionIssue", "IsPostedToSecondLedger");
            DropColumn("EXCEED.Issue", "IsPostedToSecondLedger");
            DropColumn("CRM.IssueHistory", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ItemTransfer", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ItemTransferReceive", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.JobOrder", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.JobOrderDetail", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.JobRequest", "IsPostedToSecondLedger");
            DropColumn("HRM.LeaveRequest", "IsPostedToSecondLedger");
            DropColumn("HRM.LeaveReturn", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.LineCheckList", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.LogisticPlanDetail", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.LogisticPlan", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.ServiceTool", "IsPostedToSecondLedger");
            DropColumn("Health.MedicalRequest", "IsPostedToSecondLedger");
            DropColumn("EXCEED.MonthlyDepreciation", "IsPostedToSecondLedger");
            DropColumn("CRM.NonCashTransaction", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.OtherCompanyMaintenance", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.OutsourceMaintenance", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.OwningandOperatingCost", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ProformaPurchase", "IsPostedToSecondLedger");
            DropColumn("Construction.PaymentCertificate", "IsPostedToSecondLedger");
            DropColumn("EXCEED.PaymentRequest", "IsPostedToSecondLedger");
            DropColumn("EXCEED.PaymentRequest", "MainCompanySturucture_OrgStructureId");
            DropColumn("Budget.PlanCategory", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.PMScheduleDetail", "IsPostedToSecondLedger");
            DropColumn("EXCEED.PurchaseOrder", "IsPostedToSecondLedger");
            DropColumn("CRM.PreCustomer", "IsPostedToSecondLedger");
            DropColumn("CRM.PreCustomerSchedule", "IsPostedToSecondLedger");
            DropColumn("CRM.PriceEstimationQuoteVouchers", "IsPostedToSecondLedger");
            DropColumn("ProductFollowUp.FinishedGoodsStoreReceive", "IsPostedToSecondLedger");
            DropColumn("ProductFollowUp.ProductDelivery", "IsPostedToSecondLedger");
            DropColumn("Production.ProductionFinishedGoodsStoreReceive", "IsPostedToSecondLedger");
            DropColumn("Production.ProductionStoreReceive", "IsPostedToSecondLedger");
            DropColumn("EXCEED.PropertyCount", "IsPostedToSecondLedger");
            DropColumn("EXCEED.PurchaseRequisition", "IsPostedToSecondLedger");
            DropColumn("EXCEED.PurchaseRequisitionAssignment", "IsPostedToSecondLedger");
            DropColumn("EXCEED.PurchaseReturn", "IsPostedToSecondLedger");
            DropColumn("CRM.Queue", "IsPostedToSecondLedger");
            DropColumn("CRM.QueueStatus", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ReconciliationSchedule", "IsPostedToSecondLedger");
            DropColumn("HRM.RecruitmentRequest", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.RentInternalProject", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.RentFromExternal", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.RentOtherSuppliers", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.RentRequest", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.RequestedEquipment", "IsPostedToSecondLedger");
            DropColumn("Budget.RequestedMonth", "IsPostedToSecondLedger");
            DropColumn("EXCEED.Revaluation", "IsPostedToSecondLedger");
            DropColumn("CRM.SalesOrderVoucher", "IsPostedToSecondLedger");
            DropColumn("CRM.SalesQuoteVoucher", "IsPostedToSecondLedger");
            DropColumn("CRM.SalesShipments", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.SalvageStore", "IsPostedToSecondLedger");
            DropColumn("Construction.ScheduleRevision", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.ServiceCrewWorkLog", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ServiceReceive", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.ServiceSpare", "IsPostedToSecondLedger");
            DropColumn("CRM.ServiceUsage", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.SitesVehicleTransfer", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.SiteVechicleTransferDetail", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.SiteVechicleTransferTyreDetail", "IsPostedToSecondLedger");
            DropColumn("EXCEED.StockAdjustment", "IsPostedToSecondLedger");
            DropColumn("CRM.StockBalance", "IsPostedToSecondLedger");
            DropColumn("CRM.StockLevel", "IsPostedToSecondLedger");
            DropColumn("Inventory_Archive.StoreRequisitionArchive", "IsPostedToSecondLedger");
            DropColumn("EXCEED.StoreRequisition", "IsPostedToSecondLedger");
            DropColumn("EXCEED.StoreReturn", "IsPostedToSecondLedger");
            DropColumn("CRM.StoreTransaction", "IsPostedToSecondLedger");
            DropColumn("Budget.TaskPlan", "IsPostedToSecondLedger");
            DropColumn("Budget.TaskPerformance", "IsPostedToSecondLedger");
            DropColumn("Budget.TaskPlanMonthly", "IsPostedToSecondLedger");
            DropColumn("EXCEED.TempGoodReceive", "IsPostedToSecondLedger");
            DropColumn("HRM.Termination", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.TimeSheet", "IsPostedToSecondLedger");
            DropColumn("HRM.TrainingApplicant", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.TyreTransfer", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.VehicleDemand", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.VehicleDistribution", "IsPostedToSecondLedger");
            DropColumn("FleetManagement.VehicleReturn", "IsPostedToSecondLedger");
            DropColumn("Construction.WorkInProgress", "IsPostedToSecondLedger");
            DropTable("EXCEED.GLChartOfAccountCostCenter");
            DropTable("EXCEED.GLChartOfAccountLocation");
            DropTable("EXCEED.GLChartOfAccountSubAccount");
            DropTable("EXCEED.GlSecondayLedgerLine");
            DropTable("EXCEED.GlSecondayLedger");
            DropTable("EXCEED.GlSegmentLength");
            DropTable("EXCEED.GlSegmentSequence");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.GlSegmentSequence",
                c => new
                    {
                        GlSegmentSequenceId = c.Int(nullable: false, identity: true),
                        FirstSegment = c.Int(nullable: false),
                        SecondSegment = c.Int(nullable: false),
                        ThirdSegment = c.Int(nullable: false),
                        FourthSegment = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GlSegmentSequenceId);
            
            CreateTable(
                "EXCEED.GlSegmentLength",
                c => new
                    {
                        GlSegmentLengthId = c.Int(nullable: false, identity: true),
                        AccountLength = c.Int(nullable: false),
                        LocationLength = c.Int(nullable: false),
                        CostCenterLength = c.Int(nullable: false),
                        SubAccountLength = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GlSegmentLengthId);
            
            CreateTable(
                "EXCEED.GlSecondayLedger",
                c => new
                    {
                        GlSecondayLedgerId = c.Int(nullable: false, identity: true),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        AccountType = c.Int(nullable: false),
                        AccountSegment = c.String(),
                        BranchSegment = c.String(),
                        CostCenterSegment = c.String(),
                        SubAccountSegment = c.String(),
                        ChannelSegment = c.String(),
                        Segment1 = c.String(),
                        Segment2 = c.String(),
                        Segment3 = c.String(),
                        Segment4 = c.String(),
                        Segment5 = c.String(),
                        Segment6 = c.String(),
                        Segment7 = c.String(),
                        Segment8 = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GlSecondayLedgerId);
            
            CreateTable(
                "EXCEED.GlSecondayLedgerLine",
                c => new
                    {
                        GlSecondayLedgerLineId = c.Int(nullable: false, identity: true),
                        GlSecondayLedgerId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        JournalName = c.String(),
                        Description = c.String(),
                        JournalNumber = c.String(),
                        JournalReference = c.String(),
                        CategoryName = c.String(),
                        SourcesName = c.String(),
                        EffectiveDate = c.DateTime(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PostingSource = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GlSecondayLedgerLineId);
            
            CreateTable(
                "EXCEED.GLChartOfAccountSubAccount",
                c => new
                    {
                        Values = c.String(nullable: false, maxLength: 128),
                        Description = c.String(nullable: false),
                        HasParent = c.String(),
                        Enable = c.Boolean(nullable: false),
                        Parent = c.Boolean(nullable: false),
                        AllowBudgeting = c.Boolean(nullable: false),
                        AllowPosting = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Values);
            
            CreateTable(
                "EXCEED.GLChartOfAccountLocation",
                c => new
                    {
                        Values = c.String(nullable: false, maxLength: 128),
                        Description = c.String(nullable: false),
                        HasParent = c.String(),
                        Enable = c.Boolean(nullable: false),
                        Parent = c.Boolean(nullable: false),
                        AllowBudgeting = c.Boolean(nullable: false),
                        AllowPosting = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Values);
            
            CreateTable(
                "EXCEED.GLChartOfAccountCostCenter",
                c => new
                    {
                        Values = c.String(nullable: false, maxLength: 128),
                        Description = c.String(nullable: false),
                        HasParent = c.String(),
                        Enable = c.Boolean(nullable: false),
                        Parent = c.Boolean(nullable: false),
                        AllowBudgeting = c.Boolean(nullable: false),
                        AllowPosting = c.Boolean(nullable: false),
                        Type = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Values);
            
            AddColumn("Construction.WorkInProgress", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.VehicleReturn", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.VehicleDistribution", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.VehicleDemand", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.TyreTransfer", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("HRM.TrainingApplicant", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.TimeSheet", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("HRM.Termination", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.TempGoodReceive", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("Budget.TaskPlanMonthly", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("Budget.TaskPerformance", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("Budget.TaskPlan", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.StoreTransaction", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.StoreReturn", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.StoreRequisition", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("Inventory_Archive.StoreRequisitionArchive", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.StockLevel", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.StockBalance", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.StockAdjustment", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.SiteVechicleTransferTyreDetail", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.SiteVechicleTransferDetail", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.SitesVehicleTransfer", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.ServiceUsage", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.ServiceSpare", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ServiceReceive", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.ServiceCrewWorkLog", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("Construction.ScheduleRevision", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.SalvageStore", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.SalesShipments", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.SalesQuoteVoucher", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.SalesOrderVoucher", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.Revaluation", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("Budget.RequestedMonth", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.RequestedEquipment", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.RentRequest", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.RentOtherSuppliers", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.RentFromExternal", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.RentInternalProject", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("HRM.RecruitmentRequest", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ReconciliationSchedule", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.QueueStatus", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.Queue", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseReturn", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseRequisitionAssignment", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseRequisition", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PropertyCount", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("Production.ProductionStoreReceive", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("Production.ProductionFinishedGoodsStoreReceive", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("ProductFollowUp.ProductDelivery", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("ProductFollowUp.FinishedGoodsStoreReceive", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.PriceEstimationQuoteVouchers", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.PreCustomerSchedule", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.PreCustomer", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseOrder", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.PMScheduleDetail", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("Budget.PlanCategory", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PaymentRequest", "MainCompanySturucture_OrgStructureId", c => c.Int());
            AddColumn("EXCEED.PaymentRequest", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("Construction.PaymentCertificate", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ProformaPurchase", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.OwningandOperatingCost", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.OutsourceMaintenance", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.OtherCompanyMaintenance", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.NonCashTransaction", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.MonthlyDepreciation", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("Health.MedicalRequest", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.ServiceTool", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.LogisticPlan", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.LogisticPlanDetail", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.LineCheckList", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("HRM.LeaveReturn", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("HRM.LeaveRequest", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.JobRequest", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.JobOrderDetail", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.JobOrder", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ItemTransferReceive", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ItemTransfer", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.IssueHistory", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.Issue", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("Production.ProductionIssue", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.InspectionLog", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.GoodSellingAndLending", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.SalvageStoreGoodReceiving", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.GoodReceiveDonation", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.GoodReceive", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.GlRecordJournal", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.GLRecordJournalBeg", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.FuelInventory", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ForeignPurchaseCost", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ForeignPurchase", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.FleetReturn", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.FleetReturnItem", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.FleetPettyCash", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.FleetHistory", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FixedAssetReturn", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FixedAssetRequisition", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FixedAssetIssue", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FixedAssetReceive", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FixedAssetCount", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.ExternalRentedEquipment", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.ExternalRentedEquipmentCheckList", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.ExternalMaterial", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("HRM.ExperienceRequest", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.EquipmentAttachment", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.EquipmentArrival", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("HRM.EducationRequest", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.DriverVehicleTransfer", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.DriverAssignment", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.Disposal", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.DailyCareLog", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.CRMDeposit", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.LineCreditSettlement", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.CreditSettlement", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.CreditSalesInvoice", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ContractTermination", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ContractRenewal", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ContractPayment", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ContractManagement", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("Construction.ConsReceivableInvoice", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("Construction.ConsARStandardCollection", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.CheckListLog", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("Construction.ContractClaim", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.Product", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.LineItem", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.CashSalesPartialLineItem", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.CashSalesPartialItem", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.CashSalesInvoice", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.CannibalizationDetail", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Cannibalization", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.Campaign", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.CampaignProduct", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("Budget.BudgetUsage", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("Budget.BudgetTransfer", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("Budget.BudgetRequestTransferLog", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("Budget.BudgetRequestLine", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("Budget.BudgetRequest", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("Budget.BudgetPlan", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("Budget.BudgetPlanMonthly", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("Budget.BudgetAllocation", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.BranchAnnualSalesPlan", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.Tender", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.Beginning", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.BatteryReturn", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.BatteryIssue", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.AssignTicket", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("InfoDesk.AssignTicket_HelpDesk", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.AssignedEquipment", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.AssetTransferReceive", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.AssetTransfer", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ArStandardCollection", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ArSalesOrderPartialPrint", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ArSalesOrder", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ARMiscelaneousCollection", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ArInvoice", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("APTS.DrugReceivalModel19", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApTravelOrderDeparture", "Time", c => c.DateTime(nullable: false));
            AddColumn("EXCEED.ApTravelOrder", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApTravelExpenseClaim", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApPettyCash", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApPaymentSettlement", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApPaymentSettlement", "Po", c => c.String());
            AddColumn("EXCEED.ApPaymentSettlement", "SupplierName", c => c.String());
            AddColumn("EXCEED.ApPaymentSettlement", "IsNewSupplier", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApPayment", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApPaybleInvoices", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApMiscellaneousPayment", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApDirectPayment", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("CRM.AnnualSalesPlan", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.TireReturn", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.TireIssue", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Operator", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.FleetInsurance", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Fuel", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.EquipmentPlanandEvaluations", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.DisposalandReplacementRecommendation", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.AnnualEquipmentInventoryInspection", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Accessories", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Fleets", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Accident", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.MachineryAndVehicleTransferReceive", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.MachineryAndVehicleTransfer", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.AnnualDepreciation", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.AssetDisposal", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            DropForeignKey("EXCEED.ApPaymentSettlementLine", "ApDirectPayment_ApDirectPaymentId", "EXCEED.ApDirectPayment");
            DropForeignKey("EXCEED.PurchasePaymentValidation", "PurchasePaymentId", "EXCEED.PurchasePayment");
            DropForeignKey("EXCEED.PurchasePaymentGrn", "PurchasePaymentId", "EXCEED.PurchasePayment");
            DropForeignKey("EXCEED.PurchasePaymentAttachment", "PurchasePaymentId", "EXCEED.PurchasePayment");
            DropForeignKey("EXCEED.PurchasePaymentBudgetValidation", "PurchasePaymentId", "EXCEED.PurchasePayment");
            DropForeignKey("EXCEED.PaymentRequestBudgetValidation", "PaymentRequestId", "EXCEED.PaymentRequest");
            DropForeignKey("EXCEED.PaymentRequestAttachment", "PaymentRequestId", "EXCEED.PaymentRequest");
            DropForeignKey("EXCEED.FundTransferLine", "PaymentRequestId", "EXCEED.PaymentRequest");
            DropForeignKey("EXCEED.ApTravelOrderDepartmentValidation", "ApTravelOrderId", "EXCEED.ApTravelOrder");
            DropForeignKey("EXCEED.ApTravelOrderBudgetValidation", "ApTravelOrderId", "EXCEED.ApTravelOrder");
            DropForeignKey("EXCEED.ApTravelExpenseDepartmentValidation", "ApTravelExpenseClaimId", "EXCEED.ApTravelExpenseClaim");
            DropForeignKey("EXCEED.ApTravelExpenseBudgetValidation", "ApTravelExpenseClaimId", "EXCEED.ApTravelExpenseClaim");
            DropForeignKey("EXCEED.ApTPaymentValidation", "ApTPaymentId", "EXCEED.ApTPayment");
            DropForeignKey("EXCEED.ApTPaymentRequestResponse", "ApTPaymentId", "EXCEED.ApTPayment");
            DropForeignKey("EXCEED.ApPaymentSettlementGrn", "ApPaymentSettlementId", "EXCEED.ApPaymentSettlement");
            DropForeignKey("EXCEED.APPaybleInvoicePrepayment", "ApPaybleInvoicesId", "EXCEED.ApPaybleInvoices");
            DropIndex("EXCEED.PurchasePaymentValidation", new[] { "PurchasePaymentId" });
            DropIndex("EXCEED.PurchasePaymentGrn", new[] { "PurchasePaymentId" });
            DropIndex("EXCEED.PurchasePaymentBudgetValidation", new[] { "PurchasePaymentId" });
            DropIndex("EXCEED.PurchasePaymentAttachment", new[] { "PurchasePaymentId" });
            DropIndex("EXCEED.PaymentRequestBudgetValidation", new[] { "PaymentRequestId" });
            DropIndex("EXCEED.PaymentRequestAttachment", new[] { "PaymentRequestId" });
            DropIndex("EXCEED.FundTransferLine", new[] { "PaymentRequestId" });
            DropIndex("EXCEED.ApTravelOrderDepartmentValidation", new[] { "ApTravelOrderId" });
            DropIndex("EXCEED.ApTravelOrderBudgetValidation", new[] { "ApTravelOrderId" });
            DropIndex("EXCEED.ApTravelExpenseDepartmentValidation", new[] { "ApTravelExpenseClaimId" });
            DropIndex("EXCEED.ApTravelExpenseBudgetValidation", new[] { "ApTravelExpenseClaimId" });
            DropIndex("EXCEED.ApTPaymentValidation", new[] { "ApTPaymentId" });
            DropIndex("EXCEED.ApTPaymentRequestResponse", new[] { "ApTPaymentId" });
            DropIndex("EXCEED.ApPaymentSettlementLine", new[] { "ApDirectPayment_ApDirectPaymentId" });
            DropIndex("EXCEED.ApPaymentSettlementGrn", new[] { "ApPaymentSettlementId" });
            DropIndex("EXCEED.APPaybleInvoicePrepayment", new[] { "ApPaybleInvoicesId" });
            AlterColumn("EXCEED.PaymentRequest", "EmployeeId", c => c.Int());
            AlterColumn("HRM.BranchCostCenter", "Values", c => c.String(maxLength: 128));
            AlterColumn("Global.AssignApprovalGroup", "EmployeeId", c => c.Int(nullable: false));
            AlterColumn("Global.AssignApprovalGroup", "Job", c => c.String());
            AlterColumn("Global.AssignApprovalGroup", "OpertatingUnit", c => c.String());
            AlterColumn("EXCEED.ApTravelOrderAdvancePayment", "PaymentMethod", c => c.String(nullable: false));
            AlterColumn("Global.ApprovalGroup", "OpertatingUnit", c => c.Int(nullable: false));
            AlterColumn("EXCEED.ApPaymentSettlementLine", "ApDirectPayment_ApDirectPaymentId", c => c.Int(nullable: false));
            DropColumn("EXCEED.StockClearingAccount", "Type");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "AmountNonFun");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "ExRate");
            DropColumn("EXCEED.ReconciliationBookAccount", "ReconciledDate");
            DropColumn("EXCEED.ReconciliationBookAccount", "CreditNonFun");
            DropColumn("EXCEED.ReconciliationBookAccount", "DebitNonFun");
            DropColumn("EXCEED.ReconciliationBookAccount", "ExRate");
            DropColumn("EXCEED.ReconciliationBookAccount", "InvoiceReferenceDoc");
            DropColumn("EXCEED.ReconciliationBookAccount", "PaymentReference");
            DropColumn("EXCEED.ReconciliationBankAccount", "CreditNonFun");
            DropColumn("EXCEED.ReconciliationBankAccount", "DebitNonFun");
            DropColumn("EXCEED.ReconciliationBankAccount", "ExRate");
            DropColumn("EXCEED.PurchaseRequisitionItem", "BudgetCode");
            DropColumn("EXCEED.PurchaseRequisition", "BudgetApprovedTime");
            DropColumn("EXCEED.PurchaseRequisition", "BudgetApprovedBy");
            DropColumn("EXCEED.PurchaseRequisition", "IsBudgetApproved");
            DropColumn("EXCEED.PurchaseRequisition", "BusinessUnitId");
            DropColumn("EXCEED.ProcurementSupplier", "TAccount");
            DropColumn("EXCEED.ProcurementSupplier", "TCode");
            DropColumn("EXCEED.PurchaseOrder", "CurrentApprovalStep");
            DropColumn("EXCEED.PurchaseOrder", "PurchasePaymentType");
            DropColumn("EXCEED.PurchaseOrder", "BusinessUnitId");
            DropColumn("EXCEED.PurchaseOrder", "BidAwardedSupplierID");
            DropColumn("EXCEED.PurchaseOrder", "TenderID");
            DropColumn("EXCEED.PaymentRequest", "CurrentApprovalStep");
            DropColumn("EXCEED.PaymentRequest", "IsFinalApproval");
            DropColumn("EXCEED.PaymentRequest", "ForwardedTo");
            DropColumn("EXCEED.PaymentRequest", "ForwardedFrom");
            DropColumn("EXCEED.PaymentRequest", "BudgetApprovedTime");
            DropColumn("EXCEED.PaymentRequest", "BudgetApprovedBy");
            DropColumn("EXCEED.PaymentRequest", "IsBudgetApproved");
            DropColumn("EXCEED.PaymentRequest", "Type");
            DropColumn("EXCEED.PaymentRequest", "BusinessUnitId");
            DropColumn("EXCEED.ProformaPurchase", "CurrentApprovalStep");
            DropColumn("EXCEED.ProformaPurchase", "BusinessUnitId");
            DropColumn("EXCEED.GlRecordJournal", "BusinessUnitId");
            DropColumn("EXCEED.GlRecordJournal", "IsNonFuncCurrency");
            DropColumn("EXCEED.GlRecordJournal", "IsUnPosted");
            DropColumn("EXCEED.GLRecordJournalDetail", "CreditNonFun");
            DropColumn("EXCEED.GLRecordJournalDetail", "DebitNonFun");
            DropColumn("EXCEED.GLRecordJournalDetail", "ExRate");
            DropColumn("EXCEED.GLLedgerPostingLines", "JournalId");
            DropColumn("EXCEED.GLLedgerPostingLines", "BalanceNonFun");
            DropColumn("EXCEED.GLLedgerPostingLines", "CreditNonFun");
            DropColumn("EXCEED.GLLedgerPostingLines", "DebitNonFun");
            DropColumn("EXCEED.GLLedgerPostingLines", "ExRate");
            DropColumn("EXCEED.ContractTermination", "CurrentApprovalStep");
            DropColumn("EXCEED.ContractRenewal", "CurrentApprovalStep");
            DropColumn("EXCEED.ContractPayment", "CurrentApprovalStep");
            DropColumn("EXCEED.ContractManagement", "CurrentApprovalStep");
            DropColumn("EXCEED.ContractManagement", "PurchasePaymentType");
            DropColumn("EXCEED.ContractManagement", "BusinessUnitId");
            DropColumn("EXCEED.Tender", "BusinessUnitId");
            DropColumn("EXCEED.Lot", "Category");
            DropColumn("EXCEED.BidAwardedSupplier", "IsTransferredToPo");
            DropColumn("Global.AssignApprovalGroup", "BusinessUnit");
            DropColumn("EXCEED.ArStandardCollection", "BusinessUnitId");
            DropColumn("EXCEED.ArStandardCollection", "BranchId");
            DropColumn("EXCEED.ARMiscelaneousCollection", "BusinessUnitId");
            DropColumn("EXCEED.ArInvoice", "IsFullFilled");
            DropColumn("EXCEED.ArInvoice", "BusinessUnitId");
            DropColumn("EXCEED.ApTravelOrderDeparture", "Duration");
            DropColumn("EXCEED.ApTravelOrderDeparture", "DestinationBranchId");
            DropColumn("EXCEED.ApTravelOrderDeparture", "ArrivalDate");
            DropColumn("EXCEED.ApTravelOrder", "CurrentApprovalStep");
            DropColumn("EXCEED.ApTravelOrder", "DepartmentSendForApprovalTime");
            DropColumn("EXCEED.ApTravelOrder", "DepartmentSendForApprovalBy");
            DropColumn("EXCEED.ApTravelOrder", "IsDepartmentSendForApproval");
            DropColumn("EXCEED.ApTravelOrder", "DepartmentOnlineApprovedTime");
            DropColumn("EXCEED.ApTravelOrder", "DepartmentOnlineApprovedBy");
            DropColumn("EXCEED.ApTravelOrder", "IsDepartmentOnlineApproved");
            DropColumn("EXCEED.ApTravelOrder", "BusinessUnitId");
            DropColumn("EXCEED.ApTravelOrder", "BudgetApprovedTime");
            DropColumn("EXCEED.ApTravelOrder", "BudgetApprovedBy");
            DropColumn("EXCEED.ApTravelOrder", "IsBudgetApproved");
            DropColumn("EXCEED.ApTravelOrder", "TransportReference");
            DropColumn("EXCEED.ApTravelOrderAdvancePayment", "ItemName");
            DropColumn("EXCEED.ApTravelExpenseClaim", "CurrentApprovalStep");
            DropColumn("EXCEED.ApTravelExpenseClaim", "DepartmentSendForApprovalTime");
            DropColumn("EXCEED.ApTravelExpenseClaim", "DepartmentSendForApprovalBy");
            DropColumn("EXCEED.ApTravelExpenseClaim", "IsDepartmentSendForApproval");
            DropColumn("EXCEED.ApTravelExpenseClaim", "DepartmentOnlineApprovedTime");
            DropColumn("EXCEED.ApTravelExpenseClaim", "DepartmentOnlineApprovedBy");
            DropColumn("EXCEED.ApTravelExpenseClaim", "IsDepartmentOnlineApproved");
            DropColumn("EXCEED.ApTravelExpenseClaim", "BusinessUnitId");
            DropColumn("EXCEED.ApTravelExpenseClaim", "BudgetApprovedTime");
            DropColumn("EXCEED.ApTravelExpenseClaim", "BudgetApprovedBy");
            DropColumn("EXCEED.ApTravelExpenseClaim", "IsBudgetApproved");
            DropColumn("Global.ApprovalGroup", "Step");
            DropColumn("Global.ApprovalGroup", "BusinessUnit");
            DropColumn("EXCEED.ApPettyCash", "CurrentApprovalStep");
            DropColumn("EXCEED.ApPettyCash", "BusinessUnitId");
            DropColumn("EXCEED.ApPettyCash", "BranchId");
            DropColumn("EXCEED.ApPaymentSettlementLine", "NetAmount");
            DropColumn("EXCEED.ApPaymentSettlementLine", "WithholdTaxAmount");
            DropColumn("EXCEED.ApPaymentSettlementLine", "WithholdTax");
            DropColumn("EXCEED.ApPaymentSettlementLine", "WithholdingAmount");
            DropColumn("EXCEED.ApPaymentSettlementLine", "WithholdingType");
            DropColumn("EXCEED.ApPaymentSettlementLine", "TaxAmount");
            DropColumn("EXCEED.ApPaymentSettlementLine", "GLTaxRateID");
            DropColumn("EXCEED.ApPaymentSettlementLine", "SourceId");
            DropColumn("EXCEED.ApPaymentSettlementLine", "Source");
            DropColumn("EXCEED.ApPaymentSettlement", "EmployeeId");
            DropColumn("EXCEED.ApPaymentSettlement", "CollectedByPurchaser");
            DropColumn("EXCEED.ApPayment", "IsSettled");
            DropColumn("EXCEED.ApPayment", "ForBranchId");
            DropColumn("EXCEED.ApPayment", "BusinessUnitId");
            DropColumn("EXCEED.ApPayment", "BranchId");
            DropColumn("EXCEED.ApPaybleInvoices", "ForBranchId");
            DropColumn("EXCEED.ApPaybleInvoices", "BusinessUnitId");
            DropColumn("EXCEED.ApPaybleInvoices", "BranchId");
            DropColumn("EXCEED.ApPaybleInvoices", "InvoiceSource");
            DropColumn("EXCEED.ApPaybleInvoices", "WithholdTax");
            DropColumn("EXCEED.ApPaybleInvoices", "WithholdingType");
            DropColumn("EXCEED.ApPaybleInvoices", "GLTaxRateID");
            DropColumn("EXCEED.ApMiscellaneousPaymentLine", "DeductAmount");
            DropColumn("EXCEED.ApMiscellaneousPayment", "BusinessUnitId");
            DropColumn("EXCEED.ApMiscellaneousPayment", "BranchId");
            DropColumn("EXCEED.ApMiscellaneousPayment", "Reference");
            DropColumn("EXCEED.ApDirectPayment", "BusinessUnitId");
            DropColumn("EXCEED.ApDirectPayment", "BranchId");
            DropColumn("HRM.Branch", "ParentBranchId");
            DropTable("EXCEED.PurchasePaymentValidation");
            DropTable("EXCEED.PurchasePaymentGrn");
            DropTable("EXCEED.PurchasePaymentBudgetValidation");
            DropTable("EXCEED.PurchasePayment");
            DropTable("EXCEED.PurchasePaymentAttachment");
            DropTable("EXCEED.PaymentRequestBudgetValidation");
            DropTable("EXCEED.GlCostAllocationSetup");
            DropTable("EXCEED.GLAccountConfig");
            DropTable("EXCEED.PaymentRequestAttachment");
            DropTable("EXCEED.FundTransferLine");
            DropTable("EXCEED.ApTravelOrderDepartmentValidation");
            DropTable("EXCEED.ApTravelOrderBudgetValidation");
            DropTable("EXCEED.ApTravelExpenseDepartmentValidation");
            DropTable("EXCEED.ApTravelExpenseBudgetValidation");
            DropTable("EXCEED.ApTPaymentValidation");
            DropTable("EXCEED.ApTPayment");
            DropTable("EXCEED.ApTPaymentRequestResponse");
            DropTable("EXCEED.ApPaymentSettlementGrn");
            DropTable("EXCEED.APPaybleInvoicePrepayment");
            RenameColumn(table: "EXCEED.ApPaymentSettlementLine", name: "ApDirectPayment_ApDirectPaymentId", newName: "ApDirectPaymentId");
            CreateIndex("EXCEED.PaymentRequest", "MainCompanySturucture_OrgStructureId");
            CreateIndex("EXCEED.PaymentRequest", "BranchId");
            CreateIndex("EXCEED.GlSecondayLedgerLine", "GlSecondayLedgerId");
            CreateIndex("HRM.BranchCostCenter", "Values");
            CreateIndex("EXCEED.ApPaymentSettlementLine", "ApDirectPaymentId");
            AddForeignKey("EXCEED.ApPaymentSettlementLine", "ApDirectPaymentId", "EXCEED.ApDirectPayment", "ApDirectPaymentId", cascadeDelete: true);
            AddForeignKey("EXCEED.PaymentRequest", "MainCompanySturucture_OrgStructureId", "HRM.MainCompanyStructure", "OrgStructureId");
            AddForeignKey("EXCEED.PaymentRequest", "BranchId", "HRM.Branch", "BranchId", cascadeDelete: true);
            AddForeignKey("EXCEED.GlSecondayLedgerLine", "GlSecondayLedgerId", "EXCEED.GlSecondayLedger", "GlSecondayLedgerId", cascadeDelete: true);
            AddForeignKey("HRM.BranchCostCenter", "Values", "EXCEED.GLChartOfAccountCostCenter", "Values");
        }
    }
}
