namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class secondledger : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.GlSecondayLedgerLine",
                c => new
                    {
                        GlSecondayLedgerLineId = c.Int(nullable: false, identity: true),
                        GlSecondayLedgerId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        JournalName = c.String(),
                        Description = c.String(),
                        JournalNumber = c.String(),
                        JournalReference = c.String(),
                        CategoryName = c.String(),
                        SourcesName = c.String(),
                        EffectiveDate = c.DateTime(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PostingSource = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GlSecondayLedgerLineId)
                .ForeignKey("EXCEED.GlSecondayLedger", t => t.GlSecondayLedgerId, cascadeDelete: true)
                .Index(t => t.GlSecondayLedgerId);
            
            CreateTable(
                "EXCEED.GlSecondayLedger",
                c => new
                    {
                        GlSecondayLedgerId = c.Int(nullable: false, identity: true),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        AccountType = c.Int(nullable: false),
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
                .PrimaryKey(t => t.GlSecondayLedgerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.GlSecondayLedgerLine", "GlSecondayLedgerId", "EXCEED.GlSecondayLedger");
            DropIndex("EXCEED.GlSecondayLedgerLine", new[] { "GlSecondayLedgerId" });
            DropTable("EXCEED.GlSecondayLedger");
            DropTable("EXCEED.GlSecondayLedgerLine");
        }
    }
}
