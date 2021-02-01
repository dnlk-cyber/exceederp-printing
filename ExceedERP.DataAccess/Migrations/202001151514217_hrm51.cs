namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm51 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.LeaveReturn", "IsForOther", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.LeaveReturn", "IsForOther");
        }
    }
}
