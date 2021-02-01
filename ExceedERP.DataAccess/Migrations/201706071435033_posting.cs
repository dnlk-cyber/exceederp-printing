namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class posting : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GlRecordJournal", "PostedBy", c => c.String());
            AddColumn("EXCEED.GlRecordJournal", "PostedTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.GlRecordJournal", "PostedTime");
            DropColumn("EXCEED.GlRecordJournal", "PostedBy");
        }
    }
}
