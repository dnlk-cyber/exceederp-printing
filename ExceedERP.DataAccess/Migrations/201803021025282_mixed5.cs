namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mixed5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ApTravelExpenseClaim", "ForwardedFrom", c => c.String());
            AddColumn("EXCEED.ApTravelExpenseClaim", "ForwardedTo", c => c.String());
            AddColumn("EXCEED.ApTravelExpenseClaim", "IsFinalApproval", c => c.Boolean(nullable: false));
            DropColumn("EXCEED.ApTravelExpensePayment", "Status");
            DropColumn("EXCEED.ApTravelExpensePayment", "IsOnlineApproved");
            DropColumn("EXCEED.ApTravelExpensePayment", "OnlineApprovedBy");
            DropColumn("EXCEED.ApTravelExpensePayment", "OnlineApprovedTime");
            DropColumn("EXCEED.ApTravelExpensePayment", "isVoid");
            DropColumn("EXCEED.ApTravelExpensePayment", "VoidBy");
            DropColumn("EXCEED.ApTravelExpensePayment", "VoidTime");
            DropColumn("EXCEED.ApTravelExpensePayment", "IsSendForApproval");
            DropColumn("EXCEED.ApTravelExpensePayment", "SendForApprovalBy");
            DropColumn("EXCEED.ApTravelExpensePayment", "SendForApprovalTime");
            DropColumn("EXCEED.ApTravelExpenseRefund", "Status");
            DropColumn("EXCEED.ApTravelExpenseRefund", "IsOnlineApproved");
            DropColumn("EXCEED.ApTravelExpenseRefund", "OnlineApprovedBy");
            DropColumn("EXCEED.ApTravelExpenseRefund", "OnlineApprovedTime");
            DropColumn("EXCEED.ApTravelExpenseRefund", "isVoid");
            DropColumn("EXCEED.ApTravelExpenseRefund", "VoidBy");
            DropColumn("EXCEED.ApTravelExpenseRefund", "VoidTime");
            DropColumn("EXCEED.ApTravelExpenseRefund", "IsSendForApproval");
            DropColumn("EXCEED.ApTravelExpenseRefund", "SendForApprovalBy");
            DropColumn("EXCEED.ApTravelExpenseRefund", "SendForApprovalTime");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ApTravelExpenseRefund", "SendForApprovalTime", c => c.DateTime());
            AddColumn("EXCEED.ApTravelExpenseRefund", "SendForApprovalBy", c => c.String());
            AddColumn("EXCEED.ApTravelExpenseRefund", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApTravelExpenseRefund", "VoidTime", c => c.DateTime());
            AddColumn("EXCEED.ApTravelExpenseRefund", "VoidBy", c => c.String());
            AddColumn("EXCEED.ApTravelExpenseRefund", "isVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApTravelExpenseRefund", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.ApTravelExpenseRefund", "OnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.ApTravelExpenseRefund", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApTravelExpenseRefund", "Status", c => c.String());
            AddColumn("EXCEED.ApTravelExpensePayment", "SendForApprovalTime", c => c.DateTime());
            AddColumn("EXCEED.ApTravelExpensePayment", "SendForApprovalBy", c => c.String());
            AddColumn("EXCEED.ApTravelExpensePayment", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApTravelExpensePayment", "VoidTime", c => c.DateTime());
            AddColumn("EXCEED.ApTravelExpensePayment", "VoidBy", c => c.String());
            AddColumn("EXCEED.ApTravelExpensePayment", "isVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApTravelExpensePayment", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.ApTravelExpensePayment", "OnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.ApTravelExpensePayment", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApTravelExpensePayment", "Status", c => c.String());
            DropColumn("EXCEED.ApTravelExpenseClaim", "IsFinalApproval");
            DropColumn("EXCEED.ApTravelExpenseClaim", "ForwardedTo");
            DropColumn("EXCEED.ApTravelExpenseClaim", "ForwardedFrom");
        }
    }
}
