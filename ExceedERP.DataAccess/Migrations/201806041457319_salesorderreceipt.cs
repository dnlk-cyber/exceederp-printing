namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salesorderreceipt : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ArSalesOrderPartialPrint", "IsReceiptPrinted", c => c.Boolean(nullable: false));
            DropColumn("EXCEED.ArSalesOrderPartial", "IsReceiptPrinted");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ArSalesOrderPartial", "IsReceiptPrinted", c => c.Boolean(nullable: false));
            DropColumn("EXCEED.ArSalesOrderPartialPrint", "IsReceiptPrinted");
        }
    }
}
