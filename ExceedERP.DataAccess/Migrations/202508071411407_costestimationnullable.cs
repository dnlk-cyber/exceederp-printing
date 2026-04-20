namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class costestimationnullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Printing.PrintingCostEstimation", "CoverPaperSizeId", c => c.Int());
            AlterColumn("Printing.PrintingCostEstimation", "CoverNoOfColor", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("Printing.PrintingCostEstimation", "CoverNoOfColor", c => c.Int(nullable: false));
            AlterColumn("Printing.PrintingCostEstimation", "CoverPaperSizeId", c => c.Int(nullable: false));
        }
    }
}
