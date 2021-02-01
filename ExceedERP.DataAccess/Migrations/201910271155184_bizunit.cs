namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bizunit : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GLLedgerPosting", "BusinessUnit", c => c.String());
            AddColumn("EXCEED.GLRecordJournalBegDistribution", "BusinessUnit", c => c.String());
            AddColumn("EXCEED.GLRecordJournalDetail", "BusinessUnit", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.GLRecordJournalDetail", "BusinessUnit");
            DropColumn("EXCEED.GLRecordJournalBegDistribution", "BusinessUnit");
            DropColumn("EXCEED.GLLedgerPosting", "BusinessUnit");
        }
    }
}
