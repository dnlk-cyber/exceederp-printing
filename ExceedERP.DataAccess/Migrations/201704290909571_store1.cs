namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class store1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.InventoryStore", "ItemCategory_ItemCategoryCode", "EXCEED.ItemCategory");
            DropIndex("EXCEED.InventoryStore", new[] { "ItemCategory_ItemCategoryCode" });
            DropColumn("EXCEED.InventoryStore", "BranchId");
            //DropColumn("EXCEED.InventoryStore", "ItemCategory_ItemCategoryCode");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.InventoryStore", "ItemCategory_ItemCategoryCode", c => c.String(maxLength: 128));
            AddColumn("EXCEED.InventoryStore", "BranchId", c => c.Int(nullable: false));
            CreateIndex("EXCEED.InventoryStore", "ItemCategory_ItemCategoryCode");
            AddForeignKey("EXCEED.InventoryStore", "ItemCategory_ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode");
        }
    }
}
