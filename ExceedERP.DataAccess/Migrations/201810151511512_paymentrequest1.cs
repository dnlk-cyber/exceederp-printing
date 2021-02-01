namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class paymentrequest1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.PaymentRequest", "EmployeeId", "HRM.Employee");
            DropIndex("EXCEED.PaymentRequest", new[] { "EmployeeId" });
            AddColumn("EXCEED.PaymentRequest", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PaymentRequest", "Status", c => c.String());
            AddColumn("EXCEED.PaymentRequest", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PaymentRequest", "VoidBy", c => c.String());
            AddColumn("EXCEED.PaymentRequest", "VoidTime", c => c.DateTime());
            AddColumn("EXCEED.PaymentRequest", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PaymentRequest", "OnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.PaymentRequest", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.PaymentRequest", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PaymentRequest", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.PaymentRequest", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("EXCEED.PaymentRequest", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PaymentRequest", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.PaymentRequest", "SendForApprovalBy", c => c.String());
            AddColumn("EXCEED.PaymentRequest", "SendForApprovalTime", c => c.DateTime());
            AddColumn("EXCEED.PaymentRequest", "DateCreated", c => c.DateTime());
            AddColumn("EXCEED.PaymentRequest", "LastModified", c => c.DateTime());
            AddColumn("EXCEED.PaymentRequest", "CreatedBy", c => c.String());
            AddColumn("EXCEED.PaymentRequest", "ModifiedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PaymentRequest", "ModifiedBy");
            DropColumn("EXCEED.PaymentRequest", "CreatedBy");
            DropColumn("EXCEED.PaymentRequest", "LastModified");
            DropColumn("EXCEED.PaymentRequest", "DateCreated");
            DropColumn("EXCEED.PaymentRequest", "SendForApprovalTime");
            DropColumn("EXCEED.PaymentRequest", "SendForApprovalBy");
            DropColumn("EXCEED.PaymentRequest", "OrgBranchName");
            DropColumn("EXCEED.PaymentRequest", "IsSendForApproval");
            DropColumn("EXCEED.PaymentRequest", "OnlineTransferredTime");
            DropColumn("EXCEED.PaymentRequest", "IsOnlineTransferredBy");
            DropColumn("EXCEED.PaymentRequest", "IsOnlineTransferred");
            DropColumn("EXCEED.PaymentRequest", "OnlineApprovedTime");
            DropColumn("EXCEED.PaymentRequest", "OnlineApprovedBy");
            DropColumn("EXCEED.PaymentRequest", "IsOnlineApproved");
            DropColumn("EXCEED.PaymentRequest", "VoidTime");
            DropColumn("EXCEED.PaymentRequest", "VoidBy");
            DropColumn("EXCEED.PaymentRequest", "IsVoid");
            DropColumn("EXCEED.PaymentRequest", "Status");
            DropColumn("EXCEED.PaymentRequest", "IsPostedToSecondLedger");
            CreateIndex("EXCEED.PaymentRequest", "EmployeeId");
            AddForeignKey("EXCEED.PaymentRequest", "EmployeeId", "HRM.Employee", "EmployeeId", cascadeDelete: true);
        }
    }
}
