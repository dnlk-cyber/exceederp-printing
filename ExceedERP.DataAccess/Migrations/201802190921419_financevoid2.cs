namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class financevoid2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ArInvoice", "Remark", c => c.String());
            AddColumn("EXCEED.ArInvoice", "IsOnlineTransferredBy", c => c.String());
            DropColumn("EXCEED.ArInvoice", "OnlineTransferredBy");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ArInvoice", "OnlineTransferredBy", c => c.String());
            DropColumn("EXCEED.ArInvoice", "IsOnlineTransferredBy");
            DropColumn("EXCEED.ArInvoice", "Remark");
        }
    }
}
