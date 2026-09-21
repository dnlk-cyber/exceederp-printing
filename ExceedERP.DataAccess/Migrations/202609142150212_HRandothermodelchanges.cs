namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HRandothermodelchanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.PaymentRequestLine", "GLTaxRateID", "EXCEED.GLTaxRate");
            DropIndex("EXCEED.PaymentRequestLine", new[] { "GLTaxRateID" });
            AddColumn("EXCEED.PaymentRequest", "BankAccount", c => c.String());
            AddColumn("EXCEED.PaymentRequest", "IsAuditOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PaymentRequest", "AuditOnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.PaymentRequest", "AuditOnlineApprovedTime", c => c.DateTime());
            AddColumn("HRM.LeavePayableDetails", "CurrentYear", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.LeavePayableSettings", "IncludeCurrentFiscalYearDays", c => c.Boolean(nullable: false));
            AddColumn("HRM.LeavePayableSettings", "OrgBranchName", c => c.String());
            AddColumn("HRM.LeavePayableSettings", "DateCreated", c => c.DateTime());
            AddColumn("HRM.LeavePayableSettings", "LastModified", c => c.DateTime());
            AddColumn("HRM.LeavePayableSettings", "CreatedBy", c => c.String());
            AddColumn("HRM.LeavePayableSettings", "ModifiedBy", c => c.String());
            AddColumn("HRM.WeekendExceptions", "BranchId", c => c.Int());
            AddColumn("HRM.WeekendExceptions", "IsHalfDay", c => c.Boolean(nullable: false));
            AddColumn("HRM.WeekendExceptions", "IsFullDay", c => c.Boolean(nullable: false));
            AlterColumn("EXCEED.PaymentRequestLine", "GLTaxRateID", c => c.Int());
            CreateIndex("EXCEED.PaymentRequestLine", "GLTaxRateID");
            AddForeignKey("EXCEED.PaymentRequestLine", "GLTaxRateID", "EXCEED.GLTaxRate", "GLTaxRateID");
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.PaymentRequestLine", "GLTaxRateID", "EXCEED.GLTaxRate");
            DropIndex("EXCEED.PaymentRequestLine", new[] { "GLTaxRateID" });
            AlterColumn("EXCEED.PaymentRequestLine", "GLTaxRateID", c => c.Int(nullable: false));
            DropColumn("HRM.WeekendExceptions", "IsFullDay");
            DropColumn("HRM.WeekendExceptions", "IsHalfDay");
            DropColumn("HRM.WeekendExceptions", "BranchId");
            DropColumn("HRM.LeavePayableSettings", "ModifiedBy");
            DropColumn("HRM.LeavePayableSettings", "CreatedBy");
            DropColumn("HRM.LeavePayableSettings", "LastModified");
            DropColumn("HRM.LeavePayableSettings", "DateCreated");
            DropColumn("HRM.LeavePayableSettings", "OrgBranchName");
            DropColumn("HRM.LeavePayableSettings", "IncludeCurrentFiscalYearDays");
            DropColumn("HRM.LeavePayableDetails", "CurrentYear");
            DropColumn("EXCEED.PaymentRequest", "AuditOnlineApprovedTime");
            DropColumn("EXCEED.PaymentRequest", "AuditOnlineApprovedBy");
            DropColumn("EXCEED.PaymentRequest", "IsAuditOnlineApproved");
            DropColumn("EXCEED.PaymentRequest", "BankAccount");
            CreateIndex("EXCEED.PaymentRequestLine", "GLTaxRateID");
            AddForeignKey("EXCEED.PaymentRequestLine", "GLTaxRateID", "EXCEED.GLTaxRate", "GLTaxRateID", cascadeDelete: true);
        }
    }
}
