namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nightlybuld : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FinanceSetting", "RetainedEarningAccount", c => c.String(nullable: false));
            AddColumn("EXCEED.FinanceSetting", "RetentionApAccount", c => c.String(nullable: false));
            AddColumn("EXCEED.FinanceSetting", "RetentionArAccount", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.FinanceSetting", "RetentionArAccount");
            DropColumn("EXCEED.FinanceSetting", "RetentionApAccount");
            DropColumn("EXCEED.FinanceSetting", "RetainedEarningAccount");
        }
    }
}
