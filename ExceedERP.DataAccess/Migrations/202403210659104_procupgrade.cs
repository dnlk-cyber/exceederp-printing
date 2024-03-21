namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procupgrade : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.IsoDocument",
                c => new
                    {
                        IsoDocumentId = c.Int(nullable: false, identity: true),
                        Module = c.Int(nullable: false),
                        DocumentType = c.String(),
                        URL = c.String(),
                        FormTitle = c.String(),
                        FormNumber = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.IsoDocumentId);
            
            CreateTable(
                "EXCEED.PoAttachment",
                c => new
                    {
                        PoAttachmentId = c.Int(nullable: false, identity: true),
                        PurchaseOrderID = c.Int(nullable: false),
                        FileName = c.String(),
                        FilePath = c.String(),
                    })
                .PrimaryKey(t => t.PoAttachmentId)
                .ForeignKey("EXCEED.PurchaseOrder", t => t.PurchaseOrderID, cascadeDelete: true)
                .Index(t => t.PurchaseOrderID);
            
            CreateTable(
                "EXCEED.PoChangeOfOrderItem",
                c => new
                    {
                        PoChangeOfOrderItemId = c.Int(nullable: false, identity: true),
                        PoChangeOforderId = c.Int(nullable: false),
                        CatCode = c.String(nullable: false),
                        CategoryName = c.String(nullable: false),
                        ItmCode = c.String(nullable: false),
                        ItemName = c.String(nullable: false),
                        PartNo = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsApproved = c.Boolean(nullable: false),
                        ItemSpecification = c.String(),
                        Name = c.String(),
                        ModelNo = c.String(),
                        UoM = c.String(),
                        ItemType = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GLTaxRateID = c.Int(nullable: false),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Discount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PoChangeOfOrderItemId)
                .ForeignKey("EXCEED.PoChangeOforder", t => t.PoChangeOforderId, cascadeDelete: true)
                .Index(t => t.PoChangeOforderId);
            
            CreateTable(
                "EXCEED.PoChangeOforder",
                c => new
                    {
                        PoChangeOforderId = c.Int(nullable: false, identity: true),
                        PurchaseOrderID = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        BusinessUnitId = c.Int(nullable: false),
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
                .PrimaryKey(t => t.PoChangeOforderId)
                .ForeignKey("EXCEED.PurchaseOrder", t => t.PurchaseOrderID, cascadeDelete: true)
                .Index(t => t.PurchaseOrderID);
            
            CreateTable(
                "EXCEED.PoChangeOforderValidation",
                c => new
                    {
                        PoChangeOforderValidationId = c.Int(nullable: false, identity: true),
                        PoChangeOforderId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PoChangeOforderValidationId)
                .ForeignKey("EXCEED.PoChangeOforder", t => t.PoChangeOforderId, cascadeDelete: true)
                .Index(t => t.PoChangeOforderId);
            
            CreateTable(
                "EXCEED.PurchasePaymentAuthorizeValidation",
                c => new
                    {
                        PurchasePaymentAuthorizeValidationId = c.Int(nullable: false, identity: true),
                        PurchasePaymentId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchasePaymentAuthorizeValidationId)
                .ForeignKey("EXCEED.PurchasePayment", t => t.PurchasePaymentId, cascadeDelete: true)
                .Index(t => t.PurchasePaymentId);
            
            AddColumn("EXCEED.BidPParticipatingSuppliers", "GLTaxRateID", c => c.Int(nullable: false));
            AddColumn("EXCEED.ProformaTeamMember", "Title", c => c.String());
            AddColumn("EXCEED.PurchasePayment", "IsAuthorized", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchasePayment", "AuthorizedBy", c => c.String());
            AddColumn("EXCEED.PurchasePayment", "AuthorizedTime", c => c.DateTime());
            AlterColumn("EXCEED.AnnualNeedAssesment", "BusinessUnitId", c => c.Int());
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.PurchasePaymentAuthorizeValidation", "PurchasePaymentId", "EXCEED.PurchasePayment");
            DropForeignKey("EXCEED.PoChangeOforder", "PurchaseOrderID", "EXCEED.PurchaseOrder");
            DropForeignKey("EXCEED.PoChangeOforderValidation", "PoChangeOforderId", "EXCEED.PoChangeOforder");
            DropForeignKey("EXCEED.PoChangeOfOrderItem", "PoChangeOforderId", "EXCEED.PoChangeOforder");
            DropForeignKey("EXCEED.PoAttachment", "PurchaseOrderID", "EXCEED.PurchaseOrder");
            DropIndex("EXCEED.PurchasePaymentAuthorizeValidation", new[] { "PurchasePaymentId" });
            DropIndex("EXCEED.PoChangeOforderValidation", new[] { "PoChangeOforderId" });
            DropIndex("EXCEED.PoChangeOforder", new[] { "PurchaseOrderID" });
            DropIndex("EXCEED.PoChangeOfOrderItem", new[] { "PoChangeOforderId" });
            DropIndex("EXCEED.PoAttachment", new[] { "PurchaseOrderID" });
            AlterColumn("EXCEED.AnnualNeedAssesment", "BusinessUnitId", c => c.Int(nullable: false));
            DropColumn("EXCEED.PurchasePayment", "AuthorizedTime");
            DropColumn("EXCEED.PurchasePayment", "AuthorizedBy");
            DropColumn("EXCEED.PurchasePayment", "IsAuthorized");
            DropColumn("EXCEED.ProformaTeamMember", "Title");
            DropColumn("EXCEED.BidPParticipatingSuppliers", "GLTaxRateID");
            DropTable("EXCEED.PurchasePaymentAuthorizeValidation");
            DropTable("EXCEED.PoChangeOforderValidation");
            DropTable("EXCEED.PoChangeOforder");
            DropTable("EXCEED.PoChangeOfOrderItem");
            DropTable("EXCEED.PoAttachment");
            DropTable("EXCEED.IsoDocument");
        }
    }
}
