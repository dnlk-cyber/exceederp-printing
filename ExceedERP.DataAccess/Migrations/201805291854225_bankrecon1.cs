namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bankrecon1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "Status", c => c.String());
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "VoidBy", c => c.String());
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "VoidTime", c => c.DateTime());
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "OnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "SendForApprovalBy", c => c.String());
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "SendForApprovalTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "SendForApprovalTime");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "SendForApprovalBy");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "OrgBranchName");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "IsSendForApproval");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "OnlineTransferredTime");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "IsOnlineTransferredBy");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "IsOnlineTransferred");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "OnlineApprovedTime");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "OnlineApprovedBy");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "IsOnlineApproved");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "VoidTime");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "VoidBy");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "IsVoid");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "Status");
        }
    }
}
