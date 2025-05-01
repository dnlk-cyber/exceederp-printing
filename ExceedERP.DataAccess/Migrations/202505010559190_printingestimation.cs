namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printingestimation : DbMigration
    {
        public override void Up()
        {
            AddColumn("Printing.PrintingJobOrderItem", "BindingStyle", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingJobOrderItem", "TextPaperSizeId", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingJobOrderItem", "CoverPaperSizeId", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingJobOrderItem", "PrintingCostEstimationId", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingJobOrderProduction", "PrintJobType", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingJobOrderProduction", "BindingStyle", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingJobOrderProduction", "TextPaperSizeId", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingJobOrderProduction", "CoverPaperSizeId", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingProformaInvoiceItem", "BindingStyle", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingProformaInvoiceItem", "TextPaperSizeId", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingProformaInvoiceItem", "CoverPaperSizeId", c => c.Int(nullable: false));
            DropColumn("Printing.PrintingJobOrderProduction", "JobTypeId");
            DropColumn("Printing.PrintingJobOrderProduction", "PaperSizeId");
        }
        
        public override void Down()
        {
            AddColumn("Printing.PrintingJobOrderProduction", "PaperSizeId", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingJobOrderProduction", "JobTypeId", c => c.Int(nullable: false));
            DropColumn("Printing.PrintingProformaInvoiceItem", "CoverPaperSizeId");
            DropColumn("Printing.PrintingProformaInvoiceItem", "TextPaperSizeId");
            DropColumn("Printing.PrintingProformaInvoiceItem", "BindingStyle");
            DropColumn("Printing.PrintingJobOrderProduction", "CoverPaperSizeId");
            DropColumn("Printing.PrintingJobOrderProduction", "TextPaperSizeId");
            DropColumn("Printing.PrintingJobOrderProduction", "BindingStyle");
            DropColumn("Printing.PrintingJobOrderProduction", "PrintJobType");
            DropColumn("Printing.PrintingJobOrderItem", "PrintingCostEstimationId");
            DropColumn("Printing.PrintingJobOrderItem", "CoverPaperSizeId");
            DropColumn("Printing.PrintingJobOrderItem", "TextPaperSizeId");
            DropColumn("Printing.PrintingJobOrderItem", "BindingStyle");
        }
    }
}
