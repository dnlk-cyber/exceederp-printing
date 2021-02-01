namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction24 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "Construction.WorkDelay", name: "DelayAnalysisTaskId", newName: "ID");
            RenameIndex(table: "Construction.WorkDelay", name: "IX_DelayAnalysisTaskId", newName: "IX_ID");
            CreateTable(
                "Construction.ConsARStandardCollectionDetail",
                c => new
                    {
                        ConsARStandardCollectionDetailId = c.Int(nullable: false, identity: true),
                        ConsARStandardCollectionId = c.Int(nullable: false),
                        InvoiceNumber = c.String(),
                        ApplyAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BalanceDue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdvanceAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Retention = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WithholdingType = c.Int(nullable: false),
                        WithholdingTax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ConsARStandardCollectionDetailId)
                .ForeignKey("Construction.ConsARStandardCollection", t => t.ConsARStandardCollectionId, cascadeDelete: true)
                .Index(t => t.ConsARStandardCollectionId);
            
            CreateTable(
                "Construction.ConsARStandardCollection",
                c => new
                    {
                        ConsARStandardCollectionId = c.Int(nullable: false, identity: true),
                        ReceiptID = c.Int(nullable: false),
                        ProjectInformationId = c.Int(nullable: false),
                        ReceiptType = c.Int(nullable: false),
                        ClientName = c.String(),
                        ReceiptNumber = c.String(),
                        ReceiptDate = c.DateTime(nullable: false),
                        ReceiptMethodId = c.Int(nullable: false),
                        Reference = c.String(),
                        PrepaymentBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OutstandingBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
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
                .PrimaryKey(t => t.ConsARStandardCollectionId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.ConsARStandardCollectionValidation",
                c => new
                    {
                        ConsARStandardCollectionValidationId = c.Int(nullable: false, identity: true),
                        ConsARStandardCollectionId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ConsARStandardCollectionValidationId)
                .ForeignKey("Construction.ConsARStandardCollection", t => t.ConsARStandardCollectionId, cascadeDelete: true)
                .Index(t => t.ConsARStandardCollectionId);
            
            CreateTable(
                "Construction.ConsReceivableInvoiceDetail",
                c => new
                    {
                        ConsReceivableInvoiceDetailId = c.Int(nullable: false, identity: true),
                        ConsReceivableInvoiceId = c.Int(nullable: false),
                        GLTaxRateID = c.Int(nullable: false),
                        Service = c.String(),
                        Description = c.String(),
                        AccountCode = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WorkItemPerProjectId = c.Int(nullable: false),
                        Penality = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Retention = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdvanceRepayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PreviousPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ConsReceivableInvoiceDetailId)
                .ForeignKey("Construction.ConsReceivableInvoice", t => t.ConsReceivableInvoiceId, cascadeDelete: true)
                .Index(t => t.ConsReceivableInvoiceId);
            
            CreateTable(
                "Construction.ConsReceivableInvoice",
                c => new
                    {
                        ConsReceivableInvoiceId = c.Int(nullable: false, identity: true),
                        ProjectInformationId = c.Int(nullable: false),
                        Source = c.Int(nullable: false),
                        InvoiceNumber = c.String(),
                        Client = c.String(),
                        ConstructionTransactionClass = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        PaymentTerm = c.Int(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        Reference = c.String(),
                        ConstructionInvoiceStatus = c.Int(nullable: false),
                        OperationStatus = c.Int(nullable: false),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CollectedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WorkInProgressId = c.Int(nullable: false),
                        IsInvoiceFromWorkInProgress = c.Boolean(nullable: false),
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
                .PrimaryKey(t => t.ConsReceivableInvoiceId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.ConsReceivableInvoiceValidation",
                c => new
                    {
                        ConsReceivableInvoiceValidationId = c.Int(nullable: false, identity: true),
                        ConsReceivableInvoiceId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ConsReceivableInvoiceValidationId)
                .ForeignKey("Construction.ConsReceivableInvoice", t => t.ConsReceivableInvoiceId, cascadeDelete: true)
                .Index(t => t.ConsReceivableInvoiceId);
            
            CreateTable(
                "Construction.WorkInProgress",
                c => new
                    {
                        WorkInProgressId = c.Int(nullable: false, identity: true),
                        WorkInProgressNumber = c.String(),
                        ProjectInformationId = c.Int(nullable: false),
                        ClientName = c.String(),
                        Date = c.DateTime(nullable: false),
                        WorkInProgressStatus = c.Int(nullable: false),
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
                .PrimaryKey(t => t.WorkInProgressId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.WorkInProgressDetails",
                c => new
                    {
                        WorkInProgressDetailId = c.Int(nullable: false, identity: true),
                        WorkInProgressId = c.Int(nullable: false),
                        Service = c.String(),
                        Description = c.String(),
                        AccountCode = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GLTaxRateID = c.Int(nullable: false),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WorkItemPerProjectId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.WorkInProgressDetailId)
                .ForeignKey("EXCEED.GLTaxRate", t => t.GLTaxRateID, cascadeDelete: true)
                .ForeignKey("Construction.WorkInProgress", t => t.WorkInProgressId, cascadeDelete: true)
                .Index(t => t.WorkInProgressId)
                .Index(t => t.GLTaxRateID);
            
            CreateTable(
                "Construction.WorkInProgressValidation",
                c => new
                    {
                        WorkInProgressValidationId = c.Int(nullable: false, identity: true),
                        WorkInProgressId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.WorkInProgressValidationId)
                .ForeignKey("Construction.WorkInProgress", t => t.WorkInProgressId, cascadeDelete: true)
                .Index(t => t.WorkInProgressId);
            
            AddColumn("Construction.PaymentCertificate", "GLTaxRateID", c => c.Int(nullable: false));
            AddColumn("Construction.PaymentCertificate", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.TakeOffLinePerProject", "IsDeductive", c => c.Boolean(nullable: false));
            AlterColumn("Construction.ProjectTakeOffLine", "Description", c => c.String());
            DropColumn("Construction.PaymentCertificate", "Vat");
        }
        
        public override void Down()
        {
            AddColumn("Construction.PaymentCertificate", "Vat", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropForeignKey("Construction.WorkInProgressValidation", "WorkInProgressId", "Construction.WorkInProgress");
            DropForeignKey("Construction.WorkInProgressDetails", "WorkInProgressId", "Construction.WorkInProgress");
            DropForeignKey("Construction.WorkInProgressDetails", "GLTaxRateID", "EXCEED.GLTaxRate");
            DropForeignKey("Construction.WorkInProgress", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ConsReceivableInvoiceValidation", "ConsReceivableInvoiceId", "Construction.ConsReceivableInvoice");
            DropForeignKey("Construction.ConsReceivableInvoice", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ConsReceivableInvoiceDetail", "ConsReceivableInvoiceId", "Construction.ConsReceivableInvoice");
            DropForeignKey("Construction.ConsARStandardCollectionValidation", "ConsARStandardCollectionId", "Construction.ConsARStandardCollection");
            DropForeignKey("Construction.ConsARStandardCollectionDetail", "ConsARStandardCollectionId", "Construction.ConsARStandardCollection");
            DropForeignKey("Construction.ConsARStandardCollection", "ProjectInformationId", "Construction.ProjectInformation");
            DropIndex("Construction.WorkInProgressValidation", new[] { "WorkInProgressId" });
            DropIndex("Construction.WorkInProgressDetails", new[] { "GLTaxRateID" });
            DropIndex("Construction.WorkInProgressDetails", new[] { "WorkInProgressId" });
            DropIndex("Construction.WorkInProgress", new[] { "ProjectInformationId" });
            DropIndex("Construction.ConsReceivableInvoiceValidation", new[] { "ConsReceivableInvoiceId" });
            DropIndex("Construction.ConsReceivableInvoice", new[] { "ProjectInformationId" });
            DropIndex("Construction.ConsReceivableInvoiceDetail", new[] { "ConsReceivableInvoiceId" });
            DropIndex("Construction.ConsARStandardCollectionValidation", new[] { "ConsARStandardCollectionId" });
            DropIndex("Construction.ConsARStandardCollection", new[] { "ProjectInformationId" });
            DropIndex("Construction.ConsARStandardCollectionDetail", new[] { "ConsARStandardCollectionId" });
            AlterColumn("Construction.ProjectTakeOffLine", "Description", c => c.String(maxLength: 400));
            DropColumn("Construction.TakeOffLinePerProject", "IsDeductive");
            DropColumn("Construction.PaymentCertificate", "TaxAmount");
            DropColumn("Construction.PaymentCertificate", "GLTaxRateID");
            DropTable("Construction.WorkInProgressValidation");
            DropTable("Construction.WorkInProgressDetails");
            DropTable("Construction.WorkInProgress");
            DropTable("Construction.ConsReceivableInvoiceValidation");
            DropTable("Construction.ConsReceivableInvoice");
            DropTable("Construction.ConsReceivableInvoiceDetail");
            DropTable("Construction.ConsARStandardCollectionValidation");
            DropTable("Construction.ConsARStandardCollection");
            DropTable("Construction.ConsARStandardCollectionDetail");
            RenameIndex(table: "Construction.WorkDelay", name: "IX_ID", newName: "IX_DelayAnalysisTaskId");
            RenameColumn(table: "Construction.WorkDelay", name: "ID", newName: "DelayAnalysisTaskId");
        }
    }
}
