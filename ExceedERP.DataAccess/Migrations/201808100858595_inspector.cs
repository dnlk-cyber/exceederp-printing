namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inspector : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FixedAssetInspection", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("EXCEED.FixedAssetInspection", "Accepted", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ItemInspection", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ItemInspection", "Accepted", c => c.Boolean(nullable: false));
            DropColumn("EXCEED.FixedAssetInspection", "StoreCode");
            DropColumn("EXCEED.FixedAssetInspection", "CommitteeMember1");
            DropColumn("EXCEED.FixedAssetInspection", "CommitteeMember2");
            DropColumn("EXCEED.FixedAssetInspection", "CommitteeMember3");
            DropColumn("EXCEED.FixedAssetInspection", "CommitteeDecision");
            DropColumn("EXCEED.FixedAssetInspection", "IsPostedToSecondLedger");
            DropColumn("EXCEED.FixedAssetInspection", "Status");
            DropColumn("EXCEED.FixedAssetInspection", "IsVoid");
            DropColumn("EXCEED.FixedAssetInspection", "VoidBy");
            DropColumn("EXCEED.FixedAssetInspection", "VoidTime");
            DropColumn("EXCEED.FixedAssetInspection", "IsOnlineApproved");
            DropColumn("EXCEED.FixedAssetInspection", "OnlineApprovedBy");
            DropColumn("EXCEED.FixedAssetInspection", "OnlineApprovedTime");
            DropColumn("EXCEED.FixedAssetInspection", "IsOnlineTransferred");
            DropColumn("EXCEED.FixedAssetInspection", "IsOnlineTransferredBy");
            DropColumn("EXCEED.FixedAssetInspection", "OnlineTransferredTime");
            DropColumn("EXCEED.FixedAssetInspection", "IsSendForApproval");
            DropColumn("EXCEED.FixedAssetInspection", "SendForApprovalBy");
            DropColumn("EXCEED.FixedAssetInspection", "SendForApprovalTime");
            DropColumn("EXCEED.ItemInspection", "StoreCode");
            DropColumn("EXCEED.ItemInspection", "CommitteeMember1");
            DropColumn("EXCEED.ItemInspection", "CommitteeMember2");
            DropColumn("EXCEED.ItemInspection", "CommitteeMember3");
            DropColumn("EXCEED.ItemInspection", "CommitteeDecision");
            DropColumn("EXCEED.ItemInspection", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ItemInspection", "Status");
            DropColumn("EXCEED.ItemInspection", "IsVoid");
            DropColumn("EXCEED.ItemInspection", "VoidBy");
            DropColumn("EXCEED.ItemInspection", "VoidTime");
            DropColumn("EXCEED.ItemInspection", "IsOnlineApproved");
            DropColumn("EXCEED.ItemInspection", "OnlineApprovedBy");
            DropColumn("EXCEED.ItemInspection", "OnlineApprovedTime");
            DropColumn("EXCEED.ItemInspection", "IsOnlineTransferred");
            DropColumn("EXCEED.ItemInspection", "IsOnlineTransferredBy");
            DropColumn("EXCEED.ItemInspection", "OnlineTransferredTime");
            DropColumn("EXCEED.ItemInspection", "IsSendForApproval");
            DropColumn("EXCEED.ItemInspection", "SendForApprovalBy");
            DropColumn("EXCEED.ItemInspection", "SendForApprovalTime");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ItemInspection", "SendForApprovalTime", c => c.DateTime());
            AddColumn("EXCEED.ItemInspection", "SendForApprovalBy", c => c.String());
            AddColumn("EXCEED.ItemInspection", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ItemInspection", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("EXCEED.ItemInspection", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.ItemInspection", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ItemInspection", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.ItemInspection", "OnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.ItemInspection", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ItemInspection", "VoidTime", c => c.DateTime());
            AddColumn("EXCEED.ItemInspection", "VoidBy", c => c.String());
            AddColumn("EXCEED.ItemInspection", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ItemInspection", "Status", c => c.String());
            AddColumn("EXCEED.ItemInspection", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ItemInspection", "CommitteeDecision", c => c.String(nullable: false));
            AddColumn("EXCEED.ItemInspection", "CommitteeMember3", c => c.String());
            AddColumn("EXCEED.ItemInspection", "CommitteeMember2", c => c.String());
            AddColumn("EXCEED.ItemInspection", "CommitteeMember1", c => c.String());
            AddColumn("EXCEED.ItemInspection", "StoreCode", c => c.String(nullable: false));
            AddColumn("EXCEED.FixedAssetInspection", "SendForApprovalTime", c => c.DateTime());
            AddColumn("EXCEED.FixedAssetInspection", "SendForApprovalBy", c => c.String());
            AddColumn("EXCEED.FixedAssetInspection", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FixedAssetInspection", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("EXCEED.FixedAssetInspection", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.FixedAssetInspection", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FixedAssetInspection", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.FixedAssetInspection", "OnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.FixedAssetInspection", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FixedAssetInspection", "VoidTime", c => c.DateTime());
            AddColumn("EXCEED.FixedAssetInspection", "VoidBy", c => c.String());
            AddColumn("EXCEED.FixedAssetInspection", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FixedAssetInspection", "Status", c => c.String());
            AddColumn("EXCEED.FixedAssetInspection", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FixedAssetInspection", "CommitteeDecision", c => c.String(nullable: false));
            AddColumn("EXCEED.FixedAssetInspection", "CommitteeMember3", c => c.String());
            AddColumn("EXCEED.FixedAssetInspection", "CommitteeMember2", c => c.String());
            AddColumn("EXCEED.FixedAssetInspection", "CommitteeMember1", c => c.String());
            AddColumn("EXCEED.FixedAssetInspection", "StoreCode", c => c.String(nullable: false));
            DropColumn("EXCEED.ItemInspection", "Accepted");
            DropColumn("EXCEED.ItemInspection", "EmployeeId");
            DropColumn("EXCEED.FixedAssetInspection", "Accepted");
            DropColumn("EXCEED.FixedAssetInspection", "EmployeeId");
        }
    }
}
