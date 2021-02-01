namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class store : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.InventoryStore", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.InventoryStore", "ItemCategory_ItemCategoryCode", c => c.String(maxLength: 128));
            CreateIndex("EXCEED.InventoryStore", "ItemCategory_ItemCategoryCode");
            AddForeignKey("EXCEED.InventoryStore", "ItemCategory_ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode");
            DropColumn("EXCEED.InventoryStore", "Branch");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.InventoryStore", "Branch", c => c.String(nullable: false));
            DropForeignKey("EXCEED.InventoryStore", "ItemCategory_ItemCategoryCode", "EXCEED.ItemCategory");
            DropIndex("EXCEED.InventoryStore", new[] { "ItemCategory_ItemCategoryCode" });
            DropColumn("EXCEED.InventoryStore", "ItemCategory_ItemCategoryCode");
            DropColumn("EXCEED.InventoryStore", "BranchId");
        }
    }
}
