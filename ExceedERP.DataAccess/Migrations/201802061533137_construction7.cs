namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction7 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Construction.ProjectTypeMapping", "ProjectTypeId", "Construction.ProjectType");
            DropIndex("Construction.ProjectTypeMapping", new[] { "ProjectTypeId" });
            AddColumn("Construction.ProjectInformation", "ProjectType", c => c.Int(nullable: false));
            AddColumn("Construction.WorkItemPerProject", "ProjectType", c => c.Int(nullable: false));
            AddColumn("Construction.SubContractWorkItemEstmn", "ProjectType", c => c.Int(nullable: false));
            AddColumn("Construction.EstimationAreaWorkItem", "ProjectType", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectEstimnWorkItem", "ProjectType", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectTypeMapping", "ProjectType", c => c.Int(nullable: false));
            AlterColumn("Construction.TakeOffPerProject", "TakeOffNo", c => c.String(maxLength: 200));
            DropColumn("Construction.ProjectInformation", "ProjectTypeId");
            DropColumn("Construction.WorkItemPerProject", "ProjectTypeId");
            DropColumn("Construction.SubContractWorkItemEstmn", "ProjectTypeId");
            DropColumn("Construction.EstimationAreaWorkItem", "ProjectTypeId");
            DropColumn("Construction.ProjectEstimnWorkItem", "ProjectTypeId");
            DropColumn("Construction.ProjectTypeMapping", "ProjectTypeId");
            DropTable("Construction.ProjectType");
        }
        
        public override void Down()
        {
            CreateTable(
                "Construction.ProjectType",
                c => new
                    {
                        ProjectTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ProjectTypes = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ProjectTypeId);
            
            AddColumn("Construction.ProjectTypeMapping", "ProjectTypeId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectEstimnWorkItem", "ProjectTypeId", c => c.Int(nullable: false));
            AddColumn("Construction.EstimationAreaWorkItem", "ProjectTypeId", c => c.Int(nullable: false));
            AddColumn("Construction.SubContractWorkItemEstmn", "ProjectTypeId", c => c.Int(nullable: false));
            AddColumn("Construction.WorkItemPerProject", "ProjectTypeId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectInformation", "ProjectTypeId", c => c.Int(nullable: false));
            AlterColumn("Construction.TakeOffPerProject", "TakeOffNo", c => c.String(maxLength: 100));
            DropColumn("Construction.ProjectTypeMapping", "ProjectType");
            DropColumn("Construction.ProjectEstimnWorkItem", "ProjectType");
            DropColumn("Construction.EstimationAreaWorkItem", "ProjectType");
            DropColumn("Construction.SubContractWorkItemEstmn", "ProjectType");
            DropColumn("Construction.WorkItemPerProject", "ProjectType");
            DropColumn("Construction.ProjectInformation", "ProjectType");
            CreateIndex("Construction.ProjectTypeMapping", "ProjectTypeId");
            AddForeignKey("Construction.ProjectTypeMapping", "ProjectTypeId", "Construction.ProjectType", "ProjectTypeId", cascadeDelete: true);
        }
    }
}
