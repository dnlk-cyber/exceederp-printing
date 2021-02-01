namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cont1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ContractRenewal", "Status", c => c.String());
            AddColumn("EXCEED.ContractRenewal", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ContractRenewal", "VoidBy", c => c.String());
            AddColumn("EXCEED.ContractRenewal", "VoidTime", c => c.DateTime());
            AddColumn("EXCEED.ContractRenewal", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ContractRenewal", "OnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.ContractRenewal", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.ContractRenewal", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ContractRenewal", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.ContractRenewal", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("EXCEED.ContractRenewal", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ContractRenewal", "SendForApprovalBy", c => c.String());
            AddColumn("EXCEED.ContractRenewal", "SendForApprovalTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ContractRenewal", "SendForApprovalTime");
            DropColumn("EXCEED.ContractRenewal", "SendForApprovalBy");
            DropColumn("EXCEED.ContractRenewal", "IsSendForApproval");
            DropColumn("EXCEED.ContractRenewal", "OnlineTransferredTime");
            DropColumn("EXCEED.ContractRenewal", "IsOnlineTransferredBy");
            DropColumn("EXCEED.ContractRenewal", "IsOnlineTransferred");
            DropColumn("EXCEED.ContractRenewal", "OnlineApprovedTime");
            DropColumn("EXCEED.ContractRenewal", "OnlineApprovedBy");
            DropColumn("EXCEED.ContractRenewal", "IsOnlineApproved");
            DropColumn("EXCEED.ContractRenewal", "VoidTime");
            DropColumn("EXCEED.ContractRenewal", "VoidBy");
            DropColumn("EXCEED.ContractRenewal", "IsVoid");
            DropColumn("EXCEED.ContractRenewal", "Status");
        }
    }
}
