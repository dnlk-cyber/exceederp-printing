namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class notification4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.Notification", "IsRead", c => c.Boolean(nullable: false));
            AlterColumn("EXCEED.Notification", "IsDeleted", c => c.Boolean(nullable: false));
            AlterColumn("EXCEED.Notification", "IsReminder", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.Notification", "IsReminder", c => c.String());
            AlterColumn("EXCEED.Notification", "IsDeleted", c => c.String());
            AlterColumn("EXCEED.Notification", "IsRead", c => c.String());
        }
    }
}
