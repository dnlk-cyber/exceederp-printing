namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrmergeermias : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.CourseTracking", "TrainingPlanId", c => c.Int(nullable: false));
            AddColumn("HRM.LeaveRequest", "DateCreated", c => c.DateTime());
            CreateIndex("HRM.CourseTracking", "TrainingPlanId");
            AddForeignKey("HRM.CourseTracking", "TrainingPlanId", "HRM.TrainingPlan", "TrainingPlanId", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("HRM.CourseTracking", "TrainingPlanId", "HRM.TrainingPlan");
            DropIndex("HRM.CourseTracking", new[] { "TrainingPlanId" });
            DropColumn("HRM.LeaveRequest", "DateCreated");
            DropColumn("HRM.CourseTracking", "TrainingPlanId");
        }
    }
}
