namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrmerge9 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "HRM.Holiday",
                c => new
                    {
                        HolidayId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Name = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.HolidayId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "HRM.Weekend",
                c => new
                    {
                        WeekendId = c.Int(nullable: false, identity: true),
                        IsHalfDay = c.Boolean(nullable: false),
                        Name = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.WeekendId);
            
            AlterColumn("HRM.LateComing", "Time", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("HRM.Holiday", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropIndex("HRM.Holiday", new[] { "GlFiscalYearId" });
            AlterColumn("HRM.LateComing", "Time", c => c.String());
            DropTable("HRM.Weekend");
            DropTable("HRM.Holiday");
        }
    }
}
