namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class invento6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.StoreReturnItems", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("EXCEED.StoreReturnItems", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("EXCEED.StoreReturnItems", "ItemCategoryCode");
            CreateIndex("EXCEED.StoreReturnItems", "ItemCode");
            AddForeignKey("EXCEED.StoreReturnItems", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.StoreReturnItems", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            DropColumn("EXCEED.StoreReturnItems", "UOM");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.StoreReturnItems", "UOM", c => c.String());
            DropForeignKey("EXCEED.StoreReturnItems", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.StoreReturnItems", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("EXCEED.StoreReturnItems", new[] { "ItemCode" });
            DropIndex("EXCEED.StoreReturnItems", new[] { "ItemCategoryCode" });
            AlterColumn("EXCEED.StoreReturnItems", "ItemCode", c => c.String(nullable: false));
            DropColumn("EXCEED.StoreReturnItems", "ItemCategoryCode");
        }
    }
}
