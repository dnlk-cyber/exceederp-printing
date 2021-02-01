namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pettycash : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.ApPaybleInvoices", "APSupplierRegistration_APSupplierRegistrationID", "EXCEED.APSupplierRegistration");
            DropIndex("EXCEED.ApPaybleInvoices", new[] { "APSupplierRegistration_APSupplierRegistrationID" });
            CreateTable(
                "EXCEED.ApPettyCash",
                c => new
                    {
                        ApPettyCashId = c.Int(nullable: false, identity: true),
                        ApCashierSetupId = c.Int(nullable: false),
                        PurposeOfPayment = c.String(),
                        Date = c.DateTime(nullable: false),
                        Reference = c.String(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        OnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        Status = c.String(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApPettyCashId)
                .ForeignKey("EXCEED.ApCashierSetup", t => t.ApCashierSetupId, cascadeDelete: true)
                .Index(t => t.ApCashierSetupId);
            
            CreateTable(
                "EXCEED.ApCashierSetup",
                c => new
                    {
                        ApCashierSetupId = c.Int(nullable: false, identity: true),
                        Cashier = c.String(nullable: false),
                        Description = c.String(),
                        BranchId = c.Int(nullable: false),
                        AccountCode = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        ActiveInactiveTime = c.DateTime(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApCashierSetupId);
            
            CreateTable(
                "EXCEED.ApPettyCashDistribution",
                c => new
                    {
                        ApPettyCashDistributionId = c.Int(nullable: false, identity: true),
                        ApPettyCashId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApPettyCashDistributionId)
                .ForeignKey("EXCEED.ApPettyCash", t => t.ApPettyCashId, cascadeDelete: true)
                .Index(t => t.ApPettyCashId);
            
            CreateTable(
                "EXCEED.ApPettyCashPayment",
                c => new
                    {
                        ApPettyCashPaymentId = c.Int(nullable: false, identity: true),
                        ApPettyCashId = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        PaymentMethod = c.String(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Reference = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApPettyCashPaymentId)
                .ForeignKey("EXCEED.ApPettyCash", t => t.ApPettyCashId, cascadeDelete: true)
                .Index(t => t.ApPettyCashId);
            
            CreateTable(
                "EXCEED.ApPettyCashUtilization",
                c => new
                    {
                        ApPettyCashUtilizationId = c.Int(nullable: false, identity: true),
                        ApPettyCashId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        PaidTo = c.String(nullable: false),
                        Purpose = c.String(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AccountCode = c.String(nullable: false),
                        Reference = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApPettyCashUtilizationId)
                .ForeignKey("EXCEED.ApPettyCash", t => t.ApPettyCashId, cascadeDelete: true)
                .Index(t => t.ApPettyCashId);
            
            CreateTable(
                "EXCEED.ApPettyCashValidation",
                c => new
                    {
                        ApPettyCashValidationId = c.Int(nullable: false, identity: true),
                        ApPettyCashId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApPettyCashValidationId)
                .ForeignKey("EXCEED.ApPettyCash", t => t.ApPettyCashId, cascadeDelete: true)
                .Index(t => t.ApPettyCashId);
            
            DropColumn("EXCEED.ApPaybleInvoices", "APSupplierRegistration_APSupplierRegistrationID");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ApPaybleInvoices", "APSupplierRegistration_APSupplierRegistrationID", c => c.Int());
            DropForeignKey("EXCEED.ApPettyCashValidation", "ApPettyCashId", "EXCEED.ApPettyCash");
            DropForeignKey("EXCEED.ApPettyCashUtilization", "ApPettyCashId", "EXCEED.ApPettyCash");
            DropForeignKey("EXCEED.ApPettyCashPayment", "ApPettyCashId", "EXCEED.ApPettyCash");
            DropForeignKey("EXCEED.ApPettyCashDistribution", "ApPettyCashId", "EXCEED.ApPettyCash");
            DropForeignKey("EXCEED.ApPettyCash", "ApCashierSetupId", "EXCEED.ApCashierSetup");
            DropIndex("EXCEED.ApPettyCashValidation", new[] { "ApPettyCashId" });
            DropIndex("EXCEED.ApPettyCashUtilization", new[] { "ApPettyCashId" });
            DropIndex("EXCEED.ApPettyCashPayment", new[] { "ApPettyCashId" });
            DropIndex("EXCEED.ApPettyCashDistribution", new[] { "ApPettyCashId" });
            DropIndex("EXCEED.ApPettyCash", new[] { "ApCashierSetupId" });
            DropTable("EXCEED.ApPettyCashValidation");
            DropTable("EXCEED.ApPettyCashUtilization");
            DropTable("EXCEED.ApPettyCashPayment");
            DropTable("EXCEED.ApPettyCashDistribution");
            DropTable("EXCEED.ApCashierSetup");
            DropTable("EXCEED.ApPettyCash");
            CreateIndex("EXCEED.ApPaybleInvoices", "APSupplierRegistration_APSupplierRegistrationID");
            AddForeignKey("EXCEED.ApPaybleInvoices", "APSupplierRegistration_APSupplierRegistrationID", "EXCEED.APSupplierRegistration", "APSupplierRegistrationID");
        }
    }
}
