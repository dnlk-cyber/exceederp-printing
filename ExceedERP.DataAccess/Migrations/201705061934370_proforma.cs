namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class proforma : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.RequestedSupplier", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.RequestedSupplier", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.WinningSupplier", "GLTaxRateID", c => c.Int(nullable: false));
            AddColumn("EXCEED.WinningSupplier", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            CreateIndex("EXCEED.RequestedSupplier", "ItemCategoryCode");
            CreateIndex("EXCEED.RequestedSupplier", "ItemCode");
            AddForeignKey("EXCEED.RequestedSupplier", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.RequestedSupplier", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            DropColumn("EXCEED.ProformaItem", "GLTaxRateID");
            DropColumn("EXCEED.RequestedSupplier", "InventoryItem");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.RequestedSupplier", "InventoryItem", c => c.String());
            AddColumn("EXCEED.ProformaItem", "GLTaxRateID", c => c.Int(nullable: false));
            DropForeignKey("EXCEED.RequestedSupplier", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.RequestedSupplier", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("EXCEED.RequestedSupplier", new[] { "ItemCode" });
            DropIndex("EXCEED.RequestedSupplier", new[] { "ItemCategoryCode" });
            DropColumn("EXCEED.WinningSupplier", "TaxAmount");
            DropColumn("EXCEED.WinningSupplier", "GLTaxRateID");
            DropColumn("EXCEED.RequestedSupplier", "ItemCode");
            DropColumn("EXCEED.RequestedSupplier", "ItemCategoryCode");
        }
    }
}
