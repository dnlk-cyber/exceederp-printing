namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class disruptingpurchase3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.PurchaseFollowUpItems", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.PurchaseFollowUpItems", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropIndex("EXCEED.PurchaseFollowUpItems", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.PurchaseFollowUpItems", new[] { "ItemCode" });
            AddColumn("EXCEED.PurchaseFollowUpItems", "CatCode", c => c.String(nullable: false));
            AddColumn("EXCEED.PurchaseFollowUpItems", "CategoryName", c => c.String());
            AddColumn("EXCEED.PurchaseFollowUpItems", "ItmCode", c => c.String(nullable: false));
            AddColumn("EXCEED.PurchaseFollowUpItems", "ItemName", c => c.String());
            AddColumn("EXCEED.PurchaseFollowUpItems", "ItemType", c => c.Int(nullable: false));
            DropColumn("EXCEED.PurchaseFollowUpItems", "ItemCategoryCode");
            DropColumn("EXCEED.PurchaseFollowUpItems", "ItemCode");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.PurchaseFollowUpItems", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.PurchaseFollowUpItems", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            DropColumn("EXCEED.PurchaseFollowUpItems", "ItemType");
            DropColumn("EXCEED.PurchaseFollowUpItems", "ItemName");
            DropColumn("EXCEED.PurchaseFollowUpItems", "ItmCode");
            DropColumn("EXCEED.PurchaseFollowUpItems", "CategoryName");
            DropColumn("EXCEED.PurchaseFollowUpItems", "CatCode");
            CreateIndex("EXCEED.PurchaseFollowUpItems", "ItemCode");
            CreateIndex("EXCEED.PurchaseFollowUpItems", "ItemCategoryCode");
            AddForeignKey("EXCEED.PurchaseFollowUpItems", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            AddForeignKey("EXCEED.PurchaseFollowUpItems", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
        }
    }
}
