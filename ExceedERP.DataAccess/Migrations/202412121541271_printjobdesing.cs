namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printjobdesing : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Printing.PrintingJobOrderJobDesign",
                c => new
                    {
                        PrintingJobOrderJobDesignId = c.Int(nullable: false, identity: true),
                        ReferenceId = c.Int(nullable: false),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.PrintingJobOrderJobDesignId);
            
            CreateTable(
                "Printing.PrintingJobOrderProductionJobDesign",
                c => new
                    {
                        PrintingJobOrderProductionJobDesignId = c.Int(nullable: false, identity: true),
                        ReferenceId = c.Int(nullable: false),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.PrintingJobOrderProductionJobDesignId);
            
            AddColumn("Printing.PrintingEstimationDetail", "PrintingCostEstimationId", c => c.Int(nullable: false));
            DropColumn("Printing.PrintingEstimationSummary", "PrintingCostEstimationId");
        }
        
        public override void Down()
        {
            AddColumn("Printing.PrintingEstimationSummary", "PrintingCostEstimationId", c => c.Int(nullable: false));
            DropColumn("Printing.PrintingEstimationDetail", "PrintingCostEstimationId");
            DropTable("Printing.PrintingJobOrderProductionJobDesign");
            DropTable("Printing.PrintingJobOrderJobDesign");
        }
    }
}
