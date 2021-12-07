namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrmupdates : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Performance.EmployeeMeasurement", "EmployeeId", "HRM.Employee");
            DropForeignKey("Performance.EmployeeMeasurement", "MeasurementId", "Performance.Measurement");
            DropForeignKey("Performance.EmployeeMeasurement", "MeasurementDefinitionId", "Performance.MeasurementDefinition");
            DropForeignKey("Performance.EmployeeMeasurement", "QuarterId", "HRM.Quarter");
            DropIndex("Performance.EmployeeMeasurement", new[] { "MeasurementDefinitionId" });
            DropIndex("Performance.EmployeeMeasurement", new[] { "MeasurementId" });
            DropIndex("Performance.EmployeeMeasurement", new[] { "EmployeeId" });
            DropIndex("Performance.EmployeeMeasurement", new[] { "QuarterId" });
            CreateTable(
                "Performance.EvaluationCriteriaActionType",
                c => new
                    {
                        EvaluationCriteriaActionTypeId = c.Int(nullable: false, identity: true),
                        PerformanceCriteriaActionType = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.EvaluationCriteriaActionTypeId);
            
            AddColumn("Performance.EmployeeMeasurement", "EmployeePerformanceId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeeMeasurement", "MeasurmentId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeeMeasurement", "Weight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Performance.EmployeePerformance", "GlFiscalYearId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeePerformance", "GlPeriodId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeePerformance", "EvaluationCriteriaTypeId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeePerformance", "IsApproved", c => c.Boolean(nullable: false));
            AddColumn("Performance.EmployeePerformanceScore", "EmployeeMeasurementId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeePerformanceScore", "PerformanceActionId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeePerformanceScore", "Weight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Performance.EmployeePerformanceScore", "Score", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("Performance.EmployeeMeasurement", "MeasurementDefinitionId");
            DropColumn("Performance.EmployeeMeasurement", "MeasurementId");
            DropColumn("Performance.EmployeeMeasurement", "EmployeeId");
            DropColumn("Performance.EmployeeMeasurement", "EmpMesPercentageofWeight");
            DropColumn("Performance.EmployeeMeasurement", "QuarterId");
            DropColumn("Performance.EmployeePerformanceScore", "MeasurementDefinitionId");
            DropColumn("Performance.EmployeePerformanceScore", "MeasurementId");
            DropColumn("Performance.EmployeePerformanceScore", "ActionId");
            DropColumn("Performance.EmployeePerformanceScore", "EmployeeId");
            DropColumn("Performance.EmployeePerformanceScore", "EmpPerfPercentageofWeight");
            DropColumn("Performance.EmployeePerformanceScore", "Result");
            DropColumn("Performance.EmployeePerformanceScore", "QuarterId");
            DropColumn("Performance.EmployeePerformanceScore", "FiscalYearId");
            DropColumn("Performance.EmployeePerformanceScore", "IsDepartmentLevel");
            DropColumn("Performance.EmployeePerformanceScore", "IsApproved");
        }
        
        public override void Down()
        {
            AddColumn("Performance.EmployeePerformanceScore", "IsApproved", c => c.Boolean(nullable: false));
            AddColumn("Performance.EmployeePerformanceScore", "IsDepartmentLevel", c => c.Boolean(nullable: false));
            AddColumn("Performance.EmployeePerformanceScore", "FiscalYearId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeePerformanceScore", "QuarterId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeePerformanceScore", "Result", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Performance.EmployeePerformanceScore", "EmpPerfPercentageofWeight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Performance.EmployeePerformanceScore", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeePerformanceScore", "ActionId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeePerformanceScore", "MeasurementId", c => c.Int());
            AddColumn("Performance.EmployeePerformanceScore", "MeasurementDefinitionId", c => c.Int());
            AddColumn("Performance.EmployeeMeasurement", "QuarterId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeeMeasurement", "EmpMesPercentageofWeight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Performance.EmployeeMeasurement", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeeMeasurement", "MeasurementId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeeMeasurement", "MeasurementDefinitionId", c => c.Int(nullable: false));
            DropColumn("Performance.EmployeePerformanceScore", "Score");
            DropColumn("Performance.EmployeePerformanceScore", "Weight");
            DropColumn("Performance.EmployeePerformanceScore", "PerformanceActionId");
            DropColumn("Performance.EmployeePerformanceScore", "EmployeeMeasurementId");
            DropColumn("Performance.EmployeePerformance", "IsApproved");
            DropColumn("Performance.EmployeePerformance", "EvaluationCriteriaTypeId");
            DropColumn("Performance.EmployeePerformance", "GlPeriodId");
            DropColumn("Performance.EmployeePerformance", "GlFiscalYearId");
            DropColumn("Performance.EmployeeMeasurement", "Weight");
            DropColumn("Performance.EmployeeMeasurement", "MeasurmentId");
            DropColumn("Performance.EmployeeMeasurement", "EmployeePerformanceId");
            DropTable("Performance.EvaluationCriteriaActionType");
            CreateIndex("Performance.EmployeeMeasurement", "QuarterId");
            CreateIndex("Performance.EmployeeMeasurement", "EmployeeId");
            CreateIndex("Performance.EmployeeMeasurement", "MeasurementId");
            CreateIndex("Performance.EmployeeMeasurement", "MeasurementDefinitionId");
            AddForeignKey("Performance.EmployeeMeasurement", "QuarterId", "HRM.Quarter", "QuarterId", cascadeDelete: true);
            AddForeignKey("Performance.EmployeeMeasurement", "MeasurementDefinitionId", "Performance.MeasurementDefinition", "MeasurementDefinitionId", cascadeDelete: true);
            AddForeignKey("Performance.EmployeeMeasurement", "MeasurementId", "Performance.Measurement", "MeasurementId", cascadeDelete: true);
            AddForeignKey("Performance.EmployeeMeasurement", "EmployeeId", "HRM.Employee", "EmployeeId", cascadeDelete: true);
        }
    }
}
