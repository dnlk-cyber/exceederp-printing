namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class posting1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GLLedgerPostingLines", "GlFiscalYearId", c => c.Int(nullable: false));
            AddColumn("EXCEED.GLLedgerPostingLines", "GLPeriodId", c => c.Int(nullable: false));
            DropColumn("EXCEED.GLLedgerPostingLines", "Year");
            DropColumn("EXCEED.GLLedgerPostingLines", "Period");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.GLLedgerPostingLines", "Period", c => c.String());
            AddColumn("EXCEED.GLLedgerPostingLines", "Year", c => c.String());
            DropColumn("EXCEED.GLLedgerPostingLines", "GLPeriodId");
            DropColumn("EXCEED.GLLedgerPostingLines", "GlFiscalYearId");
        }
    }
}
