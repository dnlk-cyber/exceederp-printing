namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm33 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.RequestesStatus", "ApproverId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("HRM.RequestesStatus", "ApproverId");
        }
    }
}
