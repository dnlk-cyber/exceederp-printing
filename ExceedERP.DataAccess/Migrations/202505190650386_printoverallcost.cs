namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printoverallcost : DbMigration
    {
        public override void Up()
        {
            AddColumn("Printing.PrintingOverAllCost", "AdminRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingOverAllCost", "OverHeadRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("Printing.PrintingOverAllCost", "OverHeadRate");
            DropColumn("Printing.PrintingOverAllCost", "AdminRate");
        }
    }
}
