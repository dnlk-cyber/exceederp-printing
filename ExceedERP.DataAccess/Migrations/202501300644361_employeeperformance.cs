namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class employeeperformance : DbMigration
    {
        public override void Up()
        {
            AddColumn("Performance.EmployeePerformanceSummary", "BranchId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeePerformanceSummary", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeePerformanceSummary", "OrgStructureId", c => c.Int(nullable: false));
            AlterColumn("Performance.MeasurementDefinition", "Result", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("Performance.MeasurementDefinition", "EmployeeId", c => c.Int());
            AlterColumn("Performance.MeasurementDefinition", "OrgStuructureId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("Performance.MeasurementDefinition", "OrgStuructureId", c => c.Int(nullable: false));
            AlterColumn("Performance.MeasurementDefinition", "EmployeeId", c => c.Int(nullable: false));
            AlterColumn("Performance.MeasurementDefinition", "Result", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("Performance.EmployeePerformanceSummary", "OrgStructureId");
            DropColumn("Performance.EmployeePerformanceSummary", "BusinessUnitId");
            DropColumn("Performance.EmployeePerformanceSummary", "BranchId");
        }
    }
}
