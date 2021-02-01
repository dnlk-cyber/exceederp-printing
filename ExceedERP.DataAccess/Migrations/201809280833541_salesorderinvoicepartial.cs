namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salesorderinvoicepartial : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ArInvoice", "ArSalesOrderPartialPrintId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ArInvoice", "ArSalesOrderPartialPrintId");
        }
    }
}
