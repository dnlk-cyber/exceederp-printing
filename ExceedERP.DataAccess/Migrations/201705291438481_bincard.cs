namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bincard : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "HRM.JobTitleMappingDetailTemp",
                c => new
                    {
                        JobTitleMappingDetailId = c.Int(nullable: false, identity: true),
                        LookupId = c.Int(nullable: false),
                        RelevantExp = c.String(),
                        Experience = c.String(),
                        SpecialSkill = c.String(),
                        PlannedSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        JobTitleMappingId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.JobTitleMappingDetailId)
                .ForeignKey("HRM.JobTitleMappingTemp", t => t.JobTitleMappingId, cascadeDelete: true)
                .Index(t => t.JobTitleMappingId);
            
            CreateTable(
                "HRM.JobTitleMappingTemp",
                c => new
                    {
                        JobTitleMappingId = c.Int(nullable: false, identity: true),
                        OrgStructureId = c.Int(nullable: false),
                        WorkUnitId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        Quntity = c.Int(nullable: false),
                        PlanType = c.Int(nullable: false),
                        Remark = c.String(),
                        ProjectType = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.JobTitleMappingId)
                .ForeignKey("common.Lookup", t => t.LookupId, cascadeDelete: true)
                .ForeignKey("HRM.MainCompanyStructure", t => t.OrgStructureId, cascadeDelete: true)
                .Index(t => t.OrgStructureId)
                .Index(t => t.LookupId);
            
            CreateTable(
                "HRM.JobTitleMappingMianTemp",
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
                .ForeignKey("HRM.JobTitleMappingTemp", t => t.JobTitleMappingId, cascadeDelete: true)
                .Index(t => t.JobTitleMappingId);
            
            AddColumn("EXCEED.BinCardLine", "Date", c => c.DateTime(nullable: false));
            AddColumn("HRM.Branch", "LookupId", c => c.Int());
            CreateIndex("HRM.Branch", "LookupId");
            AddForeignKey("HRM.Branch", "LookupId", "common.Lookup", "LookupId");
            DropColumn("EXCEED.BinCardLine", "DateCreated");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.BinCardLine", "DateCreated", c => c.DateTime(nullable: false));
            DropForeignKey("HRM.JobTitleMappingMianTemp", "JobTitleMappingId", "HRM.JobTitleMappingTemp");
            DropForeignKey("HRM.JobTitleMappingTemp", "OrgStructureId", "HRM.MainCompanyStructure");
            DropForeignKey("HRM.JobTitleMappingTemp", "LookupId", "common.Lookup");
            DropForeignKey("HRM.JobTitleMappingDetailTemp", "JobTitleMappingId", "HRM.JobTitleMappingTemp");
            DropForeignKey("HRM.Branch", "LookupId", "common.Lookup");
            DropIndex("HRM.JobTitleMappingMianTemp", new[] { "JobTitleMappingId" });
            DropIndex("HRM.JobTitleMappingTemp", new[] { "LookupId" });
            DropIndex("HRM.JobTitleMappingTemp", new[] { "OrgStructureId" });
            DropIndex("HRM.JobTitleMappingDetailTemp", new[] { "JobTitleMappingId" });
            DropIndex("HRM.Branch", new[] { "LookupId" });
            DropColumn("HRM.Branch", "LookupId");
            DropColumn("EXCEED.BinCardLine", "Date");
            DropTable("HRM.JobTitleMappingMianTemp");
            DropTable("HRM.JobTitleMappingTemp");
            DropTable("HRM.JobTitleMappingDetailTemp");
        }
    }
}
