namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class financeprocupgrade : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.FundTransferAttachment",
                c => new
                    {
                        FundTransferAttachmentId = c.Int(nullable: false, identity: true),
                        FundTransferId = c.Int(nullable: false),
                        FileName = c.String(),
                        FilePath = c.String(),
                    })
                .PrimaryKey(t => t.FundTransferAttachmentId)
                .ForeignKey("EXCEED.FundTransfer", t => t.FundTransferId, cascadeDelete: true)
                .Index(t => t.FundTransferId);
            
            CreateTable(
                "EXCEED.FundTransfer",
                c => new
                    {
                        FundTransferId = c.Int(nullable: false, identity: true),
                        ReasonOfTransfer = c.String(),
                        RequestDate = c.DateTime(nullable: false),
                        BranchId = c.Int(nullable: false),
                        BusinessUnitId = c.Int(nullable: false),
                        FromBranchId = c.Int(nullable: false),
                        FromBusinessUnitId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        OrgStuructureId = c.Int(nullable: false),
                        RequestorBranchId = c.Int(nullable: false),
                        RequestorBusinessUnitId = c.Int(nullable: false),
                        BankAccountId = c.Int(nullable: false),
                        Paid = c.Boolean(nullable: false),
                        IsBudgetApproved = c.Boolean(nullable: false),
                        BudgetApprovedBy = c.String(),
                        BudgetApprovedTime = c.DateTime(),
                        IsDepartmentOnlineApproved = c.Boolean(nullable: false),
                        DepartmentOnlineApprovedBy = c.String(),
                        DepartmentOnlineApprovedTime = c.DateTime(),
                        IsDepartmentSendForApproval = c.Boolean(nullable: false),
                        DepartmentSendForApprovalBy = c.String(),
                        DepartmentSendForApprovalTime = c.DateTime(),
                        ForwardedFrom = c.String(),
                        ForwardedTo = c.String(),
                        IsFinalApproval = c.Boolean(nullable: false),
                        CurrentApprovalStep = c.Int(),
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
                .PrimaryKey(t => t.FundTransferId);
            
            CreateTable(
                "EXCEED.FundTransferBudgetValidation",
                c => new
                    {
                        FundTransferBudgetValidationId = c.Int(nullable: false, identity: true),
                        FundTransferId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FundTransferBudgetValidationId)
                .ForeignKey("EXCEED.FundTransfer", t => t.FundTransferId, cascadeDelete: true)
                .Index(t => t.FundTransferId);
            
            CreateTable(
                "EXCEED.FundTransferDepartmentValidation",
                c => new
                    {
                        FundTransferDepartmentValidationId = c.Int(nullable: false, identity: true),
                        FundTransferId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FundTransferDepartmentValidationId)
                .ForeignKey("EXCEED.FundTransfer", t => t.FundTransferId, cascadeDelete: true)
                .Index(t => t.FundTransferId);
            
            CreateTable(
                "EXCEED.FundTransferDetail",
                c => new
                    {
                        FundTransferDetailId = c.Int(nullable: false, identity: true),
                        FundTransferId = c.Int(nullable: false),
                        Description = c.String(),
                        Requested = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Approved = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FundTransferDetailId)
                .ForeignKey("EXCEED.FundTransfer", t => t.FundTransferId, cascadeDelete: true)
                .Index(t => t.FundTransferId);
            
            CreateTable(
                "EXCEED.FundTransferValidation",
                c => new
                    {
                        FundTransferValidationId = c.Int(nullable: false, identity: true),
                        FundTransferId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FundTransferValidationId)
                .ForeignKey("EXCEED.FundTransfer", t => t.FundTransferId, cascadeDelete: true)
                .Index(t => t.FundTransferId);
            
            CreateTable(
                "EXCEED.ProformaTeam",
                c => new
                    {
                        ProformaTeamId = c.Int(nullable: false, identity: true),
                        ProformaPurchaseID = c.Int(nullable: false),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProformaTeamId)
                .ForeignKey("EXCEED.ProformaPurchase", t => t.ProformaPurchaseID, cascadeDelete: true)
                .Index(t => t.ProformaPurchaseID);
            
            CreateTable(
                "EXCEED.ProformaTeamDecision",
                c => new
                    {
                        ProformaTeamDecisionId = c.Int(nullable: false, identity: true),
                        ProformaTeamId = c.Int(nullable: false),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProformaTeamDecisionId)
                .ForeignKey("EXCEED.ProformaTeam", t => t.ProformaTeamId, cascadeDelete: true)
                .Index(t => t.ProformaTeamId);
            
            CreateTable(
                "EXCEED.ProformaTeamMember",
                c => new
                    {
                        ProformaTeamMemberId = c.Int(nullable: false, identity: true),
                        ProformaTeamId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        SignedBy = c.String(),
                        SginTime = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProformaTeamMemberId)
                .ForeignKey("EXCEED.ProformaTeam", t => t.ProformaTeamId, cascadeDelete: true)
                .Index(t => t.ProformaTeamId);
            
            CreateTable(
                "EXCEED.PurchasePaymentFinanceValidation",
                c => new
                    {
                        PurchasePaymentFinanceValidationId = c.Int(nullable: false, identity: true),
                        PurchasePaymentId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchasePaymentFinanceValidationId)
                .ForeignKey("EXCEED.PurchasePayment", t => t.PurchasePaymentId, cascadeDelete: true)
                .Index(t => t.PurchasePaymentId);
            
            AddColumn("EXCEED.PurchasePayment", "TenderRef", c => c.String());
            AddColumn("EXCEED.PurchasePayment", "ProformaRef", c => c.String());
            AddColumn("EXCEED.PurchasePayment", "IsFinanceSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchasePayment", "FinanceSendForApprovalBy", c => c.String());
            AddColumn("EXCEED.PurchasePayment", "FinanceSendForApprovalTime", c => c.DateTime());
            AddColumn("EXCEED.PurchasePayment", "IsFinanceApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchasePayment", "FinanceApprovedBy", c => c.String());
            AddColumn("EXCEED.PurchasePayment", "FinanceApprovedTime", c => c.DateTime());
            AlterColumn("EXCEED.GLTaxRate", "Rate", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.GLTaxRate", "CalculatedRate", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.PurchasePaymentFinanceValidation", "PurchasePaymentId", "EXCEED.PurchasePayment");
            DropForeignKey("EXCEED.ProformaTeamMember", "ProformaTeamId", "EXCEED.ProformaTeam");
            DropForeignKey("EXCEED.ProformaTeamDecision", "ProformaTeamId", "EXCEED.ProformaTeam");
            DropForeignKey("EXCEED.ProformaTeam", "ProformaPurchaseID", "EXCEED.ProformaPurchase");
            DropForeignKey("EXCEED.FundTransferValidation", "FundTransferId", "EXCEED.FundTransfer");
            DropForeignKey("EXCEED.FundTransferDetail", "FundTransferId", "EXCEED.FundTransfer");
            DropForeignKey("EXCEED.FundTransferDepartmentValidation", "FundTransferId", "EXCEED.FundTransfer");
            DropForeignKey("EXCEED.FundTransferBudgetValidation", "FundTransferId", "EXCEED.FundTransfer");
            DropForeignKey("EXCEED.FundTransferAttachment", "FundTransferId", "EXCEED.FundTransfer");
            DropIndex("EXCEED.PurchasePaymentFinanceValidation", new[] { "PurchasePaymentId" });
            DropIndex("EXCEED.ProformaTeamMember", new[] { "ProformaTeamId" });
            DropIndex("EXCEED.ProformaTeamDecision", new[] { "ProformaTeamId" });
            DropIndex("EXCEED.ProformaTeam", new[] { "ProformaPurchaseID" });
            DropIndex("EXCEED.FundTransferValidation", new[] { "FundTransferId" });
            DropIndex("EXCEED.FundTransferDetail", new[] { "FundTransferId" });
            DropIndex("EXCEED.FundTransferDepartmentValidation", new[] { "FundTransferId" });
            DropIndex("EXCEED.FundTransferBudgetValidation", new[] { "FundTransferId" });
            DropIndex("EXCEED.FundTransferAttachment", new[] { "FundTransferId" });
            AlterColumn("EXCEED.GLTaxRate", "CalculatedRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.GLTaxRate", "Rate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("EXCEED.PurchasePayment", "FinanceApprovedTime");
            DropColumn("EXCEED.PurchasePayment", "FinanceApprovedBy");
            DropColumn("EXCEED.PurchasePayment", "IsFinanceApproved");
            DropColumn("EXCEED.PurchasePayment", "FinanceSendForApprovalTime");
            DropColumn("EXCEED.PurchasePayment", "FinanceSendForApprovalBy");
            DropColumn("EXCEED.PurchasePayment", "IsFinanceSendForApproval");
            DropColumn("EXCEED.PurchasePayment", "ProformaRef");
            DropColumn("EXCEED.PurchasePayment", "TenderRef");
            DropTable("EXCEED.PurchasePaymentFinanceValidation");
            DropTable("EXCEED.ProformaTeamMember");
            DropTable("EXCEED.ProformaTeamDecision");
            DropTable("EXCEED.ProformaTeam");
            DropTable("EXCEED.FundTransferValidation");
            DropTable("EXCEED.FundTransferDetail");
            DropTable("EXCEED.FundTransferDepartmentValidation");
            DropTable("EXCEED.FundTransferBudgetValidation");
            DropTable("EXCEED.FundTransfer");
            DropTable("EXCEED.FundTransferAttachment");
        }
    }
}
