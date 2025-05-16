namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printingupdate1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Printing.PrintingJobOrderProduction", "PrintingJobOrderItemId", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingProductionAllottedRawMaterial", "EstimationMaterialType", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingProductionFollowUp", "PrintingJobOrderItemId", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingProductionFollowUp", "CoverPaperSizeId", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingProductionFollowUp", "TextNoOfColor", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingProductionFollowUp", "CoverNoOfColor", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingProductionFollowUpTask", "MachineTypeId", c => c.Int());
            AddColumn("Printing.PrintingProductionMaterialCost", "EstimationMaterialType", c => c.Int(nullable: false));
            AlterColumn("Printing.PrintingProductionFollowUp", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("Printing.PrintingJobOrderProduction", "JobOrderNo");
            DropColumn("Printing.PrintingJobOrderProduction", "PrintingJobOrderId");
            DropColumn("Printing.PrintingProductionFollowUp", "JobOrderId");
            DropColumn("Printing.PrintingProductionFollowUpTask", "PrintingmachineTypeId");
        }
        
        public override void Down()
        {
            AddColumn("Printing.PrintingProductionFollowUpTask", "PrintingmachineTypeId", c => c.Int());
            AddColumn("Printing.PrintingProductionFollowUp", "JobOrderId", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingJobOrderProduction", "PrintingJobOrderId", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingJobOrderProduction", "JobOrderNo", c => c.String());
            AlterColumn("Printing.PrintingProductionFollowUp", "Quantity", c => c.Int(nullable: false));
            DropColumn("Printing.PrintingProductionMaterialCost", "EstimationMaterialType");
            DropColumn("Printing.PrintingProductionFollowUpTask", "MachineTypeId");
            DropColumn("Printing.PrintingProductionFollowUp", "CoverNoOfColor");
            DropColumn("Printing.PrintingProductionFollowUp", "TextNoOfColor");
            DropColumn("Printing.PrintingProductionFollowUp", "CoverPaperSizeId");
            DropColumn("Printing.PrintingProductionFollowUp", "PrintingJobOrderItemId");
            DropColumn("Printing.PrintingProductionAllottedRawMaterial", "EstimationMaterialType");
            DropColumn("Printing.PrintingJobOrderProduction", "PrintingJobOrderItemId");
        }
    }
}
