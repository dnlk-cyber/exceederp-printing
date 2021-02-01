namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class invento1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GoodReceive", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.GoodReceiveItem", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.GoodReceiveDonationItem", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("EXCEED.GoodReceiveItem", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("EXCEED.GoodReceiveDonationItem", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("EXCEED.GoodReceiveItem", "ItemCategoryCode");
            CreateIndex("EXCEED.GoodReceiveItem", "ItemCode");
            CreateIndex("EXCEED.GoodReceiveDonationItem", "ItemCategoryCode");
            CreateIndex("EXCEED.GoodReceiveDonationItem", "ItemCode");
            AddForeignKey("EXCEED.GoodReceiveItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.GoodReceiveItem", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            AddForeignKey("EXCEED.GoodReceiveDonationItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.GoodReceiveDonationItem", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.GoodReceiveDonationItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.GoodReceiveDonationItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.GoodReceiveItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.GoodReceiveItem", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("EXCEED.GoodReceiveDonationItem", new[] { "ItemCode" });
            DropIndex("EXCEED.GoodReceiveDonationItem", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.GoodReceiveItem", new[] { "ItemCode" });
            DropIndex("EXCEED.GoodReceiveItem", new[] { "ItemCategoryCode" });
            AlterColumn("EXCEED.GoodReceiveDonationItem", "ItemCode", c => c.String(nullable: false));
            AlterColumn("EXCEED.GoodReceiveItem", "ItemCode", c => c.String(nullable: false));
            DropColumn("EXCEED.GoodReceiveDonationItem", "ItemCategoryCode");
            DropColumn("EXCEED.GoodReceiveItem", "ItemCategoryCode");
            DropColumn("EXCEED.GoodReceive", "BranchId");
        }
    }
}
