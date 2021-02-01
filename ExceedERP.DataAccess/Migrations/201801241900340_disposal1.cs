namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class disposal1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.AssetDisposal", "Status", c => c.String());
            AddColumn("EXCEED.AssetDisposal", "Remark", c => c.String());
            AddColumn("EXCEED.AssetDisposal", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.AssetDisposal", "VoidBy", c => c.String());
            AddColumn("EXCEED.AssetDisposal", "VoidTime", c => c.DateTime());
            AddColumn("EXCEED.AssetDisposal", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.AssetDisposal", "OnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.AssetDisposal", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.AssetDisposal", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.AssetDisposal", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.AssetDisposal", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("EXCEED.AssetDisposal", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.AssetDisposal", "SendForApprovalBy", c => c.String());
            AddColumn("EXCEED.AssetDisposal", "SendForApprovalTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.AssetDisposal", "SendForApprovalTime");
            DropColumn("EXCEED.AssetDisposal", "SendForApprovalBy");
            DropColumn("EXCEED.AssetDisposal", "IsSendForApproval");
            DropColumn("EXCEED.AssetDisposal", "OnlineTransferredTime");
            DropColumn("EXCEED.AssetDisposal", "IsOnlineTransferredBy");
            DropColumn("EXCEED.AssetDisposal", "IsOnlineTransferred");
            DropColumn("EXCEED.AssetDisposal", "OnlineApprovedTime");
            DropColumn("EXCEED.AssetDisposal", "OnlineApprovedBy");
            DropColumn("EXCEED.AssetDisposal", "IsOnlineApproved");
            DropColumn("EXCEED.AssetDisposal", "VoidTime");
            DropColumn("EXCEED.AssetDisposal", "VoidBy");
            DropColumn("EXCEED.AssetDisposal", "IsVoid");
            DropColumn("EXCEED.AssetDisposal", "Remark");
            DropColumn("EXCEED.AssetDisposal", "Status");
        }
    }
}
