namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printandfurnituremig : DbMigration
    {
        public override void Up()
        {
            AddColumn("Printing.PrintingProductionFollowUp", "IsClosed", c => c.Boolean(nullable: false));
            DropColumn("JobCosting.JobMaterialCosts", "MaterialType");
        }
        
        public override void Down()
        {
            AddColumn("JobCosting.JobMaterialCosts", "MaterialType", c => c.String());
            DropColumn("Printing.PrintingProductionFollowUp", "IsClosed");
        }
    }
}
