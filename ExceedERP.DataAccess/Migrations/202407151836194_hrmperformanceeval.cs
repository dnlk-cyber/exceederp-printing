namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrmperformanceeval : DbMigration
    {
        public override void Up()
        {
            AddColumn("Performance.EmployeePerformance", "SecondLevelApproverId", c => c.Int());
            AddColumn("Performance.EmployeePerformance", "ReasonFor2ndRejection", c => c.String());
            AddColumn("Performance.EmployeePerformance", "IsSecondLevelRejected", c => c.Boolean(nullable: false));
            AddColumn("Performance.EmployeePerformance", "IsCurrent", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Performance.EmployeePerformance", "IsCurrent");
            DropColumn("Performance.EmployeePerformance", "IsSecondLevelRejected");
            DropColumn("Performance.EmployeePerformance", "ReasonFor2ndRejection");
            DropColumn("Performance.EmployeePerformance", "SecondLevelApproverId");
        }
    }
}
