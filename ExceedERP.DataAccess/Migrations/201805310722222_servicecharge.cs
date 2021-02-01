namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class servicecharge : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.ReconciliationBankServiceChargeDistribution", "ReconciliationBankServiceChargeId", "EXCEED.ReconciliationBankServiceCharge");
            DropForeignKey("EXCEED.ReconciliationBankServiceChargeValidation", "ReconciliationBankServiceChargeId", "EXCEED.ReconciliationBankServiceCharge");
            DropIndex("EXCEED.ReconciliationBankServiceChargeDistribution", new[] { "ReconciliationBankServiceChargeId" });
            DropIndex("EXCEED.ReconciliationBankServiceChargeValidation", new[] { "ReconciliationBankServiceChargeId" });
            AddColumn("EXCEED.ReconciliationBankServiceChargeDistribution", "ReconciliationScheduleId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ReconciliationBankServiceChargeValidation", "ReconciliationScheduleId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ReconciliationSchedule", "Status", c => c.String());
            AddColumn("EXCEED.ReconciliationSchedule", "Remark", c => c.String());
            AddColumn("EXCEED.ReconciliationSchedule", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ReconciliationSchedule", "VoidBy", c => c.String());
            AddColumn("EXCEED.ReconciliationSchedule", "VoidTime", c => c.DateTime());
            AddColumn("EXCEED.ReconciliationSchedule", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ReconciliationSchedule", "OnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.ReconciliationSchedule", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.ReconciliationSchedule", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ReconciliationSchedule", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.ReconciliationSchedule", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("EXCEED.ReconciliationSchedule", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ReconciliationSchedule", "SendForApprovalBy", c => c.String());
            AddColumn("EXCEED.ReconciliationSchedule", "SendForApprovalTime", c => c.DateTime());
            CreateIndex("EXCEED.ReconciliationBankServiceChargeDistribution", "ReconciliationScheduleId");
            CreateIndex("EXCEED.ReconciliationBankServiceChargeValidation", "ReconciliationScheduleId");
            AddForeignKey("EXCEED.ReconciliationBankServiceChargeDistribution", "ReconciliationScheduleId", "EXCEED.ReconciliationSchedule", "ReconciliationScheduleId", cascadeDelete: true);
            AddForeignKey("EXCEED.ReconciliationBankServiceChargeValidation", "ReconciliationScheduleId", "EXCEED.ReconciliationSchedule", "ReconciliationScheduleId", cascadeDelete: true);
            DropColumn("EXCEED.ReconciliationBankServiceChargeDistribution", "ReconciliationBankServiceChargeId");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "Status");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "IsVoid");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "VoidBy");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "VoidTime");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "IsOnlineApproved");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "OnlineApprovedBy");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "OnlineApprovedTime");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "IsOnlineTransferred");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "IsOnlineTransferredBy");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "OnlineTransferredTime");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "IsSendForApproval");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "SendForApprovalBy");
            DropColumn("EXCEED.ReconciliationBankServiceCharge", "SendForApprovalTime");
            DropColumn("EXCEED.ReconciliationBankServiceChargeValidation", "ReconciliationBankServiceChargeId");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ReconciliationBankServiceChargeValidation", "ReconciliationBankServiceChargeId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "SendForApprovalTime", c => c.DateTime());
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "SendForApprovalBy", c => c.String());
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "OnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "VoidTime", c => c.DateTime());
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "VoidBy", c => c.String());
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ReconciliationBankServiceCharge", "Status", c => c.String());
            AddColumn("EXCEED.ReconciliationBankServiceChargeDistribution", "ReconciliationBankServiceChargeId", c => c.Int(nullable: false));
            DropForeignKey("EXCEED.ReconciliationBankServiceChargeValidation", "ReconciliationScheduleId", "EXCEED.ReconciliationSchedule");
            DropForeignKey("EXCEED.ReconciliationBankServiceChargeDistribution", "ReconciliationScheduleId", "EXCEED.ReconciliationSchedule");
            DropIndex("EXCEED.ReconciliationBankServiceChargeValidation", new[] { "ReconciliationScheduleId" });
            DropIndex("EXCEED.ReconciliationBankServiceChargeDistribution", new[] { "ReconciliationScheduleId" });
            DropColumn("EXCEED.ReconciliationSchedule", "SendForApprovalTime");
            DropColumn("EXCEED.ReconciliationSchedule", "SendForApprovalBy");
            DropColumn("EXCEED.ReconciliationSchedule", "IsSendForApproval");
            DropColumn("EXCEED.ReconciliationSchedule", "OnlineTransferredTime");
            DropColumn("EXCEED.ReconciliationSchedule", "IsOnlineTransferredBy");
            DropColumn("EXCEED.ReconciliationSchedule", "IsOnlineTransferred");
            DropColumn("EXCEED.ReconciliationSchedule", "OnlineApprovedTime");
            DropColumn("EXCEED.ReconciliationSchedule", "OnlineApprovedBy");
            DropColumn("EXCEED.ReconciliationSchedule", "IsOnlineApproved");
            DropColumn("EXCEED.ReconciliationSchedule", "VoidTime");
            DropColumn("EXCEED.ReconciliationSchedule", "VoidBy");
            DropColumn("EXCEED.ReconciliationSchedule", "IsVoid");
            DropColumn("EXCEED.ReconciliationSchedule", "Remark");
            DropColumn("EXCEED.ReconciliationSchedule", "Status");
            DropColumn("EXCEED.ReconciliationBankServiceChargeValidation", "ReconciliationScheduleId");
            DropColumn("EXCEED.ReconciliationBankServiceChargeDistribution", "ReconciliationScheduleId");
            CreateIndex("EXCEED.ReconciliationBankServiceChargeValidation", "ReconciliationBankServiceChargeId");
            CreateIndex("EXCEED.ReconciliationBankServiceChargeDistribution", "ReconciliationBankServiceChargeId");
            AddForeignKey("EXCEED.ReconciliationBankServiceChargeValidation", "ReconciliationBankServiceChargeId", "EXCEED.ReconciliationBankServiceCharge", "ReconciliationBankServiceChargeId", cascadeDelete: true);
            AddForeignKey("EXCEED.ReconciliationBankServiceChargeDistribution", "ReconciliationBankServiceChargeId", "EXCEED.ReconciliationBankServiceCharge", "ReconciliationBankServiceChargeId", cascadeDelete: true);
        }
    }
}
