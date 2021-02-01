namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class contractman2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ContractManagementItem", "ItemCategoryDesc", c => c.String());
            AddColumn("EXCEED.ContractManagementItem", "InventoryItemDesc", c => c.String());
            AddColumn("EXCEED.ContractManagementItem", "Remark", c => c.String());
            AlterColumn("EXCEED.ContractManagementItem", "ItemCategory", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ContractManagementItem", "ItemCategory", c => c.String());
            DropColumn("EXCEED.ContractManagementItem", "Remark");
            DropColumn("EXCEED.ContractManagementItem", "InventoryItemDesc");
            DropColumn("EXCEED.ContractManagementItem", "ItemCategoryDesc");
        }
    }
}
