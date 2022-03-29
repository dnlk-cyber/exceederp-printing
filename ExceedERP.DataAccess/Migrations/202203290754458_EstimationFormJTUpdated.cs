namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EstimationFormJTUpdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("PrintingEstimation.EstimationForms", "JT", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("PrintingEstimation.EstimationForms", "JT");
        }
    }
}
