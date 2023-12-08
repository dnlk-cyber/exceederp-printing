namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class crmupgradingmig : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("CRM.SalesQuoteVoucher", "OrganizationCustomerId", "CRM.OrganizationCustomer");
            DropIndex("EXCEED.ProductCategory", "IX_ThingCategory");
            DropIndex("CRM.SalesQuoteVoucher", new[] { "OrganizationCustomerId" });
            CreateTable(
                "CRM.SalesReturn",
                c => new
                    {
                        SalesReturnId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        CustomerTin = c.String(),
                        BranchId = c.Int(nullable: false),
                        CRMPeriodId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Reason = c.String(nullable: false),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        AmountDue = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Paid = c.Boolean(nullable: false),
                        BusinessUnitId = c.Int(nullable: false),
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
                .PrimaryKey(t => t.SalesReturnId);
            
            CreateTable(
                "CRM.SalesReturnLines",
                c => new
                    {
                        SalesReturnLineId = c.Int(nullable: false, identity: true),
                        SalesReturnId = c.Int(nullable: false),
                        PaymentType = c.Int(nullable: false),
                        SalesReference = c.String(),
                        ShipmentReference = c.String(),
                        ProductId = c.Int(),
                        UnitPrice = c.Decimal(nullable: false, precision: 20, scale: 4),
                        Quantity = c.Decimal(nullable: false, precision: 20, scale: 4),
                        SubTotal = c.Decimal(nullable: false, precision: 20, scale: 4),
                        TaxTypeId = c.Int(),
                        TaxAmount = c.Decimal(nullable: false, precision: 20, scale: 4),
                        TotalAmount = c.Decimal(nullable: false, precision: 20, scale: 4),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SalesReturnLineId)
                .ForeignKey("CRM.SalesReturn", t => t.SalesReturnId, cascadeDelete: true)
                .Index(t => t.SalesReturnId);
            
            CreateTable(
                "CRM.SalesReturnValidation",
                c => new
                    {
                        SalesReturnValidationId = c.Int(nullable: false, identity: true),
                        SalesReturnId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SalesReturnValidationId)
                .ForeignKey("CRM.SalesReturn", t => t.SalesReturnId, cascadeDelete: true)
                .Index(t => t.SalesReturnId);
            
            AddColumn("CRM.SalesOrderVoucher", "ReceiptReference", c => c.String());
            AddColumn("CRM.SalesOrderVoucher", "Status", c => c.String());
            AddColumn("CRM.SalesQuoteVoucher", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("CRM.SalesQuoteVoucher", "OrgStructureId", c => c.Int(nullable: false));
            AddColumn("CRM.SalesQuoteVoucher", "CreatedAtBranchId", c => c.Int(nullable: false));
            AddColumn("CRM.SalesQuoteVoucher", "CreatedAtBusinessUnitId", c => c.Int(nullable: false));
            DropColumn("EXCEED.ProductCategory", "Index");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ProductCategory", "Index", c => c.Int(nullable: false));
            DropForeignKey("CRM.SalesReturnValidation", "SalesReturnId", "CRM.SalesReturn");
            DropForeignKey("CRM.SalesReturnLines", "SalesReturnId", "CRM.SalesReturn");
            DropIndex("CRM.SalesReturnValidation", new[] { "SalesReturnId" });
            DropIndex("CRM.SalesReturnLines", new[] { "SalesReturnId" });
            DropColumn("CRM.SalesQuoteVoucher", "CreatedAtBusinessUnitId");
            DropColumn("CRM.SalesQuoteVoucher", "CreatedAtBranchId");
            DropColumn("CRM.SalesQuoteVoucher", "OrgStructureId");
            DropColumn("CRM.SalesQuoteVoucher", "BusinessUnitId");
            DropColumn("CRM.SalesOrderVoucher", "Status");
            DropColumn("CRM.SalesOrderVoucher", "ReceiptReference");
            DropTable("CRM.SalesReturnValidation");
            DropTable("CRM.SalesReturnLines");
            DropTable("CRM.SalesReturn");
            CreateIndex("CRM.SalesQuoteVoucher", "OrganizationCustomerId");
            CreateIndex("EXCEED.ProductCategory", "Index", unique: true, name: "IX_ThingCategory");
            AddForeignKey("CRM.SalesQuoteVoucher", "OrganizationCustomerId", "CRM.OrganizationCustomer", "OrganizationCustomerID", cascadeDelete: true);
        }
    }
}
