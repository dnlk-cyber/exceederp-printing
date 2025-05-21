namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printingproforma1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Printing.PrintingProformaInvoice", "TransporCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingProformaInvoice", "GLTaxRateID", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingProformaInvoice", "TransportCostTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("Printing.PrintingProformaInvoiceItem", "TransportCost");
            DropColumn("Printing.PrintingProformaInvoiceItem", "GLTaxRateTransportId");
            DropColumn("Printing.PrintingProformaInvoiceItem", "GrandTotalIncludingTransport");
        }
        
        public override void Down()
        {
            AddColumn("Printing.PrintingProformaInvoiceItem", "GrandTotalIncludingTransport", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingProformaInvoiceItem", "GLTaxRateTransportId", c => c.Int());
            AddColumn("Printing.PrintingProformaInvoiceItem", "TransportCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("Printing.PrintingProformaInvoice", "TransportCostTotal");
            DropColumn("Printing.PrintingProformaInvoice", "GLTaxRateID");
            DropColumn("Printing.PrintingProformaInvoice", "TransporCost");
        }
    }
}
