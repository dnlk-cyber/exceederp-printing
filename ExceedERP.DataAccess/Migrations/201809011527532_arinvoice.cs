namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class arinvoice : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ArInvoiceLines", "ArSalesOrderLineId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ArInvoiceLines", "ArSalesOrderLineId");
        }
    }
}
