namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class additemnametoinventory : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GoodReceiveItem", "ItemName", c => c.String());
            AddColumn("EXCEED.GoodSellingAndLendingItem", "ItemCategoryCode", c => c.String(maxLength: 128));
            AddColumn("EXCEED.GoodSellingAndLendingItem", "ItemName", c => c.String());
            AddColumn("EXCEED.IssueItem", "ItemName", c => c.String());
            AddColumn("EXCEED.StoreRequisitionItem", "ItemName", c => c.String());
            AddColumn("EXCEED.ItemTransferItem", "ItemName", c => c.String());
            AddColumn("EXCEED.ItemTransferItemReceive", "ItemName", c => c.String());
            AddColumn("EXCEED.StockAdjustmentItems", "ItemName", c => c.String());
            AddColumn("EXCEED.StoreReturnItems", "ItemName", c => c.String());
            AddColumn("EXCEED.TempGoodReceiveItem", "ItemName", c => c.String());
            //AlterColumn("EXCEED.GoodSellingAndLendingItem", "ItemCode", c => c.String(nullable: false, maxLength: 128));
           // CreateIndex("EXCEED.GoodSellingAndLendingItem", "ItemCategoryCode");
            //CreateIndex("EXCEED.GoodSellingAndLendingItem", "ItemCode");
           // AddForeignKey("EXCEED.GoodSellingAndLendingItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
           // AddForeignKey("EXCEED.GoodSellingAndLendingItem", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode");
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.GoodSellingAndLendingItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.GoodSellingAndLendingItem", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("EXCEED.GoodSellingAndLendingItem", new[] { "ItemCode" });
            DropIndex("EXCEED.GoodSellingAndLendingItem", new[] { "ItemCategoryCode" });
            AlterColumn("EXCEED.GoodSellingAndLendingItem", "ItemCode", c => c.String(nullable: false));
            DropColumn("EXCEED.TempGoodReceiveItem", "ItemName");
            DropColumn("EXCEED.StoreReturnItems", "ItemName");
            DropColumn("EXCEED.StockAdjustmentItems", "ItemName");
            DropColumn("EXCEED.ItemTransferItemReceive", "ItemName");
            DropColumn("EXCEED.ItemTransferItem", "ItemName");
            DropColumn("EXCEED.StoreRequisitionItem", "ItemName");
            DropColumn("EXCEED.IssueItem", "ItemName");
            DropColumn("EXCEED.GoodSellingAndLendingItem", "ItemName");
            DropColumn("EXCEED.GoodSellingAndLendingItem", "ItemCategoryCode");
            DropColumn("EXCEED.GoodReceiveItem", "ItemName");
        }
    }
}
