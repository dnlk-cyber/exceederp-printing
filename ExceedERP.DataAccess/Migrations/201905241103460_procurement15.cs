namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procurement15 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.PlanItem", "InventoryPlanID", "EXCEED.InventoryPlan");
            DropForeignKey("EXCEED.PlanValidation", "InventoryPlanID", "EXCEED.InventoryPlan");
            DropIndex("EXCEED.PlanItem", new[] { "InventoryPlanID" });
            DropIndex("EXCEED.PlanValidation", new[] { "InventoryPlanID" });
            CreateTable(
                "EXCEED.ProcurementPlanItem",
                c => new
                    {
                        ProcurementPlanItemId = c.Int(nullable: false, identity: true),
                        ProcurementPlanId = c.Int(nullable: false),
                        CatCode = c.String(nullable: false),
                        CategoryName = c.String(),
                        ItmCode = c.String(nullable: false),
                        ItemName = c.String(),
                        PartNo = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemSpecification = c.String(),
                        UoM = c.String(),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemType = c.Int(nullable: false),
                        Type = c.String(),
                        BudgetSource = c.Int(nullable: false),
                        ProcurementCategory = c.Int(nullable: false),
                        ProcurementMethod = c.Int(nullable: false),
                        TenderFloatingTime = c.DateTime(),
                        PurchaseDeliveryTime = c.DateTime(),
                        Period1 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period2 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period3 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period4 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period5 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period6 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period7 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period8 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period9 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period10 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period11 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period12 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProcurementPlanItemId)
                .ForeignKey("EXCEED.ProcurementPlan", t => t.ProcurementPlanId, cascadeDelete: true)
                .Index(t => t.ProcurementPlanId);
            
            CreateTable(
                "EXCEED.ProcurementPlan",
                c => new
                    {
                        ProcurementPlanId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        Name = c.String(),
                        Remark = c.String(),
                        Status = c.String(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProcurementPlanId);
            
            CreateTable(
                "EXCEED.ProcurementPlanValidation",
                c => new
                    {
                        ProcurementPlanValidationId = c.Int(nullable: false, identity: true),
                        ProcurementPlanId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProcurementPlanValidationId)
                .ForeignKey("EXCEED.ProcurementPlan", t => t.ProcurementPlanId, cascadeDelete: true)
                .Index(t => t.ProcurementPlanId);
            
            DropTable("EXCEED.InventoryPlan");
            DropTable("EXCEED.PlanItem");
            DropTable("EXCEED.PlanValidation");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.PlanValidation",
                c => new
                    {
                        PlanValidationID = c.Int(nullable: false, identity: true),
                        InventoryPlanID = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PlanValidationID);
            
            CreateTable(
                "EXCEED.PlanItem",
                c => new
                    {
                        PlanItemID = c.Int(nullable: false, identity: true),
                        InventoryPlanID = c.Int(nullable: false),
                        CatCode = c.String(nullable: false),
                        CategoryName = c.String(),
                        ItmCode = c.String(nullable: false),
                        ItemName = c.String(),
                        PartNo = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemSpecification = c.String(),
                        UoM = c.String(),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemType = c.Int(nullable: false),
                        Type = c.String(),
                        BudgetSource = c.Int(nullable: false),
                        ProcurementCategory = c.Int(nullable: false),
                        ProcurementMethod = c.Int(nullable: false),
                        TenderFloatingTime = c.DateTime(),
                        PurchaseDeliveryTime = c.DateTime(),
                        Period1 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period2 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period3 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period4 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period5 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period6 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period7 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period8 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period9 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period10 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period11 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Period12 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PlanItemID);
            
            CreateTable(
                "EXCEED.InventoryPlan",
                c => new
                    {
                        InventoryPlanId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        Name = c.String(),
                        Remark = c.String(),
                        Status = c.String(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.InventoryPlanId);
            
            DropForeignKey("EXCEED.ProcurementPlanValidation", "ProcurementPlanId", "EXCEED.ProcurementPlan");
            DropForeignKey("EXCEED.ProcurementPlanItem", "ProcurementPlanId", "EXCEED.ProcurementPlan");
            DropIndex("EXCEED.ProcurementPlanValidation", new[] { "ProcurementPlanId" });
            DropIndex("EXCEED.ProcurementPlanItem", new[] { "ProcurementPlanId" });
            DropTable("EXCEED.ProcurementPlanValidation");
            DropTable("EXCEED.ProcurementPlan");
            DropTable("EXCEED.ProcurementPlanItem");
            CreateIndex("EXCEED.PlanValidation", "InventoryPlanID");
            CreateIndex("EXCEED.PlanItem", "InventoryPlanID");
            AddForeignKey("EXCEED.PlanValidation", "InventoryPlanID", "EXCEED.InventoryPlan", "InventoryPlanId", cascadeDelete: true);
            AddForeignKey("EXCEED.PlanItem", "InventoryPlanID", "EXCEED.InventoryPlan", "InventoryPlanId", cascadeDelete: true);
        }
    }
}
