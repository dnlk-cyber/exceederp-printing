namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class po3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PurchaseOrder", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseOrder", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.PurchaseOrder", "OnlineTransferredTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PurchaseOrder", "OnlineTransferredTime");
            DropColumn("EXCEED.PurchaseOrder", "IsOnlineTransferredBy");
            DropColumn("EXCEED.PurchaseOrder", "IsOnlineTransferred");
        }
    }
}
