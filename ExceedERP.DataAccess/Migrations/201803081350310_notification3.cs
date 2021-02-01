namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class notification3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.Notification", "DeadLine", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.Notification", "DeadLine", c => c.DateTime(nullable: false));
        }
    }
}
