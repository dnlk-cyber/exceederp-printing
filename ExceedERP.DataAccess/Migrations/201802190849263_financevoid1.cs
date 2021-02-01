namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class financevoid1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ApPaybleInvoices", "Remark", c => c.String());
            AddColumn("EXCEED.ApPaybleInvoices", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.ApPettyCash", "Remark", c => c.String());
            AddColumn("EXCEED.ApPettyCash", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.ApTravelExpenseClaim", "Remark", c => c.String());
            AddColumn("EXCEED.ApTravelExpenseClaim", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.ApTravelOrder", "Remark", c => c.String());
            AddColumn("EXCEED.ApTravelOrder", "IsOnlineTransferredBy", c => c.String());
            DropColumn("EXCEED.ApPaybleInvoices", "OnlineTransferredBy");
            DropColumn("EXCEED.ApPettyCash", "OnlineTransferredBy");
            DropColumn("EXCEED.ApTravelExpenseClaim", "OnlineTransferredBy");
            DropColumn("EXCEED.ApTravelOrder", "OnlineTransferredBy");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ApTravelOrder", "OnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.ApTravelExpenseClaim", "OnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.ApPettyCash", "OnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.ApPaybleInvoices", "OnlineTransferredBy", c => c.String());
            DropColumn("EXCEED.ApTravelOrder", "IsOnlineTransferredBy");
            DropColumn("EXCEED.ApTravelOrder", "Remark");
            DropColumn("EXCEED.ApTravelExpenseClaim", "IsOnlineTransferredBy");
            DropColumn("EXCEED.ApTravelExpenseClaim", "Remark");
            DropColumn("EXCEED.ApPettyCash", "IsOnlineTransferredBy");
            DropColumn("EXCEED.ApPettyCash", "Remark");
            DropColumn("EXCEED.ApPaybleInvoices", "IsOnlineTransferredBy");
            DropColumn("EXCEED.ApPaybleInvoices", "Remark");
        }
    }
}
