namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fintech2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GLJournalCategoriers", "Enable", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.GLJournalCategoriers", "Enable");
        }
    }
}
