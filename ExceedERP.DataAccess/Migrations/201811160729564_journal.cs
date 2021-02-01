namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class journal : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GlRecordJournal", "Source", c => c.String());
            AddColumn("EXCEED.GlRecordJournal", "SourceId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.GlRecordJournal", "SourceId");
            DropColumn("EXCEED.GlRecordJournal", "Source");
        }
    }
}
