namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction42 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Construction.ProjectCustomWorkItemContent", "ProjectCustomWorkItemId", "Construction.ProjectCustomWorkItem");
            DropForeignKey("Construction.ProjectCustomWorkItemContent", "WorkItemPerProjectId", "Construction.WorkItemPerProject");
            DropIndex("Construction.ProjectCustomWorkItemContent", new[] { "WorkItemPerProjectId" });
            DropIndex("Construction.ProjectCustomWorkItemContent", new[] { "ProjectCustomWorkItemId" });
            CreateTable(
                "Construction.EstimationTemplateCustomWorkItemDetail",
                c => new
                    {
                        EstimationTemplateCustomWorkItemDetailId = c.Int(nullable: false, identity: true),
                        ProjectEstimationWorkItemId = c.Int(nullable: false),
                        ParentId = c.Int(nullable: false),
                        QuantityPerUnit = c.Decimal(nullable: false, precision: 18, scale: 4),
                        UnitCost = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EstimationTemplateCustomWorkItemDetailId)
                .ForeignKey("Construction.ProjectEstimnWorkItem", t => t.ProjectEstimationWorkItemId, cascadeDelete: true)
                .Index(t => t.ProjectEstimationWorkItemId);
            
            CreateTable(
                "Construction.GenericProjectCustomWorkItemDetail",
                c => new
                    {
                        GenericProjectCustomWorkItemDetailId = c.Int(nullable: false, identity: true),
                        ProjectEstimationWorkItemId = c.Int(nullable: false),
                        GenericWorkItemPerProjectId = c.Int(nullable: false),
                        QuantityPerUnit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GenericProjectCustomWorkItemDetailId)
                .ForeignKey("Construction.ProjectEstimnWorkItem", t => t.ProjectEstimationWorkItemId, cascadeDelete: true)
                .Index(t => t.ProjectEstimationWorkItemId);
            
            CreateTable(
                "Construction.ProjectCustomWorkItemDetail",
                c => new
                    {
                        ProjectCustomWorkItemDetailId = c.Int(nullable: false, identity: true),
                        ProjectEstimationWorkItemId = c.Int(nullable: false),
                        WorkItemPerProjectId = c.Int(nullable: false),
                        QuantityPerUnit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectCustomWorkItemDetailId)
                .ForeignKey("Construction.ProjectEstimnWorkItem", t => t.ProjectEstimationWorkItemId, cascadeDelete: true)
                .Index(t => t.ProjectEstimationWorkItemId);
            
            AddColumn("Construction.WorkItemPerProject", "ContainsWorkItemAsInput", c => c.Boolean(nullable: false));
            AddColumn("Construction.ProjectEstimnWorkItem", "ContainsWorkItemAsInput", c => c.Boolean(nullable: false));
            AddColumn("Construction.GenericWorkItemPerProject", "ContainsWorkItemAsInput", c => c.Boolean(nullable: false));
            AddColumn("Construction.WorkItemDuration", "HasSubWorkItems", c => c.Boolean(nullable: false));
            DropTable("Construction.ProjectCustomWorkItemContent");
        }
        
        public override void Down()
        {
            CreateTable(
                "Construction.ProjectCustomWorkItemContent",
                c => new
                    {
                        ProjectCustomWorkItemContentId = c.Int(nullable: false, identity: true),
                        WorkItemPerProjectId = c.Int(nullable: false),
                        ProjectCustomWorkItemId = c.Int(nullable: false),
                        QuantityPerUnit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectCustomWorkItemContentId);
            
            DropForeignKey("Construction.ProjectCustomWorkItemDetail", "ProjectEstimationWorkItemId", "Construction.ProjectEstimnWorkItem");
            DropForeignKey("Construction.GenericProjectCustomWorkItemDetail", "ProjectEstimationWorkItemId", "Construction.ProjectEstimnWorkItem");
            DropForeignKey("Construction.EstimationTemplateCustomWorkItemDetail", "ProjectEstimationWorkItemId", "Construction.ProjectEstimnWorkItem");
            DropIndex("Construction.ProjectCustomWorkItemDetail", new[] { "ProjectEstimationWorkItemId" });
            DropIndex("Construction.GenericProjectCustomWorkItemDetail", new[] { "ProjectEstimationWorkItemId" });
            DropIndex("Construction.EstimationTemplateCustomWorkItemDetail", new[] { "ProjectEstimationWorkItemId" });
            DropColumn("Construction.WorkItemDuration", "HasSubWorkItems");
            DropColumn("Construction.GenericWorkItemPerProject", "ContainsWorkItemAsInput");
            DropColumn("Construction.ProjectEstimnWorkItem", "ContainsWorkItemAsInput");
            DropColumn("Construction.WorkItemPerProject", "ContainsWorkItemAsInput");
            DropTable("Construction.ProjectCustomWorkItemDetail");
            DropTable("Construction.GenericProjectCustomWorkItemDetail");
            DropTable("Construction.EstimationTemplateCustomWorkItemDetail");
            CreateIndex("Construction.ProjectCustomWorkItemContent", "ProjectCustomWorkItemId");
            CreateIndex("Construction.ProjectCustomWorkItemContent", "WorkItemPerProjectId");
            AddForeignKey("Construction.ProjectCustomWorkItemContent", "WorkItemPerProjectId", "Construction.WorkItemPerProject", "WorkItemPerProjectId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectCustomWorkItemContent", "ProjectCustomWorkItemId", "Construction.ProjectCustomWorkItem", "ProjectCustomWorkItemId", cascadeDelete: true);
        }
    }
}
