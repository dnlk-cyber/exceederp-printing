namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class periodcountbincard : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.StoreItemAssignment", "Type", c => c.Int(nullable: false));
            AddColumn("EXCEED.BinCardLine", "GlFiscalYearId", c => c.Int(nullable: false));
            AddColumn("EXCEED.PropertyCount", "GlFiscalYearId", c => c.Int(nullable: false));
            AddColumn("EXCEED.PropertyCount", "IsMidOfPeriod", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PropertyCount", "IsMidOfPeriod");
            DropColumn("EXCEED.PropertyCount", "GlFiscalYearId");
            DropColumn("EXCEED.BinCardLine", "GlFiscalYearId");
            DropColumn("EXCEED.StoreItemAssignment", "Type");
        }
    }
}
