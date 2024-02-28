namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class manufacturingcatego : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Manufacturing.ManifucturingTaskCategory",
                c => new
                    {
                        ManifucturingTaskCategoryId = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Name = c.String(),
                        StandardHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HourRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ManifucturingTaskCategoryId);
            
            CreateTable(
                "Manufacturing.ManufacturingMaterialCategory",
                c => new
                    {
                        ManufacturingMaterialCategoryId = c.Int(nullable: false, identity: true),
                        InventoryCategoryCode = c.String(),
                        ManufacturingCategory = c.String(),
                        CostAccountCode = c.String(),
                        InventoryAccountCode = c.String(),
                        ProductionAccountCode = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ManufacturingMaterialCategoryId);
            
            CreateTable(
                "Manufacturing.ManufacturingMaterialCategoryDetail",
                c => new
                    {
                        ManufacturingMaterialCategoryDetailId = c.Int(nullable: false, identity: true),
                        InventorySubCategoryCode = c.String(),
                        ItemName = c.String(),
                        ItemCode = c.String(),
                        UnitOfMeasurement = c.String(),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsActive = c.Boolean(nullable: false),
                        ManufacturingMaterialCategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ManufacturingMaterialCategoryDetailId);
            
            AddColumn("Manufacturing.FurnitureMaterialCosts", "TaskType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Manufacturing.FurnitureMaterialCosts", "TaskType");
            DropTable("Manufacturing.ManufacturingMaterialCategoryDetail");
            DropTable("Manufacturing.ManufacturingMaterialCategory");
            DropTable("Manufacturing.ManifucturingTaskCategory");
        }
    }
}
