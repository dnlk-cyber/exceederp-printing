namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JobOrderAddToJobStatus : DbMigration
    {
        public override void Up()
        {
            CreateIndex("ProductFollowUp.JobStatuses", "JobId");
            AddForeignKey("ProductFollowUp.JobStatuses", "JobId", "ProductFollowUp.Job", "JobId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("ProductFollowUp.JobStatuses", "JobId", "ProductFollowUp.Job");
            DropIndex("ProductFollowUp.JobStatuses", new[] { "JobId" });
        }
    }
}
