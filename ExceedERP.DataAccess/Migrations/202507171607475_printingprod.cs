namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printingprod : DbMigration
    {
        public override void Up()
        {
            AddColumn("Printing.PrintingProductionFollowUp", "Status", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUp", "Remark", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUp", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("Printing.PrintingProductionFollowUp", "VoidBy", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUp", "VoidTime", c => c.DateTime());
            AddColumn("Printing.PrintingProductionFollowUp", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("Printing.PrintingProductionFollowUp", "OnlineApprovedBy", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUp", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("Printing.PrintingProductionFollowUp", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("Printing.PrintingProductionFollowUp", "IsOnlineTransferredBy", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUp", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("Printing.PrintingProductionFollowUp", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Printing.PrintingProductionFollowUp", "OrgBranchName", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUp", "SendForApprovalBy", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUp", "SendForApprovalTime", c => c.DateTime());
            AddColumn("Printing.PrintingProductionFollowUp", "DateCreated", c => c.DateTime());
            AddColumn("Printing.PrintingProductionFollowUp", "LastModified", c => c.DateTime());
            AddColumn("Printing.PrintingProductionFollowUp", "CreatedBy", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUp", "ModifiedBy", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUpTask", "Status", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUpTask", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("Printing.PrintingProductionFollowUpTask", "VoidBy", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUpTask", "VoidTime", c => c.DateTime());
            AddColumn("Printing.PrintingProductionFollowUpTask", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("Printing.PrintingProductionFollowUpTask", "OnlineApprovedBy", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUpTask", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("Printing.PrintingProductionFollowUpTask", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("Printing.PrintingProductionFollowUpTask", "IsOnlineTransferredBy", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUpTask", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("Printing.PrintingProductionFollowUpTask", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Printing.PrintingProductionFollowUpTask", "OrgBranchName", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUpTask", "SendForApprovalBy", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUpTask", "SendForApprovalTime", c => c.DateTime());
            DropColumn("Printing.PrintingProductionFollowUp", "IsPrePressChecked");
            DropColumn("Printing.PrintingProductionFollowUp", "IsPressChecked");
            DropColumn("Printing.PrintingProductionFollowUp", "IsPostPressChecked");
            DropColumn("Printing.PrintingProductionFollowUp", "PrePressCheckedBy");
            DropColumn("Printing.PrintingProductionFollowUp", "PressCheckedBy");
            DropColumn("Printing.PrintingProductionFollowUp", "PostPressCheckedBy");
            DropColumn("Printing.PrintingProductionFollowUp", "PrePressCheckedTime");
            DropColumn("Printing.PrintingProductionFollowUp", "PressCheckedTime");
            DropColumn("Printing.PrintingProductionFollowUp", "PostPressCheckedTime");
            DropColumn("Printing.PrintingProductionFollowUp", "IsPrePressApproved");
            DropColumn("Printing.PrintingProductionFollowUp", "IsPressApproved");
            DropColumn("Printing.PrintingProductionFollowUp", "IsPostPressApproved");
            DropColumn("Printing.PrintingProductionFollowUp", "PrePressApprovedBy");
            DropColumn("Printing.PrintingProductionFollowUp", "PressApprovedBy");
            DropColumn("Printing.PrintingProductionFollowUp", "PostPressApprovedBy");
            DropColumn("Printing.PrintingProductionFollowUp", "PrePressApprovedTime");
            DropColumn("Printing.PrintingProductionFollowUp", "PressApprovedTime");
            DropColumn("Printing.PrintingProductionFollowUp", "PostPressApprovedTime");
        }
        
        public override void Down()
        {
            AddColumn("Printing.PrintingProductionFollowUp", "PostPressApprovedTime", c => c.DateTime());
            AddColumn("Printing.PrintingProductionFollowUp", "PressApprovedTime", c => c.DateTime());
            AddColumn("Printing.PrintingProductionFollowUp", "PrePressApprovedTime", c => c.DateTime());
            AddColumn("Printing.PrintingProductionFollowUp", "PostPressApprovedBy", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUp", "PressApprovedBy", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUp", "PrePressApprovedBy", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUp", "IsPostPressApproved", c => c.Boolean(nullable: false));
            AddColumn("Printing.PrintingProductionFollowUp", "IsPressApproved", c => c.Boolean(nullable: false));
            AddColumn("Printing.PrintingProductionFollowUp", "IsPrePressApproved", c => c.Boolean(nullable: false));
            AddColumn("Printing.PrintingProductionFollowUp", "PostPressCheckedTime", c => c.DateTime());
            AddColumn("Printing.PrintingProductionFollowUp", "PressCheckedTime", c => c.DateTime());
            AddColumn("Printing.PrintingProductionFollowUp", "PrePressCheckedTime", c => c.DateTime());
            AddColumn("Printing.PrintingProductionFollowUp", "PostPressCheckedBy", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUp", "PressCheckedBy", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUp", "PrePressCheckedBy", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUp", "IsPostPressChecked", c => c.Boolean(nullable: false));
            AddColumn("Printing.PrintingProductionFollowUp", "IsPressChecked", c => c.Boolean(nullable: false));
            AddColumn("Printing.PrintingProductionFollowUp", "IsPrePressChecked", c => c.Boolean(nullable: false));
            DropColumn("Printing.PrintingProductionFollowUpTask", "SendForApprovalTime");
            DropColumn("Printing.PrintingProductionFollowUpTask", "SendForApprovalBy");
            DropColumn("Printing.PrintingProductionFollowUpTask", "OrgBranchName");
            DropColumn("Printing.PrintingProductionFollowUpTask", "IsSendForApproval");
            DropColumn("Printing.PrintingProductionFollowUpTask", "OnlineTransferredTime");
            DropColumn("Printing.PrintingProductionFollowUpTask", "IsOnlineTransferredBy");
            DropColumn("Printing.PrintingProductionFollowUpTask", "IsOnlineTransferred");
            DropColumn("Printing.PrintingProductionFollowUpTask", "OnlineApprovedTime");
            DropColumn("Printing.PrintingProductionFollowUpTask", "OnlineApprovedBy");
            DropColumn("Printing.PrintingProductionFollowUpTask", "IsOnlineApproved");
            DropColumn("Printing.PrintingProductionFollowUpTask", "VoidTime");
            DropColumn("Printing.PrintingProductionFollowUpTask", "VoidBy");
            DropColumn("Printing.PrintingProductionFollowUpTask", "IsVoid");
            DropColumn("Printing.PrintingProductionFollowUpTask", "Status");
            DropColumn("Printing.PrintingProductionFollowUp", "ModifiedBy");
            DropColumn("Printing.PrintingProductionFollowUp", "CreatedBy");
            DropColumn("Printing.PrintingProductionFollowUp", "LastModified");
            DropColumn("Printing.PrintingProductionFollowUp", "DateCreated");
            DropColumn("Printing.PrintingProductionFollowUp", "SendForApprovalTime");
            DropColumn("Printing.PrintingProductionFollowUp", "SendForApprovalBy");
            DropColumn("Printing.PrintingProductionFollowUp", "OrgBranchName");
            DropColumn("Printing.PrintingProductionFollowUp", "IsSendForApproval");
            DropColumn("Printing.PrintingProductionFollowUp", "OnlineTransferredTime");
            DropColumn("Printing.PrintingProductionFollowUp", "IsOnlineTransferredBy");
            DropColumn("Printing.PrintingProductionFollowUp", "IsOnlineTransferred");
            DropColumn("Printing.PrintingProductionFollowUp", "OnlineApprovedTime");
            DropColumn("Printing.PrintingProductionFollowUp", "OnlineApprovedBy");
            DropColumn("Printing.PrintingProductionFollowUp", "IsOnlineApproved");
            DropColumn("Printing.PrintingProductionFollowUp", "VoidTime");
            DropColumn("Printing.PrintingProductionFollowUp", "VoidBy");
            DropColumn("Printing.PrintingProductionFollowUp", "IsVoid");
            DropColumn("Printing.PrintingProductionFollowUp", "Remark");
            DropColumn("Printing.PrintingProductionFollowUp", "Status");
        }
    }
}
