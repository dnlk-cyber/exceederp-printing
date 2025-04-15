namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class costingmerge : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "Printing.DailyLaborRates", newSchema: "JobCosting");
            MoveTable(name: "Printing.DeliveredJobs", newSchema: "JobCosting");
            MoveTable(name: "Printing.DLRHours", newSchema: "JobCosting");
            MoveTable(name: "Printing.DLRSalaries", newSchema: "JobCosting");
            MoveTable(name: "Printing.FinishedJobs", newSchema: "JobCosting");
            MoveTable(name: "Printing.IdleTimes", newSchema: "JobCosting");
            MoveTable(name: "Printing.IdleTimeNonProductives", newSchema: "JobCosting");
            MoveTable(name: "Printing.IdleTimeOTs", newSchema: "JobCosting");
            MoveTable(name: "Printing.IdleTimeProductives", newSchema: "JobCosting");
            MoveTable(name: "Printing.IdleTimeCategories", newSchema: "JobCosting");
            MoveTable(name: "Printing.JcPensions", newSchema: "JobCosting");
            MoveTable(name: "Printing.JobChangeOfOrders", newSchema: "JobCosting");
            MoveTable(name: "Printing.JobCosts", newSchema: "JobCosting");
            MoveTable(name: "Printing.JobLaborCosts", newSchema: "JobCosting");
            MoveTable(name: "Printing.JobMaterialCosts", newSchema: "JobCosting");
            MoveTable(name: "Printing.JobMaterialReturns", newSchema: "JobCosting");
            MoveTable(name: "Printing.JobOrderSummaryCostCenterDetail", newSchema: "JobCosting");
            MoveTable(name: "Printing.JobOrderSummary", newSchema: "JobCosting");
            MoveTable(name: "Printing.JobOrderTimeRegistration", newSchema: "JobCosting");
            MoveTable(name: "Printing.JobOrderTimeSheet", newSchema: "JobCosting");
            MoveTable(name: "Printing.Machines", newSchema: "JobCosting");
            MoveTable(name: "Printing.YearlyOverHeadRates", newSchema: "JobCosting");
            DropForeignKey("Printing.IdleTimeNonProductiveNonControllables", "IdleTimeID", "Printing.IdleTimes");
            DropIndex("Printing.IdleTimeNonProductiveNonControllables", new[] { "IdleTimeID" });
            CreateTable(
                "JobCosting.IdleTimeNonProductiveNonControllables",
                c => new
                    {
                        IdleTimeNonProductiveNonControllableID = c.Int(nullable: false, identity: true),
                        IdleTimeID = c.Int(nullable: false),
                        IdleTimeCategoryId = c.String(nullable: false),
                        CostCenterId = c.Int(nullable: false),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.IdleTimeNonProductiveNonControllableID)
                .ForeignKey("JobCosting.IdleTimes", t => t.IdleTimeID, cascadeDelete: true)
                .Index(t => t.IdleTimeID);
            
            DropColumn("JobCosting.JobOrderTimeSheet", "FurnitureJobOrderTimeRegistrationId");
            DropTable("Printing.IdleTimeNonProductiveNonControllables");
        }
        
        public override void Down()
        {
            CreateTable(
                "Printing.IdleTimeNonProductiveNonControllables",
                c => new
                    {
                        PrintingIdleTimeNonProductiveNonControllableID = c.Int(nullable: false, identity: true),
                        IdleTimeID = c.Int(nullable: false),
                        IdleTimeCategoryId = c.String(nullable: false),
                        CostCenterId = c.Int(nullable: false),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.PrintingIdleTimeNonProductiveNonControllableID);
            
            AddColumn("JobCosting.JobOrderTimeSheet", "FurnitureJobOrderTimeRegistrationId", c => c.Int(nullable: false));
            DropForeignKey("JobCosting.IdleTimeNonProductiveNonControllables", "IdleTimeID", "JobCosting.IdleTimes");
            DropIndex("JobCosting.IdleTimeNonProductiveNonControllables", new[] { "IdleTimeID" });
            DropTable("JobCosting.IdleTimeNonProductiveNonControllables");
            CreateIndex("Printing.IdleTimeNonProductiveNonControllables", "IdleTimeID");
            AddForeignKey("Printing.IdleTimeNonProductiveNonControllables", "IdleTimeID", "Printing.IdleTimes", "IdleTimeID", cascadeDelete: true);
            MoveTable(name: "JobCosting.YearlyOverHeadRates", newSchema: "Printing");
            MoveTable(name: "JobCosting.Machines", newSchema: "Printing");
            MoveTable(name: "JobCosting.JobOrderTimeSheet", newSchema: "Printing");
            MoveTable(name: "JobCosting.JobOrderTimeRegistration", newSchema: "Printing");
            MoveTable(name: "JobCosting.JobOrderSummary", newSchema: "Printing");
            MoveTable(name: "JobCosting.JobOrderSummaryCostCenterDetail", newSchema: "Printing");
            MoveTable(name: "JobCosting.JobMaterialReturns", newSchema: "Printing");
            MoveTable(name: "JobCosting.JobMaterialCosts", newSchema: "Printing");
            MoveTable(name: "JobCosting.JobLaborCosts", newSchema: "Printing");
            MoveTable(name: "JobCosting.JobCosts", newSchema: "Printing");
            MoveTable(name: "JobCosting.JobChangeOfOrders", newSchema: "Printing");
            MoveTable(name: "JobCosting.JcPensions", newSchema: "Printing");
            MoveTable(name: "JobCosting.IdleTimeCategories", newSchema: "Printing");
            MoveTable(name: "JobCosting.IdleTimeProductives", newSchema: "Printing");
            MoveTable(name: "JobCosting.IdleTimeOTs", newSchema: "Printing");
            MoveTable(name: "JobCosting.IdleTimeNonProductives", newSchema: "Printing");
            MoveTable(name: "JobCosting.IdleTimes", newSchema: "Printing");
            MoveTable(name: "JobCosting.FinishedJobs", newSchema: "Printing");
            MoveTable(name: "JobCosting.DLRSalaries", newSchema: "Printing");
            MoveTable(name: "JobCosting.DLRHours", newSchema: "Printing");
            MoveTable(name: "JobCosting.DeliveredJobs", newSchema: "Printing");
            MoveTable(name: "JobCosting.DailyLaborRates", newSchema: "Printing");
        }
    }
}
