namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class manufacturingmig : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.FurnitureJobAssignment",
                c => new
                    {
                        FurnitureJobAssignmentId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        TaskCategoryId = c.Int(nullable: false),
                        TimeSpent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaskAssignedDate = c.DateTime(nullable: false),
                        TaskEndDate = c.DateTime(nullable: false),
                        FurnitureJobOrderProductionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FurnitureJobAssignmentId)
                .ForeignKey("EXCEED.FurnitureJobOrderProduction", t => t.FurnitureJobOrderProductionId, cascadeDelete: true)
                .Index(t => t.FurnitureJobOrderProductionId);
            
            CreateTable(
                "EXCEED.FurnitureJobOrderProduction",
                c => new
                    {
                        FurnitureJobOrderProductionId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        JobTypeId = c.Int(nullable: false),
                        JobNo = c.String(),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DeliveryDate = c.DateTime(nullable: false),
                        FurnitureJobOrderFormId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FurnitureJobOrderProductionId);
            
            CreateTable(
                "EXCEED.FurnitureProductionBillOfLabor",
                c => new
                    {
                        FurnitureProductionBillOfLaborId = c.Int(nullable: false, identity: true),
                        ManufacturingTaskCategoryId = c.Int(nullable: false),
                        WorkShopType = c.Int(nullable: false),
                        NumberOfEmloyees = c.Int(nullable: false),
                        EstimatedMinute = c.Int(nullable: false),
                        FurnitureJobOrderProductionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FurnitureProductionBillOfLaborId)
                .ForeignKey("EXCEED.FurnitureJobOrderProduction", t => t.FurnitureJobOrderProductionId, cascadeDelete: true)
                .Index(t => t.FurnitureJobOrderProductionId);
            
            CreateTable(
                "EXCEED.FurnitureProductionBillOfMaterial",
                c => new
                    {
                        FurnitureProductionBillOfMaterialId = c.Int(nullable: false, identity: true),
                        ManufacturingMaterialCategoryId = c.Int(nullable: false),
                        ManufacturingMaterialCategoryItemId = c.Int(nullable: false),
                        ItemCode = c.String(),
                        UnitOfMeasurement = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        WorkshopType = c.Int(nullable: false),
                        FurnitureJobOrderProductionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FurnitureProductionBillOfMaterialId)
                .ForeignKey("EXCEED.FurnitureJobOrderProduction", t => t.FurnitureJobOrderProductionId, cascadeDelete: true)
                .Index(t => t.FurnitureJobOrderProductionId);
            
            AddColumn("Manufacturing.FurnitureCostEstimations", "IsCurrent", c => c.Boolean(nullable: false));
            AddColumn("Manufacturing.FurnitureJobOrderItem", "JobNo", c => c.String());
            AddColumn("Manufacturing.FurnitureProformaInvoiceItem", "FurnitureEstimationId", c => c.Int(nullable: false));
            AddColumn("Manufacturing.FurnitureProformaInvoiceItem", "IsActive", c => c.Boolean(nullable: false));
            DropColumn("EXCEED.FurnitureJobOrderForm", "JobNo");
            DropColumn("Manufacturing.FurnitureProformaInvoice", "FurnitureEstimationId");
        }
        
        public override void Down()
        {
            AddColumn("Manufacturing.FurnitureProformaInvoice", "FurnitureEstimationId", c => c.Int(nullable: false));
            AddColumn("EXCEED.FurnitureJobOrderForm", "JobNo", c => c.String());
            DropForeignKey("EXCEED.FurnitureProductionBillOfMaterial", "FurnitureJobOrderProductionId", "EXCEED.FurnitureJobOrderProduction");
            DropForeignKey("EXCEED.FurnitureProductionBillOfLabor", "FurnitureJobOrderProductionId", "EXCEED.FurnitureJobOrderProduction");
            DropForeignKey("EXCEED.FurnitureJobAssignment", "FurnitureJobOrderProductionId", "EXCEED.FurnitureJobOrderProduction");
            DropIndex("EXCEED.FurnitureProductionBillOfMaterial", new[] { "FurnitureJobOrderProductionId" });
            DropIndex("EXCEED.FurnitureProductionBillOfLabor", new[] { "FurnitureJobOrderProductionId" });
            DropIndex("EXCEED.FurnitureJobAssignment", new[] { "FurnitureJobOrderProductionId" });
            DropColumn("Manufacturing.FurnitureProformaInvoiceItem", "IsActive");
            DropColumn("Manufacturing.FurnitureProformaInvoiceItem", "FurnitureEstimationId");
            DropColumn("Manufacturing.FurnitureJobOrderItem", "JobNo");
            DropColumn("Manufacturing.FurnitureCostEstimations", "IsCurrent");
            DropTable("EXCEED.FurnitureProductionBillOfMaterial");
            DropTable("EXCEED.FurnitureProductionBillOfLabor");
            DropTable("EXCEED.FurnitureJobOrderProduction");
            DropTable("EXCEED.FurnitureJobAssignment");
        }
    }
}
