namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printingcostestim : DbMigration
    {
        public override void Up()
        {
            AddColumn("Printing.PrintingCostEstimation", "NumberOfPagesForDesign", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Printing.PrintingCostEstimation", "NumberOfPagesForDesign");
        }
    }
}
