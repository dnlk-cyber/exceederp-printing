namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gl1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GlRecordJournal", "BranchId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.GlRecordJournal", "BranchId");
        }
    }
}
