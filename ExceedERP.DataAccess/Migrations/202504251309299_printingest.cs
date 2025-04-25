namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printingest : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Printing.PrintingMachineDetail",
                c => new
                    {
                        PrintingMachineDetailId = c.Int(nullable: false, identity: true),
                        PaperSizeId = c.Int(nullable: false),
                        SectionUp = c.Int(nullable: false),
                        PrintingMachineTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PrintingMachineDetailId);
            
            AddColumn("Printing.PrintingCostEstimation", "PrintJobType", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingCostEstimation", "TextPaperSizeId", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingCostEstimation", "CoverPaperSizeId", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingCostEstimation", "Pages", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingCostEstimation", "TextMachineTypeId", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingCostEstimation", "CoverMachineTypeId", c => c.Int());
            AddColumn("Printing.PrintingCostEstimation", "Pack", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingCostEstimation", "BindingStyle", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingCostEstimation", "UseLength", c => c.Boolean(nullable: false));
            AddColumn("Printing.PrintingCostEstimation", "TextPrintSide", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingCostEstimation", "CoverPrintSide", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingEstimationDetail", "PrintJobType", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingEstimationLaborCost", "PressCategory", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingEstimationLaborCost", "LabourActivity", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingEstimationLaborCost", "UOM", c => c.String());
            AddColumn("Printing.PrintingJobOrderItem", "PrintJobType", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingMachineType", "PaperSizeId", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingMachineType", "PrintSide", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingMaterialCategoryItem", "PaperType", c => c.Int());
            AddColumn("Printing.PrintingPaperSize", "PrintPaperSize", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingProcess", "PressCategory", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingProcess", "LabourActivity", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingProcess", "PrintingMachineTypeId", c => c.Int());
            AddColumn("Printing.PrintingProcess", "UOM", c => c.String(nullable: false));
            AddColumn("Printing.PrintingProductionLaborCost", "PressCategory", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingProformaInvoiceItem", "PrintJobType", c => c.Int(nullable: false));
            AlterColumn("Printing.PrintingMachineType", "MachineTypeName", c => c.String(nullable: false));
            AlterColumn("Printing.PrintingPaperSize", "NoOfPagesToA1Ratio", c => c.Int(nullable: false));
            DropColumn("Printing.PrintingCostEstimation", "PaperSizeId");
            DropColumn("Printing.PrintingCostEstimation", "JobTypeId");
            DropColumn("Printing.PrintingCostEstimation", "BindingStyleId");
            DropColumn("Printing.PrintingCostEstimation", "TotalTextNoPages");
            DropColumn("Printing.PrintingCostEstimation", "CoverPaperSize");
            DropColumn("Printing.PrintingCostEstimation", "NoUpPagesPerA1");
            DropColumn("Printing.PrintingCostEstimation", "ProcessCategoryId");
            DropColumn("Printing.PrintingCostEstimation", "TypeOfWorkId");
            DropColumn("Printing.PrintingCostEstimation", "EstimatedHrs");
            DropColumn("Printing.PrintingCostEstimation", "DLaborRate");
            DropColumn("Printing.PrintingCostEstimation", "TotalCost");
            DropColumn("Printing.PrintingCostEstimation", "OverHeadRate");
            DropColumn("Printing.PrintingCostEstimation", "OverHeadCost");
            DropColumn("Printing.PrintingCostEstimation", "TotalLaborCost");
            DropColumn("Printing.PrintingEstimationLaborCost", "ProcessCategory");
            DropColumn("Printing.PrintingEstimationLaborCost", "PrintingProcessName");
            DropColumn("Printing.PrintingEstimationLaborCost", "PrintingMachineTypeId");
            DropColumn("Printing.PrintingEstimationLaborCost", "EstProductionPerHour");
            DropColumn("Printing.PrintingEstimationLaborCost", "EstimatedHours");
            DropColumn("Printing.PrintingEstimationMaterialCost", "PaperWeight");
            DropColumn("Printing.PrintingJobOrderProduction", "BindingStyleId");
            DropColumn("Printing.PrintingMachineType", "PaperFeedStyle");
            DropColumn("Printing.PrintingMachineType", "NumberUnits");
            DropColumn("Printing.PrintingMachineType", "PrintingHourlyChargeRate");
            DropColumn("Printing.PrintingMachineType", "MakeReadyHours");
            DropColumn("Printing.PrintingMachineType", "MachineStatus");
            DropColumn("Printing.PrintingMachineType", "Remark");
            DropColumn("Printing.PrintingMachineType", "PrintingCostCenterId");
            DropColumn("Printing.PrintingPaperSize", "NoOfPagesPerSheet");
            DropColumn("Printing.PrintingPaperSize", "NoOfPagesPerPlate");
            DropColumn("Printing.PrintingPaperSize", "PrintingMachineTypeId");
            DropColumn("Printing.PrintingProcess", "PrintingCategory");
            DropColumn("Printing.PrintingProcess", "PrintingProcessName");
            DropColumn("Printing.PrintingProcess", "EstProductionPerHour");
            DropColumn("Printing.PrintingProcess", "ProcessName");
            DropColumn("Printing.PrintingProcess", "HoursRequired");
            DropColumn("Printing.PrintingProcess", "Quantity");
            DropColumn("Printing.PrintingProcess", "ProcessScope");
            DropColumn("Printing.PrintingProcess", "SetupCost");
            DropColumn("Printing.PrintingProcess", "CostPerItem");
            DropColumn("Printing.PrintingProcess", "Remark");
            DropColumn("Printing.PrintingProductionLaborCost", "ProcessCategory");
        }
        
        public override void Down()
        {
            AddColumn("Printing.PrintingProductionLaborCost", "ProcessCategory", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingProcess", "Remark", c => c.String());
            AddColumn("Printing.PrintingProcess", "CostPerItem", c => c.Double(nullable: false));
            AddColumn("Printing.PrintingProcess", "SetupCost", c => c.Double(nullable: false));
            AddColumn("Printing.PrintingProcess", "ProcessScope", c => c.String());
            AddColumn("Printing.PrintingProcess", "Quantity", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingProcess", "HoursRequired", c => c.Double(nullable: false));
            AddColumn("Printing.PrintingProcess", "ProcessName", c => c.String());
            AddColumn("Printing.PrintingProcess", "EstProductionPerHour", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingProcess", "PrintingProcessName", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingProcess", "PrintingCategory", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingPaperSize", "PrintingMachineTypeId", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingPaperSize", "NoOfPagesPerPlate", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingPaperSize", "NoOfPagesPerSheet", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingMachineType", "PrintingCostCenterId", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingMachineType", "Remark", c => c.String());
            AddColumn("Printing.PrintingMachineType", "MachineStatus", c => c.String());
            AddColumn("Printing.PrintingMachineType", "MakeReadyHours", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingMachineType", "PrintingHourlyChargeRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingMachineType", "NumberUnits", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingMachineType", "PaperFeedStyle", c => c.String());
            AddColumn("Printing.PrintingJobOrderProduction", "BindingStyleId", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingEstimationMaterialCost", "PaperWeight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingEstimationLaborCost", "EstimatedHours", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingEstimationLaborCost", "EstProductionPerHour", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingEstimationLaborCost", "PrintingMachineTypeId", c => c.Int());
            AddColumn("Printing.PrintingEstimationLaborCost", "PrintingProcessName", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingEstimationLaborCost", "ProcessCategory", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingCostEstimation", "TotalLaborCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingCostEstimation", "OverHeadCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingCostEstimation", "OverHeadRate", c => c.Double(nullable: false));
            AddColumn("Printing.PrintingCostEstimation", "TotalCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingCostEstimation", "DLaborRate", c => c.Double(nullable: false));
            AddColumn("Printing.PrintingCostEstimation", "EstimatedHrs", c => c.Double(nullable: false));
            AddColumn("Printing.PrintingCostEstimation", "TypeOfWorkId", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingCostEstimation", "ProcessCategoryId", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingCostEstimation", "NoUpPagesPerA1", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingCostEstimation", "CoverPaperSize", c => c.Double(nullable: false));
            AddColumn("Printing.PrintingCostEstimation", "TotalTextNoPages", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingCostEstimation", "BindingStyleId", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingCostEstimation", "JobTypeId", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingCostEstimation", "PaperSizeId", c => c.Int(nullable: false));
            AlterColumn("Printing.PrintingPaperSize", "NoOfPagesToA1Ratio", c => c.Double(nullable: false));
            AlterColumn("Printing.PrintingMachineType", "MachineTypeName", c => c.String());
            DropColumn("Printing.PrintingProformaInvoiceItem", "PrintJobType");
            DropColumn("Printing.PrintingProductionLaborCost", "PressCategory");
            DropColumn("Printing.PrintingProcess", "UOM");
            DropColumn("Printing.PrintingProcess", "PrintingMachineTypeId");
            DropColumn("Printing.PrintingProcess", "LabourActivity");
            DropColumn("Printing.PrintingProcess", "PressCategory");
            DropColumn("Printing.PrintingPaperSize", "PrintPaperSize");
            DropColumn("Printing.PrintingMaterialCategoryItem", "PaperType");
            DropColumn("Printing.PrintingMachineType", "PrintSide");
            DropColumn("Printing.PrintingMachineType", "PaperSizeId");
            DropColumn("Printing.PrintingJobOrderItem", "PrintJobType");
            DropColumn("Printing.PrintingEstimationLaborCost", "UOM");
            DropColumn("Printing.PrintingEstimationLaborCost", "LabourActivity");
            DropColumn("Printing.PrintingEstimationLaborCost", "PressCategory");
            DropColumn("Printing.PrintingEstimationDetail", "PrintJobType");
            DropColumn("Printing.PrintingCostEstimation", "CoverPrintSide");
            DropColumn("Printing.PrintingCostEstimation", "TextPrintSide");
            DropColumn("Printing.PrintingCostEstimation", "UseLength");
            DropColumn("Printing.PrintingCostEstimation", "BindingStyle");
            DropColumn("Printing.PrintingCostEstimation", "Pack");
            DropColumn("Printing.PrintingCostEstimation", "CoverMachineTypeId");
            DropColumn("Printing.PrintingCostEstimation", "TextMachineTypeId");
            DropColumn("Printing.PrintingCostEstimation", "Pages");
            DropColumn("Printing.PrintingCostEstimation", "CoverPaperSizeId");
            DropColumn("Printing.PrintingCostEstimation", "TextPaperSizeId");
            DropColumn("Printing.PrintingCostEstimation", "PrintJobType");
            DropTable("Printing.PrintingMachineDetail");
        }
    }
}
