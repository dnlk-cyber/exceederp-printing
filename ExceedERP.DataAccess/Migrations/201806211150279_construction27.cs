namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction27 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Construction.EarnedValuePerItem", "WorkItemPerProjectId", "Construction.WorkItemPerProject");
            DropIndex("Construction.EarnedValuePerItem", new[] { "WorkItemPerProjectId" });
            CreateTable(
                "Construction.EarnedValueManagementPeriods",
                c => new
                    {
                        EarnedValueManagementPeriodId = c.Int(nullable: false, identity: true),
                        ConstructionPeriodId = c.Int(nullable: false),
                        ProjectInformationId = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EarnedValueManagementPeriodId)
                .ForeignKey("Construction.ConstructionPeriod", t => t.ConstructionPeriodId, cascadeDelete: true)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ConstructionPeriodId)
                .Index(t => t.ProjectInformationId);
            
            AddColumn("Construction.EarnedValuePerItem", "EarnedValueManagementPeriodId", c => c.Int(nullable: false));
            CreateIndex("Construction.EarnedValuePerItem", "EarnedValueManagementPeriodId");
            AddForeignKey("Construction.EarnedValuePerItem", "EarnedValueManagementPeriodId", "Construction.EarnedValueManagementPeriods", "EarnedValueManagementPeriodId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("Construction.EarnedValuePerItem", "EarnedValueManagementPeriodId", "Construction.EarnedValueManagementPeriods");
            DropForeignKey("Construction.EarnedValueManagementPeriods", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.EarnedValueManagementPeriods", "ConstructionPeriodId", "Construction.ConstructionPeriod");
            DropIndex("Construction.EarnedValuePerItem", new[] { "EarnedValueManagementPeriodId" });
            DropIndex("Construction.EarnedValueManagementPeriods", new[] { "ProjectInformationId" });
            DropIndex("Construction.EarnedValueManagementPeriods", new[] { "ConstructionPeriodId" });
            DropColumn("Construction.EarnedValuePerItem", "EarnedValueManagementPeriodId");
            DropTable("Construction.EarnedValueManagementPeriods");
            CreateIndex("Construction.EarnedValuePerItem", "WorkItemPerProjectId");
            AddForeignKey("Construction.EarnedValuePerItem", "WorkItemPerProjectId", "Construction.WorkItemPerProject", "WorkItemPerProjectId", cascadeDelete: true);
        }
    }
}
