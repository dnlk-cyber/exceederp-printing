namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class furnituresetting : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Manufacturing.FurnitureStandardJobType",
                c => new
                    {
                        FurnitureStandardJobTypeId = c.Int(nullable: false, identity: true),
                        IsStandard = c.Boolean(nullable: false),
                        JobNo = c.String(),
                        JobTypeName = c.String(),
                        Unit = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureStandardJobTypeId);
            
            CreateTable(
                "Manufacturing.StandardBillOfLabor",
                c => new
                    {
                        StandardBillOfLaborId = c.Int(nullable: false, identity: true),
                        ManufacturingTaskCategoryId = c.Int(nullable: false),
                        WorkShopType = c.Int(nullable: false),
                        NumberOfEmloyees = c.Int(nullable: false),
                        EstimatedMinute = c.Int(nullable: false),
                        FurnitureStandardJobTypeId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StandardBillOfLaborId)
                .ForeignKey("Manufacturing.FurnitureStandardJobType", t => t.FurnitureStandardJobTypeId, cascadeDelete: true)
                .Index(t => t.FurnitureStandardJobTypeId);
            
            CreateTable(
                "Manufacturing.StandardBillOfMaterial",
                c => new
                    {
                        StandardBillOfMaterialId = c.Int(nullable: false, identity: true),
                        ManufacturingMaterialCategoryId = c.Int(nullable: false),
                        ManufacturingMaterialCategoryItemId = c.Int(nullable: false),
                        ItemCode = c.String(),
                        UnitOfMeasurement = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        WorkshopType = c.Int(nullable: false),
                        FurnitureStandardJobTypeId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StandardBillOfMaterialId)
                .ForeignKey("Manufacturing.FurnitureStandardJobType", t => t.FurnitureStandardJobTypeId, cascadeDelete: true)
                .Index(t => t.FurnitureStandardJobTypeId);
            
            AddColumn("Manufacturing.FurnitureBillOfQuantity", "IsCustomizedJob", c => c.Boolean(nullable: false));
            AddColumn("Manufacturing.FurnitureBillOfQuantity", "JobTypeCategory", c => c.Int(nullable: false));
            AddColumn("Manufacturing.FurnitureBillOfQuantity", "JobNo", c => c.String());
            AddColumn("Manufacturing.FurnitureBillOfQuantity", "JobTypeName", c => c.String());
            AddColumn("Manufacturing.FurnitureBillOfQuantity", "Unit", c => c.String());
            AlterColumn("Manufacturing.FurnitureBillOfQuantity", "JobTypeId", c => c.Int());
        }
        
        public override void Down()
        {
            DropForeignKey("Manufacturing.StandardBillOfMaterial", "FurnitureStandardJobTypeId", "Manufacturing.FurnitureStandardJobType");
            DropForeignKey("Manufacturing.StandardBillOfLabor", "FurnitureStandardJobTypeId", "Manufacturing.FurnitureStandardJobType");
            DropIndex("Manufacturing.StandardBillOfMaterial", new[] { "FurnitureStandardJobTypeId" });
            DropIndex("Manufacturing.StandardBillOfLabor", new[] { "FurnitureStandardJobTypeId" });
            AlterColumn("Manufacturing.FurnitureBillOfQuantity", "JobTypeId", c => c.Int(nullable: false));
            DropColumn("Manufacturing.FurnitureBillOfQuantity", "Unit");
            DropColumn("Manufacturing.FurnitureBillOfQuantity", "JobTypeName");
            DropColumn("Manufacturing.FurnitureBillOfQuantity", "JobNo");
            DropColumn("Manufacturing.FurnitureBillOfQuantity", "JobTypeCategory");
            DropColumn("Manufacturing.FurnitureBillOfQuantity", "IsCustomizedJob");
            DropTable("Manufacturing.StandardBillOfMaterial");
            DropTable("Manufacturing.StandardBillOfLabor");
            DropTable("Manufacturing.FurnitureStandardJobType");
        }
    }
}
