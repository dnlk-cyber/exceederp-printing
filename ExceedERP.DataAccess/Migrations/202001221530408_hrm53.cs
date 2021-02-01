namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm53 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Performance.EmployeePerformance", "EmployeeId", "HRM.Employee");
            DropForeignKey("Performance.EmployeePerformance", "LookupId", "Common.Lookup");
            DropIndex("Performance.EmployeePerformance", new[] { "EmployeeId" });
            DropIndex("Performance.EmployeePerformance", new[] { "LookupId" });
            CreateTable(
                "Performance.EmployeePerformanceScore",
                c => new
                    {
                        EmployeePerformanceScoreId = c.Int(nullable: false, identity: true),
                        MeasurementDefinitionId = c.Int(nullable: false),
                        MeasurementId = c.Int(nullable: false),
                        ActionId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        EmpPerfPercentageofWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Result = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QuarterId = c.Int(nullable: false),
                        FiscalYearId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.EmployeePerformanceScoreId);
            
            AddColumn("Performance.EmployeePerformance", "Weight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Performance.EmployeePerformance", "Result", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Performance.EmployeePerformance", "Remark", c => c.String());
            AlterColumn("Performance.PerformanceAction", "MeasurementDefinitionId", c => c.Int());
            AlterColumn("Performance.PerformanceAction", "MeasurementId", c => c.Int());
            DropColumn("Performance.EmployeePerformance", "LookupId");
        }
        
        public override void Down()
        {
            AddColumn("Performance.EmployeePerformance", "LookupId", c => c.Int(nullable: false));
            AlterColumn("Performance.PerformanceAction", "MeasurementId", c => c.Int(nullable: false));
            AlterColumn("Performance.PerformanceAction", "MeasurementDefinitionId", c => c.Int(nullable: false));
            DropColumn("Performance.EmployeePerformance", "Remark");
            DropColumn("Performance.EmployeePerformance", "Result");
            DropColumn("Performance.EmployeePerformance", "Weight");
            DropTable("Performance.EmployeePerformanceScore");
            CreateIndex("Performance.EmployeePerformance", "LookupId");
            CreateIndex("Performance.EmployeePerformance", "EmployeeId");
            AddForeignKey("Performance.EmployeePerformance", "LookupId", "Common.Lookup", "LookupId", cascadeDelete: true);
            AddForeignKey("Performance.EmployeePerformance", "EmployeeId", "HRM.Employee", "EmployeeId", cascadeDelete: true);
        }
    }
}
