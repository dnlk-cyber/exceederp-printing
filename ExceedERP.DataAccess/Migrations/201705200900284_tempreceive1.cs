namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tempreceive1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PurchaseReturnItems", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.PurchaseReturnItems", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("EXCEED.PurchaseReturnValidation", "Status", c => c.Int(nullable: false));
            CreateIndex("EXCEED.PurchaseReturnItems", "ItemCategoryCode");
            CreateIndex("EXCEED.PurchaseReturnItems", "ItemCode");
            AddForeignKey("EXCEED.PurchaseReturnItems", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.PurchaseReturnItems", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            DropColumn("EXCEED.PurchaseReturnItems", "InventoryItem");
            DropColumn("EXCEED.PurchaseReturnItems", "UOM");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.PurchaseReturnItems", "UOM", c => c.String());
            AddColumn("EXCEED.PurchaseReturnItems", "InventoryItem", c => c.String(nullable: false));
            DropForeignKey("EXCEED.PurchaseReturnItems", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.PurchaseReturnItems", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("EXCEED.PurchaseReturnItems", new[] { "ItemCode" });
            DropIndex("EXCEED.PurchaseReturnItems", new[] { "ItemCategoryCode" });
            AlterColumn("EXCEED.PurchaseReturnValidation", "Status", c => c.String());
            DropColumn("EXCEED.PurchaseReturnItems", "ItemCode");
            DropColumn("EXCEED.PurchaseReturnItems", "ItemCategoryCode");
        }
    }
}
