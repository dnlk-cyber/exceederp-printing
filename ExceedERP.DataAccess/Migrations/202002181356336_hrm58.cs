namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm58 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.Termination", "IsDirect", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.Termination", "IsDirect");
        }
    }
}
