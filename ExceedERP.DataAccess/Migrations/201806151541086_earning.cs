namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class earning : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.GlEarningLedgerLine",
                c => new
                    {
                        GlEarningLedgerLineId = c.Int(nullable: false, identity: true),
                        GlEarningLedgerId = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        MonthlyAmount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        YearlyAmount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GlEarningLedgerLineId)
                .ForeignKey("EXCEED.GlEarningLedger", t => t.GlEarningLedgerId, cascadeDelete: true)
                .Index(t => t.GlEarningLedgerId);
            
            CreateTable(
                "EXCEED.GlEarningLedger",
                c => new
                    {
                        GlEarningLedgerId = c.Int(nullable: false, identity: true),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Type = c.Int(nullable: false),
                        AccountSegment = c.String(),
                        BranchSegment = c.String(),
                        CostCenterSegment = c.String(),
                        SubAccountSegment = c.String(),
                        ChannelSegment = c.String(),
                        Segment1 = c.String(),
                        Segment2 = c.String(),
                        Segment3 = c.String(),
                        Segment4 = c.String(),
                        Segment5 = c.String(),
                        Segment6 = c.String(),
                        Segment7 = c.String(),
                        Segment8 = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GlEarningLedgerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.GlEarningLedgerLine", "GlEarningLedgerId", "EXCEED.GlEarningLedger");
            DropIndex("EXCEED.GlEarningLedgerLine", new[] { "GlEarningLedgerId" });
            DropTable("EXCEED.GlEarningLedger");
            DropTable("EXCEED.GlEarningLedgerLine");
        }
    }
}
