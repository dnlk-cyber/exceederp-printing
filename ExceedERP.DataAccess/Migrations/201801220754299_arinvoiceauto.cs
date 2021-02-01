namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class arinvoiceauto : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ArInvoice", "ArSalesOrderId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ArInvoice", "IsManual", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ArInvoice", "IsManual");
            DropColumn("EXCEED.ArInvoice", "ArSalesOrderId");
        }
    }
}
