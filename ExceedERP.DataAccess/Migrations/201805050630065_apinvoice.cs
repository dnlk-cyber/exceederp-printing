namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class apinvoice : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.ApPaybleInvoices", "Source", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ApPaybleInvoices", "Source", c => c.String());
        }
    }
}
