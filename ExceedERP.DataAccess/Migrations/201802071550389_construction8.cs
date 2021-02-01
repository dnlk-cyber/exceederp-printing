namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction8 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Construction.EstimationCustomWorkItemContent",
                c => new
                    {
                        EstimationCustomWorkItemContentId = c.Int(nullable: false, identity: true),
                        ProjectEstimationWorkItemId = c.Int(nullable: false),
                        ProjectEstimationCustomWorkItemId = c.Int(nullable: false),
                        QuantityPerUnit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EstimationCustomWorkItemContentId)
                .ForeignKey("Construction.ProjectEstimationCustomWorkItem", t => t.ProjectEstimationCustomWorkItemId, cascadeDelete: true)
                .ForeignKey("Construction.ProjectEstimnWorkItem", t => t.ProjectEstimationWorkItemId, cascadeDelete: true)
                .Index(t => t.ProjectEstimationWorkItemId)
                .Index(t => t.ProjectEstimationCustomWorkItemId);
            
            CreateTable(
                "Construction.ProjectEstimationCustomWorkItem",
                c => new
                    {
                        ProjectEstimationCustomWorkItemId = c.Int(nullable: false, identity: true),
                        WorkItemName = c.String(maxLength: 200),
                        TotalUnitRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectType = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectEstimationCustomWorkItemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Construction.EstimationCustomWorkItemContent", "ProjectEstimationWorkItemId", "Construction.ProjectEstimnWorkItem");
            DropForeignKey("Construction.EstimationCustomWorkItemContent", "ProjectEstimationCustomWorkItemId", "Construction.ProjectEstimationCustomWorkItem");
            DropIndex("Construction.EstimationCustomWorkItemContent", new[] { "ProjectEstimationCustomWorkItemId" });
            DropIndex("Construction.EstimationCustomWorkItemContent", new[] { "ProjectEstimationWorkItemId" });
            DropTable("Construction.ProjectEstimationCustomWorkItem");
            DropTable("Construction.EstimationCustomWorkItemContent");
        }
    }
}
