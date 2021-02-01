namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class noreceipt : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.BidPurchasingSuppliers", "ReceiptNotRequired", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.BidPurchasingSuppliers", "ReceiptNotRequired");
        }
    }
}
