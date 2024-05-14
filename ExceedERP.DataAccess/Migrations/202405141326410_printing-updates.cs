namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printingupdates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Printing.PrintCostCenter",
                c => new
                    {
                        PrintCostCenterId = c.Int(nullable: false, identity: true),
                        PrintCostCenterCode = c.String(),
                        PrintCostCenterName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PrintCostCenterId);
            
            CreateTable(
                "Printing.PrintingMachineList",
                c => new
                    {
                        PrintingMachineListId = c.Int(nullable: false, identity: true),
                        MachineTypeCode = c.String(),
                        PrintingCostCenterId = c.Int(nullable: false),
                        PaperFeedStyle = c.String(),
                        NumberUnits = c.Int(nullable: false),
                        MachineTypeName = c.String(),
                        PrintingHourlyChargeRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MakereadyHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MachineStatus = c.String(),
                        NumberOfColors = c.Int(nullable: false),
                        OutputRate = c.Double(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PrintingMachineListId);
            
            CreateTable(
                "Printing.PrintingProcess",
                c => new
                    {
                        PrintingProcessId = c.Int(nullable: false, identity: true),
                        PrintingCategory = c.Int(nullable: false),
                        ProcessCode = c.String(),
                        ProcessName = c.String(),
                        HoursRequired = c.Double(nullable: false),
                        Quantity = c.Int(nullable: false),
                        ProcessScope = c.String(),
                        SetupCost = c.Double(nullable: false),
                        CostPerItem = c.Double(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PrintingProcessId);
            
            CreateTable(
                "Printing.PrintItemCategory",
                c => new
                    {
                        PrintItemCategoryId = c.Int(nullable: false, identity: true),
                        InventoryCategoryCode = c.String(),
                        PrintCategory = c.String(),
                        CostAccountCode = c.String(),
                        InventoryAccountCode = c.String(),
                        ProductionAccountCode = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.PrintItemCategoryId);
            
            CreateTable(
                "Printing.PrintLabourRate",
                c => new
                    {
                        PrintLaborRateId = c.Int(nullable: false, identity: true),
                        PeriodCode = c.String(),
                        PeriodName = c.String(),
                        NormalHourRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverTimeHourRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverHeadRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PrintLaborRateId);
            
            CreateTable(
                "Printing.PrintMachinePaperList",
                c => new
                    {
                        PrintMachinePaperListId = c.Int(nullable: false, identity: true),
                        PrintMachineTypeId = c.Int(nullable: false),
                        PaperSizeCode = c.String(),
                        Width = c.Double(nullable: false),
                        Length = c.Double(nullable: false),
                        PaperName = c.String(),
                        NumberOfPagesPerSheet = c.Int(nullable: false),
                        NumberOfPagesPerPlate = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PrintMachinePaperListId);
            
            CreateTable(
                "Printing.PrintWorkOrderType",
                c => new
                    {
                        PrintWorkOrderTypeId = c.Int(nullable: false, identity: true),
                        PrintWorkOrderTypeCode = c.String(),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PrintWorkOrderTypeId);
            
        }
        
        public override void Down()
        {
            DropTable("Printing.PrintWorkOrderType");
            DropTable("Printing.PrintMachinePaperList");
            DropTable("Printing.PrintLabourRate");
            DropTable("Printing.PrintItemCategory");
            DropTable("Printing.PrintingProcess");
            DropTable("Printing.PrintingMachineList");
            DropTable("Printing.PrintCostCenter");
        }
    }
}
