namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class constr : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Construction.Task", "WorkItemPerProjectId", "Construction.WorkItemPerProject");
            DropIndex("Construction.Task", new[] { "WorkItemPerProjectId" });
            CreateTable(
                "Construction.DetailedWorkProgress",
                c => new
                    {
                        DetailedWorkProgressId = c.Int(nullable: false, identity: true),
                        ResourceUtilizationDayId = c.Int(nullable: false),
                        WorkItemPerProjectId = c.Int(nullable: false),
                        PlannedQuantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PlannedAmount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ActualQuantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ActualAmount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PreviousPlannedQuantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PreviousPlannedAmount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PreviousActualQuantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PreviousActualAmount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TodatePlannedQuantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TodatePlannedAmount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TodateActualQuantity = c.Decimal(nullable: false, precision: 18, scale: 4),
                        TodateActualAmount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DetailedWorkProgressId)
                .ForeignKey("Construction.WorkItemPerProject", t => t.WorkItemPerProjectId, cascadeDelete: true)
                .Index(t => t.WorkItemPerProjectId);
            
           // AddColumn("Construction.Task", "WorkItemDurationId", c => c.Int());
            AddColumn("Construction.WorkItemDuration", "BudgetCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.WorkItemDuration", "DirectCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.WorkItemDuration", "MaterialCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.WorkItemDuration", "LabourCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.WorkItemDuration", "EquipmentCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.WorkItemDuration", "ProfitCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.WorkItemDuration", "OverHeadCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.WorkItemDuration", "TotalUnitRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.WorkItemDuration", "WorkCategoryId", c => c.Int(nullable: false));
            AddColumn("Construction.WorkItemDuration", "ConfigurationId", c => c.Int(nullable: false));
            AddColumn("Construction.WorkItemDuration", "BlockId", c => c.Int(nullable: false));
            AddColumn("Construction.WorkItemDuration", "SubProjectTypeId", c => c.Int(nullable: false));
            CreateIndex("Construction.Task", "WorkItemDurationId");
            AddForeignKey("Construction.Task", "WorkItemDurationId", "Construction.WorkItemDuration", "WorkItemDurationId");
            DropColumn("Construction.Task", "WorkItemPerProjectId");
        }
        
        public override void Down()
        {
            AddColumn("Construction.Task", "WorkItemPerProjectId", c => c.Int());
            DropForeignKey("Construction.DetailedWorkProgress", "WorkItemPerProjectId", "Construction.WorkItemPerProject");
            DropForeignKey("Construction.Task", "WorkItemDurationId", "Construction.WorkItemDuration");
            DropIndex("Construction.DetailedWorkProgress", new[] { "WorkItemPerProjectId" });
            DropIndex("Construction.Task", new[] { "WorkItemDurationId" });
            DropColumn("Construction.WorkItemDuration", "SubProjectTypeId");
            DropColumn("Construction.WorkItemDuration", "BlockId");
            DropColumn("Construction.WorkItemDuration", "ConfigurationId");
            DropColumn("Construction.WorkItemDuration", "WorkCategoryId");
            DropColumn("Construction.WorkItemDuration", "TotalUnitRate");
            DropColumn("Construction.WorkItemDuration", "OverHeadCost");
            DropColumn("Construction.WorkItemDuration", "ProfitCost");
            DropColumn("Construction.WorkItemDuration", "EquipmentCost");
            DropColumn("Construction.WorkItemDuration", "LabourCost");
            DropColumn("Construction.WorkItemDuration", "MaterialCost");
            DropColumn("Construction.WorkItemDuration", "DirectCost");
            DropColumn("Construction.WorkItemDuration", "BudgetCost");
            DropColumn("Construction.Task", "WorkItemDurationId");
            DropTable("Construction.DetailedWorkProgress");
            CreateIndex("Construction.Task", "WorkItemPerProjectId");
            AddForeignKey("Construction.Task", "WorkItemPerProjectId", "Construction.WorkItemPerProject", "WorkItemPerProjectId");
        }
    }
}
