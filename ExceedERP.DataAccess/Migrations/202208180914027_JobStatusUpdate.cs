namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JobStatusUpdate : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "ProductFollowUp.JobStatusEntityTypeConfiguration", newName: "JobStatuses");
            CreateIndex("ProductFollowUp.JobStatusLineItems", "JobStatusId");
            CreateIndex("ProductFollowUp.ProductionJobTypePlans", "WeeklyProductionPlanId");
            AddForeignKey("ProductFollowUp.JobStatusLineItems", "JobStatusId", "ProductFollowUp.JobStatuses", "JobStatusId", cascadeDelete: true);
            AddForeignKey("ProductFollowUp.ProductionJobTypePlans", "WeeklyProductionPlanId", "ProductFollowUp.WeeklyProductionPlans", "WeeklyProductionPlanId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("ProductFollowUp.ProductionJobTypePlans", "WeeklyProductionPlanId", "ProductFollowUp.WeeklyProductionPlans");
            DropForeignKey("ProductFollowUp.JobStatusLineItems", "JobStatusId", "ProductFollowUp.JobStatuses");
            DropIndex("ProductFollowUp.ProductionJobTypePlans", new[] { "WeeklyProductionPlanId" });
            DropIndex("ProductFollowUp.JobStatusLineItems", new[] { "JobStatusId" });
            RenameTable(name: "ProductFollowUp.JobStatuses", newName: "JobStatusEntityTypeConfiguration");
        }
    }
}
