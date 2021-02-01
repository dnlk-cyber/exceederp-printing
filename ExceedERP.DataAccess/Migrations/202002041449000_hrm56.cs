namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm56 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.Placement", "IsPend", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.Placement", "IsPend");
        }
    }
}
