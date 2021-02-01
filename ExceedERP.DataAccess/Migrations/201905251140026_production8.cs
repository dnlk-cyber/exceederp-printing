namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class production8 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Production.GumProcessingOrderItem", "GumProductionOrderItemId", "Production.GumProductionOrderItem");
            DropForeignKey("Production.GumProcessingOrderItem", "ProcessingPlantId", "Production.ProcessingPlant");
            DropForeignKey("Production.SeedProcessingOrderItem", "ProcessingPlantId", "Production.ProcessingPlant");
            DropForeignKey("Production.SeedProcessingOrderItem", "SeedProductionOrderItemId", "Production.SeedProductionOrderItem");
            DropIndex("Production.GumProcessingOrderItem", new[] { "GumProductionOrderItemId" });
            DropIndex("Production.GumProcessingOrderItem", new[] { "ProcessingPlantId" });
            DropIndex("Production.SeedProcessingOrderItem", new[] { "SeedProductionOrderItemId" });
            DropIndex("Production.SeedProcessingOrderItem", new[] { "ProcessingPlantId" });
            CreateTable(
                "Production.GumProcessingOrder",
                c => new
                    {
                        GumProcessingOrderId = c.Int(nullable: false, identity: true),
                        ProcessingPlantId = c.Int(nullable: false),
                        ProductionPeriodId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GumProcessingOrderId)
                .ForeignKey("Production.ProcessingPlant", t => t.ProcessingPlantId, cascadeDelete: true)
                .ForeignKey("Production.ProductionPeriod", t => t.ProductionPeriodId, cascadeDelete: true)
                .Index(t => t.ProcessingPlantId)
                .Index(t => t.ProductionPeriodId);
            
            CreateTable(
                "Production.CleanedProductSaleItemPerMonth",
                c => new
                    {
                        CleanedProductSaleItemPerMonthId = c.Int(nullable: false, identity: true),
                        CleanedProductSaleItemId = c.Int(nullable: false),
                        ProductionPeriodId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CleanedProductSaleItemPerMonthId)
                .ForeignKey("Production.CleanedProductSaleItem", t => t.CleanedProductSaleItemId, cascadeDelete: true)
                .ForeignKey("Production.ProductionPeriod", t => t.ProductionPeriodId, cascadeDelete: true)
                .Index(t => t.CleanedProductSaleItemId)
                .Index(t => t.ProductionPeriodId);
            
            CreateTable(
                "Production.CleanedProductSaleItem",
                c => new
                    {
                        CleanedProductSaleItemId = c.Int(nullable: false, identity: true),
                        CleanedProductSaleId = c.Int(nullable: false),
                        ItemCode = c.String(),
                        ExternalSaleQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OnFarmSaleQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPriceExternal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPriceOnFarm = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExternalSaleAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OnFarmSaleAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalSaleAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CleanedProductSaleItemId)
                .ForeignKey("Production.CleanedProductSale", t => t.CleanedProductSaleId, cascadeDelete: true)
                .Index(t => t.CleanedProductSaleId);
            
            CreateTable(
                "Production.CleanedProductSale",
                c => new
                    {
                        CleanedProductSaleId = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CleanedProductSaleId)
                .ForeignKey("HRM.Branch", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .Index(t => t.BranchId)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "Production.SeedProcessingOrder",
                c => new
                    {
                        SeedProcessingOrderId = c.Int(nullable: false, identity: true),
                        ProcessingPlantId = c.Int(nullable: false),
                        ProductionPeriodId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SeedProcessingOrderId)
                .ForeignKey("Production.ProcessingPlant", t => t.ProcessingPlantId, cascadeDelete: true)
                .ForeignKey("Production.ProductionPeriod", t => t.ProductionPeriodId, cascadeDelete: true)
                .Index(t => t.ProcessingPlantId)
                .Index(t => t.ProductionPeriodId);
            
            CreateTable(
                "Production.CustomCleaningAndStorage",
                c => new
                    {
                        CustomCleaningAndStorageId = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CustomCleaningAndStorageId)
                .ForeignKey("HRM.Branch", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .Index(t => t.BranchId)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "Production.CustomCleaningAndStorageItemPerMonth",
                c => new
                    {
                        CustomCleaningAndStorageItemPerMonthId = c.Int(nullable: false, identity: true),
                        CustomCleaningAndStorageItemId = c.Int(nullable: false),
                        ProductionPeriodId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CustomCleaningAndStorageItemPerMonthId)
                .ForeignKey("Production.CustomCleaningAndStorageItem", t => t.CustomCleaningAndStorageItemId, cascadeDelete: true)
                .ForeignKey("Production.ProductionPeriod", t => t.ProductionPeriodId, cascadeDelete: true)
                .Index(t => t.CustomCleaningAndStorageItemId)
                .Index(t => t.ProductionPeriodId);
            
            CreateTable(
                "Production.CustomCleaningAndStorageItem",
                c => new
                    {
                        CustomCleaningAndStorageItemId = c.Int(nullable: false, identity: true),
                        CustomCleaningAndStorageId = c.Int(nullable: false),
                        ItemCode = c.String(),
                        ExternalQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OnFarmQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPriceExternal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPriceOnFarm = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExternalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OnFarmAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CustomCleaningAndStorageItemId)
                .ForeignKey("Production.CustomCleaningAndStorage", t => t.CustomCleaningAndStorageId, cascadeDelete: true)
                .Index(t => t.CustomCleaningAndStorageId);
            
            CreateTable(
                "Production.IssueAndProcessingOrderItemMapping",
                c => new
                    {
                        IssueAndProcessingOrderItemMappingId = c.Int(nullable: false, identity: true),
                        ProcessingOrderItemId = c.Int(nullable: false),
                        ProductionIssueItemId = c.Int(nullable: false),
                        UsedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Type = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.IssueAndProcessingOrderItemMappingId)
                .ForeignKey("Production.ProductionIssueItem", t => t.ProductionIssueItemId, cascadeDelete: true)
                .Index(t => t.ProductionIssueItemId);
            
            CreateTable(
                "Production.ProductionIssueItem",
                c => new
                    {
                        ProductionIssueItemId = c.Int(nullable: false, identity: true),
                        ProductionIssueId = c.Int(nullable: false),
                        ItemCategoryCode = c.String(nullable: false, maxLength: 128),
                        ItemCode = c.String(nullable: false, maxLength: 128),
                        IssuedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdditionalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProductionIssueItemId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode, cascadeDelete: true)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode, cascadeDelete: true)
                .ForeignKey("Production.ProductionIssue", t => t.ProductionIssueId, cascadeDelete: true)
                .Index(t => t.ProductionIssueId)
                .Index(t => t.ItemCategoryCode)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "Production.ProductionIssue",
                c => new
                    {
                        ProductionIssueId = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        StoreCode = c.String(nullable: false),
                        SupplierAccountCode = c.String(),
                        Reference = c.String(),
                        IssueDate = c.DateTime(nullable: false),
                        ProductionPeriodId = c.Int(nullable: false),
                        ProcessingPlantId = c.Int(nullable: false),
                        RecipientId = c.Int(nullable: false),
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
                .PrimaryKey(t => t.ProductionIssueId);
            
            CreateTable(
                "Production.ProductionIssueDistribution",
                c => new
                    {
                        ProductionIssueDistributionId = c.Int(nullable: false, identity: true),
                        ProductionIssueId = c.Int(nullable: false),
                        IssueItemIDTrackBack = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        AdditionalDescription = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProductionIssueDistributionId)
                .ForeignKey("Production.ProductionIssue", t => t.ProductionIssueId, cascadeDelete: true)
                .Index(t => t.ProductionIssueId);
            
            CreateTable(
                "Production.ProductionIssueValidation",
                c => new
                    {
                        ProductionIssueValidationId = c.Int(nullable: false, identity: true),
                        ProductionIssueId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProductionIssueValidationId)
                .ForeignKey("Production.ProductionIssue", t => t.ProductionIssueId, cascadeDelete: true)
                .Index(t => t.ProductionIssueId);
            
            CreateTable(
                "Production.ProductionFinishedGoodsStoreReceiveDistribution",
                c => new
                    {
                        ProductionFinishedGoodsStoreReceiveDistributionId = c.Int(nullable: false, identity: true),
                        ProductionFinishedGoodsStoreReceiveId = c.Int(nullable: false),
                        ReceiveItemIDTrackBack = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        AdditionalDescription = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProductionFinishedGoodsStoreReceiveDistributionId)
                .ForeignKey("Production.ProductionFinishedGoodsStoreReceive", t => t.ProductionFinishedGoodsStoreReceiveId, cascadeDelete: true)
                .Index(t => t.ProductionFinishedGoodsStoreReceiveId);
            
            CreateTable(
                "Production.ProductionStoreReceiveDistribution",
                c => new
                    {
                        ProductionStoreReceiveDistributionId = c.Int(nullable: false, identity: true),
                        ProductionStoreReceiveId = c.Int(nullable: false),
                        ReceiveItemIDTrackBack = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        AdditionalDescription = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProductionStoreReceiveDistributionId)
                .ForeignKey("Production.ProductionStoreReceive", t => t.ProductionStoreReceiveId, cascadeDelete: true)
                .Index(t => t.ProductionStoreReceiveId);
            
            CreateTable(
                "Production.ProjectedMonthlyCashFlow",
                c => new
                    {
                        ProjectedMonthlyCashFlowId = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectedMonthlyCashFlowId)
                .ForeignKey("HRM.Branch", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .Index(t => t.BranchId)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "Production.ProjectedMonthlyCashFlowItem",
                c => new
                    {
                        ProjectedMonthlyCashFlowItemId = c.Int(nullable: false, identity: true),
                        ProjectedMonthlyCashFlowId = c.Int(nullable: false),
                        ProductionPeriodId = c.Int(nullable: false),
                        CleanProductSales = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CustomCleaningIncome = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ByproductSales = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalIncome = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RawProductPurchase = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OtherDirectCosts = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProcessingAndStorage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Maintenance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Administration = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HeadOfficeAllocation = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalExpenditure = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Depreciation = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCashExpenditure = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetCashIncome = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CumulativeCashIncome = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdditionalIncome = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RevisedNetCashIncome = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RevisedCumulativeCashIncome = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectedMonthlyCashFlowItemId)
                .ForeignKey("Production.ProductionPeriod", t => t.ProductionPeriodId, cascadeDelete: true)
                .ForeignKey("Production.ProjectedMonthlyCashFlow", t => t.ProjectedMonthlyCashFlowId, cascadeDelete: true)
                .Index(t => t.ProjectedMonthlyCashFlowId)
                .Index(t => t.ProductionPeriodId);
            
            CreateTable(
                "Production.RawProductFinishedGoodMapping",
                c => new
                    {
                        RawProductFinishedGoodMappingId = c.Int(nullable: false, identity: true),
                        ItemCategoryCode = c.String(),
                        RawItemCode = c.String(nullable: false),
                        ByproductItemCode = c.String(nullable: false),
                        CleanedItemCode = c.String(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RawProductFinishedGoodMappingId);
            
            CreateTable(
                "Production.RawProductPurchaseItemPerMonth",
                c => new
                    {
                        RawProductPurchaseItemPerMonthId = c.Int(nullable: false, identity: true),
                        RawProductPurchaseItemId = c.Int(nullable: false),
                        ProductionPeriodId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RawProductPurchaseItemPerMonthId)
                .ForeignKey("Production.ProductionPeriod", t => t.ProductionPeriodId, cascadeDelete: true)
                .ForeignKey("Production.RawProductPurchaseItem", t => t.RawProductPurchaseItemId, cascadeDelete: true)
                .Index(t => t.RawProductPurchaseItemId)
                .Index(t => t.ProductionPeriodId);
            
            CreateTable(
                "Production.RawProductPurchaseItem",
                c => new
                    {
                        RawProductPurchaseItemId = c.Int(nullable: false, identity: true),
                        RawProductPurchaseId = c.Int(nullable: false),
                        ItemCode = c.String(),
                        ExternalPurchaseQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OnFarmPurchaseQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPriceExternal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPriceOnFarm = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExternalPurchaseAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OnFarmPurchaseAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPurchaseAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RawProductPurchaseItemId)
                .ForeignKey("Production.RawProductPurchase", t => t.RawProductPurchaseId, cascadeDelete: true)
                .Index(t => t.RawProductPurchaseId);
            
            CreateTable(
                "Production.RawProductPurchase",
                c => new
                    {
                        RawProductPurchaseId = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RawProductPurchaseId)
                .ForeignKey("HRM.Branch", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .Index(t => t.BranchId)
                .Index(t => t.GlFiscalYearId);
            
            AddColumn("Production.GumProcessingOrderItem", "GumProcessingOrderId", c => c.Int(nullable: false));
            AddColumn("Production.GumProcessingOrderItem", "ItemCategoryCode", c => c.String(maxLength: 128));
            AddColumn("Production.GumProcessingOrderItem", "ItemCode", c => c.String(maxLength: 128));
            AddColumn("Production.GumProcessingOrderItem", "TotalProductionCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Production.SeedProcessingOrderItem", "SeedProcessingOrderId", c => c.Int(nullable: false));
            AddColumn("Production.SeedProcessingOrderItem", "ItemCategoryCode", c => c.String(maxLength: 128));
            AddColumn("Production.SeedProcessingOrderItem", "ItemCode", c => c.String(maxLength: 128));
            AddColumn("Production.SeedProcessingOrderItem", "TotalProductionCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            CreateIndex("Production.GumProcessingOrderItem", "GumProcessingOrderId");
            CreateIndex("Production.GumProcessingOrderItem", "ItemCategoryCode");
            CreateIndex("Production.GumProcessingOrderItem", "ItemCode");
            CreateIndex("Production.SeedProcessingOrderItem", "SeedProcessingOrderId");
            CreateIndex("Production.SeedProcessingOrderItem", "ItemCategoryCode");
            CreateIndex("Production.SeedProcessingOrderItem", "ItemCode");
            AddForeignKey("Production.GumProcessingOrderItem", "GumProcessingOrderId", "Production.GumProcessingOrder", "GumProcessingOrderId", cascadeDelete: true);
            AddForeignKey("Production.GumProcessingOrderItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode");
            AddForeignKey("Production.GumProcessingOrderItem", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode");
            AddForeignKey("Production.SeedProcessingOrderItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode");
            AddForeignKey("Production.SeedProcessingOrderItem", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode");
           // AddForeignKey("Production.SeedProcessingOrderItem", "SeedProcessingOrderId", "Production.SeedProcessingOrder", "SeedProcessingOrderId", cascadeDelete: true);
            DropColumn("Production.GumProcessingOrderItem", "GumProductionOrderItemId");
            DropColumn("Production.GumProcessingOrderItem", "ProcessingPlantId");
            DropColumn("Production.GumProcessingOrderItem", "GumName");
            DropColumn("Production.SeedProcessingOrderItem", "SeedProductionOrderItemId");
            DropColumn("Production.SeedProcessingOrderItem", "ProcessingPlantId");
            DropColumn("Production.SeedProcessingOrderItem", "SeedName");
        }
        
        public override void Down()
        {
            AddColumn("Production.SeedProcessingOrderItem", "SeedName", c => c.String());
            AddColumn("Production.SeedProcessingOrderItem", "ProcessingPlantId", c => c.Int(nullable: false));
            AddColumn("Production.SeedProcessingOrderItem", "SeedProductionOrderItemId", c => c.Int(nullable: false));
            AddColumn("Production.GumProcessingOrderItem", "GumName", c => c.String());
            AddColumn("Production.GumProcessingOrderItem", "ProcessingPlantId", c => c.Int(nullable: false));
            AddColumn("Production.GumProcessingOrderItem", "GumProductionOrderItemId", c => c.Int(nullable: false));
            DropForeignKey("Production.RawProductPurchaseItemPerMonth", "RawProductPurchaseItemId", "Production.RawProductPurchaseItem");
            DropForeignKey("Production.RawProductPurchaseItem", "RawProductPurchaseId", "Production.RawProductPurchase");
            DropForeignKey("Production.RawProductPurchase", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("Production.RawProductPurchase", "BranchId", "HRM.Branch");
            DropForeignKey("Production.RawProductPurchaseItemPerMonth", "ProductionPeriodId", "Production.ProductionPeriod");
            DropForeignKey("Production.ProjectedMonthlyCashFlowItem", "ProjectedMonthlyCashFlowId", "Production.ProjectedMonthlyCashFlow");
            DropForeignKey("Production.ProjectedMonthlyCashFlowItem", "ProductionPeriodId", "Production.ProductionPeriod");
            DropForeignKey("Production.ProjectedMonthlyCashFlow", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("Production.ProjectedMonthlyCashFlow", "BranchId", "HRM.Branch");
            DropForeignKey("Production.ProductionStoreReceiveDistribution", "ProductionStoreReceiveId", "Production.ProductionStoreReceive");
            DropForeignKey("Production.ProductionFinishedGoodsStoreReceiveDistribution", "ProductionFinishedGoodsStoreReceiveId", "Production.ProductionFinishedGoodsStoreReceive");
            DropForeignKey("Production.IssueAndProcessingOrderItemMapping", "ProductionIssueItemId", "Production.ProductionIssueItem");
            DropForeignKey("Production.ProductionIssueValidation", "ProductionIssueId", "Production.ProductionIssue");
            DropForeignKey("Production.ProductionIssueItem", "ProductionIssueId", "Production.ProductionIssue");
            DropForeignKey("Production.ProductionIssueDistribution", "ProductionIssueId", "Production.ProductionIssue");
            DropForeignKey("Production.ProductionIssueItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("Production.ProductionIssueItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Production.CustomCleaningAndStorageItemPerMonth", "ProductionPeriodId", "Production.ProductionPeriod");
            DropForeignKey("Production.CustomCleaningAndStorageItemPerMonth", "CustomCleaningAndStorageItemId", "Production.CustomCleaningAndStorageItem");
            DropForeignKey("Production.CustomCleaningAndStorageItem", "CustomCleaningAndStorageId", "Production.CustomCleaningAndStorage");
            DropForeignKey("Production.CustomCleaningAndStorage", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("Production.CustomCleaningAndStorage", "BranchId", "HRM.Branch");
            DropForeignKey("Production.SeedProcessingOrderItem", "SeedProcessingOrderId", "Production.SeedProcessingOrder");
            DropForeignKey("Production.SeedProcessingOrder", "ProductionPeriodId", "Production.ProductionPeriod");
            DropForeignKey("Production.SeedProcessingOrder", "ProcessingPlantId", "Production.ProcessingPlant");
            DropForeignKey("Production.SeedProcessingOrderItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("Production.SeedProcessingOrderItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Production.CleanedProductSaleItemPerMonth", "ProductionPeriodId", "Production.ProductionPeriod");
            DropForeignKey("Production.CleanedProductSaleItemPerMonth", "CleanedProductSaleItemId", "Production.CleanedProductSaleItem");
            DropForeignKey("Production.CleanedProductSaleItem", "CleanedProductSaleId", "Production.CleanedProductSale");
            DropForeignKey("Production.CleanedProductSale", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("Production.CleanedProductSale", "BranchId", "HRM.Branch");
            DropForeignKey("Production.GumProcessingOrderItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("Production.GumProcessingOrderItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Production.GumProcessingOrderItem", "GumProcessingOrderId", "Production.GumProcessingOrder");
            DropForeignKey("Production.GumProcessingOrder", "ProductionPeriodId", "Production.ProductionPeriod");
            DropForeignKey("Production.GumProcessingOrder", "ProcessingPlantId", "Production.ProcessingPlant");
            DropIndex("Production.RawProductPurchase", new[] { "GlFiscalYearId" });
            DropIndex("Production.RawProductPurchase", new[] { "BranchId" });
            DropIndex("Production.RawProductPurchaseItem", new[] { "RawProductPurchaseId" });
            DropIndex("Production.RawProductPurchaseItemPerMonth", new[] { "ProductionPeriodId" });
            DropIndex("Production.RawProductPurchaseItemPerMonth", new[] { "RawProductPurchaseItemId" });
            DropIndex("Production.ProjectedMonthlyCashFlowItem", new[] { "ProductionPeriodId" });
            DropIndex("Production.ProjectedMonthlyCashFlowItem", new[] { "ProjectedMonthlyCashFlowId" });
            DropIndex("Production.ProjectedMonthlyCashFlow", new[] { "GlFiscalYearId" });
            DropIndex("Production.ProjectedMonthlyCashFlow", new[] { "BranchId" });
            DropIndex("Production.ProductionStoreReceiveDistribution", new[] { "ProductionStoreReceiveId" });
            DropIndex("Production.ProductionFinishedGoodsStoreReceiveDistribution", new[] { "ProductionFinishedGoodsStoreReceiveId" });
            DropIndex("Production.ProductionIssueValidation", new[] { "ProductionIssueId" });
            DropIndex("Production.ProductionIssueDistribution", new[] { "ProductionIssueId" });
            DropIndex("Production.ProductionIssueItem", new[] { "ItemCode" });
            DropIndex("Production.ProductionIssueItem", new[] { "ItemCategoryCode" });
            DropIndex("Production.ProductionIssueItem", new[] { "ProductionIssueId" });
            DropIndex("Production.IssueAndProcessingOrderItemMapping", new[] { "ProductionIssueItemId" });
            DropIndex("Production.CustomCleaningAndStorageItem", new[] { "CustomCleaningAndStorageId" });
            DropIndex("Production.CustomCleaningAndStorageItemPerMonth", new[] { "ProductionPeriodId" });
            DropIndex("Production.CustomCleaningAndStorageItemPerMonth", new[] { "CustomCleaningAndStorageItemId" });
            DropIndex("Production.CustomCleaningAndStorage", new[] { "GlFiscalYearId" });
            DropIndex("Production.CustomCleaningAndStorage", new[] { "BranchId" });
            DropIndex("Production.SeedProcessingOrder", new[] { "ProductionPeriodId" });
            DropIndex("Production.SeedProcessingOrder", new[] { "ProcessingPlantId" });
            DropIndex("Production.SeedProcessingOrderItem", new[] { "ItemCode" });
            DropIndex("Production.SeedProcessingOrderItem", new[] { "ItemCategoryCode" });
            DropIndex("Production.SeedProcessingOrderItem", new[] { "SeedProcessingOrderId" });
            DropIndex("Production.CleanedProductSale", new[] { "GlFiscalYearId" });
            DropIndex("Production.CleanedProductSale", new[] { "BranchId" });
            DropIndex("Production.CleanedProductSaleItem", new[] { "CleanedProductSaleId" });
            DropIndex("Production.CleanedProductSaleItemPerMonth", new[] { "ProductionPeriodId" });
            DropIndex("Production.CleanedProductSaleItemPerMonth", new[] { "CleanedProductSaleItemId" });
            DropIndex("Production.GumProcessingOrder", new[] { "ProductionPeriodId" });
            DropIndex("Production.GumProcessingOrder", new[] { "ProcessingPlantId" });
            DropIndex("Production.GumProcessingOrderItem", new[] { "ItemCode" });
            DropIndex("Production.GumProcessingOrderItem", new[] { "ItemCategoryCode" });
            DropIndex("Production.GumProcessingOrderItem", new[] { "GumProcessingOrderId" });
            DropColumn("Production.SeedProcessingOrderItem", "TotalProductionCost");
            DropColumn("Production.SeedProcessingOrderItem", "ItemCode");
            DropColumn("Production.SeedProcessingOrderItem", "ItemCategoryCode");
            DropColumn("Production.SeedProcessingOrderItem", "SeedProcessingOrderId");
            DropColumn("Production.GumProcessingOrderItem", "TotalProductionCost");
            DropColumn("Production.GumProcessingOrderItem", "ItemCode");
            DropColumn("Production.GumProcessingOrderItem", "ItemCategoryCode");
            DropColumn("Production.GumProcessingOrderItem", "GumProcessingOrderId");
            DropTable("Production.RawProductPurchase");
            DropTable("Production.RawProductPurchaseItem");
            DropTable("Production.RawProductPurchaseItemPerMonth");
            DropTable("Production.RawProductFinishedGoodMapping");
            DropTable("Production.ProjectedMonthlyCashFlowItem");
            DropTable("Production.ProjectedMonthlyCashFlow");
            DropTable("Production.ProductionStoreReceiveDistribution");
            DropTable("Production.ProductionFinishedGoodsStoreReceiveDistribution");
            DropTable("Production.ProductionIssueValidation");
            DropTable("Production.ProductionIssueDistribution");
            DropTable("Production.ProductionIssue");
            DropTable("Production.ProductionIssueItem");
            DropTable("Production.IssueAndProcessingOrderItemMapping");
            DropTable("Production.CustomCleaningAndStorageItem");
            DropTable("Production.CustomCleaningAndStorageItemPerMonth");
            DropTable("Production.CustomCleaningAndStorage");
            DropTable("Production.SeedProcessingOrder");
            DropTable("Production.CleanedProductSale");
            DropTable("Production.CleanedProductSaleItem");
            DropTable("Production.CleanedProductSaleItemPerMonth");
            DropTable("Production.GumProcessingOrder");
            CreateIndex("Production.SeedProcessingOrderItem", "ProcessingPlantId");
            CreateIndex("Production.SeedProcessingOrderItem", "SeedProductionOrderItemId");
            CreateIndex("Production.GumProcessingOrderItem", "ProcessingPlantId");
            CreateIndex("Production.GumProcessingOrderItem", "GumProductionOrderItemId");
            AddForeignKey("Production.SeedProcessingOrderItem", "SeedProductionOrderItemId", "Production.SeedProductionOrderItem", "SeedProductionOrderItemId", cascadeDelete: true);
            AddForeignKey("Production.SeedProcessingOrderItem", "ProcessingPlantId", "Production.ProcessingPlant", "ProcessingPlantId", cascadeDelete: true);
            AddForeignKey("Production.GumProcessingOrderItem", "ProcessingPlantId", "Production.ProcessingPlant", "ProcessingPlantId", cascadeDelete: true);
            AddForeignKey("Production.GumProcessingOrderItem", "GumProductionOrderItemId", "Production.GumProductionOrderItem", "GumProductionOrderItemId", cascadeDelete: true);
        }
    }
}
