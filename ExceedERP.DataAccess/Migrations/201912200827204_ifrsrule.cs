namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ifrsrule : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.IfrsSocfFinancingActivity", "Rule", c => c.Int(nullable: false));
            AddColumn("EXCEED.IfrsSocfInvestingActivity", "Rule", c => c.Int(nullable: false));
            AddColumn("EXCEED.IfrsSocfOperatingActivity", "Rule", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.IfrsSocfOperatingActivity", "Rule");
            DropColumn("EXCEED.IfrsSocfInvestingActivity", "Rule");
            DropColumn("EXCEED.IfrsSocfFinancingActivity", "Rule");
        }
    }
}
