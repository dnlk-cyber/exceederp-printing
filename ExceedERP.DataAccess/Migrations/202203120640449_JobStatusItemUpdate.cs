namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JobStatusItemUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("ProductFollowUp.JobStatusLineItems", "RemainigQuantiy", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("ProductFollowUp.JobStatusLineItems", "TotalQuantiy", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("ProductFollowUp.JobStatusLineItems", "TotalQuantiy");
            DropColumn("ProductFollowUp.JobStatusLineItems", "RemainigQuantiy");
        }
    }
}
