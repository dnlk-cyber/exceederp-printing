namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JobStatusItemUpdateNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("ProductFollowUp.JobStatusLineItems", "RemainigQuantiy", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("ProductFollowUp.JobStatusLineItems", "TotalQuantiy", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("ProductFollowUp.JobStatusLineItems", "TotalQuantiy", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("ProductFollowUp.JobStatusLineItems", "RemainigQuantiy", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
