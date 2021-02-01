namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction10 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Construction.ProjectMaterialExpense", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ResourceUtilizationWorkItem", "ConstructionPeriodId", "Construction.ConstructionPeriod");
            DropIndex("Construction.ProjectMaterialExpense", new[] { "ProjectInformationId" });
            DropIndex("Construction.ResourceUtilizationWorkItem", new[] { "ConstructionPeriodId" });
            CreateTable(
                "Construction.ResourceUtilizationPeriod",
                c => new
                    {
                        ResourceUtilizationPeriodId = c.Int(nullable: false, identity: true),
                        ConstructionPeriodId = c.Int(nullable: false),
                        ProjectInformationId = c.Int(nullable: false),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ResourceUtilizationPeriodId)
                .ForeignKey("Construction.ConstructionPeriod", t => t.ConstructionPeriodId, cascadeDelete: true)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ConstructionPeriodId)
                .Index(t => t.ProjectInformationId);
            
            AddColumn("Construction.MaterialPriceIndex", "IsExpired", c => c.Boolean(nullable: false));
            AddColumn("Construction.ProjectMaterialExpense", "BranchId", c => c.Int(nullable: false));
            AddColumn("Construction.ResourceUtilizationWorkItem", "ResourceUtilizationPeriodId", c => c.Int(nullable: false));
            DropColumn("Construction.ProjectMaterialExpense", "ProjectInformationId");
            DropColumn("Construction.ResourceUtilizationWorkItem", "ConstructionPeriodId");
        }
        
        public override void Down()
        {
            AddColumn("Construction.ResourceUtilizationWorkItem", "ConstructionPeriodId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectMaterialExpense", "ProjectInformationId", c => c.Int(nullable: false));
            DropForeignKey("Construction.ResourceUtilizationPeriod", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.ResourceUtilizationPeriod", "ConstructionPeriodId", "Construction.ConstructionPeriod");
            DropIndex("Construction.ResourceUtilizationPeriod", new[] { "ProjectInformationId" });
            DropIndex("Construction.ResourceUtilizationPeriod", new[] { "ConstructionPeriodId" });
            DropColumn("Construction.ResourceUtilizationWorkItem", "ResourceUtilizationPeriodId");
            DropColumn("Construction.ProjectMaterialExpense", "BranchId");
            DropColumn("Construction.MaterialPriceIndex", "IsExpired");
            DropTable("Construction.ResourceUtilizationPeriod");
            CreateIndex("Construction.ResourceUtilizationWorkItem", "ConstructionPeriodId");
            CreateIndex("Construction.ProjectMaterialExpense", "ProjectInformationId");
            AddForeignKey("Construction.ResourceUtilizationWorkItem", "ConstructionPeriodId", "Construction.ConstructionPeriod", "ConstructionPeriodId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectMaterialExpense", "ProjectInformationId", "Construction.ProjectInformation", "ProjectInformationId", cascadeDelete: true);
        }
    }
}
