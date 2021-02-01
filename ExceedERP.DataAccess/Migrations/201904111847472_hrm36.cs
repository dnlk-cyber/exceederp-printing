namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm36 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.Leave", "RequestId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("HRM.Leave", "RequestId");
        }
    }
}
