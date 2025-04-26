namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printingupdate : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "Manufacturing.FurnitureDailyLaborRates", newName: "FurnitureDailyLaborRate");
            RenameTable(name: "Manufacturing.FurnitureDeliveredJobs", newName: "FurnitureDeliveredJob");
            RenameTable(name: "Manufacturing.FurnitureJobCosts", newName: "FurnitureJobCost");
            RenameTable(name: "Manufacturing.FurnitureDLRHours", newName: "FurnitureDLRHour");
            RenameTable(name: "Manufacturing.FurnitureDLRSalaries", newName: "FurnitureDLRSalary");
            RenameTable(name: "Manufacturing.FurnitureFinishedJobs", newName: "FurnitureFinishedJob");
            RenameTable(name: "Manufacturing.FurnitureIdleTimeCategories", newName: "FurnitureIdleTimeCategory");
            RenameTable(name: "Manufacturing.FurnitureIdleTimeNonProductives", newName: "FurnitureIdleTimeNonProductiveControllable");
            RenameTable(name: "Manufacturing.FurnitureIdleTimeNonProductiveNonControllables", newName: "FurnitureIdleTimeNonProductiveNonControllable");
            RenameTable(name: "Manufacturing.FurnitureIdleTimeOTs", newName: "FurnitureIdleTimeOT");
            RenameTable(name: "Manufacturing.FurnitureIdleTimeProductives", newName: "FurnitureIdleTimeProductive");
            RenameTable(name: "Manufacturing.FurnitureIdleTimes", newName: "FurnitureIdleTime");
            RenameTable(name: "Manufacturing.FurnitureJcPensions", newName: "FurnitureJcPension");
            RenameTable(name: "Manufacturing.FurnitureJobChangeOfOrders", newName: "FurnitureJobChangeOfOrder");
            RenameTable(name: "Manufacturing.FurnitureJobLaborCosts", newName: "FurnitureJobLaborCost");
            RenameTable(name: "Manufacturing.FurnitureJobMaterialCosts", newName: "FurnitureJobMaterialCost");
            RenameTable(name: "Manufacturing.FurnitureJobMaterialReturns", newName: "FurnitureJobMaterialReturn");
            RenameTable(name: "Manufacturing.FurnitureYearlyOverHeadRates", newName: "FurnitureYearlyOverHeadRate");
            MoveTable(name: "Manufacturing.FurnitureDailyLaborRate", newSchema: "EXCEED");
            MoveTable(name: "Manufacturing.FurnitureDeliveredJob", newSchema: "EXCEED");
            MoveTable(name: "Manufacturing.FurnitureJobCost", newSchema: "EXCEED");
            MoveTable(name: "Manufacturing.FurnitureDLRHour", newSchema: "EXCEED");
            MoveTable(name: "Manufacturing.FurnitureDLRSalary", newSchema: "EXCEED");
            MoveTable(name: "Manufacturing.FurnitureFinishedJob", newSchema: "EXCEED");
            MoveTable(name: "Manufacturing.FurnitureIdleTimeCategory", newSchema: "EXCEED");
            MoveTable(name: "Manufacturing.FurnitureIdleTimeNonProductiveControllable", newSchema: "EXCEED");
            MoveTable(name: "Manufacturing.FurnitureIdleTimeNonProductiveNonControllable", newSchema: "EXCEED");
            MoveTable(name: "Manufacturing.FurnitureIdleTimeOT", newSchema: "EXCEED");
            MoveTable(name: "Manufacturing.FurnitureIdleTimeProductive", newSchema: "EXCEED");
            MoveTable(name: "Manufacturing.FurnitureIdleTime", newSchema: "EXCEED");
            MoveTable(name: "Manufacturing.FurnitureJcPension", newSchema: "EXCEED");
            MoveTable(name: "Manufacturing.FurnitureJobChangeOfOrder", newSchema: "EXCEED");
            MoveTable(name: "Manufacturing.FurnitureJobLaborCost", newSchema: "EXCEED");
            MoveTable(name: "Manufacturing.FurnitureJobMaterialCost", newSchema: "EXCEED");
            MoveTable(name: "Manufacturing.FurnitureJobMaterialReturn", newSchema: "EXCEED");
            MoveTable(name: "Manufacturing.FurnitureJobOrderSummaryCostCenterDetail", newSchema: "EXCEED");
            MoveTable(name: "Manufacturing.FurnitureJobOrderSummary", newSchema: "EXCEED");
            MoveTable(name: "Manufacturing.FurnitureJobOrderTimeRegistration", newSchema: "EXCEED");
            MoveTable(name: "Manufacturing.FurnitureJobOrderTimeSheet", newSchema: "EXCEED");
            MoveTable(name: "Manufacturing.FurnitureYearlyOverHeadRate", newSchema: "EXCEED");
            MoveTable(name: "Printing.PrintingJobType", newSchema: "EXCEED");
            MoveTable(name: "Printing.ProductDeliveryItems", newSchema: "EXCEED");
            DropTable("Printing.Settings");
        }
        
        public override void Down()
        {
            CreateTable(
                "Printing.Settings",
                c => new
                    {
                        SettingsId = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(nullable: false, maxLength: 150),
                        CompanyAddress = c.String(nullable: false, maxLength: 15),
                        CompanyPhone = c.String(nullable: false, maxLength: 15),
                        CompanyEmail = c.String(nullable: false, maxLength: 50),
                        CompanyMail = c.String(nullable: false, maxLength: 50),
                        CompanyVatNo = c.String(nullable: false, maxLength: 50),
                        CompanyTIN = c.String(nullable: false, maxLength: 50),
                        CompanyLogo = c.Binary(nullable: false),
                        CompanyScreen = c.Binary(nullable: false),
                        SMSSenderStatus = c.String(nullable: false, maxLength: 15),
                        EditGrace = c.Int(nullable: false),
                        FiscalYearStart = c.DateTime(nullable: false),
                        DashboardCalendar = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.SettingsId);
            
            MoveTable(name: "EXCEED.ProductDeliveryItems", newSchema: "Printing");
            MoveTable(name: "EXCEED.PrintingJobType", newSchema: "Printing");
            MoveTable(name: "EXCEED.FurnitureYearlyOverHeadRate", newSchema: "Manufacturing");
            MoveTable(name: "EXCEED.FurnitureJobOrderTimeSheet", newSchema: "Manufacturing");
            MoveTable(name: "EXCEED.FurnitureJobOrderTimeRegistration", newSchema: "Manufacturing");
            MoveTable(name: "EXCEED.FurnitureJobOrderSummary", newSchema: "Manufacturing");
            MoveTable(name: "EXCEED.FurnitureJobOrderSummaryCostCenterDetail", newSchema: "Manufacturing");
            MoveTable(name: "EXCEED.FurnitureJobMaterialReturn", newSchema: "Manufacturing");
            MoveTable(name: "EXCEED.FurnitureJobMaterialCost", newSchema: "Manufacturing");
            MoveTable(name: "EXCEED.FurnitureJobLaborCost", newSchema: "Manufacturing");
            MoveTable(name: "EXCEED.FurnitureJobChangeOfOrder", newSchema: "Manufacturing");
            MoveTable(name: "EXCEED.FurnitureJcPension", newSchema: "Manufacturing");
            MoveTable(name: "EXCEED.FurnitureIdleTime", newSchema: "Manufacturing");
            MoveTable(name: "EXCEED.FurnitureIdleTimeProductive", newSchema: "Manufacturing");
            MoveTable(name: "EXCEED.FurnitureIdleTimeOT", newSchema: "Manufacturing");
            MoveTable(name: "EXCEED.FurnitureIdleTimeNonProductiveNonControllable", newSchema: "Manufacturing");
            MoveTable(name: "EXCEED.FurnitureIdleTimeNonProductiveControllable", newSchema: "Manufacturing");
            MoveTable(name: "EXCEED.FurnitureIdleTimeCategory", newSchema: "Manufacturing");
            MoveTable(name: "EXCEED.FurnitureFinishedJob", newSchema: "Manufacturing");
            MoveTable(name: "EXCEED.FurnitureDLRSalary", newSchema: "Manufacturing");
            MoveTable(name: "EXCEED.FurnitureDLRHour", newSchema: "Manufacturing");
            MoveTable(name: "EXCEED.FurnitureJobCost", newSchema: "Manufacturing");
            MoveTable(name: "EXCEED.FurnitureDeliveredJob", newSchema: "Manufacturing");
            MoveTable(name: "EXCEED.FurnitureDailyLaborRate", newSchema: "Manufacturing");
            RenameTable(name: "Manufacturing.FurnitureYearlyOverHeadRate", newName: "FurnitureYearlyOverHeadRates");
            RenameTable(name: "Manufacturing.FurnitureJobMaterialReturn", newName: "FurnitureJobMaterialReturns");
            RenameTable(name: "Manufacturing.FurnitureJobMaterialCost", newName: "FurnitureJobMaterialCosts");
            RenameTable(name: "Manufacturing.FurnitureJobLaborCost", newName: "FurnitureJobLaborCosts");
            RenameTable(name: "Manufacturing.FurnitureJobChangeOfOrder", newName: "FurnitureJobChangeOfOrders");
            RenameTable(name: "Manufacturing.FurnitureJcPension", newName: "FurnitureJcPensions");
            RenameTable(name: "Manufacturing.FurnitureIdleTime", newName: "FurnitureIdleTimes");
            RenameTable(name: "Manufacturing.FurnitureIdleTimeProductive", newName: "FurnitureIdleTimeProductives");
            RenameTable(name: "Manufacturing.FurnitureIdleTimeOT", newName: "FurnitureIdleTimeOTs");
            RenameTable(name: "Manufacturing.FurnitureIdleTimeNonProductiveNonControllable", newName: "FurnitureIdleTimeNonProductiveNonControllables");
            RenameTable(name: "Manufacturing.FurnitureIdleTimeNonProductiveControllable", newName: "FurnitureIdleTimeNonProductives");
            RenameTable(name: "Manufacturing.FurnitureIdleTimeCategory", newName: "FurnitureIdleTimeCategories");
            RenameTable(name: "Manufacturing.FurnitureFinishedJob", newName: "FurnitureFinishedJobs");
            RenameTable(name: "Manufacturing.FurnitureDLRSalary", newName: "FurnitureDLRSalaries");
            RenameTable(name: "Manufacturing.FurnitureDLRHour", newName: "FurnitureDLRHours");
            RenameTable(name: "Manufacturing.FurnitureJobCost", newName: "FurnitureJobCosts");
            RenameTable(name: "Manufacturing.FurnitureDeliveredJob", newName: "FurnitureDeliveredJobs");
            RenameTable(name: "Manufacturing.FurnitureDailyLaborRate", newName: "FurnitureDailyLaborRates");
        }
    }
}
