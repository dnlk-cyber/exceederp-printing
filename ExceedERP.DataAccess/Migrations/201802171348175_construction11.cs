namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction11 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Construction.Block", "NumberOfBlocks", c => c.Int(nullable: false));
            AlterColumn("Construction.WorkItemPerProject", "ItemName", c => c.String());
            AlterColumn("Construction.ProjectEstimationCustomWorkItem", "WorkItemName", c => c.String());
            AlterColumn("Construction.ProjectEstimnWorkItem", "ItemName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("Construction.ProjectEstimnWorkItem", "ItemName", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("Construction.ProjectEstimationCustomWorkItem", "WorkItemName", c => c.String(maxLength: 200));
            AlterColumn("Construction.WorkItemPerProject", "ItemName", c => c.String(maxLength: 200));
            DropColumn("Construction.Block", "NumberOfBlocks");
        }
    }
}
