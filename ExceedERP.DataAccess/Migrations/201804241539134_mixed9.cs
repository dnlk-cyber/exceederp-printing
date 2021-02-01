namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mixed9 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.BidPurchasingSuppliers", "ReceiptNo", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.BidPurchasingSuppliers", "ReceiptNo", c => c.String(nullable: false));
        }
    }
}
