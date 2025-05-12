namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printmodification : DbMigration
    {
        public override void Up()
        {
            AddColumn("Printing.PrintingCostEstimation", "Copy", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingCostEstimation", "Staples", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingMaterialCategoryItem", "ConsumptionRate", c => c.Int(nullable: false));
            AlterColumn("Printing.PrintingProformaInvoice", "DeliveryPeriod", c => c.String());
            AlterColumn("Printing.PrintingProformaInvoice", "PriceValidityPeriod", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("Printing.PrintingProformaInvoice", "PriceValidityPeriod", c => c.Int(nullable: false));
            AlterColumn("Printing.PrintingProformaInvoice", "DeliveryPeriod", c => c.Int(nullable: false));
            DropColumn("Printing.PrintingMaterialCategoryItem", "ConsumptionRate");
            DropColumn("Printing.PrintingCostEstimation", "Staples");
            DropColumn("Printing.PrintingCostEstimation", "Copy");
        }
    }
}
