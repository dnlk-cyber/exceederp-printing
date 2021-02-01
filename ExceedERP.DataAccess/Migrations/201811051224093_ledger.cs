namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ledger : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GLLedgerPostingLines", "Remark", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.GLLedgerPostingLines", "Remark");
        }
    }
}
