namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class aptravel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ApTravelExpenseClaim",
                c => new
                    {
                        ApTravelExpenseClaimId = c.Int(nullable: false, identity: true),
                        ApTravelOrderId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        BranchId = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        PurposeOfTravel = c.String(),
                        ClaimDate = c.DateTime(nullable: false),
                        PaymentMethod = c.String(nullable: false),
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
                .PrimaryKey(t => t.ApTravelExpenseClaimId)
                .ForeignKey("EXCEED.ApTravelOrder", t => t.ApTravelOrderId, cascadeDelete: true)
                .Index(t => t.ApTravelOrderId);
            
            CreateTable(
                "EXCEED.ApTravelExpenseDistribution",
                c => new
                    {
                        ApTravelExpenseDistributionId = c.Int(nullable: false, identity: true),
                        ApTravelExpenseClaimId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApTravelExpenseDistributionId)
                .ForeignKey("EXCEED.ApTravelExpenseClaim", t => t.ApTravelExpenseClaimId, cascadeDelete: true)
                .Index(t => t.ApTravelExpenseClaimId);
            
            CreateTable(
                "EXCEED.ApTravelExpenseExpense",
                c => new
                    {
                        ApTravelExpenseExpenseId = c.Int(nullable: false, identity: true),
                        ApTravelExpenseClaimId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        Description = c.String(),
                        UoM = c.String(),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApTravelExpenseExpenseId)
                .ForeignKey("EXCEED.ApTravelExpenseClaim", t => t.ApTravelExpenseClaimId, cascadeDelete: true)
                .Index(t => t.ApTravelExpenseClaimId);
            
            CreateTable(
                "EXCEED.ApTravelExpensePayment",
                c => new
                    {
                        ApTravelExpensePaymentId = c.Int(nullable: false, identity: true),
                        ApTravelExpenseClaimId = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        AccountCode = c.String(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentMethod = c.String(nullable: false),
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
                .PrimaryKey(t => t.ApTravelExpensePaymentId)
                .ForeignKey("EXCEED.ApTravelExpenseClaim", t => t.ApTravelExpenseClaimId, cascadeDelete: true)
                .Index(t => t.ApTravelExpenseClaimId);
            
            CreateTable(
                "EXCEED.ApTravelExpenseRefund",
                c => new
                    {
                        ApTravelExpenseRefundId = c.Int(nullable: false, identity: true),
                        ApTravelExpenseClaimId = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        AccountCode = c.String(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentMethod = c.String(nullable: false),
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
                .PrimaryKey(t => t.ApTravelExpenseRefundId)
                .ForeignKey("EXCEED.ApTravelExpenseClaim", t => t.ApTravelExpenseClaimId, cascadeDelete: true)
                .Index(t => t.ApTravelExpenseClaimId);
            
            CreateTable(
                "EXCEED.ApTravelOrder",
                c => new
                    {
                        ApTravelOrderId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        BranchId = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        PurposeOfTravel = c.String(),
                        RequestDate = c.DateTime(nullable: false),
                        AdvanceAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentMethod = c.String(nullable: false),
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
                .PrimaryKey(t => t.ApTravelOrderId);
            
            CreateTable(
                "EXCEED.ApTravelOrderAdvancePayment",
                c => new
                    {
                        ApTravelOrderAdvancePaymentId = c.Int(nullable: false, identity: true),
                        ApTravelOrderId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        Description = c.String(),
                        UoM = c.String(),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApTravelOrderAdvancePaymentId)
                .ForeignKey("EXCEED.ApTravelOrder", t => t.ApTravelOrderId, cascadeDelete: true)
                .Index(t => t.ApTravelOrderId);
            
            CreateTable(
                "EXCEED.ApTravelOrderDeparture",
                c => new
                    {
                        ApTravelOrderDepartureId = c.Int(nullable: false, identity: true),
                        ApTravelOrderId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        From = c.String(),
                        To = c.String(),
                        Date = c.DateTime(nullable: false),
                        Time = c.DateTime(nullable: false),
                        TransportMode = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApTravelOrderDepartureId)
                .ForeignKey("EXCEED.ApTravelOrder", t => t.ApTravelOrderId, cascadeDelete: true)
                .Index(t => t.ApTravelOrderId);
            
            CreateTable(
                "EXCEED.ApTravelOrderDistribution",
                c => new
                    {
                        ApTravelOrderDistributionId = c.Int(nullable: false, identity: true),
                        ApPettyCashId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        ApTravelOrder_ApTravelOrderId = c.Int(),
                    })
                .PrimaryKey(t => t.ApTravelOrderDistributionId)
                .ForeignKey("EXCEED.ApPettyCash", t => t.ApPettyCashId, cascadeDelete: true)
                .ForeignKey("EXCEED.ApTravelOrder", t => t.ApTravelOrder_ApTravelOrderId)
                .Index(t => t.ApPettyCashId)
                .Index(t => t.ApTravelOrder_ApTravelOrderId);
            
            CreateTable(
                "EXCEED.ApTravelOrderValidation",
                c => new
                    {
                        ApTravelOrderValidationId = c.Int(nullable: false, identity: true),
                        ApTravelOrderId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApTravelOrderValidationId)
                .ForeignKey("EXCEED.ApTravelOrder", t => t.ApTravelOrderId, cascadeDelete: true)
                .Index(t => t.ApTravelOrderId);
            
            CreateTable(
                "EXCEED.ApTravelExpenseValidation",
                c => new
                    {
                        ApTravelExpenseValidationId = c.Int(nullable: false, identity: true),
                        ApTravelOrderId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        ApTravelExpenseClaim_ApTravelExpenseClaimId = c.Int(),
                    })
                .PrimaryKey(t => t.ApTravelExpenseValidationId)
                .ForeignKey("EXCEED.ApTravelOrder", t => t.ApTravelOrderId, cascadeDelete: true)
                .ForeignKey("EXCEED.ApTravelExpenseClaim", t => t.ApTravelExpenseClaim_ApTravelExpenseClaimId)
                .Index(t => t.ApTravelOrderId)
                .Index(t => t.ApTravelExpenseClaim_ApTravelExpenseClaimId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.ApTravelExpenseValidation", "ApTravelExpenseClaim_ApTravelExpenseClaimId", "EXCEED.ApTravelExpenseClaim");
            DropForeignKey("EXCEED.ApTravelExpenseValidation", "ApTravelOrderId", "EXCEED.ApTravelOrder");
            DropForeignKey("EXCEED.ApTravelExpenseClaim", "ApTravelOrderId", "EXCEED.ApTravelOrder");
            DropForeignKey("EXCEED.ApTravelOrderValidation", "ApTravelOrderId", "EXCEED.ApTravelOrder");
            DropForeignKey("EXCEED.ApTravelOrderDistribution", "ApTravelOrder_ApTravelOrderId", "EXCEED.ApTravelOrder");
            DropForeignKey("EXCEED.ApTravelOrderDistribution", "ApPettyCashId", "EXCEED.ApPettyCash");
            DropForeignKey("EXCEED.ApTravelOrderDeparture", "ApTravelOrderId", "EXCEED.ApTravelOrder");
            DropForeignKey("EXCEED.ApTravelOrderAdvancePayment", "ApTravelOrderId", "EXCEED.ApTravelOrder");
            DropForeignKey("EXCEED.ApTravelExpenseRefund", "ApTravelExpenseClaimId", "EXCEED.ApTravelExpenseClaim");
            DropForeignKey("EXCEED.ApTravelExpensePayment", "ApTravelExpenseClaimId", "EXCEED.ApTravelExpenseClaim");
            DropForeignKey("EXCEED.ApTravelExpenseExpense", "ApTravelExpenseClaimId", "EXCEED.ApTravelExpenseClaim");
            DropForeignKey("EXCEED.ApTravelExpenseDistribution", "ApTravelExpenseClaimId", "EXCEED.ApTravelExpenseClaim");
            DropIndex("EXCEED.ApTravelExpenseValidation", new[] { "ApTravelExpenseClaim_ApTravelExpenseClaimId" });
            DropIndex("EXCEED.ApTravelExpenseValidation", new[] { "ApTravelOrderId" });
            DropIndex("EXCEED.ApTravelOrderValidation", new[] { "ApTravelOrderId" });
            DropIndex("EXCEED.ApTravelOrderDistribution", new[] { "ApTravelOrder_ApTravelOrderId" });
            DropIndex("EXCEED.ApTravelOrderDistribution", new[] { "ApPettyCashId" });
            DropIndex("EXCEED.ApTravelOrderDeparture", new[] { "ApTravelOrderId" });
            DropIndex("EXCEED.ApTravelOrderAdvancePayment", new[] { "ApTravelOrderId" });
            DropIndex("EXCEED.ApTravelExpenseRefund", new[] { "ApTravelExpenseClaimId" });
            DropIndex("EXCEED.ApTravelExpensePayment", new[] { "ApTravelExpenseClaimId" });
            DropIndex("EXCEED.ApTravelExpenseExpense", new[] { "ApTravelExpenseClaimId" });
            DropIndex("EXCEED.ApTravelExpenseDistribution", new[] { "ApTravelExpenseClaimId" });
            DropIndex("EXCEED.ApTravelExpenseClaim", new[] { "ApTravelOrderId" });
            DropTable("EXCEED.ApTravelExpenseValidation");
            DropTable("EXCEED.ApTravelOrderValidation");
            DropTable("EXCEED.ApTravelOrderDistribution");
            DropTable("EXCEED.ApTravelOrderDeparture");
            DropTable("EXCEED.ApTravelOrderAdvancePayment");
            DropTable("EXCEED.ApTravelOrder");
            DropTable("EXCEED.ApTravelExpenseRefund");
            DropTable("EXCEED.ApTravelExpensePayment");
            DropTable("EXCEED.ApTravelExpenseExpense");
            DropTable("EXCEED.ApTravelExpenseDistribution");
            DropTable("EXCEED.ApTravelExpenseClaim");
        }
    }
}
