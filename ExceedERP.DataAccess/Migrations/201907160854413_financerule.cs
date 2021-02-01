namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class financerule : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GlRuleValue", "CalculateProfitTax", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.GlRuleValue", "CalculateProfitTax");
        }
    }
}
