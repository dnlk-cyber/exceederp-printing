namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ledgerupdates : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GLLedgerPostingLines", "JournalName", c => c.String());
            AddColumn("EXCEED.GLLedgerPostingLines", "Description", c => c.String());
            AddColumn("EXCEED.GLLedgerPostingLines", "JournalReference", c => c.String());
            AddColumn("EXCEED.GLLedgerPostingLines", "CategoryName", c => c.String());
            AddColumn("EXCEED.GLLedgerPostingLines", "SourcesName", c => c.String());
            AddColumn("EXCEED.GLLedgerPosting", "ChannelSegment", c => c.String());
            AddColumn("EXCEED.GLRecordJournalDetail", "AccountType", c => c.Int(nullable: false));
            AddColumn("EXCEED.GLRecordJournalDetail", "AccountSegment", c => c.String());
            AddColumn("EXCEED.GLRecordJournalDetail", "BranchSegment", c => c.String());
            AddColumn("EXCEED.GLRecordJournalDetail", "CostCenterSegment", c => c.String());
            AddColumn("EXCEED.GLRecordJournalDetail", "SubAccountSegment", c => c.String());
            AddColumn("EXCEED.GLRecordJournalDetail", "ChannelSegment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.GLRecordJournalDetail", "ChannelSegment");
            DropColumn("EXCEED.GLRecordJournalDetail", "SubAccountSegment");
            DropColumn("EXCEED.GLRecordJournalDetail", "CostCenterSegment");
            DropColumn("EXCEED.GLRecordJournalDetail", "BranchSegment");
            DropColumn("EXCEED.GLRecordJournalDetail", "AccountSegment");
            DropColumn("EXCEED.GLRecordJournalDetail", "AccountType");
            DropColumn("EXCEED.GLLedgerPosting", "ChannelSegment");
            DropColumn("EXCEED.GLLedgerPostingLines", "SourcesName");
            DropColumn("EXCEED.GLLedgerPostingLines", "CategoryName");
            DropColumn("EXCEED.GLLedgerPostingLines", "JournalReference");
            DropColumn("EXCEED.GLLedgerPostingLines", "Description");
            DropColumn("EXCEED.GLLedgerPostingLines", "JournalName");
        }
    }
}
