namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JobStatusDateNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("ProductFollowUp.JobStatusEntityTypeConfiguration", "StartDate", c => c.DateTime());
            AlterColumn("ProductFollowUp.JobStatusEntityTypeConfiguration", "EndDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("ProductFollowUp.JobStatusEntityTypeConfiguration", "EndDate", c => c.DateTime(nullable: false));
            AlterColumn("ProductFollowUp.JobStatusEntityTypeConfiguration", "StartDate", c => c.DateTime(nullable: false));
        }
    }
}
