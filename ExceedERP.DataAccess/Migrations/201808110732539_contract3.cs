namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class contract3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ContractAttachement",
                c => new
                    {
                        ContractAttachementId = c.Int(nullable: false, identity: true),
                        ContractManagementID = c.Int(nullable: false),
                        FileName = c.String(),
                        FilePath = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ContractAttachementId)
                .ForeignKey("EXCEED.ContractManagement", t => t.ContractManagementID, cascadeDelete: true)
                .Index(t => t.ContractManagementID);
            
            CreateTable(
                "EXCEED.ContractPayment",
                c => new
                    {
                        ContractPaymentId = c.Int(nullable: false, identity: true),
                        ContractManagementID = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        PaymentAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ApplyPenality = c.Boolean(nullable: false),
                        PenalityAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ForwardedFrom = c.String(),
                        ForwardedTo = c.String(),
                        IsFinalApproval = c.Boolean(nullable: false),
                        IsPostedToSecondLedger = c.Boolean(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        IsOnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ContractPaymentId)
                .ForeignKey("EXCEED.ContractManagement", t => t.ContractManagementID, cascadeDelete: true)
                .Index(t => t.ContractManagementID);
            
            CreateTable(
                "EXCEED.ContractPaymentAttachement",
                c => new
                    {
                        ContractPaymentAttachementId = c.Int(nullable: false, identity: true),
                        ContractPaymentId = c.Int(nullable: false),
                        FileName = c.String(),
                        FilePath = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ContractPaymentAttachementId)
                .ForeignKey("EXCEED.ContractPayment", t => t.ContractPaymentId, cascadeDelete: true)
                .Index(t => t.ContractPaymentId);
            
            CreateTable(
                "EXCEED.ContractPaymentDistribution",
                c => new
                    {
                        ContractPaymentDistributionId = c.Int(nullable: false, identity: true),
                        ContractPaymentId = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        AdditionalDescription = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ContractPaymentDistributionId)
                .ForeignKey("EXCEED.ContractPayment", t => t.ContractPaymentId, cascadeDelete: true)
                .Index(t => t.ContractPaymentId);
            
            CreateTable(
                "EXCEED.ContractPaymentValidation",
                c => new
                    {
                        ContractPaymentValidationId = c.Int(nullable: false, identity: true),
                        ContractPaymentId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ContractPaymentValidationId)
                .ForeignKey("EXCEED.ContractPayment", t => t.ContractPaymentId, cascadeDelete: true)
                .Index(t => t.ContractPaymentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.ContractPaymentValidation", "ContractPaymentId", "EXCEED.ContractPayment");
            DropForeignKey("EXCEED.ContractPaymentDistribution", "ContractPaymentId", "EXCEED.ContractPayment");
            DropForeignKey("EXCEED.ContractPaymentAttachement", "ContractPaymentId", "EXCEED.ContractPayment");
            DropForeignKey("EXCEED.ContractPayment", "ContractManagementID", "EXCEED.ContractManagement");
            DropForeignKey("EXCEED.ContractAttachement", "ContractManagementID", "EXCEED.ContractManagement");
            DropIndex("EXCEED.ContractPaymentValidation", new[] { "ContractPaymentId" });
            DropIndex("EXCEED.ContractPaymentDistribution", new[] { "ContractPaymentId" });
            DropIndex("EXCEED.ContractPaymentAttachement", new[] { "ContractPaymentId" });
            DropIndex("EXCEED.ContractPayment", new[] { "ContractManagementID" });
            DropIndex("EXCEED.ContractAttachement", new[] { "ContractManagementID" });
            DropTable("EXCEED.ContractPaymentValidation");
            DropTable("EXCEED.ContractPaymentDistribution");
            DropTable("EXCEED.ContractPaymentAttachement");
            DropTable("EXCEED.ContractPayment");
            DropTable("EXCEED.ContractAttachement");
        }
    }
}
