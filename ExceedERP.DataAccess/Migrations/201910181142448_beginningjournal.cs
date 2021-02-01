namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class beginningjournal : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.GLRecordJournalBegDistribution",
                c => new
                    {
                        GLRecordJournalBegDistributionId = c.Int(nullable: false, identity: true),
                        GLRecordJournalBegId = c.Int(nullable: false),
                        AccountCode = c.String(nullable: false),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdditionalDescription = c.String(),
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
                .PrimaryKey(t => t.GLRecordJournalBegDistributionId)
                .ForeignKey("EXCEED.GLRecordJournalBeg", t => t.GLRecordJournalBegId, cascadeDelete: true)
                .Index(t => t.GLRecordJournalBegId);
            
            CreateTable(
                "EXCEED.GLRecordJournalBeg",
                c => new
                    {
                        GLRecordJournalBegId = c.Int(nullable: false, identity: true),
                        JournalName = c.String(nullable: false),
                        Description = c.String(),
                        FiscalYear = c.String(),
                        PeriodName = c.String(nullable: false),
                        CategoryName = c.String(nullable: false),
                        SourcesName = c.String(),
                        JournalNumber = c.String(),
                        EffectiveDate = c.DateTime(nullable: false),
                        Posting = c.String(),
                        isReversed = c.Boolean(nullable: false),
                        isPosted = c.Boolean(nullable: false),
                        IsPostedToSecondLedger = c.Boolean(nullable: false),
                        PostedBy = c.String(),
                        PostedTime = c.DateTime(),
                        Remark = c.String(),
                        Source = c.String(),
                        SourceId = c.String(),
                        IsVoidDoc = c.Boolean(nullable: false),
                        ReferenceDoc = c.String(),
                        PaymentReference = c.String(),
                        InvoiceReferenceDoc = c.String(),
                        BranchId = c.Int(nullable: false),
                        Status = c.String(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GLRecordJournalBegId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.GLRecordJournalBegDistribution", "GLRecordJournalBegId", "EXCEED.GLRecordJournalBeg");
            DropIndex("EXCEED.GLRecordJournalBegDistribution", new[] { "GLRecordJournalBegId" });
            DropTable("EXCEED.GLRecordJournalBeg");
            DropTable("EXCEED.GLRecordJournalBegDistribution");
        }
    }
}
