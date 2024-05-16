namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printingcost : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Printing.PrintingCostEstimation",
                c => new
                    {
                        PrintingCostEstimationId = c.Int(nullable: false, identity: true),
                        PaperSizeId = c.Int(nullable: false),
                        PaperTypeId = c.Int(nullable: false),
                        ProductQuantity = c.Int(nullable: false),
                        TotalTextNoPages = c.Int(nullable: false),
                        BindingStyleId = c.Int(nullable: false),
                        Width = c.Double(nullable: false),
                        Length = c.Double(nullable: false),
                        TextNoOfColors = c.Int(nullable: false),
                        CoverNoOfColor = c.Int(nullable: false),
                        CoverPaperSize = c.Double(nullable: false),
                        CoverRequired = c.Boolean(nullable: false),
                        OnlyOneSideCover = c.Boolean(nullable: false),
                        NoUpPagesPerA1 = c.Int(nullable: false),
                        ProcessCategoryId = c.Int(nullable: false),
                        TypeOfWorkId = c.Int(nullable: false),
                        EstimatedHrs = c.Double(nullable: false),
                        DLaborRate = c.Double(nullable: false),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverHeadRate = c.Double(nullable: false),
                        OverHeadCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalLaborCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PrintingCostEstimationId);
            
        }
        
        public override void Down()
        {
            DropTable("Printing.PrintingCostEstimation");
        }
    }
}
