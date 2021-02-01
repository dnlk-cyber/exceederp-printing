namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class qtydecimalsales : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.ArInvoiceLines", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ArSalesOrderLine", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ArSalesOrderLine", "Quantity", c => c.Int(nullable: false));
            AlterColumn("EXCEED.ArInvoiceLines", "Quantity", c => c.Int(nullable: false));
        }
    }
}
