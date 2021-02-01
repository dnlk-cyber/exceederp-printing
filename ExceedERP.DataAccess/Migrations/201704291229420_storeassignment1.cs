namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class storeassignment1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.StoreItemAssignment", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("EXCEED.StoreItemAssignment", new[] { "ItemCode" });
            AddColumn("EXCEED.StoreItemAssignment", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("EXCEED.StoreItemAssignment", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("EXCEED.StoreItemAssignment", "ItemCategoryCode");
            CreateIndex("EXCEED.StoreItemAssignment", "ItemCode");
            AddForeignKey("EXCEED.StoreItemAssignment", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            AddForeignKey("EXCEED.StoreItemAssignment", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.StoreItemAssignment", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.StoreItemAssignment", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropIndex("EXCEED.StoreItemAssignment", new[] { "ItemCode" });
            DropIndex("EXCEED.StoreItemAssignment", new[] { "ItemCategoryCode" });
            AlterColumn("EXCEED.StoreItemAssignment", "ItemCode", c => c.String(maxLength: 128));
            DropColumn("EXCEED.StoreItemAssignment", "ItemCategoryCode");
            CreateIndex("EXCEED.StoreItemAssignment", "ItemCode");
            AddForeignKey("EXCEED.StoreItemAssignment", "ItemCode", "EXCEED.InventoryItem", "ItemCode");
        }
    }
}
