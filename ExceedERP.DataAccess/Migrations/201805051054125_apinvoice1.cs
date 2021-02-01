namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class apinvoice1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ApPaybleInvoices", "IsNewSupplier", c => c.Boolean(nullable: false));
            AlterColumn("EXCEED.ApPaybleInvoices", "SupplierId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ApPaybleInvoices", "SupplierId", c => c.String(nullable: false));
            DropColumn("EXCEED.ApPaybleInvoices", "IsNewSupplier");
        }
    }
}
