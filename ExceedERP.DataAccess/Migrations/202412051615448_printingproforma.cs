namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printingproforma : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Printing.PrintingProformaInvoiceItem",
                c => new
                    {
                        PrintingProformaInvoiceItemId = c.Int(nullable: false, identity: true),
                        PrintingCostEstimationId = c.Int(nullable: false),
                        JobTypeId = c.Int(nullable: false),
                        UnitOfMeasurment = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TransportCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GLTaxRateID = c.Int(nullable: false),
                        Remark = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        PrintingProformaInvoiceId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PrintingProformaInvoiceItemId);
            
            CreateTable(
                "Printing.PrintingProformaInvoice",
                c => new
                    {
                        PrintingProformaInvoiceId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        DeliveryPeriod = c.Int(nullable: false),
                        DeliveryDate = c.DateTime(nullable: false),
                        PriceValidityPeriod = c.Int(nullable: false),
                        ModeOfPayment = c.String(),
                        AdvancePaymentInPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
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
                .PrimaryKey(t => t.PrintingProformaInvoiceId);
            
            DropColumn("Printing.PrintingOverAllCost", "UnitPrice");
            DropColumn("Printing.PrintingOverAllCost", "TotalBeforeVAT");
            DropColumn("Printing.PrintingOverAllCost", "VAT");
        }
        
        public override void Down()
        {
            AddColumn("Printing.PrintingOverAllCost", "VAT", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingOverAllCost", "TotalBeforeVAT", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingOverAllCost", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropTable("Printing.PrintingProformaInvoice");
            DropTable("Printing.PrintingProformaInvoiceItem");
        }
    }
}
