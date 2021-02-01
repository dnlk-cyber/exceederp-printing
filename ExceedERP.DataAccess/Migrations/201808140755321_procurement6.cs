namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procurement6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ApPaybleInvoices", "SourceId", c => c.String());
            AddColumn("EXCEED.ForeignPurchase", "LcTtCadRef", c => c.String());
            AddColumn("EXCEED.ForeignPurchase", "Percentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ForeignPurchase", "Percentage");
            DropColumn("EXCEED.ForeignPurchase", "LcTtCadRef");
            DropColumn("EXCEED.ApPaybleInvoices", "SourceId");
        }
    }
}
