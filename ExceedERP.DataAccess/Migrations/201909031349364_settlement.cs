namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class settlement : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ApPaymentSettlementDistribution",
                c => new
                    {
                        ApPaymentSettlementDistributionId = c.Int(nullable: false, identity: true),
                        ApPaymentSettlementId = c.Int(nullable: false),
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
                .PrimaryKey(t => t.ApPaymentSettlementDistributionId)
                .ForeignKey("EXCEED.ApPaymentSettlement", t => t.ApPaymentSettlementId, cascadeDelete: true)
                .Index(t => t.ApPaymentSettlementId);
            
            CreateTable(
                "EXCEED.ApPaymentSettlement",
                c => new
                    {
                        ApPaymentSettlementId = c.Int(nullable: false, identity: true),
                        SupplierId = c.String(),
                        IsNewSupplier = c.Boolean(nullable: false),
                        SupplierName = c.String(),
                        Reference = c.String(),
                        PaymentDate = c.DateTime(nullable: false),
                        Po = c.String(),
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
                .PrimaryKey(t => t.ApPaymentSettlementId);
            
            CreateTable(
                "EXCEED.ApPaymentSettlementLine",
                c => new
                    {
                        ApPaymentSettlementLineId = c.Int(nullable: false, identity: true),
                        ApPaymentSettlementId = c.Int(nullable: false),
                        ApDirectPaymentId = c.Int(nullable: false),
                        Purpose = c.String(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AccountCode = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApPaymentSettlementLineId)
                .ForeignKey("EXCEED.ApDirectPayment", t => t.ApDirectPaymentId, cascadeDelete: true)
                .ForeignKey("EXCEED.ApPaymentSettlement", t => t.ApPaymentSettlementId, cascadeDelete: true)
                .Index(t => t.ApPaymentSettlementId)
                .Index(t => t.ApDirectPaymentId);
            
            CreateTable(
                "EXCEED.ApPaymentSettlementValidation",
                c => new
                    {
                        ApPaymentSettlementValidationId = c.Int(nullable: false, identity: true),
                        ApPaymentSettlementId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApPaymentSettlementValidationId)
                .ForeignKey("EXCEED.ApPaymentSettlement", t => t.ApPaymentSettlementId, cascadeDelete: true)
                .Index(t => t.ApPaymentSettlementId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.ApPaymentSettlementValidation", "ApPaymentSettlementId", "EXCEED.ApPaymentSettlement");
            DropForeignKey("EXCEED.ApPaymentSettlementLine", "ApPaymentSettlementId", "EXCEED.ApPaymentSettlement");
            DropForeignKey("EXCEED.ApPaymentSettlementLine", "ApDirectPaymentId", "EXCEED.ApDirectPayment");
            DropForeignKey("EXCEED.ApPaymentSettlementDistribution", "ApPaymentSettlementId", "EXCEED.ApPaymentSettlement");
            DropIndex("EXCEED.ApPaymentSettlementValidation", new[] { "ApPaymentSettlementId" });
            DropIndex("EXCEED.ApPaymentSettlementLine", new[] { "ApDirectPaymentId" });
            DropIndex("EXCEED.ApPaymentSettlementLine", new[] { "ApPaymentSettlementId" });
            DropIndex("EXCEED.ApPaymentSettlementDistribution", new[] { "ApPaymentSettlementId" });
            DropTable("EXCEED.ApPaymentSettlementValidation");
            DropTable("EXCEED.ApPaymentSettlementLine");
            DropTable("EXCEED.ApPaymentSettlement");
            DropTable("EXCEED.ApPaymentSettlementDistribution");
        }
    }
}
