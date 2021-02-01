namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction6 : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "Task.Task", newSchema: "Construction");
            MoveTable(name: "Task.Dependency", newSchema: "Construction");
            MoveTable(name: "Task.ResourceAssignment", newSchema: "Construction");
            MoveTable(name: "Task.Resource", newSchema: "Construction");
            MoveTable(name: "Task.ResourceType", newSchema: "Construction");
            CreateTable(
                "Construction.ScheduleRevision",
                c => new
                    {
                        ScheduleRevisionId = c.Int(nullable: false, identity: true),
                        ProjectInformationId = c.Int(nullable: false),
                        Version = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        DateOfApproval = c.DateTime(),
                        ChangeDescription = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ScheduleRevisionId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
            AddColumn("Construction.Task", "Version", c => c.Int(nullable: false));
            AddColumn("Construction.Task", "PreviousVersionTaskId", c => c.Int());
            AddColumn("Construction.Task", "BudgetCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.DeformedBar", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("Construction.MaterialPriceIndex", "GLPeriodId", c => c.Int(nullable: false));
            AddColumn("Construction.MaterialPriceIndex", "DateOfInformation", c => c.DateTime(nullable: false));
            AddColumn("Construction.ResourceUtilizationWorkItem", "ConstructionPeriodId", c => c.Int(nullable: false));
            CreateIndex("Construction.DeformedBar", "ItemCode");
            CreateIndex("Construction.MaterialPriceIndex", "GLPeriodId");
            CreateIndex("Construction.ResourceUtilizationWorkItem", "ConstructionPeriodId");
            AddForeignKey("Construction.DeformedBar", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("Construction.MaterialPriceIndex", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
            AddForeignKey("Construction.ResourceUtilizationWorkItem", "ConstructionPeriodId", "Construction.ConstructionPeriod", "ConstructionPeriodId", cascadeDelete: true);
            DropColumn("Construction.DeformedBar", "ConcatnatedCode");
        }
        
        public override void Down()
        {
            AddColumn("Construction.DeformedBar", "ConcatnatedCode", c => c.String());
            DropForeignKey("Construction.ScheduleRevision", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ResourceUtilizationWorkItem", "ConstructionPeriodId", "Construction.ConstructionPeriod");
            DropForeignKey("Construction.MaterialPriceIndex", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("Construction.DeformedBar", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("Construction.ScheduleRevision", new[] { "ProjectInformationId" });
            DropIndex("Construction.ResourceUtilizationWorkItem", new[] { "ConstructionPeriodId" });
            DropIndex("Construction.MaterialPriceIndex", new[] { "GLPeriodId" });
            DropIndex("Construction.DeformedBar", new[] { "ItemCode" });
            DropColumn("Construction.ResourceUtilizationWorkItem", "ConstructionPeriodId");
            DropColumn("Construction.MaterialPriceIndex", "DateOfInformation");
            DropColumn("Construction.MaterialPriceIndex", "GLPeriodId");
            DropColumn("Construction.DeformedBar", "ItemCode");
            DropColumn("Construction.Task", "BudgetCost");
            DropColumn("Construction.Task", "PreviousVersionTaskId");
            DropColumn("Construction.Task", "Version");
            DropTable("Construction.ScheduleRevision");
            MoveTable(name: "Construction.ResourceType", newSchema: "Task");
            MoveTable(name: "Construction.Resource", newSchema: "Task");
            MoveTable(name: "Construction.ResourceAssignment", newSchema: "Task");
            MoveTable(name: "Construction.Dependency", newSchema: "Task");
            MoveTable(name: "Construction.Task", newSchema: "Task");
        }
    }
}
