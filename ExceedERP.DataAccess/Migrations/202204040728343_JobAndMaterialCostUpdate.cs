namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JobAndMaterialCostUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("PrintingEstimation.MaterialCosts", "JobId", c => c.Int());
            AddColumn("ProductFollowUp.Job", "IsChecked", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("ProductFollowUp.Job", "IsChecked");
            DropColumn("PrintingEstimation.MaterialCosts", "JobId");
        }
    }
}
