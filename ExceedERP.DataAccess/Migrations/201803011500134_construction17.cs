namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction17 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Construction.ProjectEstimationCustomWorkItem", "ItemCode", c => c.String());
            AddColumn("Construction.ProjectCustomWorkItem", "ItemCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("Construction.ProjectCustomWorkItem", "ItemCode");
            DropColumn("Construction.ProjectEstimationCustomWorkItem", "ItemCode");
        }
    }
}
