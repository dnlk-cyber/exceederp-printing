namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JobStatusEndDateRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("ProductFollowUp.JobStatusEntityTypeConfiguration", "EndDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("ProductFollowUp.JobStatusEntityTypeConfiguration", "EndDate", c => c.DateTime());
        }
    }
}
