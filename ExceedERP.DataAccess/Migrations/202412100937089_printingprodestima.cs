namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printingprodestima : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Printing.PrintingEstimationMargin",
                c => new
                    {
                        PrintingEstimationMarginId = c.Int(nullable: false, identity: true),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProfitMargin = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsApprovedMargin = c.Boolean(nullable: false),
                        PrintingEstimationDetailId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PrintingEstimationMarginId);
            
            CreateTable(
                "Printing.PrintingJobOrderProduction",
                c => new
                    {
                        PrintingJobOrderProductionId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        JobTypeId = c.Int(nullable: false),
                        PaperSizeId = c.Int(nullable: false),
                        BindingStyleId = c.Int(nullable: false),
                        TextNoOfColors = c.Int(nullable: false),
                        CoverNoOfColor = c.Int(nullable: false),
                        TotalTextNoPages = c.Int(nullable: false),
                        JobOrderNo = c.String(),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        JobDescription = c.String(),
                        DateOrdered = c.DateTime(nullable: false),
                        DeliveryDate = c.DateTime(nullable: false),
                        PrintingJobOrderId = c.Int(nullable: false),
                        IsSendForFinishing = c.Boolean(nullable: false),
                        SendForFinishingBy = c.String(),
                        SendForFinishingDate = c.DateTime(),
                        IsClosed = c.Boolean(nullable: false),
                        ClosedBy = c.String(),
                        DateClosed = c.DateTime(),
                        Status = c.String(),
                        Remark = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        IsOnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PrintingJobOrderProductionId);
            
            CreateTable(
                "Printing.PrintingProductionLaborCost",
                c => new
                    {
                        PrintingProductionLaborCostId = c.Int(nullable: false, identity: true),
                        ProcessCategory = c.Int(nullable: false),
                        PrintingProcessId = c.Int(),
                        PrintingMachineTypeId = c.Int(),
                        EstimatedHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LaborRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PrintingJobOrderProductionId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PrintingProductionLaborCostId);
            
            CreateTable(
                "Printing.PrintingProductionMaterialCost",
                c => new
                    {
                        PrintingProductionMaterialCostId = c.Int(nullable: false, identity: true),
                        PrintingMaterialCategoryId = c.Int(nullable: false),
                        PrintingMaterialCategoryItemId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitOfMeasurment = c.String(),
                        PrintingJobOrderProductionId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PrintingProductionMaterialCostId);
            
            AddColumn("Printing.PrintingEstimationDetail", "GraphicsCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingEstimationDetail", "TotalCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingEstimationDetail", "UnitCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingEstimationDetail", "ProfitMargin", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingEstimationDetail", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingEstimationDetail", "TotalPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingEstimationDetail", "IsCalculated", c => c.Boolean(nullable: false));
            DropColumn("Printing.PrintingEstimationDetail", "GrandTotalCost");
        }
        
        public override void Down()
        {
            AddColumn("Printing.PrintingEstimationDetail", "GrandTotalCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("Printing.PrintingEstimationDetail", "IsCalculated");
            DropColumn("Printing.PrintingEstimationDetail", "TotalPrice");
            DropColumn("Printing.PrintingEstimationDetail", "UnitPrice");
            DropColumn("Printing.PrintingEstimationDetail", "ProfitMargin");
            DropColumn("Printing.PrintingEstimationDetail", "UnitCost");
            DropColumn("Printing.PrintingEstimationDetail", "TotalCost");
            DropColumn("Printing.PrintingEstimationDetail", "GraphicsCost");
            DropTable("Printing.PrintingProductionMaterialCost");
            DropTable("Printing.PrintingProductionLaborCost");
            DropTable("Printing.PrintingJobOrderProduction");
            DropTable("Printing.PrintingEstimationMargin");
        }
    }
}
