namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class estimation : DbMigration
    {
        public override void Up()
        {
            AddColumn("Printing.PrintingOverAllCost", "IndirectMaterialCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingOverAllCost", "MaterialHandlingStorageWaste", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingOverAllCost", "GraphicsRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingOverAllCost", "GLTaxRateID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Printing.PrintingOverAllCost", "GLTaxRateID");
            DropColumn("Printing.PrintingOverAllCost", "GraphicsRate");
            DropColumn("Printing.PrintingOverAllCost", "MaterialHandlingStorageWaste");
            DropColumn("Printing.PrintingOverAllCost", "IndirectMaterialCost");
        }
    }
}
