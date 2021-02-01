namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tempreceive : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.TempGoodReceiveItem", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.TempGoodReceive", "BranchId", c => c.Int(nullable: false));
            AlterColumn("EXCEED.TempGoodReceiveItem", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("EXCEED.TempGoodReceiveValidation", "Status", c => c.Int(nullable: false));
            CreateIndex("EXCEED.TempGoodReceiveItem", "ItemCategoryCode");
            CreateIndex("EXCEED.TempGoodReceiveItem", "ItemCode");
            AddForeignKey("EXCEED.TempGoodReceiveItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.TempGoodReceiveItem", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            DropColumn("EXCEED.TempGoodReceiveItem", "UOM");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.TempGoodReceiveItem", "UOM", c => c.String());
            DropForeignKey("EXCEED.TempGoodReceiveItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.TempGoodReceiveItem", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("EXCEED.TempGoodReceiveItem", new[] { "ItemCode" });
            DropIndex("EXCEED.TempGoodReceiveItem", new[] { "ItemCategoryCode" });
            AlterColumn("EXCEED.TempGoodReceiveValidation", "Status", c => c.String());
            AlterColumn("EXCEED.TempGoodReceiveItem", "ItemCode", c => c.String(nullable: false));
            DropColumn("EXCEED.TempGoodReceive", "BranchId");
            DropColumn("EXCEED.TempGoodReceiveItem", "ItemCategoryCode");
        }
    }
}
