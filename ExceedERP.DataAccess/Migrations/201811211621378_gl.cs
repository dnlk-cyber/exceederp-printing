namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gl : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GlRecordJournal", "IsVoidDoc", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.GlRecordJournal", "IsVoidDoc");
        }
    }
}
