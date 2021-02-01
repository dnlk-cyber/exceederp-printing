namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class transfer1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ItemTransfer", "StoreRequisitionID", c => c.Int(nullable: false));
            AddColumn("EXCEED.ItemTransferItem", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("EXCEED.ItemTransferItem", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("EXCEED.ItemTransferItem", "ItemCategoryCode");
            CreateIndex("EXCEED.ItemTransferItem", "ItemCode");
            AddForeignKey("EXCEED.ItemTransferItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.ItemTransferItem", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            DropColumn("EXCEED.ItemTransfer", "RequisitionNo");
            DropColumn("EXCEED.ItemTransferItem", "UOM");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ItemTransferItem", "UOM", c => c.String());
            AddColumn("EXCEED.ItemTransfer", "RequisitionNo", c => c.String());
            DropForeignKey("EXCEED.ItemTransferItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.ItemTransferItem", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("EXCEED.ItemTransferItem", new[] { "ItemCode" });
            DropIndex("EXCEED.ItemTransferItem", new[] { "ItemCategoryCode" });
            AlterColumn("EXCEED.ItemTransferItem", "ItemCode", c => c.String(nullable: false));
            DropColumn("EXCEED.ItemTransferItem", "ItemCategoryCode");
            DropColumn("EXCEED.ItemTransfer", "StoreRequisitionID");
        }
    }
}
