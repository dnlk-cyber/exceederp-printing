namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class contractman3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FinanceSetting", "RetentionPercentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.FinanceSetting", "RetentionPercentage");
        }
    }
}
