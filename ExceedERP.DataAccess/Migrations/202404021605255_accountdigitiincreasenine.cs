namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class accountdigitiincreasenine : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.GLLedgerPostingLines", "Debit", c => c.Decimal(nullable: false, precision: 18, scale: 9));
            AlterColumn("EXCEED.GLLedgerPostingLines", "Credit", c => c.Decimal(nullable: false, precision: 18, scale: 9));
            AlterColumn("EXCEED.GLRecordJournalDetail", "Debit", c => c.Decimal(nullable: false, precision: 18, scale: 9));
            AlterColumn("EXCEED.GLRecordJournalDetail", "Credit", c => c.Decimal(nullable: false, precision: 18, scale: 9));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.GLRecordJournalDetail", "Credit", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.GLRecordJournalDetail", "Debit", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.GLLedgerPostingLines", "Credit", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.GLLedgerPostingLines", "Debit", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
    }
}
