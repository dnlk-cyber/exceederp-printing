namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrmerge : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "HRM.JobTitleMappingMian",
                c => new
                    {
                        JobTitleMappingMianId = c.Int(nullable: false, identity: true),
                        MainFunction = c.String(),
                        JobTitleMappingId = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.JobTitleMappingMianId)
                .ForeignKey("HRM.JobTitleMapping", t => t.JobTitleMappingId, cascadeDelete: true)
                .Index(t => t.JobTitleMappingId);
            
            CreateTable(
                "HRM.RegionNation",
                c => new
                    {
                        RegionNationId = c.Int(nullable: false, identity: true),
                        Region = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RegionNationId)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .Index(t => t.LookupId);
            
            AddColumn("HRM.Placement", "isCurrent", c => c.Boolean(nullable: false));
            DropColumn("HRM.JobTitleMappingDetail", "MainFunction");
        }
        
        public override void Down()
        {
            AddColumn("HRM.JobTitleMappingDetail", "MainFunction", c => c.String());
            DropForeignKey("HRM.RegionNation", "LookupId", "common.Lookup");
            DropForeignKey("HRM.JobTitleMappingMian", "JobTitleMappingId", "HRM.JobTitleMapping");
            DropIndex("HRM.RegionNation", new[] { "LookupId" });
            DropIndex("HRM.JobTitleMappingMian", new[] { "JobTitleMappingId" });
            DropColumn("HRM.Placement", "isCurrent");
            DropTable("HRM.RegionNation");
            DropTable("HRM.JobTitleMappingMian");
        }
    }
}
