namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction19 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Construction.WorkItemDuration", "TakeOffPerProjectId", "Construction.TakeOffPerProject");
            DropIndex("Construction.WorkItemDuration", new[] { "TakeOffPerProjectId" });
            AddColumn("Construction.WorkItemDuration", "WorkItemPerProjectId", c => c.Int(nullable: false));
            AddColumn("Construction.WorkItemDuration", "ItemCode", c => c.String());
            CreateIndex("Construction.WorkItemDuration", "WorkItemPerProjectId");
            AddForeignKey("Construction.WorkItemDuration", "WorkItemPerProjectId", "Construction.WorkItemPerProject", "WorkItemPerProjectId", cascadeDelete: true);
            DropColumn("Construction.WorkItemDuration", "WorkItemId");
            DropColumn("Construction.WorkItemDuration", "TakeOffPerProjectId");
        }
        
        public override void Down()
        {
            AddColumn("Construction.WorkItemDuration", "TakeOffPerProjectId", c => c.Int(nullable: false));
            AddColumn("Construction.WorkItemDuration", "WorkItemId", c => c.Int(nullable: false));
            DropForeignKey("Construction.WorkItemDuration", "WorkItemPerProjectId", "Construction.WorkItemPerProject");
            DropIndex("Construction.WorkItemDuration", new[] { "WorkItemPerProjectId" });
            DropColumn("Construction.WorkItemDuration", "ItemCode");
            DropColumn("Construction.WorkItemDuration", "WorkItemPerProjectId");
            CreateIndex("Construction.WorkItemDuration", "TakeOffPerProjectId");
            AddForeignKey("Construction.WorkItemDuration", "TakeOffPerProjectId", "Construction.TakeOffPerProject", "TakeOffPerProjectId", cascadeDelete: true);
        }
    }
}
