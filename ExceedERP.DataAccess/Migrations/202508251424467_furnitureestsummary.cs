namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class furnitureestsummary : DbMigration
    {
        public override void Up()
        {
            AddColumn("Manufacturing.EstimationDetail", "FurnitureEstimationId", c => c.Int(nullable: false));
            DropColumn("Manufacturing.EstimationSummary", "FurnitureEstimationId");
        }
        
        public override void Down()
        {
            AddColumn("Manufacturing.EstimationSummary", "FurnitureEstimationId", c => c.Int(nullable: false));
            DropColumn("Manufacturing.EstimationDetail", "FurnitureEstimationId");
        }
    }
}
