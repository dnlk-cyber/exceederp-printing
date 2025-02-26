namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prinintupgchanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("Printing.PrintingCostEstimation", "JobDescription", c => c.String());
            AddColumn("Printing.PrintingEstimationDetail", "JobDescription", c => c.String());
            AddColumn("Printing.PrintingJobOrderItem", "JobDescription", c => c.String());
            AddColumn("Printing.PrintingProformaInvoiceItem", "JobDescription", c => c.String());
            AddColumn("Printing.PrintingProformaInvoiceItem", "GLTaxRateTransportId", c => c.Int());
            AddColumn("Printing.PrintingProformaInvoiceItem", "GrandTotalIncludingTransport", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("Printing.PrintingProformaInvoiceItem", "GrandTotalIncludingTransport");
            DropColumn("Printing.PrintingProformaInvoiceItem", "GLTaxRateTransportId");
            DropColumn("Printing.PrintingProformaInvoiceItem", "JobDescription");
            DropColumn("Printing.PrintingJobOrderItem", "JobDescription");
            DropColumn("Printing.PrintingEstimationDetail", "JobDescription");
            DropColumn("Printing.PrintingCostEstimation", "JobDescription");
        }
    }
}
