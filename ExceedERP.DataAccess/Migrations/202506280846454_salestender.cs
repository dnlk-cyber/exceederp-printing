namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salestender : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "CRM.SalesLotEvaluationByDocument",
                c => new
                    {
                        SalesLotEvaluationByDocumentId = c.Int(nullable: false, identity: true),
                        SalesLotId = c.Int(nullable: false),
                        TinNumber = c.String(),
                        Document = c.String(),
                        IsNotAvailable = c.Boolean(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SalesLotEvaluationByDocumentId);
            
            CreateTable(
                "CRM.SalesLotItems",
                c => new
                    {
                        SalesLotItemId = c.Int(nullable: false, identity: true),
                        ProductCategoryId = c.Int(nullable: false),
                        ProductCategoryDesc = c.String(),
                        ProductID = c.Int(),
                        ProductName = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UOM = c.String(),
                        ProductArea = c.String(),
                        ProductAreaNumber = c.Int(),
                        SalesLotId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SalesLotItemId)
                .ForeignKey("CRM.SalesLots", t => t.SalesLotId, cascadeDelete: true)
                .Index(t => t.SalesLotId);
            
            CreateTable(
                "CRM.SalesLots",
                c => new
                    {
                        SalesLotId = c.Int(nullable: false, identity: true),
                        LotName = c.String(),
                        Description = c.String(),
                        IsSendForInvoice = c.Boolean(nullable: false),
                        SaleTenderId = c.Int(nullable: false),
                        BidParticpatePlace = c.String(),
                        BranchId = c.Int(nullable: false),
                        BusinessUnitiId = c.Int(nullable: false),
                        StoreCode = c.String(),
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
                .PrimaryKey(t => t.SalesLotId)
                .ForeignKey("CRM.SaleTenders", t => t.SaleTenderId, cascadeDelete: true)
                .Index(t => t.SaleTenderId);
            
            CreateTable(
                "CRM.SalesLotPurchases",
                c => new
                    {
                        SalesLotPurchaseId = c.Int(nullable: false, identity: true),
                        CustomerName = c.String(),
                        TinNumber = c.String(nullable: false),
                        Representative = c.String(),
                        Address = c.String(),
                        Telephone = c.String(),
                        PurchaseDate = c.DateTime(nullable: false),
                        ReceiptNo = c.String(),
                        ReceiptNotRequired = c.Boolean(nullable: false),
                        Remark = c.String(),
                        SalesLotId = c.Int(nullable: false),
                        LotName = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SalesLotPurchaseId)
                .ForeignKey("CRM.SalesLots", t => t.SalesLotId, cascadeDelete: true)
                .Index(t => t.SalesLotId);
            
            CreateTable(
                "CRM.SaleTenders",
                c => new
                    {
                        SaleTenderId = c.Int(nullable: false, identity: true),
                        FloatDate = c.DateTime(nullable: false),
                        ClosingDate = c.DateTime(nullable: false),
                        EvaluationDate = c.DateTime(nullable: false),
                        TenderName = c.String(),
                        TenderReference = c.String(),
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
                .PrimaryKey(t => t.SaleTenderId);
            
            CreateTable(
                "CRM.Winners",
                c => new
                    {
                        WinnerId = c.Int(nullable: false, identity: true),
                        SalesLotId = c.Int(nullable: false),
                        SalesLotName = c.String(),
                        SalesLotItemId = c.Int(nullable: false),
                        CustomerName = c.String(),
                        TinNumber = c.String(),
                        CustormerAddress = c.String(),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WinningPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProducCategoryId = c.Int(nullable: false),
                        ProductCategoryDesc = c.String(),
                        ProductID = c.Int(nullable: false),
                        ProductName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.WinnerId)
                .ForeignKey("CRM.SalesLots", t => t.SalesLotId, cascadeDelete: true)
                .Index(t => t.SalesLotId);
            
            CreateTable(
                "CRM.SalesLotParticipantItems",
                c => new
                    {
                        SalesLotParticipantItemId = c.Int(nullable: false, identity: true),
                        SalesLotParticipantId = c.Int(nullable: false),
                        ProductCategoryId = c.Int(nullable: false),
                        ProductID = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EvaluationDate = c.DateTime(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BasePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SalesLotParticipantItemId)
                .ForeignKey("CRM.SalesLotParticipants", t => t.SalesLotParticipantId, cascadeDelete: true)
                .Index(t => t.SalesLotParticipantId);
            
            CreateTable(
                "CRM.SalesLotParticipants",
                c => new
                    {
                        SalesLotParticipantId = c.Int(nullable: false, identity: true),
                        CustomerName = c.String(),
                        PurchaseId = c.Int(nullable: false),
                        TinNumber = c.String(nullable: false),
                        Representative = c.String(),
                        Address = c.String(),
                        Telephone = c.String(),
                        PurchaseDate = c.DateTime(nullable: false),
                        ReceiptNo = c.String(),
                        ReceiptNotRequired = c.Boolean(nullable: false),
                        Remark = c.String(),
                        SalesLotId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SalesLotParticipantId)
                .ForeignKey("CRM.SalesLots", t => t.SalesLotId, cascadeDelete: true)
                .Index(t => t.SalesLotId);
            
            CreateTable(
                "CRM.TenderWinnerValidations",
                c => new
                    {
                        TenderWinnerValidationId = c.Int(nullable: false, identity: true),
                        SalesLotId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TenderWinnerValidationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("CRM.SalesLotParticipantItems", "SalesLotParticipantId", "CRM.SalesLotParticipants");
            DropForeignKey("CRM.SalesLotParticipants", "SalesLotId", "CRM.SalesLots");
            DropForeignKey("CRM.Winners", "SalesLotId", "CRM.SalesLots");
            DropForeignKey("CRM.SalesLots", "SaleTenderId", "CRM.SaleTenders");
            DropForeignKey("CRM.SalesLotPurchases", "SalesLotId", "CRM.SalesLots");
            DropForeignKey("CRM.SalesLotItems", "SalesLotId", "CRM.SalesLots");
            DropIndex("CRM.SalesLotParticipants", new[] { "SalesLotId" });
            DropIndex("CRM.SalesLotParticipantItems", new[] { "SalesLotParticipantId" });
            DropIndex("CRM.Winners", new[] { "SalesLotId" });
            DropIndex("CRM.SalesLotPurchases", new[] { "SalesLotId" });
            DropIndex("CRM.SalesLots", new[] { "SaleTenderId" });
            DropIndex("CRM.SalesLotItems", new[] { "SalesLotId" });
            DropTable("CRM.TenderWinnerValidations");
            DropTable("CRM.SalesLotParticipants");
            DropTable("CRM.SalesLotParticipantItems");
            DropTable("CRM.Winners");
            DropTable("CRM.SaleTenders");
            DropTable("CRM.SalesLotPurchases");
            DropTable("CRM.SalesLots");
            DropTable("CRM.SalesLotItems");
            DropTable("CRM.SalesLotEvaluationByDocument");
        }
    }
}
