namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tender4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.Tender", "Remark", c => c.String());
            AddColumn("EXCEED.Tender", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.Tender", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.Tender", "OnlineTransferredTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.Tender", "OnlineTransferredTime");
            DropColumn("EXCEED.Tender", "IsOnlineTransferredBy");
            DropColumn("EXCEED.Tender", "IsOnlineTransferred");
            DropColumn("EXCEED.Tender", "Remark");
        }
    }
}
