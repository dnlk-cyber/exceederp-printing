namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class additionaldesc : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GLLedgerPostingLines", "AdditionalDescription", c => c.String());
            AddColumn("EXCEED.ReconciliationBookAccount", "AdditionalDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ReconciliationBookAccount", "AdditionalDescription");
            DropColumn("EXCEED.GLLedgerPostingLines", "AdditionalDescription");
        }
    }
}
