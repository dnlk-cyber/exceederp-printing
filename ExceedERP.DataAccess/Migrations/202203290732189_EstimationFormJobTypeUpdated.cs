namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EstimationFormJobTypeUpdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("PrintingEstimation.EstimationForms", "JobTypeId", c => c.Int(nullable: false));
            DropColumn("PrintingEstimation.EstimationForms", "JobType");
        }
        
        public override void Down()
        {
            AddColumn("PrintingEstimation.EstimationForms", "JobType", c => c.Int(nullable: false));
            DropColumn("PrintingEstimation.EstimationForms", "JobTypeId");
        }
    }
}
