namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printingmachinepaper : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "Printing.PrintingMachineList", newSchema: "EXCEED");
            CreateTable(
                "Printing.PrintingMachineType",
                c => new
                    {
                        PrintingMachineTypeId = c.Int(nullable: false, identity: true),
                        MachineTypeName = c.String(),
                        NumberOfColors = c.Int(nullable: false),
                        OutputRate = c.Double(nullable: false),
                        PaperFeedStyle = c.String(),
                        NumberUnits = c.Int(nullable: false),
                        PrintingHourlyChargeRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MakeReadyHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MachineStatus = c.String(),
                        Remark = c.String(),
                        PrintingCostCenterId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PrintingMachineTypeId);
            
            CreateTable(
                "Printing.PrintingPaperSize",
                c => new
                    {
                        PrintingPaperSizeId = c.Int(nullable: false, identity: true),
                        PaperSizeName = c.String(),
                        NoOfPagesPerSheet = c.Int(nullable: false),
                        NoOfPagesPerPlate = c.Int(nullable: false),
                        Width = c.Double(nullable: false),
                        Length = c.Double(nullable: false),
                        NoOfPagesToA1Ratio = c.Double(nullable: false),
                        Remark = c.String(),
                        PrintingMachineTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PrintingPaperSizeId);
            
            CreateTable(
                "Printing.PrintingProductProcess",
                c => new
                    {
                        PrintingProductProcessId = c.Int(nullable: false, identity: true),
                        JobTypeId = c.Int(nullable: false),
                        PrintingProcessCategory = c.Int(nullable: false),
                        ProcessName = c.String(),
                        HoursRequired = c.Double(nullable: false),
                        QtyToDone = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.PrintingProductProcessId);
            
            DropColumn("Printing.PrintingProcess", "ProcessCode");
        }
        
        public override void Down()
        {
            AddColumn("Printing.PrintingProcess", "ProcessCode", c => c.String());
            DropTable("Printing.PrintingProductProcess");
            DropTable("Printing.PrintingPaperSize");
            DropTable("Printing.PrintingMachineType");
            MoveTable(name: "EXCEED.PrintingMachineList", newSchema: "Printing");
        }
    }
}
