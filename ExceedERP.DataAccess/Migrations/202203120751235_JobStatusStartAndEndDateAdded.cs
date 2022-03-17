namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JobStatusStartAndEndDateAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("ProductFollowUp.JobStatusEntityTypeConfiguration", "StartDate", c => c.DateTime(nullable: false));
            AddColumn("ProductFollowUp.JobStatusEntityTypeConfiguration", "EndDate", c => c.DateTime());
            DropColumn("ProductFollowUp.JobStatusEntityTypeConfiguration", "Date");
        }
        
        public override void Down()
        {
            AddColumn("ProductFollowUp.JobStatusEntityTypeConfiguration", "Date", c => c.DateTime(nullable: false));
            DropColumn("ProductFollowUp.JobStatusEntityTypeConfiguration", "EndDate");
            DropColumn("ProductFollowUp.JobStatusEntityTypeConfiguration", "StartDate");
        }
    }
}
