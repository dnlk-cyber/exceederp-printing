namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class jobfollow : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("ProductFollowUp.JobCategories", "JobCategorys_JobCategoryId", "ProductFollowUp.JobCategories");
            DropIndex("ProductFollowUp.JobCategories", new[] { "JobCategorys_JobCategoryId" });
            DropColumn("ProductFollowUp.JobCategories", "JobCategorys_JobCategoryId");
        }
        
        public override void Down()
        {
            AddColumn("ProductFollowUp.JobCategories", "JobCategorys_JobCategoryId", c => c.Int());
            CreateIndex("ProductFollowUp.JobCategories", "JobCategorys_JobCategoryId");
            AddForeignKey("ProductFollowUp.JobCategories", "JobCategorys_JobCategoryId", "ProductFollowUp.JobCategories", "JobCategoryId");
        }
    }
}
