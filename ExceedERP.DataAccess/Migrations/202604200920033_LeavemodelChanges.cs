namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LeavemodelChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.LeaveRequest", "LookupId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("HRM.LeaveRequest", "LookupId");
        }
    }
}
