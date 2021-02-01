namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class itemlocation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.GlRetainedEarningAccount",
                c => new
                    {
                        GlRetainedEarningAccountId = c.Int(nullable: false, identity: true),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Type = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GlRetainedEarningAccountId);
            
            CreateTable(
                "Construction.PurchasePriceComparison",
                c => new
                    {
                        PurchasePriceComparisonId = c.Int(nullable: false, identity: true),
                        ProjectInformationId = c.Int(nullable: false),
                        ItemName = c.String(),
                        UnitOfMeasurementId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SelectedPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SelectedSupplier = c.String(),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VatAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchasePriceComparisonId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.PurchaseSupplier",
                c => new
                    {
                        PurchaseSupplierId = c.Int(nullable: false, identity: true),
                        PurchasePriceComparisonId = c.Int(nullable: false),
                        SupplierName = c.String(),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsSelectedSupplier = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchaseSupplierId)
                .ForeignKey("Construction.PurchasePriceComparison", t => t.PurchasePriceComparisonId, cascadeDelete: true)
                .Index(t => t.PurchasePriceComparisonId);
            
            AddColumn("EXCEED.StoreItemAssignment", "ItemLocation", c => c.String());
            AddColumn("Construction.MachinaryRent", "ApprovedCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.WorkItemPerProject", "TotalQuantityOfWork", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.WorkItemPerProject", "TotalUnitCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ProjectMaterial", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ProjectMaterial", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ProjectMaterial", "TotalCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EstimationCustomWorkItemContent", "QuantityPerUnit", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.EstimationCustomWorkItemContent", "UnitCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ProjectEstimnWorkItem", "TotalUnitCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ProjectEstmnDirectLabour", "UnitFactory", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ProjectEstmnEquipment", "UnitFactory", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.MachinaryRent", "Rental", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.MachinaryRent", "RentalWithFuel", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.MaterialPriceIndex", "TotalCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.MaterialPriceIndex", "ApprovedCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ResourceUtilizationDirectLabour", "UnitFactory", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ResourceUtilizationDirectLabour", "IndexedHourlySalary", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ResourceUtilizationDirectLabour", "TotalCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ResourceUtilizationWorkItem", "DirectContractUnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ResourceUtilizationWorkItem", "Income", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ResourceUtilizationWorkItem", "MaterialCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ResourceUtilizationWorkItem", "EquipmentCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ResourceUtilizationWorkItem", "LaborCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ResourceUtilizationWorkItem", "TotalDirectCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ResourceUtilizationEquipment", "UnitFactory", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ResourceUtilizationEquipment", "RentalCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ResourceUtilizationEquipment", "TotalCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ResourceUtilizationMaterial", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ResourceUtilizationMaterial", "TotalCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            DropForeignKey("Construction.PurchaseSupplier", "PurchasePriceComparisonId", "Construction.PurchasePriceComparison");
            DropForeignKey("Construction.PurchasePriceComparison", "ProjectInformationId", "Construction.ProjectInformation");
            DropIndex("Construction.PurchaseSupplier", new[] { "PurchasePriceComparisonId" });
            DropIndex("Construction.PurchasePriceComparison", new[] { "ProjectInformationId" });
            AlterColumn("Construction.ResourceUtilizationMaterial", "TotalCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ResourceUtilizationMaterial", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ResourceUtilizationEquipment", "TotalCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ResourceUtilizationEquipment", "RentalCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ResourceUtilizationEquipment", "UnitFactory", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ResourceUtilizationWorkItem", "TotalDirectCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ResourceUtilizationWorkItem", "LaborCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ResourceUtilizationWorkItem", "EquipmentCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ResourceUtilizationWorkItem", "MaterialCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ResourceUtilizationWorkItem", "Income", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ResourceUtilizationWorkItem", "DirectContractUnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ResourceUtilizationDirectLabour", "TotalCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ResourceUtilizationDirectLabour", "IndexedHourlySalary", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ResourceUtilizationDirectLabour", "UnitFactory", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.MaterialPriceIndex", "ApprovedCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.MaterialPriceIndex", "TotalCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.MachinaryRent", "RentalWithFuel", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.MachinaryRent", "Rental", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ProjectEstmnEquipment", "UnitFactory", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ProjectEstmnDirectLabour", "UnitFactory", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ProjectEstimnWorkItem", "TotalUnitCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EstimationCustomWorkItemContent", "UnitCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.EstimationCustomWorkItemContent", "QuantityPerUnit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ProjectMaterial", "TotalCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ProjectMaterial", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ProjectMaterial", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.WorkItemPerProject", "TotalUnitCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.WorkItemPerProject", "TotalQuantityOfWork", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("Construction.MachinaryRent", "ApprovedCost");
            DropColumn("EXCEED.StoreItemAssignment", "ItemLocation");
            DropTable("Construction.PurchaseSupplier");
            DropTable("Construction.PurchasePriceComparison");
            DropTable("EXCEED.GlRetainedEarningAccount");
        }
    }
}
