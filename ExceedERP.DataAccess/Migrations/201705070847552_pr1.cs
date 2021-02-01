namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pr1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ProformaPurchase", "Remark", c => c.String());
            AddColumn("EXCEED.ProformaPurchase", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ProformaPurchase", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.ProformaPurchase", "OnlineTransferredTime", c => c.DateTime());
            AlterColumn("EXCEED.PurchaseValidation", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.PurchaseValidation", "Status", c => c.String());
            DropColumn("EXCEED.ProformaPurchase", "OnlineTransferredTime");
            DropColumn("EXCEED.ProformaPurchase", "IsOnlineTransferredBy");
            DropColumn("EXCEED.ProformaPurchase", "IsOnlineTransferred");
            DropColumn("EXCEED.ProformaPurchase", "Remark");
        }
    }
}
