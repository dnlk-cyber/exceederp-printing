namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction32 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Construction.ConstructionCustomerAccount",
                c => new
                    {
                        ConstructionCustomerAccountId = c.Int(nullable: false, identity: true),
                        ProjectInformationId = c.Int(nullable: false),
                        AccountCode = c.String(),
                        Description = c.String(),
                        AccountType = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ConstructionCustomerAccountId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.ConstructionSubPeriod",
                c => new
                    {
                        ConstructionSubPeriodId = c.Int(nullable: false, identity: true),
                        ConstructionPeriodId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        DateFrom = c.DateTime(nullable: false),
                        DateTo = c.DateTime(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ConstructionSubPeriodId)
                .ForeignKey("Construction.ConstructionPeriod", t => t.ConstructionPeriodId, cascadeDelete: true)
                .Index(t => t.ConstructionPeriodId);
            
            CreateTable(
                "Construction.ProjectDirectMaterialExpense",
                c => new
                    {
                        ProjectDirectMaterialExpenseId = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        ItemCategoryCode = c.String(nullable: false, maxLength: 128),
                        ItemCode = c.String(nullable: false, maxLength: 128),
                        Unit = c.String(),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DeliveredQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ConsumedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TranIn = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TranOut = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DeliveredQuantityPrevMonth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ConsumedQuantityPrevMonth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterilOnSitePrevMonth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DeliveredQuantityToDate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ConsumedQuantityToDate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ShipedQuantityToDate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterialOnSiteToDate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StockBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CostOfConsumedMaterialThisMonth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CostOfConsumedMaterialPrevMonth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CostOfConsumedMaterialToDate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StockAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterialOnSiteAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectDirectMaterialExpenseId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode, cascadeDelete: true)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode, cascadeDelete: true)
                .Index(t => t.ItemCategoryCode)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "Construction.ProjectIndirectMaterialExpense",
                c => new
                    {
                        ProjectIndirectMaterialExpenseId = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        ItemCategoryCode = c.String(nullable: false, maxLength: 128),
                        ItemCode = c.String(nullable: false, maxLength: 128),
                        Unit = c.String(),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DeliveredQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ConsumedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TranIn = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TranOut = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DeliveredQuantityPrevMonth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ConsumedQuantityPrevMonth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterilOnSitePrevMonth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DeliveredQuantityToDate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ConsumedQuantityToDate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ShipedQuantityToDate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterialOnSiteToDate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StockBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CostOfConsumedMaterialThisMonth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CostOfConsumedMaterialPrevMonth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CostOfConsumedMaterialToDate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StockAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterialOnSiteAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectIndirectMaterialExpenseId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode, cascadeDelete: true)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode, cascadeDelete: true)
                .Index(t => t.ItemCategoryCode)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "Construction.TaskDetailBreakDown",
                c => new
                    {
                        TaskDetailBreakDownId = c.Int(nullable: false, identity: true),
                        TaskDetailId = c.Int(nullable: false),
                        ConstructionSubPeriodId = c.Int(nullable: false),
                        PlannedQuantityOfWork = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlannedMaterialCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlannedEquipmentCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlannedLaborCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlannedDirectCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlannedProfitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlannedOverHeadCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TaskDetailBreakDownId)
                .ForeignKey("Construction.TaskDetail", t => t.TaskDetailId, cascadeDelete: true)
                .Index(t => t.TaskDetailId);
            
            AlterColumn("Construction.ConsARStandardCollectionDetail", "ApplyAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ConsARStandardCollectionDetail", "BalanceDue", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ConsARStandardCollectionDetail", "AdvanceAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ConsARStandardCollectionDetail", "Retention", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ConsARStandardCollectionDetail", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ConsARStandardCollectionDetail", "WithholdingTaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ConsReceivableInvoiceDetail", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ConsReceivableInvoiceDetail", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ConsReceivableInvoiceDetail", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ConsReceivableInvoiceDetail", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ConsReceivableInvoiceDetail", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ConsReceivableInvoiceDetail", "Penality", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ConsReceivableInvoiceDetail", "Retention", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ConsReceivableInvoiceDetail", "AdvanceRepayment", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ConsReceivableInvoiceDetail", "NetAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ConsReceivableInvoiceDistribution", "Debit", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ConsReceivableInvoiceDistribution", "Credit", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ConsStandardCollectionDistribution", "Debit", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ConsStandardCollectionDistribution", "Credit", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.WorkInProgressDetails", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.WorkInProgressDetails", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.WorkInProgressDetails", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.WorkInProgressDetails", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.WorkInProgressDetails", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.WorkInProgressDistribution", "Debit", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.WorkInProgressDistribution", "Credit", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            DropForeignKey("Construction.TaskDetailBreakDown", "TaskDetailId", "Construction.TaskDetail");
            DropForeignKey("Construction.ProjectIndirectMaterialExpense", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("Construction.ProjectIndirectMaterialExpense", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Construction.ProjectDirectMaterialExpense", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("Construction.ProjectDirectMaterialExpense", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Construction.ConstructionSubPeriod", "ConstructionPeriodId", "Construction.ConstructionPeriod");
            DropForeignKey("Construction.ConstructionCustomerAccount", "ProjectInformationId", "Construction.ProjectInformation");
            DropIndex("Construction.TaskDetailBreakDown", new[] { "TaskDetailId" });
            DropIndex("Construction.ProjectIndirectMaterialExpense", new[] { "ItemCode" });
            DropIndex("Construction.ProjectIndirectMaterialExpense", new[] { "ItemCategoryCode" });
            DropIndex("Construction.ProjectDirectMaterialExpense", new[] { "ItemCode" });
            DropIndex("Construction.ProjectDirectMaterialExpense", new[] { "ItemCategoryCode" });
            DropIndex("Construction.ConstructionSubPeriod", new[] { "ConstructionPeriodId" });
            DropIndex("Construction.ConstructionCustomerAccount", new[] { "ProjectInformationId" });
            AlterColumn("Construction.WorkInProgressDistribution", "Credit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.WorkInProgressDistribution", "Debit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.WorkInProgressDetails", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.WorkInProgressDetails", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.WorkInProgressDetails", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.WorkInProgressDetails", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.WorkInProgressDetails", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ConsStandardCollectionDistribution", "Credit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ConsStandardCollectionDistribution", "Debit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ConsReceivableInvoiceDistribution", "Credit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ConsReceivableInvoiceDistribution", "Debit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ConsReceivableInvoiceDetail", "NetAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ConsReceivableInvoiceDetail", "AdvanceRepayment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ConsReceivableInvoiceDetail", "Retention", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ConsReceivableInvoiceDetail", "Penality", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ConsReceivableInvoiceDetail", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ConsReceivableInvoiceDetail", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ConsReceivableInvoiceDetail", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ConsReceivableInvoiceDetail", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ConsReceivableInvoiceDetail", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ConsARStandardCollectionDetail", "WithholdingTaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ConsARStandardCollectionDetail", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ConsARStandardCollectionDetail", "Retention", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ConsARStandardCollectionDetail", "AdvanceAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ConsARStandardCollectionDetail", "BalanceDue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ConsARStandardCollectionDetail", "ApplyAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropTable("Construction.TaskDetailBreakDown");
            DropTable("Construction.ProjectIndirectMaterialExpense");
            DropTable("Construction.ProjectDirectMaterialExpense");
            DropTable("Construction.ConstructionSubPeriod");
            DropTable("Construction.ConstructionCustomerAccount");
        }
    }
}
