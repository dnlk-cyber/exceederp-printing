namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm17 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.LateComingSummary", "Check", c => c.Boolean(nullable: false));
            AddColumn("HRM.LateComingSummary", "Processed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.LateComingSummary", "Processed");
            DropColumn("HRM.LateComingSummary", "Check");
        }
    }
}
