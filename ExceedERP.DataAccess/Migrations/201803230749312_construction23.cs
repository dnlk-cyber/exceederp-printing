namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction23 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Construction.ProjectSpecificSettingAttribute",
                c => new
                    {
                        ProjectSpecificSettingAttributeId = c.Int(nullable: false, identity: true),
                        ProjectInformationId = c.Int(nullable: false),
                        Attribute = c.Int(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectSpecificSettingAttributeId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.ResourceUtilizationDays",
                c => new
                    {
                        ResourceUtilizationDayId = c.Int(nullable: false, identity: true),
                        ResourceUtilizationPeriodId = c.Int(nullable: false),
                        Name = c.String(),
                        DateFrom = c.DateTime(nullable: false),
                        DateTo = c.DateTime(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ResourceUtilizationDayId)
                .ForeignKey("Construction.ResourceUtilizationPeriod", t => t.ResourceUtilizationPeriodId, cascadeDelete: true)
                .Index(t => t.ResourceUtilizationPeriodId);
            
            AddColumn("Construction.ResourceUtilizationWorkItem", "ResourceUtilizationDayId", c => c.Int(nullable: false));
            DropColumn("Construction.ResourceUtilizationWorkItem", "ResourceUtilizationPeriodId");
        }
        
        public override void Down()
        {
            AddColumn("Construction.ResourceUtilizationWorkItem", "ResourceUtilizationPeriodId", c => c.Int(nullable: false));
            DropForeignKey("Construction.ResourceUtilizationDays", "ResourceUtilizationPeriodId", "Construction.ResourceUtilizationPeriod");
            DropForeignKey("Construction.ProjectSpecificSettingAttribute", "ProjectInformationId", "Construction.ProjectInformation");
            DropIndex("Construction.ResourceUtilizationDays", new[] { "ResourceUtilizationPeriodId" });
            DropIndex("Construction.ProjectSpecificSettingAttribute", new[] { "ProjectInformationId" });
            DropColumn("Construction.ResourceUtilizationWorkItem", "ResourceUtilizationDayId");
            DropTable("Construction.ResourceUtilizationDays");
            DropTable("Construction.ProjectSpecificSettingAttribute");
        }
    }
}
