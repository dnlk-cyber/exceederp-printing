namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommiteandDciplinechanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.CommitteMinute", "DisciplinaryCaseId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("HRM.CommitteMinute", "DisciplinaryCaseId");
        }
    }
}
