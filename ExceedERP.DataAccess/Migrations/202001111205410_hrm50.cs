namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm50 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.LeaveEntitle", "IsAdded", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.LeaveEntitle", "IsAdded");
        }
    }
}
