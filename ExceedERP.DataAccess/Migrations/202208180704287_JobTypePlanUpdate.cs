namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JobTypePlanUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("ProductFollowUp.ProductionJobTypePlans", "JobType_JobCategoryId", c => c.Int());
            CreateIndex("ProductFollowUp.ProductionJobTypePlans", "JobType_JobCategoryId");
            AddForeignKey("ProductFollowUp.ProductionJobTypePlans", "JobType_JobCategoryId", "ProductFollowUp.JobCategories", "JobCategoryId");
        }
        
        public override void Down()
        {
            DropForeignKey("ProductFollowUp.ProductionJobTypePlans", "JobType_JobCategoryId", "ProductFollowUp.JobCategories");
            DropIndex("ProductFollowUp.ProductionJobTypePlans", new[] { "JobType_JobCategoryId" });
            DropColumn("ProductFollowUp.ProductionJobTypePlans", "JobType_JobCategoryId");
        }
    }
}
