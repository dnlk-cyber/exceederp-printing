namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fourdigitar : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.ArInvoice", "LineAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ArInvoice", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ArInvoice", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ArInvoice", "AmountDue", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ArInvoiceLines", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ArInvoiceLines", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ArInvoiceLines", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ArInvoiceLines", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ArSalesOrderLine", "SalesPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ArSalesOrderLine", "Discount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ArSalesOrderLine", "Commission", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ArSalesOrderLine", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ArSalesOrderLine", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ArSalesOrderLine", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ArStandardCollection", "ReceiptAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ArStandardCollection", "UnidentifiedBalance", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ArStandardCollection", "AppliedBalance", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ArStandardCollection", "PrepaymentBalance", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ArStandardCollectionLines", "ApplyAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ArStandardCollectionLines", "BalanceDue", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ArStandardCollectionLines", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ArStandardCollectionLines", "WithholdingAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ArStandardCollectionLines", "WithholdTaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ArStandardCollectionLines", "NetAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ArStandardCollectionLines", "PrepaymentAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ArStandardCollectionLines", "PrepaymentAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ArStandardCollectionLines", "NetAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ArStandardCollectionLines", "WithholdTaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ArStandardCollectionLines", "WithholdingAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ArStandardCollectionLines", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ArStandardCollectionLines", "BalanceDue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ArStandardCollectionLines", "ApplyAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ArStandardCollection", "PrepaymentBalance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ArStandardCollection", "AppliedBalance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ArStandardCollection", "UnidentifiedBalance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ArStandardCollection", "ReceiptAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ArSalesOrderLine", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ArSalesOrderLine", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ArSalesOrderLine", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ArSalesOrderLine", "Commission", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ArSalesOrderLine", "Discount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ArSalesOrderLine", "SalesPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ArInvoiceLines", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ArInvoiceLines", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ArInvoiceLines", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ArInvoiceLines", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ArInvoice", "AmountDue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ArInvoice", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ArInvoice", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ArInvoice", "LineAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
