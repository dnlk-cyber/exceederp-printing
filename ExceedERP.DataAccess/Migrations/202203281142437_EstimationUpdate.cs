namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EstimationUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("PrintingEstimation.EstimationForms", "JobType", c => c.Int(nullable: false));
            AddColumn("PrintingEstimation.EstimationForms", "IsChecked", c => c.Boolean(nullable: false));
            AddColumn("PrintingEstimation.EstimationForms", "IsApproved", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("PrintingEstimation.EstimationForms", "IsApproved");
            DropColumn("PrintingEstimation.EstimationForms", "IsChecked");
            DropColumn("PrintingEstimation.EstimationForms", "JobType");
        }
    }
}
