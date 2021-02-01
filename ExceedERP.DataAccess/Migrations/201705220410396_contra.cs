namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class contra : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ContractManagementItem", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.ContractManagementItem", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("EXCEED.ContractManagementItem", "ItemCategoryCode");
            CreateIndex("EXCEED.ContractManagementItem", "ItemCode");
            AddForeignKey("EXCEED.ContractManagementItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.ContractManagementItem", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            DropColumn("EXCEED.ContractManagementItem", "ItemCategory");
            DropColumn("EXCEED.ContractManagementItem", "InventoryItem");
            DropColumn("EXCEED.ContractManagementItem", "ItemCategoryDesc");
            DropColumn("EXCEED.ContractManagementItem", "InventoryItemDesc");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ContractManagementItem", "InventoryItemDesc", c => c.String());
            AddColumn("EXCEED.ContractManagementItem", "ItemCategoryDesc", c => c.String());
            AddColumn("EXCEED.ContractManagementItem", "InventoryItem", c => c.String(nullable: false));
            AddColumn("EXCEED.ContractManagementItem", "ItemCategory", c => c.String(nullable: false));
            DropForeignKey("EXCEED.ContractManagementItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.ContractManagementItem", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("EXCEED.ContractManagementItem", new[] { "ItemCode" });
            DropIndex("EXCEED.ContractManagementItem", new[] { "ItemCategoryCode" });
            DropColumn("EXCEED.ContractManagementItem", "ItemCode");
            DropColumn("EXCEED.ContractManagementItem", "ItemCategoryCode");
        }
    }
}
