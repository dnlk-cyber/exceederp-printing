namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TerminationRequestUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.Termination", "IsApplied", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.Termination", "IsApplied");
        }
    }
}
