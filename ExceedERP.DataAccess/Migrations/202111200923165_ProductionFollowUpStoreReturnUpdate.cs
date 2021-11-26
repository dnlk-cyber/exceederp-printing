namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductionFollowUpStoreReturnUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("ProductFollowUp.ProductDelivery", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("ProductFollowUp.ProductDelivery", "Status", c => c.String());
            AddColumn("ProductFollowUp.ProductDelivery", "Remark", c => c.String());
            AddColumn("ProductFollowUp.ProductDelivery", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("ProductFollowUp.ProductDelivery", "VoidBy", c => c.String());
            AddColumn("ProductFollowUp.ProductDelivery", "VoidTime", c => c.DateTime());
            AddColumn("ProductFollowUp.ProductDelivery", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("ProductFollowUp.ProductDelivery", "OnlineApprovedBy", c => c.String());
            AddColumn("ProductFollowUp.ProductDelivery", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("ProductFollowUp.ProductDelivery", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("ProductFollowUp.ProductDelivery", "IsOnlineTransferredBy", c => c.String());
            AddColumn("ProductFollowUp.ProductDelivery", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("ProductFollowUp.ProductDelivery", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("ProductFollowUp.ProductDelivery", "OrgBranchName", c => c.String());
            AddColumn("ProductFollowUp.ProductDelivery", "SendForApprovalBy", c => c.String());
            AddColumn("ProductFollowUp.ProductDelivery", "SendForApprovalTime", c => c.DateTime());
            AddColumn("ProductFollowUp.ProductDelivery", "DateCreated", c => c.DateTime());
            AddColumn("ProductFollowUp.ProductDelivery", "LastModified", c => c.DateTime());
            AddColumn("ProductFollowUp.ProductDelivery", "CreatedBy", c => c.String());
            AddColumn("ProductFollowUp.ProductDelivery", "ModifiedBy", c => c.String());
            AddColumn("EXCEED.StoreReturn", "IssueNumber", c => c.String());
            AddColumn("EXCEED.StoreReturn", "JobOrderId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.StoreReturn", "JobOrderId");
            DropColumn("EXCEED.StoreReturn", "IssueNumber");
            DropColumn("ProductFollowUp.ProductDelivery", "ModifiedBy");
            DropColumn("ProductFollowUp.ProductDelivery", "CreatedBy");
            DropColumn("ProductFollowUp.ProductDelivery", "LastModified");
            DropColumn("ProductFollowUp.ProductDelivery", "DateCreated");
            DropColumn("ProductFollowUp.ProductDelivery", "SendForApprovalTime");
            DropColumn("ProductFollowUp.ProductDelivery", "SendForApprovalBy");
            DropColumn("ProductFollowUp.ProductDelivery", "OrgBranchName");
            DropColumn("ProductFollowUp.ProductDelivery", "IsSendForApproval");
            DropColumn("ProductFollowUp.ProductDelivery", "OnlineTransferredTime");
            DropColumn("ProductFollowUp.ProductDelivery", "IsOnlineTransferredBy");
            DropColumn("ProductFollowUp.ProductDelivery", "IsOnlineTransferred");
            DropColumn("ProductFollowUp.ProductDelivery", "OnlineApprovedTime");
            DropColumn("ProductFollowUp.ProductDelivery", "OnlineApprovedBy");
            DropColumn("ProductFollowUp.ProductDelivery", "IsOnlineApproved");
            DropColumn("ProductFollowUp.ProductDelivery", "VoidTime");
            DropColumn("ProductFollowUp.ProductDelivery", "VoidBy");
            DropColumn("ProductFollowUp.ProductDelivery", "IsVoid");
            DropColumn("ProductFollowUp.ProductDelivery", "Remark");
            DropColumn("ProductFollowUp.ProductDelivery", "Status");
            DropColumn("ProductFollowUp.ProductDelivery", "IsPostedToSecondLedger");
        }
    }
}
