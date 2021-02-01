namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class journalremark : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GlRecordJournal", "Remark", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.GlRecordJournal", "Remark");
        }
    }
}
