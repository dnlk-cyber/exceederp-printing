namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class commminute : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.CommitteMinute", "CommitteeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.CommitteMinute", "CommitteeId");
        }
    }
}
