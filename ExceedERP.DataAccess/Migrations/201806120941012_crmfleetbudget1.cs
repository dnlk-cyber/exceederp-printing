namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class crmfleetbudget1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "CRM.SalesInvoicePaymentDistibution",
                c => new
                    {
                        SalesInvoicePaymentDistibutionId = c.Int(nullable: false, identity: true),
                        CashSalesInvoiceId = c.Int(nullable: false),
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
                .PrimaryKey(t => t.SalesInvoicePaymentDistibutionId)
                .ForeignKey("CRM.CashSalesInvoice", t => t.CashSalesInvoiceId, cascadeDelete: true)
                .Index(t => t.CashSalesInvoiceId);
            
            AddColumn("CRM.LineItem", "WithholdingType", c => c.String());
            AddColumn("CRM.LineItem", "WithholdingAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("CRM.LineItem", "WithholdTax", c => c.Boolean(nullable: false));
            AddColumn("CRM.LineItem", "WithholdTaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("CRM.AdvancePaymentLine", "AccountCode", c => c.String());
            AddColumn("CRM.CashSalesInvoice", "OrganizationCustomerId", c => c.Int(nullable: false));
            AddColumn("CRM.OrganizationCustomer", "IsTemporary", c => c.Boolean(nullable: false));
            AddColumn("CRM.SalesOrderVoucher", "OrganizationCustomerId", c => c.Int(nullable: false));
            AddColumn("CRM.SalesQuoteVoucher", "OrganizationCustomerId", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.LineCheckList", "Name", c => c.String(nullable: false));
            CreateIndex("CRM.CashSalesInvoice", "OrganizationCustomerId");
            CreateIndex("CRM.SalesOrderVoucher", "OrganizationCustomerId");
            CreateIndex("CRM.SalesQuoteVoucher", "OrganizationCustomerId");
            AddForeignKey("CRM.CashSalesInvoice", "OrganizationCustomerId", "CRM.OrganizationCustomer", "OrganizationCustomerID", cascadeDelete: true);
            AddForeignKey("CRM.SalesOrderVoucher", "OrganizationCustomerId", "CRM.OrganizationCustomer", "OrganizationCustomerID", cascadeDelete: true);
            AddForeignKey("CRM.SalesQuoteVoucher", "OrganizationCustomerId", "CRM.OrganizationCustomer", "OrganizationCustomerID", cascadeDelete: true);
            DropColumn("CRM.LineItem", "FreightCost");
            DropColumn("CRM.LineItem", "InsuranceCost");
            DropColumn("CRM.LineItem", "OtherCost");
            DropColumn("CRM.LineItem", "TaxWithHolding");
            DropColumn("CRM.LineItem", "WithHoldingTax");
            DropColumn("CRM.CashSalesInvoice", "Consignee");
            DropColumn("CRM.SalesOrderVoucher", "Consignee");
            DropColumn("CRM.SalesQuoteVoucher", "Consignee");
        }
        
        public override void Down()
        {
            AddColumn("CRM.SalesQuoteVoucher", "Consignee", c => c.String());
            AddColumn("CRM.SalesOrderVoucher", "Consignee", c => c.String(nullable: false));
            AddColumn("CRM.CashSalesInvoice", "Consignee", c => c.String(nullable: false));
            AddColumn("CRM.LineItem", "WithHoldingTax", c => c.String());
            AddColumn("CRM.LineItem", "TaxWithHolding", c => c.Boolean(nullable: false));
            AddColumn("CRM.LineItem", "OtherCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("CRM.LineItem", "InsuranceCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("CRM.LineItem", "FreightCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropForeignKey("CRM.SalesQuoteVoucher", "OrganizationCustomerId", "CRM.OrganizationCustomer");
            DropForeignKey("CRM.SalesOrderVoucher", "OrganizationCustomerId", "CRM.OrganizationCustomer");
            DropForeignKey("CRM.SalesInvoicePaymentDistibution", "CashSalesInvoiceId", "CRM.CashSalesInvoice");
            DropForeignKey("CRM.CashSalesInvoice", "OrganizationCustomerId", "CRM.OrganizationCustomer");
            DropIndex("CRM.SalesQuoteVoucher", new[] { "OrganizationCustomerId" });
            DropIndex("CRM.SalesOrderVoucher", new[] { "OrganizationCustomerId" });
            DropIndex("CRM.SalesInvoicePaymentDistibution", new[] { "CashSalesInvoiceId" });
            DropIndex("CRM.CashSalesInvoice", new[] { "OrganizationCustomerId" });
            AlterColumn("FleetManagement.LineCheckList", "Name", c => c.String());
            DropColumn("CRM.SalesQuoteVoucher", "OrganizationCustomerId");
            DropColumn("CRM.SalesOrderVoucher", "OrganizationCustomerId");
            DropColumn("CRM.OrganizationCustomer", "IsTemporary");
            DropColumn("CRM.CashSalesInvoice", "OrganizationCustomerId");
            DropColumn("CRM.AdvancePaymentLine", "AccountCode");
            DropColumn("CRM.LineItem", "WithholdTaxAmount");
            DropColumn("CRM.LineItem", "WithholdTax");
            DropColumn("CRM.LineItem", "WithholdingAmount");
            DropColumn("CRM.LineItem", "WithholdingType");
            DropTable("CRM.SalesInvoicePaymentDistibution");
        }
    }
}
