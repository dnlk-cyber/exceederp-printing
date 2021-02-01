namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm35 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.LeaveRequest", "RequesterId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("HRM.LeaveRequest", "RequesterId");
        }
    }
}
