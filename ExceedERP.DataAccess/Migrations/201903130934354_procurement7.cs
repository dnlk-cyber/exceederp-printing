namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procurement7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.BidAwardedSupplierItem", "Discount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.BidAwardedSupplierItem", "FinalUPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.BidAwardedSupplier", "TenderReference", c => c.String());
            AddColumn("EXCEED.WinningSupplierItem", "Discount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.WinningSupplierItem", "FinalUPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.WinningSupplierItem", "FinalUPrice");
            DropColumn("EXCEED.WinningSupplierItem", "Discount");
            DropColumn("EXCEED.BidAwardedSupplier", "TenderReference");
            DropColumn("EXCEED.BidAwardedSupplierItem", "FinalUPrice");
            DropColumn("EXCEED.BidAwardedSupplierItem", "Discount");
        }
    }
}
