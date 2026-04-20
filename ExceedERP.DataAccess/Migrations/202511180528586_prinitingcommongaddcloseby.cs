namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prinitingcommongaddcloseby : DbMigration
    {
        public override void Up()
        {
            AddColumn("Printing.PrintingProductionFollowUp", "ClosedBy", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUp", "DateClosed", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("Printing.PrintingProductionFollowUp", "DateClosed");
            DropColumn("Printing.PrintingProductionFollowUp", "ClosedBy");
        }
    }
}
