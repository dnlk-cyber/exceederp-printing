namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class constructions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Construction.ProjectMaterialExpense",
                c => new
                    {
                        ProjectMaterialExpenseId = c.Int(nullable: false, identity: true),
                        ProjectInformationId = c.Int(nullable: false),
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
                .PrimaryKey(t => t.ProjectMaterialExpenseId)
                .ForeignKey("EXCEED.GLPeriod", t => t.GLPeriodId, cascadeDelete: true)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode, cascadeDelete: true)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode, cascadeDelete: true)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId)
                .Index(t => t.GLPeriodId)
                .Index(t => t.ItemCategoryCode)
                .Index(t => t.ItemCode);
            
            AddColumn("EXCEED.ItemTransfer", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ItemTransferReceive", "BranchId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("Construction.ProjectMaterialExpense", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ProjectMaterialExpense", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("Construction.ProjectMaterialExpense", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Construction.ProjectMaterialExpense", "GLPeriodId", "EXCEED.GLPeriod");
            DropIndex("Construction.ProjectMaterialExpense", new[] { "ItemCode" });
            DropIndex("Construction.ProjectMaterialExpense", new[] { "ItemCategoryCode" });
            DropIndex("Construction.ProjectMaterialExpense", new[] { "GLPeriodId" });
            DropIndex("Construction.ProjectMaterialExpense", new[] { "ProjectInformationId" });
            DropColumn("EXCEED.ItemTransferReceive", "BranchId");
            DropColumn("EXCEED.ItemTransfer", "BranchId");
            DropTable("Construction.ProjectMaterialExpense");
        }
    }
}
