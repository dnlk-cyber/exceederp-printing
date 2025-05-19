namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printingcost1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Printing.PrintingCostEstimation", "UseAdvancedCalculations", c => c.Boolean(nullable: false));
            DropColumn("Printing.PrintingOverAllCost", "GLTaxRateID");
        }
        
        public override void Down()
        {
            AddColumn("Printing.PrintingOverAllCost", "GLTaxRateID", c => c.Int(nullable: false));
            DropColumn("Printing.PrintingCostEstimation", "UseAdvancedCalculations");
        }
    }
}
