namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm14 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.PlacementHistory", "EmployementType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.PlacementHistory", "EmployementType");
        }
    }
}
