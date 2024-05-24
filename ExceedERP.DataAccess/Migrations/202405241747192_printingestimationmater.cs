namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printingestimationmater : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Printing.PrintingEstimationLaborCost",
                c => new
                    {
                        PrintingEstimationLaborCostId = c.Int(nullable: false, identity: true),
                        ProcessCategory = c.Int(nullable: false),
                        PrintingProcessId = c.Int(nullable: false),
                        EstimatedHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LaborRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PrintingCostEstimationId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PrintingEstimationLaborCostId);
            
            CreateTable(
                "Printing.PrintingEstimationMaterialCost",
                c => new
                    {
                        PrintingEstimationMaterialCostId = c.Int(nullable: false, identity: true),
                        PrintingMaterialCategoryId = c.Int(nullable: false),
                        PrintingMaterialCategoryItemId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitOfMeasurment = c.String(),
                        PrintingCostEstimationId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PrintingEstimationMaterialCostId);
            
            CreateTable(
                "Printing.PrintingOverAllCost",
                c => new
                    {
                        PrintingOverAllCostId = c.Int(nullable: false, identity: true),
                        MaterialCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LaborCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalProductionCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdminstrativeCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProfitMargin = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GraphicsCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalBeforeVAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GrandTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PrintingCostEstimationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PrintingOverAllCostId);
            
        }
        
        public override void Down()
        {
            DropTable("Printing.PrintingOverAllCost");
            DropTable("Printing.PrintingEstimationMaterialCost");
            DropTable("Printing.PrintingEstimationLaborCost");
        }
    }
}
