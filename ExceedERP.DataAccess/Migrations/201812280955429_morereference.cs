namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class morereference : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GLLedgerPostingLines", "ReferenceDoc", c => c.String());
            AddColumn("EXCEED.GLLedgerPostingLines", "PaymentReference", c => c.String());
            AddColumn("EXCEED.GLLedgerPostingLines", "InvoiceReferenceDoc", c => c.String());
            AddColumn("EXCEED.GlRecordJournal", "ReferenceDoc", c => c.String());
            AddColumn("EXCEED.GlRecordJournal", "PaymentReference", c => c.String());
            AddColumn("EXCEED.GlRecordJournal", "InvoiceReferenceDoc", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.GlRecordJournal", "InvoiceReferenceDoc");
            DropColumn("EXCEED.GlRecordJournal", "PaymentReference");
            DropColumn("EXCEED.GlRecordJournal", "ReferenceDoc");
            DropColumn("EXCEED.GLLedgerPostingLines", "InvoiceReferenceDoc");
            DropColumn("EXCEED.GLLedgerPostingLines", "PaymentReference");
            DropColumn("EXCEED.GLLedgerPostingLines", "ReferenceDoc");
        }
    }
}
