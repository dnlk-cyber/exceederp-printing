namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printingchanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("Printing.PrintingEstimationLaborCost", "PrintingProcessName", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingEstimationLaborCost", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingEstimationLaborCost", "EstProductionPerHour", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingEstimationMaterialCost", "PaperWeight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingEstimationMaterialCost", "EstimationMaterialType", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingEstimationMaterialCost", "WastageInPercentage", c => c.Double(nullable: false));
            AddColumn("Printing.PrintingJobType", "JobType", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingProcess", "PrintingProcessName", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingProcess", "EstProductionPerHour", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingProcess", "LaborRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("Printing.PrintingCostEstimation", "JobDescription");
            DropColumn("Printing.PrintingEstimationDetail", "JobDescription");
            DropColumn("Printing.PrintingJobOrderItem", "JobDescription");
            DropColumn("Printing.PrintingProformaInvoiceItem", "JobDescription");
            DropTable("Printing.PrintingJobOrderJobDesign");
            DropTable("Printing.PrintingJobOrderProductionJobDesign");
        }
        
        public override void Down()
        {
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
            
            AddColumn("Printing.PrintingProformaInvoiceItem", "JobDescription", c => c.String());
            AddColumn("Printing.PrintingJobOrderItem", "JobDescription", c => c.String());
            AddColumn("Printing.PrintingEstimationDetail", "JobDescription", c => c.String());
            AddColumn("Printing.PrintingCostEstimation", "JobDescription", c => c.String());
            DropColumn("Printing.PrintingProcess", "LaborRate");
            DropColumn("Printing.PrintingProcess", "EstProductionPerHour");
            DropColumn("Printing.PrintingProcess", "PrintingProcessName");
            DropColumn("Printing.PrintingJobType", "JobType");
            DropColumn("Printing.PrintingEstimationMaterialCost", "WastageInPercentage");
            DropColumn("Printing.PrintingEstimationMaterialCost", "EstimationMaterialType");
            DropColumn("Printing.PrintingEstimationMaterialCost", "PaperWeight");
            DropColumn("Printing.PrintingEstimationLaborCost", "EstProductionPerHour");
            DropColumn("Printing.PrintingEstimationLaborCost", "Quantity");
            DropColumn("Printing.PrintingEstimationLaborCost", "PrintingProcessName");
        }
    }
}
