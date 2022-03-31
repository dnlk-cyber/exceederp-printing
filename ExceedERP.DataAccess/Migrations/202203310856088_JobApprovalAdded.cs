namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JobApprovalAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("ProductFollowUp.Job", "IsApproved", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("ProductFollowUp.Job", "IsApproved");
        }
    }
}
