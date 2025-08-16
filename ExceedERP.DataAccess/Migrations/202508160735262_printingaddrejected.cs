namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printingaddrejected : DbMigration
    {
        public override void Up()
        {
            AddColumn("Printing.PrintingEstimationSummary", "IsRejected", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Printing.PrintingEstimationSummary", "IsRejected");
        }
    }
}
