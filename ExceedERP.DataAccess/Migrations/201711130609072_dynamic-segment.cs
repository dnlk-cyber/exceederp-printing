namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dynamicsegment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.GLChartOfAccountSegment",
                c => new
                    {
                        GLChartOfAccountSegmentId = c.Int(nullable: false, identity: true),
                        GLSegmentSetupId = c.Int(nullable: false),
                        Values = c.String(),
                        Description = c.String(nullable: false),
                        HasParent = c.String(),
                        Enable = c.Boolean(nullable: false),
                        Parent = c.Boolean(nullable: false),
                        AllowBudgeting = c.Boolean(nullable: false),
                        AllowPosting = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GLChartOfAccountSegmentId)
                .ForeignKey("EXCEED.GLSegmentSetup", t => t.GLSegmentSetupId, cascadeDelete: true)
                .Index(t => t.GLSegmentSetupId);
            
            CreateTable(
                "EXCEED.GLSegmentSetup",
                c => new
                    {
                        GLSegmentSetupId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        SegmentLength = c.Int(nullable: false),
                        Sequence = c.Int(nullable: false),
                        Displayed = c.Boolean(nullable: false),
                        Enabled = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GLSegmentSetupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.GLChartOfAccountSegment", "GLSegmentSetupId", "EXCEED.GLSegmentSetup");
            DropIndex("EXCEED.GLChartOfAccountSegment", new[] { "GLSegmentSetupId" });
            DropTable("EXCEED.GLSegmentSetup");
            DropTable("EXCEED.GLChartOfAccountSegment");
        }
    }
}
