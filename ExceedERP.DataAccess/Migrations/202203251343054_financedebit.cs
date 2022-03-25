namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class financedebit : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GlRecordJournal", "DebitSum", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.GlRecordJournal", "DebitSum");
        }
    }
}
