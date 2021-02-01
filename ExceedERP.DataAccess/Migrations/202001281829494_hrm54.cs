namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm54 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Performance.EmployeePerformanceScore", "IsDepartmentLevel", c => c.Boolean(nullable: false));
            AddColumn("Performance.EmployeePerformanceScore", "IsApproved", c => c.Boolean(nullable: false));
            AlterColumn("Performance.EmployeePerformanceScore", "MeasurementDefinitionId", c => c.Int());
            AlterColumn("Performance.EmployeePerformanceScore", "MeasurementId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("Performance.EmployeePerformanceScore", "MeasurementId", c => c.Int(nullable: false));
            AlterColumn("Performance.EmployeePerformanceScore", "MeasurementDefinitionId", c => c.Int(nullable: false));
            DropColumn("Performance.EmployeePerformanceScore", "IsApproved");
            DropColumn("Performance.EmployeePerformanceScore", "IsDepartmentLevel");
        }
    }
}
