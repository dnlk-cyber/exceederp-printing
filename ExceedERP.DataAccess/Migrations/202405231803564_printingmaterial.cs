namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printingmaterial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Printing.PrintingMaterialCategory",
                c => new
                    {
                        PrintingMaterialCategoryId = c.Int(nullable: false, identity: true),
                        InventoryCategoryCode = c.String(),
                        CategoryName = c.String(),
                        CostAccountCode = c.String(),
                        InventoryAccountCode = c.String(),
                        ProductionAccountCode = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.PrintingMaterialCategoryId);
            
            CreateTable(
                "Printing.PrintingMaterialCategoryItem",
                c => new
                    {
                        PrintingMaterialCategoryItemId = c.Int(nullable: false, identity: true),
                        InventorySubCategoryCode = c.String(),
                        ItemName = c.String(),
                        ItemCode = c.String(),
                        UnitOfMeasurement = c.String(),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsActive = c.Boolean(nullable: false),
                        Width = c.Double(nullable: false),
                        Length = c.Double(nullable: false),
                        Weight = c.Double(nullable: false),
                        PrintingMaterialCategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PrintingMaterialCategoryItemId);
            
            AddColumn("Printing.PrintingCostEstimation", "CustomerId", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingCostEstimation", "JobTypeId", c => c.Int(nullable: false));
            DropColumn("Printing.PrintingCostEstimation", "PaperTypeId");
        }
        
        public override void Down()
        {
            AddColumn("Printing.PrintingCostEstimation", "PaperTypeId", c => c.Int(nullable: false));
            DropColumn("Printing.PrintingCostEstimation", "JobTypeId");
            DropColumn("Printing.PrintingCostEstimation", "CustomerId");
            DropTable("Printing.PrintingMaterialCategoryItem");
            DropTable("Printing.PrintingMaterialCategory");
        }
    }
}
