namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class manufacturingfurni : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Manufacturing.DirectLaborCosts", "FurnitureCategoryId", "Manufacturing.FurnitureCategories");
            DropForeignKey("Manufacturing.DirectLaborCosts", "FurnitureEstimationId", "Manufacturing.FurnitureCostEstimations");
            DropIndex("Manufacturing.DirectLaborCosts", new[] { "FurnitureCategoryId" });
            DropIndex("Manufacturing.DirectLaborCosts", new[] { "FurnitureEstimationId" });
            AddColumn("Manufacturing.DirectLaborCosts", "TaskCategoryId", c => c.Int(nullable: false));
            AddColumn("Manufacturing.FurnitureMaterialCosts", "UnitOfMeasurment", c => c.String());
            AddColumn("Manufacturing.FurnitureOverallCosts", "LabourCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Manufacturing.FurnitureOverallCosts", "ManufacturingCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Manufacturing.FurnitureOverallCosts", "AdministrativeCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Manufacturing.FurnitureOverallCosts", "GrandTotalCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Manufacturing.FurnitureOverallCosts", "ProfitMargin", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Manufacturing.FurnitureOverallCosts", "FinalPriceIncludingProfit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("Manufacturing.FurnitureOverallCosts", "LaborCost");
            DropColumn("Manufacturing.FurnitureOverallCosts", "T");
            DropColumn("Manufacturing.FurnitureOverallCosts", "MarginalCost");
            DropColumn("Manufacturing.FurnitureOverallCosts", "NBT");
            DropColumn("Manufacturing.FurnitureOverallCosts", "TInpercent");
            DropColumn("Manufacturing.FurnitureOverallCosts", "GT");
            DropColumn("Manufacturing.FurnitureOverallCosts", "Graphic");
            DropColumn("Manufacturing.FurnitureOverallCosts", "EstimatedNetPrice");
        }
        
        public override void Down()
        {
            AddColumn("Manufacturing.FurnitureOverallCosts", "EstimatedNetPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Manufacturing.FurnitureOverallCosts", "Graphic", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Manufacturing.FurnitureOverallCosts", "GT", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Manufacturing.FurnitureOverallCosts", "TInpercent", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Manufacturing.FurnitureOverallCosts", "NBT", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Manufacturing.FurnitureOverallCosts", "MarginalCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Manufacturing.FurnitureOverallCosts", "T", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Manufacturing.FurnitureOverallCosts", "LaborCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("Manufacturing.FurnitureOverallCosts", "FinalPriceIncludingProfit");
            DropColumn("Manufacturing.FurnitureOverallCosts", "ProfitMargin");
            DropColumn("Manufacturing.FurnitureOverallCosts", "GrandTotalCost");
            DropColumn("Manufacturing.FurnitureOverallCosts", "AdministrativeCost");
            DropColumn("Manufacturing.FurnitureOverallCosts", "ManufacturingCost");
            DropColumn("Manufacturing.FurnitureOverallCosts", "LabourCost");
            DropColumn("Manufacturing.FurnitureMaterialCosts", "UnitOfMeasurment");
            DropColumn("Manufacturing.DirectLaborCosts", "TaskCategoryId");
            CreateIndex("Manufacturing.DirectLaborCosts", "FurnitureEstimationId");
            CreateIndex("Manufacturing.DirectLaborCosts", "FurnitureCategoryId");
            AddForeignKey("Manufacturing.DirectLaborCosts", "FurnitureEstimationId", "Manufacturing.FurnitureCostEstimations", "FurnitureEstimationId", cascadeDelete: true);
            AddForeignKey("Manufacturing.DirectLaborCosts", "FurnitureCategoryId", "Manufacturing.FurnitureCategories", "FurnitureCategoryId", cascadeDelete: true);
        }
    }
}
