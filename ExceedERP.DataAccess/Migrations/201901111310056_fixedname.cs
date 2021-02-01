namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedname : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ApMiscellaneousPayment", "LineAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApMiscellaneousPayment", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApMiscellaneousPayment", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.BidAwardedSupplierItem", "Name", c => c.String());
            AddColumn("EXCEED.LotItems", "Name", c => c.String());
            AddColumn("EXCEED.ProformaItem", "Name", c => c.String());
            AddColumn("EXCEED.WinningSupplierItem", "Name", c => c.String());
            AddColumn("EXCEED.PurchaseOrderItem", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PurchaseOrderItem", "Name");
            DropColumn("EXCEED.WinningSupplierItem", "Name");
            DropColumn("EXCEED.ProformaItem", "Name");
            DropColumn("EXCEED.LotItems", "Name");
            DropColumn("EXCEED.BidAwardedSupplierItem", "Name");
            DropColumn("EXCEED.ApMiscellaneousPayment", "TotalAmount");
            DropColumn("EXCEED.ApMiscellaneousPayment", "TaxAmount");
            DropColumn("EXCEED.ApMiscellaneousPayment", "LineAmount");
        }
    }
}
