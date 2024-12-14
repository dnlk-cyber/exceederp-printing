namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printestimation : DbMigration
    {
        public override void Up()
        {
            AddColumn("Printing.PrintingCostEstimation", "JobDescription", c => c.String());
            AddColumn("Printing.PrintingEstimationDetail", "JobDescription", c => c.String());
            AddColumn("Printing.PrintingJobOrderItem", "JobDescription", c => c.String());
            AddColumn("Printing.PrintingProformaInvoiceItem", "JobDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("Printing.PrintingProformaInvoiceItem", "JobDescription");
            DropColumn("Printing.PrintingJobOrderItem", "JobDescription");
            DropColumn("Printing.PrintingEstimationDetail", "JobDescription");
            DropColumn("Printing.PrintingCostEstimation", "JobDescription");
        }
    }
}
