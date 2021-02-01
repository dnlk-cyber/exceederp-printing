namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fintech1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.IfrsScoiContinuingOperation", "NoteNumber", c => c.String());
            AddColumn("EXCEED.IfrsScoiDiscontinuedOperation", "NoteNumber", c => c.String());
            AddColumn("EXCEED.IfrsScoiOtherComprehensiveIncome", "NoteNumber", c => c.String());
            AddColumn("EXCEED.IfrsSocfFinancingActivity", "NoteNumber", c => c.String());
            AddColumn("EXCEED.IfrsSocfInvestingActivity", "NoteNumber", c => c.String());
            AddColumn("EXCEED.IfrsSocfOperatingActivity", "NoteNumber", c => c.String());
            AddColumn("EXCEED.IfrsSofpAsset", "NoteNumber", c => c.String());
            AddColumn("EXCEED.IfrsSofpEquityAndLiablity", "NoteNumber", c => c.String());
            AddColumn("EXCEED.IfrsStatementOfChangesInEquity", "NoteNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.IfrsStatementOfChangesInEquity", "NoteNumber");
            DropColumn("EXCEED.IfrsSofpEquityAndLiablity", "NoteNumber");
            DropColumn("EXCEED.IfrsSofpAsset", "NoteNumber");
            DropColumn("EXCEED.IfrsSocfOperatingActivity", "NoteNumber");
            DropColumn("EXCEED.IfrsSocfInvestingActivity", "NoteNumber");
            DropColumn("EXCEED.IfrsSocfFinancingActivity", "NoteNumber");
            DropColumn("EXCEED.IfrsScoiOtherComprehensiveIncome", "NoteNumber");
            DropColumn("EXCEED.IfrsScoiDiscontinuedOperation", "NoteNumber");
            DropColumn("EXCEED.IfrsScoiContinuingOperation", "NoteNumber");
        }
    }
}
