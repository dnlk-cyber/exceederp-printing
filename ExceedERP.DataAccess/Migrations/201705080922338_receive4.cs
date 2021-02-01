namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class receive4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.PropertyCountItem", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("EXCEED.PropertyCountItem", new[] { "ItemCode" });
            AddColumn("EXCEED.PropertyCount", "Status", c => c.String());
            AddColumn("EXCEED.PropertyCount", "Remark", c => c.String());
            AddColumn("EXCEED.PropertyCount", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PropertyCount", "VoidBy", c => c.String());
            AddColumn("EXCEED.PropertyCount", "VoidTime", c => c.DateTime());
            AddColumn("EXCEED.PropertyCount", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PropertyCount", "OnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.PropertyCount", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.PropertyCount", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PropertyCount", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.PropertyCount", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("EXCEED.PropertyCount", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PropertyCount", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.PropertyCount", "SendForApprovalBy", c => c.String());
            AddColumn("EXCEED.PropertyCount", "SendForApprovalTime", c => c.DateTime());
            AddColumn("EXCEED.PropertyCount", "DateCreated", c => c.DateTime());
            AddColumn("EXCEED.PropertyCount", "LastModified", c => c.DateTime());
            AddColumn("EXCEED.PropertyCount", "CreatedBy", c => c.String());
            AddColumn("EXCEED.PropertyCount", "ModifiedBy", c => c.String());
            AlterColumn("EXCEED.PropertyCountItem", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("EXCEED.PropertyCountItem", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("EXCEED.PropertyCountItem", "ItemCategoryCode");
            CreateIndex("EXCEED.PropertyCountItem", "ItemCode");
            AddForeignKey("EXCEED.PropertyCountItem", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            AddForeignKey("EXCEED.PropertyCountItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.PropertyCountItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.PropertyCountItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropIndex("EXCEED.PropertyCountItem", new[] { "ItemCode" });
            DropIndex("EXCEED.PropertyCountItem", new[] { "ItemCategoryCode" });
            AlterColumn("EXCEED.PropertyCountItem", "ItemCategoryCode", c => c.String());
            AlterColumn("EXCEED.PropertyCountItem", "ItemCode", c => c.String(maxLength: 128));
            DropColumn("EXCEED.PropertyCount", "ModifiedBy");
            DropColumn("EXCEED.PropertyCount", "CreatedBy");
            DropColumn("EXCEED.PropertyCount", "LastModified");
            DropColumn("EXCEED.PropertyCount", "DateCreated");
            DropColumn("EXCEED.PropertyCount", "SendForApprovalTime");
            DropColumn("EXCEED.PropertyCount", "SendForApprovalBy");
            DropColumn("EXCEED.PropertyCount", "OrgBranchName");
            DropColumn("EXCEED.PropertyCount", "IsSendForApproval");
            DropColumn("EXCEED.PropertyCount", "OnlineTransferredTime");
            DropColumn("EXCEED.PropertyCount", "IsOnlineTransferredBy");
            DropColumn("EXCEED.PropertyCount", "IsOnlineTransferred");
            DropColumn("EXCEED.PropertyCount", "OnlineApprovedTime");
            DropColumn("EXCEED.PropertyCount", "OnlineApprovedBy");
            DropColumn("EXCEED.PropertyCount", "IsOnlineApproved");
            DropColumn("EXCEED.PropertyCount", "VoidTime");
            DropColumn("EXCEED.PropertyCount", "VoidBy");
            DropColumn("EXCEED.PropertyCount", "IsVoid");
            DropColumn("EXCEED.PropertyCount", "Remark");
            DropColumn("EXCEED.PropertyCount", "Status");
            CreateIndex("EXCEED.PropertyCountItem", "ItemCode");
            AddForeignKey("EXCEED.PropertyCountItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode");
        }
    }
}
