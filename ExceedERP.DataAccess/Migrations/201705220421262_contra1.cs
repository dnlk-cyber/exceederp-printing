namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class contra1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ContractManagement", "Status", c => c.String());
            AddColumn("EXCEED.ContractManagement", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ContractManagement", "VoidBy", c => c.String());
            AddColumn("EXCEED.ContractManagement", "VoidTime", c => c.DateTime());
            AddColumn("EXCEED.ContractManagement", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ContractManagement", "OnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.ContractManagement", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.ContractManagement", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ContractManagement", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.ContractManagement", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("EXCEED.ContractManagement", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ContractManagement", "SendForApprovalBy", c => c.String());
            AddColumn("EXCEED.ContractManagement", "SendForApprovalTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ContractManagement", "SendForApprovalTime");
            DropColumn("EXCEED.ContractManagement", "SendForApprovalBy");
            DropColumn("EXCEED.ContractManagement", "IsSendForApproval");
            DropColumn("EXCEED.ContractManagement", "OnlineTransferredTime");
            DropColumn("EXCEED.ContractManagement", "IsOnlineTransferredBy");
            DropColumn("EXCEED.ContractManagement", "IsOnlineTransferred");
            DropColumn("EXCEED.ContractManagement", "OnlineApprovedTime");
            DropColumn("EXCEED.ContractManagement", "OnlineApprovedBy");
            DropColumn("EXCEED.ContractManagement", "IsOnlineApproved");
            DropColumn("EXCEED.ContractManagement", "VoidTime");
            DropColumn("EXCEED.ContractManagement", "VoidBy");
            DropColumn("EXCEED.ContractManagement", "IsVoid");
            DropColumn("EXCEED.ContractManagement", "Status");
        }
    }
}
