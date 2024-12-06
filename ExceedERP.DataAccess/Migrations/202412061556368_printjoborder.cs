namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printjoborder : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Printing.PrintingJobOrderItem",
                c => new
                    {
                        PrintingJobOrderItemId = c.Int(nullable: false, identity: true),
                        JobTypeId = c.Int(nullable: false),
                        Description = c.String(),
                        NumberOfPages = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsClosed = c.Boolean(nullable: false),
                        Remark = c.String(),
                        PrintingJobOrderId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PrintingJobOrderItemId);
            
            CreateTable(
                "Printing.PrintingJobOrder",
                c => new
                    {
                        PrintingJobOrderId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        JobStartDate = c.DateTime(nullable: false),
                        DeliveryDate = c.DateTime(nullable: false),
                        CRVNo = c.String(),
                        AdvancePaymentInPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentType = c.Int(nullable: false),
                        PrintingProformaInvoiceId = c.Int(nullable: false),
                        JobClosed = c.Boolean(nullable: false),
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
                .PrimaryKey(t => t.PrintingJobOrderId);
            
            AddColumn("Printing.PrintingProformaInvoiceItem", "NumberOfPages", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingProformaInvoiceItem", "GrandTotalIncludingVAT", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Printing.PrintingProformaInvoiceItem", "GLTaxRateID", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("Printing.PrintingProformaInvoiceItem", "GLTaxRateID", c => c.Int(nullable: false));
            DropColumn("Printing.PrintingProformaInvoiceItem", "GrandTotalIncludingVAT");
            DropColumn("Printing.PrintingProformaInvoiceItem", "NumberOfPages");
            DropTable("Printing.PrintingJobOrder");
            DropTable("Printing.PrintingJobOrderItem");
        }
    }
}
