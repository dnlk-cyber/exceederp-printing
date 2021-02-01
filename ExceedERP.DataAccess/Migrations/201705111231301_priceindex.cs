namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class priceindex : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ItemPriceIndex", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.ItemPriceIndex", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("EXCEED.ItemPriceIndex", "ItemCategoryCode");
            CreateIndex("EXCEED.ItemPriceIndex", "ItemCode");
            AddForeignKey("EXCEED.ItemPriceIndex", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.ItemPriceIndex", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            DropColumn("EXCEED.ItemPriceIndex", "ItemCategory");
            DropColumn("EXCEED.ItemPriceIndex", "InventoryItem");
            DropColumn("EXCEED.ItemPriceIndex", "ItemCategoryDesc");
            DropColumn("EXCEED.ItemPriceIndex", "InventoryItemDesc");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ItemPriceIndex", "InventoryItemDesc", c => c.String());
            AddColumn("EXCEED.ItemPriceIndex", "ItemCategoryDesc", c => c.String());
            AddColumn("EXCEED.ItemPriceIndex", "InventoryItem", c => c.String(nullable: false));
            AddColumn("EXCEED.ItemPriceIndex", "ItemCategory", c => c.String(nullable: false));
            DropForeignKey("EXCEED.ItemPriceIndex", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.ItemPriceIndex", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("EXCEED.ItemPriceIndex", new[] { "ItemCode" });
            DropIndex("EXCEED.ItemPriceIndex", new[] { "ItemCategoryCode" });
            DropColumn("EXCEED.ItemPriceIndex", "ItemCode");
            DropColumn("EXCEED.ItemPriceIndex", "ItemCategoryCode");
        }
    }
}
