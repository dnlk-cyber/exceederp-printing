namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class payableinventory : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ApPaybleInvoices", "Grn", c => c.String());
            AddColumn("EXCEED.ApPaybleInvoices", "Source", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ApPaybleInvoices", "Source");
            DropColumn("EXCEED.ApPaybleInvoices", "Grn");
        }
    }
}
