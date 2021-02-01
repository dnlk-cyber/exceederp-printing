namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrmerge8 : DbMigration
    {
        public override void Up()
        {
           // MoveTable(name: "EXCEED.JobTitleMappingDetailTemp", newSchema: "HRM");
            CreateTable(
                "HRM.JobTitleMappingMainTemp",
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
                //.ForeignKey("HRM.JobTitleMappingTemp", t => t.JobTitleMappingId, cascadeDelete: true)
                .Index(t => t.JobTitleMappingId);
            
            CreateTable(
                "HRM.JobTitleMappingMain",
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("HRM.JobTitleMappingMain", "JobTitleMappingId", "HRM.JobTitleMapping");
            DropForeignKey("HRM.JobTitleMappingMainTemp", "JobTitleMappingId", "HRM.JobTitleMappingTemp");
            DropIndex("HRM.JobTitleMappingMain", new[] { "JobTitleMappingId" });
            DropIndex("HRM.JobTitleMappingMainTemp", new[] { "JobTitleMappingId" });
            DropTable("HRM.JobTitleMappingMain");
            DropTable("HRM.JobTitleMappingMainTemp");
            MoveTable(name: "HRM.JobTitleMappingDetailTemp", newSchema: "EXCEED");
        }
    }
}
