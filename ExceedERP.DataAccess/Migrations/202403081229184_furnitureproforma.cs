namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class furnitureproforma : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.FurnitureJobOrderForm",
                c => new
                    {
                        FurnitureJobOrderFormId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        JobTypeId = c.Int(nullable: false),
                        UnitOfMeasurment = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPriceBeforeVAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPriceWithVAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DeliveryDate = c.DateTime(nullable: false),
                        CRVNo = c.String(),
                        AdvancePaymentInPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FurnitureProformaInvoiceId = c.Int(nullable: false),
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
                .PrimaryKey(t => t.FurnitureJobOrderFormId);
            
            CreateTable(
                "Manufacturing.FurnitureProformaInvoiceItem",
                c => new
                    {
                        FurnitureProformaInvoiceItemId = c.Int(nullable: false, identity: true),
                        JobTypeId = c.Int(nullable: false),
                        UnitOfMeasurment = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPriceBeforeVAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPriceWithVAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        FurnitureProformaInvoiceId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureProformaInvoiceItemId)
                .ForeignKey("Manufacturing.FurnitureProformaInvoice", t => t.FurnitureProformaInvoiceId, cascadeDelete: true)
                .Index(t => t.FurnitureProformaInvoiceId);
            
            CreateTable(
                "Manufacturing.FurnitureProformaInvoice",
                c => new
                    {
                        FurnitureProformaInvoiceId = c.Int(nullable: false, identity: true),
                        FurnitureEstimationId = c.Int(nullable: false),
                        CustomerId = c.Int(nullable: false),
                        DeliveryPeriod = c.Int(nullable: false),
                        DeliveryDate = c.DateTime(nullable: false),
                        PriceValidityPeriod = c.Int(nullable: false),
                        ModeOfPayment = c.String(),
                        PaymentType = c.Int(nullable: false),
                        HaveEstimation = c.Boolean(nullable: false),
                        IsExistingCustomer = c.Boolean(nullable: false),
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
                .PrimaryKey(t => t.FurnitureProformaInvoiceId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Manufacturing.FurnitureProformaInvoiceItem", "FurnitureProformaInvoiceId", "Manufacturing.FurnitureProformaInvoice");
            DropIndex("Manufacturing.FurnitureProformaInvoiceItem", new[] { "FurnitureProformaInvoiceId" });
            DropTable("Manufacturing.FurnitureProformaInvoice");
            DropTable("Manufacturing.FurnitureProformaInvoiceItem");
            DropTable("EXCEED.FurnitureJobOrderForm");
        }
    }
}
