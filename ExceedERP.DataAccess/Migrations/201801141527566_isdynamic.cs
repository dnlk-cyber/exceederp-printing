namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class isdynamic : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GlRuleValue", "DynamicCoA", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.GlRuleValue", "DynamicCoA");
        }
    }
}
