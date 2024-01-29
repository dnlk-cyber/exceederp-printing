namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prodestimationupgrade : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "PrintingEstimation.DirectLaborCosts",
                c => new
                    {
                        DirectLaborCostId = c.Int(nullable: false, identity: true),
                        FurnitureCategoryId = c.Int(nullable: false),
                        FurnitureCostEstimationId = c.Int(nullable: false),
                        WorkingTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CostPerHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DirectLaborCostId)
                .ForeignKey("PrintingEstimation.FurnitureCategories", t => t.FurnitureCategoryId, cascadeDelete: true)
                .ForeignKey("PrintingEstimation.FurnitureCostEstimations", t => t.FurnitureCostEstimationId, cascadeDelete: true)
                .Index(t => t.FurnitureCategoryId)
                .Index(t => t.FurnitureCostEstimationId);
            
            CreateTable(
                "PrintingEstimation.FurnitureCategories",
                c => new
                    {
                        FurnitureCategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WorkingTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CostPerHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureCategoryId);
            
            CreateTable(
                "PrintingEstimation.FurnitureCostEstimations",
                c => new
                    {
                        FurnitureCostEstimationId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        JobTypeId = c.Int(nullable: false),
                        JT = c.String(),
                        JobNumber = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        Others = c.String(),
                        IsChecked = c.Boolean(nullable: false),
                        CheckedBy = c.String(),
                        ApprovedBy = c.String(),
                        IsApproved = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureCostEstimationId);
            
            CreateTable(
                "PrintingEstimation.FurnitureMaterialCosts",
                c => new
                    {
                        FurnitureMaterialCostId = c.Int(nullable: false, identity: true),
                        FurnitureCostEstimationId = c.Int(nullable: false),
                        InventoryCategoryId = c.String(),
                        InventoryId = c.String(),
                        JobId = c.Int(),
                        StoreCode = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureMaterialCostId)
                .ForeignKey("PrintingEstimation.FurnitureCostEstimations", t => t.FurnitureCostEstimationId, cascadeDelete: true)
                .Index(t => t.FurnitureCostEstimationId);
            
            CreateTable(
                "PrintingEstimation.FurnitureOverallCosts",
                c => new
                    {
                        FurnitureOverallCostId = c.Int(nullable: false, identity: true),
                        MaterialCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LaborCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverHeadCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        T = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MarginalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NBT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TInpercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Graphic = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EstimatedNetPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FurnitureCostEstimationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FurnitureOverallCostId)
                .ForeignKey("PrintingEstimation.FurnitureCostEstimations", t => t.FurnitureCostEstimationId, cascadeDelete: true)
                .Index(t => t.FurnitureCostEstimationId);
            
            AlterColumn("PrintingEstimation.EstimationForms", "JobNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("PrintingEstimation.FurnitureOverallCosts", "FurnitureCostEstimationId", "PrintingEstimation.FurnitureCostEstimations");
            DropForeignKey("PrintingEstimation.FurnitureMaterialCosts", "FurnitureCostEstimationId", "PrintingEstimation.FurnitureCostEstimations");
            DropForeignKey("PrintingEstimation.DirectLaborCosts", "FurnitureCostEstimationId", "PrintingEstimation.FurnitureCostEstimations");
            DropForeignKey("PrintingEstimation.DirectLaborCosts", "FurnitureCategoryId", "PrintingEstimation.FurnitureCategories");
            DropIndex("PrintingEstimation.FurnitureOverallCosts", new[] { "FurnitureCostEstimationId" });
            DropIndex("PrintingEstimation.FurnitureMaterialCosts", new[] { "FurnitureCostEstimationId" });
            DropIndex("PrintingEstimation.DirectLaborCosts", new[] { "FurnitureCostEstimationId" });
            DropIndex("PrintingEstimation.DirectLaborCosts", new[] { "FurnitureCategoryId" });
            AlterColumn("PrintingEstimation.EstimationForms", "JobNumber", c => c.String(nullable: false));
            DropTable("PrintingEstimation.FurnitureOverallCosts");
            DropTable("PrintingEstimation.FurnitureMaterialCosts");
            DropTable("PrintingEstimation.FurnitureCostEstimations");
            DropTable("PrintingEstimation.FurnitureCategories");
            DropTable("PrintingEstimation.DirectLaborCosts");
        }
    }
}
