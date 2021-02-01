namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class po : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PurchaseOrder", "PurchaseRequisitionID", c => c.Int(nullable: false));
            AddColumn("EXCEED.PurchaseOrderItem", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.PurchaseOrderItem", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("EXCEED.PurchaseOrderItem", "ItemCategoryCode");
            CreateIndex("EXCEED.PurchaseOrderItem", "ItemCode");
            AddForeignKey("EXCEED.PurchaseOrderItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.PurchaseOrderItem", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            DropColumn("EXCEED.PurchaseOrder", "PRNo");
            DropColumn("EXCEED.PurchaseOrderItem", "ItemCategory");
            DropColumn("EXCEED.PurchaseOrderItem", "InventoryItem");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.PurchaseOrderItem", "InventoryItem", c => c.String(nullable: false));
            AddColumn("EXCEED.PurchaseOrderItem", "ItemCategory", c => c.String());
            AddColumn("EXCEED.PurchaseOrder", "PRNo", c => c.Int(nullable: false));
            DropForeignKey("EXCEED.PurchaseOrderItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.PurchaseOrderItem", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("EXCEED.PurchaseOrderItem", new[] { "ItemCode" });
            DropIndex("EXCEED.PurchaseOrderItem", new[] { "ItemCategoryCode" });
            DropColumn("EXCEED.PurchaseOrderItem", "ItemCode");
            DropColumn("EXCEED.PurchaseOrderItem", "ItemCategoryCode");
            DropColumn("EXCEED.PurchaseOrder", "PurchaseRequisitionID");
        }
    }
}
