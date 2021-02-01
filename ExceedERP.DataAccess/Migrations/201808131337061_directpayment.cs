namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class directpayment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ApDirectPaymentDistribution",
                c => new
                    {
                        ApDirectPaymentDistributionId = c.Int(nullable: false, identity: true),
                        ApDirectPaymentId = c.Int(nullable: false),
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
                .PrimaryKey(t => t.ApDirectPaymentDistributionId)
                .ForeignKey("EXCEED.ApDirectPayment", t => t.ApDirectPaymentId, cascadeDelete: true)
                .Index(t => t.ApDirectPaymentId);
            
            CreateTable(
                "EXCEED.ApDirectPayment",
                c => new
                    {
                        ApDirectPaymentId = c.Int(nullable: false, identity: true),
                        SourceId = c.String(),
                        SupplierId = c.String(),
                        IsNewSupplier = c.Boolean(nullable: false),
                        SupplierName = c.String(),
                        PaymentMethod = c.String(nullable: false),
                        PaymentType = c.Int(nullable: false),
                        PaymentReference = c.String(),
                        PaymentDate = c.DateTime(nullable: false),
                        Purchaser = c.Int(nullable: false),
                        Grn = c.String(),
                        Po = c.String(),
                        IsDirectPayment = c.Boolean(nullable: false),
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
                .PrimaryKey(t => t.ApDirectPaymentId);
            
            CreateTable(
                "EXCEED.ApDirectPaymentLine",
                c => new
                    {
                        ApDirectPaymentLineId = c.Int(nullable: false, identity: true),
                        ApDirectPaymentId = c.Int(nullable: false),
                        Purpose = c.String(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AccountCode = c.String(),
                        GLTaxRateID = c.Int(nullable: false),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WithholdingType = c.Int(nullable: false),
                        WithholdingAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WithholdTax = c.Boolean(nullable: false),
                        WithholdTaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetPayable = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApDirectPaymentLineId)
                .ForeignKey("EXCEED.ApDirectPayment", t => t.ApDirectPaymentId, cascadeDelete: true)
                .Index(t => t.ApDirectPaymentId);
            
            CreateTable(
                "EXCEED.ApDirectPaymentValidation",
                c => new
                    {
                        ApDirectPaymentValidationId = c.Int(nullable: false, identity: true),
                        ApDirectPaymentId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApDirectPaymentValidationId)
                .ForeignKey("EXCEED.ApDirectPayment", t => t.ApDirectPaymentId, cascadeDelete: true)
                .Index(t => t.ApDirectPaymentId);
            
            CreateTable(
                "EXCEED.ForeignPurchaseCostValidation",
                c => new
                    {
                        ForeignPurchaseCostValidationId = c.Int(nullable: false, identity: true),
                        ForeignPurchaseCostId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ForeignPurchaseCostValidationId)
                .ForeignKey("EXCEED.ForeignPurchaseCost", t => t.ForeignPurchaseCostId, cascadeDelete: true)
                .Index(t => t.ForeignPurchaseCostId);
            
            AddColumn("EXCEED.ForeignPurchaseCost", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ForeignPurchaseCost", "Status", c => c.String());
            AddColumn("EXCEED.ForeignPurchaseCost", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ForeignPurchaseCost", "VoidBy", c => c.String());
            AddColumn("EXCEED.ForeignPurchaseCost", "VoidTime", c => c.DateTime());
            AddColumn("EXCEED.ForeignPurchaseCost", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ForeignPurchaseCost", "OnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.ForeignPurchaseCost", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.ForeignPurchaseCost", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ForeignPurchaseCost", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.ForeignPurchaseCost", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("EXCEED.ForeignPurchaseCost", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ForeignPurchaseCost", "SendForApprovalBy", c => c.String());
            AddColumn("EXCEED.ForeignPurchaseCost", "SendForApprovalTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.ForeignPurchaseCostValidation", "ForeignPurchaseCostId", "EXCEED.ForeignPurchaseCost");
            DropForeignKey("EXCEED.ApDirectPaymentValidation", "ApDirectPaymentId", "EXCEED.ApDirectPayment");
            DropForeignKey("EXCEED.ApDirectPaymentLine", "ApDirectPaymentId", "EXCEED.ApDirectPayment");
            DropForeignKey("EXCEED.ApDirectPaymentDistribution", "ApDirectPaymentId", "EXCEED.ApDirectPayment");
            DropIndex("EXCEED.ForeignPurchaseCostValidation", new[] { "ForeignPurchaseCostId" });
            DropIndex("EXCEED.ApDirectPaymentValidation", new[] { "ApDirectPaymentId" });
            DropIndex("EXCEED.ApDirectPaymentLine", new[] { "ApDirectPaymentId" });
            DropIndex("EXCEED.ApDirectPaymentDistribution", new[] { "ApDirectPaymentId" });
            DropColumn("EXCEED.ForeignPurchaseCost", "SendForApprovalTime");
            DropColumn("EXCEED.ForeignPurchaseCost", "SendForApprovalBy");
            DropColumn("EXCEED.ForeignPurchaseCost", "IsSendForApproval");
            DropColumn("EXCEED.ForeignPurchaseCost", "OnlineTransferredTime");
            DropColumn("EXCEED.ForeignPurchaseCost", "IsOnlineTransferredBy");
            DropColumn("EXCEED.ForeignPurchaseCost", "IsOnlineTransferred");
            DropColumn("EXCEED.ForeignPurchaseCost", "OnlineApprovedTime");
            DropColumn("EXCEED.ForeignPurchaseCost", "OnlineApprovedBy");
            DropColumn("EXCEED.ForeignPurchaseCost", "IsOnlineApproved");
            DropColumn("EXCEED.ForeignPurchaseCost", "VoidTime");
            DropColumn("EXCEED.ForeignPurchaseCost", "VoidBy");
            DropColumn("EXCEED.ForeignPurchaseCost", "IsVoid");
            DropColumn("EXCEED.ForeignPurchaseCost", "Status");
            DropColumn("EXCEED.ForeignPurchaseCost", "IsPostedToSecondLedger");
            DropTable("EXCEED.ForeignPurchaseCostValidation");
            DropTable("EXCEED.ApDirectPaymentValidation");
            DropTable("EXCEED.ApDirectPaymentLine");
            DropTable("EXCEED.ApDirectPayment");
            DropTable("EXCEED.ApDirectPaymentDistribution");
        }
    }
}
