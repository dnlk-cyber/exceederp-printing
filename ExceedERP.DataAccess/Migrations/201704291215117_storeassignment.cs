namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class storeassignment : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.StoreItemAssignmentInventoryItem", "StoreItemAssignment_StoreItemAssignmentID", "EXCEED.StoreItemAssignment");
            DropForeignKey("EXCEED.StoreItemAssignmentInventoryItem", "InventoryItem_ItemCode", "EXCEED.InventoryItem");
            DropIndex("EXCEED.StoreItemAssignmentInventoryItem", new[] { "StoreItemAssignment_StoreItemAssignmentID" });
            DropIndex("EXCEED.StoreItemAssignmentInventoryItem", new[] { "InventoryItem_ItemCode" });
            AddColumn("EXCEED.StoreItemAssignment", "AvgPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.StoreItemAssignment", "ItemCode", c => c.String(maxLength: 128));
            CreateIndex("EXCEED.StoreItemAssignment", "ItemCode");
            AddForeignKey("EXCEED.StoreItemAssignment", "ItemCode", "EXCEED.InventoryItem", "ItemCode");
            DropColumn("EXCEED.StoreItemAssignment", "ItemCategoryCode");
            DropColumn("EXCEED.StoreItemAssignment", "UOM");
            DropTable("EXCEED.StoreItemAssignmentInventoryItem");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.StoreItemAssignmentInventoryItem",
                c => new
                    {
                        StoreItemAssignment_StoreItemAssignmentID = c.Int(nullable: false),
                        InventoryItem_ItemCode = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.StoreItemAssignment_StoreItemAssignmentID, t.InventoryItem_ItemCode });
            
            AddColumn("EXCEED.StoreItemAssignment", "UOM", c => c.String());
            AddColumn("EXCEED.StoreItemAssignment", "ItemCategoryCode", c => c.String(nullable: false));
            DropForeignKey("EXCEED.StoreItemAssignment", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("EXCEED.StoreItemAssignment", new[] { "ItemCode" });
            AlterColumn("EXCEED.StoreItemAssignment", "ItemCode", c => c.String(nullable: false));
            DropColumn("EXCEED.StoreItemAssignment", "AvgPrice");
            CreateIndex("EXCEED.StoreItemAssignmentInventoryItem", "InventoryItem_ItemCode");
            CreateIndex("EXCEED.StoreItemAssignmentInventoryItem", "StoreItemAssignment_StoreItemAssignmentID");
            AddForeignKey("EXCEED.StoreItemAssignmentInventoryItem", "InventoryItem_ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.StoreItemAssignmentInventoryItem", "StoreItemAssignment_StoreItemAssignmentID", "EXCEED.StoreItemAssignment", "StoreItemAssignmentID", cascadeDelete: true);
        }
    }
}
