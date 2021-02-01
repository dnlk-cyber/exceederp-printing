namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class notification2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.Notification", "IpAddress", c => c.String());
            AddColumn("EXCEED.Notification", "ResponsibleRoles", c => c.String());
            AddColumn("EXCEED.Notification", "DeadLine", c => c.DateTime(nullable: false));
            AddColumn("EXCEED.PurchaseOrder", "IsManual", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PurchaseOrder", "IsManual");
            DropColumn("EXCEED.Notification", "DeadLine");
            DropColumn("EXCEED.Notification", "ResponsibleRoles");
            DropColumn("EXCEED.Notification", "IpAddress");
        }
    }
}
