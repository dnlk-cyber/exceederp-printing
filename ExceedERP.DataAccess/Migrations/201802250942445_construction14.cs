namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction14 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Construction.ProjectEstimnBoqBreakdown", "UnitOfMeasurementId", "Construction.UnitOfMeasurement");
            DropForeignKey("Construction.WorkItemPerProject", "UnitOfMeasurementId", "Construction.UnitOfMeasurement");
            DropIndex("Construction.ProjectEstimnBoqBreakdown", new[] { "UnitOfMeasurementId" });
            DropIndex("Construction.WorkItemPerProject", new[] { "UnitOfMeasurementId" });
            AddColumn("Construction.WorkItemPerProject", "IsSubCategory", c => c.Boolean(nullable: false));
            AddColumn("Construction.WorkItemPerProject", "CopiedFromId", c => c.Int());
            AddColumn("Construction.ProjectEstimationCustomWorkItem", "WorkCategoryId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectCustomWorkItem", "WorkCategoryId", c => c.Int(nullable: false));
            AlterColumn("Construction.WorkItemPerProject", "UnitOfMeasurementId", c => c.Int());
            AlterColumn("Construction.ProjectCustomWorkItem", "WorkItemName", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("Construction.ProjectCustomWorkItem", "WorkItemName", c => c.String(maxLength: 200));
            AlterColumn("Construction.WorkItemPerProject", "UnitOfMeasurementId", c => c.Int(nullable: false));
            DropColumn("Construction.ProjectCustomWorkItem", "WorkCategoryId");
            DropColumn("Construction.ProjectEstimationCustomWorkItem", "WorkCategoryId");
            DropColumn("Construction.WorkItemPerProject", "CopiedFromId");
            DropColumn("Construction.WorkItemPerProject", "IsSubCategory");
            CreateIndex("Construction.WorkItemPerProject", "UnitOfMeasurementId");
            CreateIndex("Construction.ProjectEstimnBoqBreakdown", "UnitOfMeasurementId");
            AddForeignKey("Construction.WorkItemPerProject", "UnitOfMeasurementId", "Construction.UnitOfMeasurement", "UnitOfMeasurementId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstimnBoqBreakdown", "UnitOfMeasurementId", "Construction.UnitOfMeasurement", "UnitOfMeasurementId", cascadeDelete: true);
        }
    }
}
