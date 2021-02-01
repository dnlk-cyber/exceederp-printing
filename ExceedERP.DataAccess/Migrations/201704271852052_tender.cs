namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tender : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.BidPurchasingSuppliers", "ReceiptNo", c => c.String(nullable: false));
            AddColumn("EXCEED.Tender", "ProcurementCategory", c => c.Int(nullable: false));
            DropColumn("EXCEED.BidPurchasingSuppliers", "Receipt");
            DropColumn("EXCEED.Tender", "TenderMethod");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.Tender", "TenderMethod", c => c.Int(nullable: false));
            AddColumn("EXCEED.BidPurchasingSuppliers", "Receipt", c => c.String());
            DropColumn("EXCEED.Tender", "ProcurementCategory");
            DropColumn("EXCEED.BidPurchasingSuppliers", "ReceiptNo");
        }
    }
}
